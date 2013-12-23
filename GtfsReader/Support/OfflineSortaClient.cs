using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using SmartRoutes.Reader;

namespace SmartRoutes.GtfsReader.Support
{
    public class OfflineSortaClient : IGtfsClient
    {
        private readonly string _archivePath;

        public OfflineSortaClient(string archivePath)
        {
            _archivePath = archivePath;
        }

        public async Task<ClientResponse> GetArchiveContent()
        {
            // read the bytes
            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(_archivePath, FileMode.Open))
            {
                await fileStream.CopyToAsync(memoryStream);
            }

            // generate the response
            return new ClientResponse
            {
                RequestUri = new Uri(_archivePath, UriKind.RelativeOrAbsolute),
                StatusCode = HttpStatusCode.OK,
                Headers = new ClientResponseHeaders(),
                Content = memoryStream.ToArray()
            };
        }
    }
}