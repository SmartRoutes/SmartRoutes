using System;
using System.Net.Http;
using System.Threading.Tasks;
using SmartRoutes.Reader;

namespace SmartRoutes.GtfsReader.Support
{
    public class OnlineSortaClient : IGtfsClient
    {
        private static readonly Uri ArchiveUri = new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip");
        private readonly ScraperClient _scraperClient;

        public OnlineSortaClient()
        {
            _scraperClient = new ScraperClient();
        }

        public async Task<ClientResponse> GetArchiveContent()
        {
            // get the response
            var response = await _scraperClient.GetAsync(ArchiveUri, HttpCompletionOption.ResponseContentRead);
            ClientResponse clientResponse = await ClientResponse.Create(ArchiveUri, response);

            return clientResponse;
        }
    }
}