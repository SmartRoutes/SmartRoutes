using System.IO;
using System.Threading.Tasks;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;
using Scraper;

namespace OdjfsScraper.Support
{
    public class DownloadingOdjfsClient : OdjfsClient
    {
        private readonly string _directory;
        private bool _hasDirectoryBeenChecked;

        public DownloadingOdjfsClient(string directory)
        {
            _directory = directory;
            _hasDirectoryBeenChecked = false;
        }

        protected override async Task HandleChildCareDocumentResponse(ChildCare childCare, ClientResponse response)
        {
            await WriteChildCareBytes(childCare.ExternalUrlId, response);
        }

        protected override async Task HandleListDocumentResponse(County county, ClientResponse response)
        {
            string countyName = county == null ? "all" : county.Name;
            await WriteBytes("County_" + countyName + "_Current_{0}_{1}.html", response);
        }

        protected override async Task HandleChildCareDocumentResponse(ChildCareStub childCareStub, ClientResponse response)
        {
            await WriteChildCareBytes(childCareStub.ExternalUrlId, response);
        }

        private async Task WriteChildCareBytes(string externalUrlId, ClientResponse response)
        {
            await WriteBytes("ChildCare_" + externalUrlId + "_Current_{0}_{1}.html", response);
        }

        private async Task WriteBytes(string fileNameFormat, ClientResponse response)
        {
            // make sure the directory exists before writing to it...
            if (!_hasDirectoryBeenChecked)
            {
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }
                _hasDirectoryBeenChecked = true;
            }

            // generate a path for the child care
            string path = Path.Combine(_directory, string.Format(
                fileNameFormat,
                response.StatusCode,
                response.Content.GetSha256Hash()));

            if (!File.Exists(path))
            {
                using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    // write to the file
                    await outputStream.WriteAsync(response.Content, 0, response.Content.Length);
                }
            }
        }
    }
}