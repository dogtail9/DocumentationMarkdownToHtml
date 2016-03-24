using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationMarkdownToHtml
{
    public static class FileConstants
    {
        public static string DocFolder { get; } = "Documentation";

        public static string GulpScript { get; } = "\\gulpfile.js";

        public static string HtmlTemplate { get; } = "\\Template.html";

        public static string NpmPackages { get; } = "\\package.json";

        public static string VsCodeTasks { get; } = "\\.vscode\\tasks.json";

        public static string TestMarkdown { get; } = "\\Markdown\\Test.md";

        public static string TestImage { get; } = "\\Markdown\\Media\\Image.png";
        
        public static IEnumerable<string> NoneUpdatableFiles { get; } = new List<string>
        {
            TestMarkdown,
            TestImage
        };

        public static List<string> UpdatableFiles = new List<string>
        {
            HtmlTemplate,
            GulpScript,
            NpmPackages,
            VsCodeTasks
        };

        public static IEnumerable<string> AllFiles
        {
            get
            {
                return NoneUpdatableFiles.Concat(UpdatableFiles);
            }
        }
    }
}
