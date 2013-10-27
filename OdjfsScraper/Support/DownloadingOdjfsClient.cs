using System.IO;
using System.Threading.Tasks;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
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

        protected override async Task HandleChildCareDocumentBytes(ChildCare childCare, byte[] bytes)
        {
            CheckDirectory();
            await WriteBytes("child_care_" + childCare.ExternalUrlId + "_{0}.html", bytes);
        }

        protected override async Task HandleListDocumentBytes(County county, byte[] bytes)
        {
            CheckDirectory();
            string countyName = county == null ? "all" : county.Name;
            await WriteBytes("list_" + countyName + "_{0}.html", bytes);
        }

        private void CheckDirectory()
        {
            if (!_hasDirectoryBeenChecked)
            {
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }
                _hasDirectoryBeenChecked = true;
            }
        }

        private async Task WriteBytes(string fileNameFormat, byte[] bytes)
        {
            // generate a path for the child care
            string path = Path.Combine(_directory, string.Format(fileNameFormat, bytes.GetSha256Hash()));

            if (!File.Exists(path))
            {
                using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    // write to the file
                    await outputStream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
        }
    }
}