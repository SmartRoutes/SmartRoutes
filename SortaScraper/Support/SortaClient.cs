using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Scraper;

namespace SortaScraper.Support
{
    public class SortaClient : ISortaClient
    {
        private const string ArchiveUri = "http://www.go-metro.com/uploads/GTFS/google_transit_info.zip";
        private readonly ScraperClient _scraperClient;

        public SortaClient()
        {
            _scraperClient = new ScraperClient();
        }

        public async Task<HttpResponseHeaders> GetArchiveHeaders()
        {
            // make the request
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri(ArchiveUri)
            };

            // get the response
            HttpResponseMessage response = await _scraperClient.SendAsync(request);
            return response.Headers;
        }

        public async Task<HttpResponseMessage> GetArchiveContent()
        {
            return await _scraperClient.GetAsync(ArchiveUri, HttpCompletionOption.ResponseContentRead);
        }
    }
}