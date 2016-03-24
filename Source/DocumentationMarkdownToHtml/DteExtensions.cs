using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationMarkdownToHtml
{
    public static class DteExtensions
    {
        public static void ShowStatusBarMessage(this string message)
        {
            DTE _dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
            _dte.StatusBar.Text = message;
        }

        public static SolutionFolder GetSolutionFolderEx(this Solution solution, string folderName)
        {
            Project solutionFolder = (from p in ((Solution2)solution).Projects.OfType<Project>()
                                      where p.Name.Equals(folderName)
                                      select p).FirstOrDefault();

            return (SolutionFolder)solutionFolder?.Object;
        }

        public static SolutionFolder GetSolutionFolderEx(this SolutionFolder solutionFolder, string folderName)
        {
            ProjectItem folder = (from p in solutionFolder.Parent.ProjectItems.OfType<ProjectItem>()
                                  where p.Name.Equals(folderName)
                                  select p).FirstOrDefault();

            return (SolutionFolder)((Project)folder?.Object)?.Object;
        }

        public static SolutionFolder AddSolutionFolderEx(this Solution solution, string folderName)
        {
            SolutionFolder folder = solution.GetSolutionFolderEx(folderName);

            if (folder == null)
            {
                folder = (SolutionFolder)((Solution4)solution).AddSolutionFolder(folderName).Object;
            }

            return folder;
        }

        public static SolutionFolder AddSolutionFolderEx(this SolutionFolder solutionFolder, string folderName)
        {
            SolutionFolder folder = solutionFolder.GetSolutionFolderEx(folderName);

            if (folder == null)
            {
                folder = (SolutionFolder)solutionFolder.AddSolutionFolder(folderName).Object;
            }

            return folder;
        }
    }
}
