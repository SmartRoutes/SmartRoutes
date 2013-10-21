using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Support
{
    public class DownloadingClient : BaseClient
    {
        private readonly string _directory;

        public DownloadingClient(string directory)
        {
            _directory = directory;
        }

        protected override async Task HandleChildCareDocumentBytes(ChildCare childCare, byte[] bytes)
        {
            await WriteBytes(childCare.ExternalUrlId + "_{0}.html", bytes);
        }

        protected override async Task HandleListDocumentBytes(byte[] bytes)
        {
            await WriteBytes("list_{0}.html", bytes);
        }

        private async Task WriteBytes(string fileNameFormat, byte[] bytes)
        {
            // generate a hash of the contents
            var sha = new SHA256Managed();
            byte[] hashBytes = sha.ComputeHash(bytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", "");

            // generate a path for the child care
            string path = Path.Combine(_directory, string.Format(fileNameFormat, hash));

            using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                // write to the file
                await outputStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}