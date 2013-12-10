using System;
using System.Net.Http;
using System.Threading.Tasks;
using SmartRoutes.Scraper;

namespace SmartRoutes.SortaScraper.Support
{
    public class OnlineSortaClient : ISortaClient
    {
        private static readonly Uri ArchiveUri = new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip");
        private readonly ScraperClient _scraperClient;

        public OnlineSortaClient()
        {
            _scraperClient = new ScraperClient();
        }

        public async Task<ClientResponseHeaders> GetArchiveHeaders()
        {
            // make the request
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = ArchiveUri
            };

            // get the response
            HttpResponseMessage response = await _scraperClient.SendAsync(request);
            ClientResponse clientResponse = await ClientResponse.Create(ArchiveUri, response);

            return clientResponse.Headers;
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