using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationMarkdownToHtml
{
    public class FileData
    {
        public string Version { get; set; }

        public byte[] Content { get; set; }

        public bool FileUpdatedInRepo { get; set; }
    }

    public class DirectoryData
    {
        public Project SlnFolder { get; set; }

        public DirectoryInfo DiskFolder { get; set; }
    }
}
