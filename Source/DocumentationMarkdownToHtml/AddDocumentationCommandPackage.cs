//------------------------------------------------------------------------------
// <copyright file="AddDocumentationCommandPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace DocumentationMarkdownToHtml
{
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidAddDocumentationCommandPackageString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(SourceControlProvider), "Markdown to Html", "Repository Type", 0, 0, true, Sort = 0)]
    [ProvideOptionPage(typeof(GitHubSettings), "Markdown to Html", "GitHub Settings", 0, 0, true, Sort = 1)]
    [ProvideOptionPage(typeof(TfsSettings), "Markdown to Html", "Tfs Settings", 0, 0, true, Sort = 2)]
    [ProvideOptionPage(typeof(VsCodeSettings), "Markdown to Html", "Visual Studio Code Settings", 0, 0, true, Sort = 3)]
    public sealed class AddDocumentationCommandPackage : Package
    {
        DTE _dte;
        Solution2 solution;

        RelayCommand addDocumentationCommand;
        RelayCommand checkForUpdateCommand;
        RelayCommand openInVSCodeCommand;
        List<RelayCommand> updateCommands = new List<RelayCommand>();

        IFileDownloader fileDownloader;

        Dictionary<string, FileData> files = new Dictionary<string, FileData>();

        protected override void Initialize()
        {
            SetupSettingsChangedEventHandlers();

            LoadFileDownloader();

            _dte = GetService(typeof(DTE)) as DTE;
            solution = (Solution2)_dte.Solution;

            foreach (string file in FileConstants.AllFiles)
            {
                files.Add(file, GetPersistedVersion(file));
            }

            AddDocumentationCommand();
            CheckForUpdatesCommand();
            OpenInVSCodeCommand();
            updateCommands.Add(UpdateFileCommand(PackageIds.UpdateGulpFileCommandId, FileConstants.GulpScript));
            updateCommands.Add(UpdateFileCommand(PackageIds.UpdateHtmlTemplateCommandId, FileConstants.HtmlTemplate));
            updateCommands.Add(UpdateFileCommand(PackageIds.UpdatePackageJsonCommandId, FileConstants.NpmPackages));
            updateCommands.Add(UpdateFileCommand(PackageIds.UpdateVsCodeTasksJsonCommandId, FileConstants.VsCodeTasks));

            base.Initialize();
        }

        private void AddDocumentationCommand()
        {
            addDocumentationCommand = new RelayCommand(this, PackageIds.AddDocumentationCommandId, PackageGuids.guidAddDocumentationCommandPackageCmdSet,
            (sender, e) =>
            {
                var path = new FileInfo(solution.FileName).Directory.FullName;

                foreach (var file in FileConstants.AllFiles)
                {
                    files[file] = Download(file);
                }

                solution.Globals["DocState"] = "Added";
                solution.Globals.VariablePersists["DocState"] = true;
            },
            (sender, e) =>
            {
                var cmd = (OleMenuCommand)sender;
                ShowWhenNotAdded(cmd);
            });
        }

        private void OpenInVSCodeCommand()
        {
            openInVSCodeCommand = new RelayCommand(this, PackageIds.OpenInVsCodeCommandId, PackageGuids.guidAddDocumentationCommandPackageCmdSet,
            (sender, e) =>
            {
                var path = new FileInfo(solution.FileName).Directory.FullName;

                var npmProcessInfo = new ProcessStartInfo();
                npmProcessInfo.FileName = @"C:\Program Files (x86)\nodejs\npm.cmd";
                npmProcessInfo.Arguments = "install";
                npmProcessInfo.WorkingDirectory = $"{path}\\{FileConstants.DocFolder}";
                npmProcessInfo.UseShellExecute = true;
                var process2 = System.Diagnostics.Process.Start(npmProcessInfo);              

                var vsCodeProcessInfo = new ProcessStartInfo(VsCodeCommand);
                vsCodeProcessInfo.Arguments = $"\"{path}\\{FileConstants.DocFolder}\"";
                vsCodeProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
                vsCodeProcessInfo.UseShellExecute = true;
                var process = System.Diagnostics.Process.Start(vsCodeProcessInfo);
            },
            (sender, e) =>
            {
                var cmd = (OleMenuCommand)sender;
                ShowWhenAdded(cmd);
                cmd.Text = ((VsCodeSettings)GetDialogPage(typeof(VsCodeSettings))).UseInsiders ? "Open in Visual Studio Code Insiders" : "Open in Visual Studio Code";
            });
        }

        private void CheckForUpdatesCommand()
        {
            checkForUpdateCommand = new RelayCommand(this, PackageIds.CheckForUpdatesCommandId, PackageGuids.guidAddDocumentationCommandPackageCmdSet,
            (sender, e) =>
            {
                bool updatesFound = false;

                foreach (var file in FileConstants.UpdatableFiles)
                {
                    FileData templateData = fileDownloader.Download(file);
                    files[file].FileUpdatedInRepo = templateData.Version == files[file].Version ? false : true;

                    if (files[file].FileUpdatedInRepo)
                        updatesFound = true;
                }

                if (updatesFound)
                {
                    "Found updates for the documentation files.".ShowStatusBarMessage();
                }
                else
                {
                    "No updates for the documentation files where found.".ShowStatusBarMessage();
                }
            },
            (sender, e) =>
            {
                var cmd = (OleMenuCommand)sender;
                ShowWhenAdded(cmd);
            });
        }

        private RelayCommand UpdateFileCommand(int commandId, string path)
        {
            return new RelayCommand(this, commandId, PackageGuids.guidAddDocumentationCommandPackageCmdSet,
            (sender, e) =>
            {
                files[path] = Download(path);
            },
            (sender, e) =>
            {
                var cmd = (OleMenuCommand)sender;
                ShowWhenAdded(cmd, path);
            });
        }

        private FileData GetPersistedVersion(string path)
        {
            FileData data = new FileData();
            string key = path.Replace("\\", string.Empty).Replace(".", string.Empty);
            if (solution.Globals.VariableExists[key])
            {
                data.Version = solution.Globals[key].ToString();
            }

            data.FileUpdatedInRepo = false;

            return data;
        }

        private FileData Download(string path)
        {
            string localPath = $"{FileConstants.DocFolder}\\{path}";
            string fileName = Path.GetFileName(localPath);
            string dir = Path.GetDirectoryName(localPath);

            DirectoryData documentationDir = CreateFolder(dir);
            FileData templateData = fileDownloader.Download(path);

            File.WriteAllBytes($"{documentationDir.DiskFolder.FullName}\\{fileName}", templateData.Content);
            documentationDir.SlnFolder.ProjectItems.AddFromFile($"{documentationDir.DiskFolder.FullName}\\{fileName}");

            string key = path.Replace("\\", string.Empty).Replace(".", string.Empty);
            solution.Globals[key] = templateData.Version;
            solution.Globals.VariablePersists[key] = true;

            return templateData;
        }

        private DirectoryData CreateFolder(string path)
        {
            var root = new FileInfo(solution.FileName).Directory.FullName;

            DirectoryInfo currentDir = new DirectoryInfo(root);
            SolutionFolder current = null;
            string diskPath = string.Empty;

            string[] folders = path.Split('\\');

            for (int i = 0; i < folders.Length; i++)
            {
                current = current?.AddSolutionFolderEx(folders[i]) ?? _dte.Solution.AddSolutionFolderEx(folders[i]);

                diskPath += $"\\{folders[i]}";
                if (!Directory.Exists(diskPath))
                    currentDir = Directory.CreateDirectory($"{root}{diskPath}");
            }

            return new DirectoryData { SlnFolder = (Project)current.Parent, DiskFolder = currentDir };
        }

        private void ShowWhenNotAdded(OleMenuCommand cmd)
        {
            if (solution.Globals.VariableExists["DocState"] && (solution.Globals["DocState"].ToString() == "Added"))
            {
                cmd.Visible = false;
            }
            else
            {
                cmd.Visible = true;
            }
        }

        private void ShowWhenAdded(OleMenuCommand cmd, string path = "")
        {
            bool q = true;

            if (files.ContainsKey(path))
                q = files[path].FileUpdatedInRepo;

            if (solution.Globals.VariableExists["DocState"] && (solution.Globals["DocState"].ToString() == "Added") && q)
            {
                cmd.Visible = true;
            }
            else
            {
                cmd.Visible = false;
            }
        }

        private void LoadFileDownloader()
        {
            switch (RepositoryType)
            {
                case RepoType.GitHub:
                    fileDownloader = new GitHubFileDownloader(GitHubAccount, GitHubRepo, GitHubSourceFolder);
                    break;
                case RepoType.TFS:
                    fileDownloader = new TfsFileDownloader(TfsUri, TfsSourceUri);
                    break;
                default:
                    break;
            }
        }

        private void SetupSettingsChangedEventHandlers()
        {
            TfsSettings tfsSettings = (TfsSettings)GetDialogPage(typeof(TfsSettings));
            tfsSettings.Changed += (sender, e) =>
            {
                LoadFileDownloader();
            };

            GitHubSettings gitHubSettings = (GitHubSettings)GetDialogPage(typeof(GitHubSettings));
            gitHubSettings.Changed += (sender, e) =>
            {
                LoadFileDownloader();
            };

            SourceControlProvider sourceControlProvider = (SourceControlProvider)GetDialogPage(typeof(SourceControlProvider));
            sourceControlProvider.Changed += (sender, e) =>
            {
                LoadFileDownloader();
            };
        }

        public string TfsUri => ((TfsSettings)GetDialogPage(typeof(TfsSettings))).TfsUri;

        public string TfsSourceUri => ((TfsSettings)GetDialogPage(typeof(TfsSettings))).TfsSourceUri;

        public string GitHubAccount => ((GitHubSettings)GetDialogPage(typeof(GitHubSettings))).GitHubAccount;

        public string GitHubRepo => ((GitHubSettings)GetDialogPage(typeof(GitHubSettings))).GitHubRepo;
            
        public string GitHubSourceFolder => ((GitHubSettings)GetDialogPage(typeof(GitHubSettings))).GitHubSourceFolder;
            
        public RepoType RepositoryType => ((SourceControlProvider)GetDialogPage(typeof(SourceControlProvider))).RepositoryType;
                
        public string VsCodeCommand =>  ((VsCodeSettings)GetDialogPage(typeof(VsCodeSettings))).UseInsiders ? "code-insiders" : "code";
    }
}