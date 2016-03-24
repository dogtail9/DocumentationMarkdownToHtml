using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationMarkdownToHtml
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    public enum RepoType
    {
        GitHub,
        TFS
    }

    public class SourceControlProvider : DialogPage
    {
        public event ChangedEventHandler Changed;

        [Category("Markdown to Html")]
        [DisplayName("Repository Type")]
        [Description("Chose the repository type you want to use.")]
        public RepoType RepositoryType { get; set; } = RepoType.GitHub;

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (Changed != null)
                Changed(this, e);
        }
    }
    
    public class VsCodeSettings : DialogPage
    {
        [Category("Markdown to Html")]
        [DisplayName("Use Visual Studio Code Insiders")]
        [Description("Use Visual Studio Code Insiders")]
        public bool UseInsiders { get; set; } = false;
    }

    public class TfsSettings : DialogPage
    {
        public event ChangedEventHandler Changed;

        [Category("Markdown to Html")]
        [DisplayName("TFS Uri")]
        [Description("The TFS connection string")]
        public string TfsUri { get; set; } = "http://[SERVERNAME]:8080/tfs/DefaultCollection";

        [Category("Markdown to Html")]
        [DisplayName("Source Uri")]
        [Description("The path to the folder in source control where the files are located.")]
        public string TfsSourceUri { get; set; } = "$/[TEMAPROJECT]/Main/Tools/MarkdownBuild";

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (Changed != null)
                Changed(this, e);
        }
    }

    public class GitHubSettings : DialogPage
    {
        public event ChangedEventHandler Changed;

        [Category("Markdown to Html")]
        [DisplayName("GitHub Account")]
        [Description("Your GitHub account name.")]
        public string GitHubAccount { get; set; } = "dogtail9";

        [Category("Markdown to Html")]
        [DisplayName("GitHub Repo")]
        [Description("The name of the repository.")]
        public string GitHubRepo { get; set; } = "MarkdownToHtmlWithGulp";

        [Category("Markdown to Html")]
        [DisplayName("GitHub Source Folder")]
        [Description("The path to the folder in source control where the files are located.")]
        public string GitHubSourceFolder { get; set; } = "Source/Documentation";

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (Changed != null)
                Changed(this, e);
        }
    }
}
