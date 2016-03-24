using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationMarkdownToHtml
{
    public interface IFileDownloader
    {
        FileData Download(string path);
    }

    public class GitHubFileDownloader : IFileDownloader
    {
        private string gitHubAccount;
        private string gitHubRepo;
        private string sourceFolder;


        public GitHubFileDownloader(string gitHubAccount, string gitHubRepo, string sourceFolder)
        {
            this.gitHubAccount = gitHubAccount;
            this.gitHubRepo = gitHubRepo;
            this.sourceFolder = sourceFolder;
        }

        public FileData Download(string path)
        {
            path = path.Replace('\\', '/');

            $"Downloading {path} from GitHub ...".ShowStatusBarMessage();

            FileData data = new FileData();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

                string url = $"https://api.github.com/repos/{gitHubAccount}/{gitHubRepo}/contents/{sourceFolder}{path}";
                //string uri = $"https://api.github.com/repos/dogtail9/MarkdownToHtmlWithGulp/contents/Source{path}";
                var content = httpClient.GetStringAsync(url).Result;
                var jobject = JObject.Parse(content);
                data.Version = (string)jobject["sha"];
                data.Content = Convert.FromBase64String((string)jobject["content"]);
                data.FileUpdatedInRepo = false;
            }

            return data;
        }
    }

    public class TfsFileDownloader : IFileDownloader
    {
        private string tfsUri;
        private string sourceUri;

        public TfsFileDownloader(string tfsUri, string sourceUri)
        {
            this.tfsUri = tfsUri;
            this.sourceUri = sourceUri;
        }

        public FileData Download(string path)
        {
            path = path.Replace('\\', '/');

            $"Downloading {path} from TFS ...".ShowStatusBarMessage();

            FileData data = new FileData();

            using (HttpClient httpClient = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
            {
                string url = $"{tfsUri}/_apis/tfvc/items?scopepath={sourceUri}{path}";
                //string uri = $"http://zander:8080/tfs/DefaultCollection/_apis/tfvc/items?scopepath=$/MDEV/MDEV/Main/Tools/MarkdownBuild{path}";
                var content = httpClient.GetStringAsync(url).Result;
                var jobject = JObject.Parse(content);
                data.Version = jobject["value"].First["version"].ToString();
                var fileData = httpClient.GetByteArrayAsync(jobject["value"].First["url"].ToString()).Result;
                var file = fileData;
                data.Content = file;
                data.FileUpdatedInRepo = false;
            }

            return data;
        }
    }
}
