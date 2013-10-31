using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Model.Odjfs;
using Model.Odjfs.ChildCareStubs;
using NLog;
using OdjfsScraper.Parsers;
using OdjfsScraper.Support;
using Scraper;

namespace OdjfsScraper.Scrapers
{
    public class ChildCareStubListScraper : IChildCareStubListScraper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IOdjfsClient _odjfsClient;
        private readonly IListDocumentParser _parser;

        public ChildCareStubListScraper(IOdjfsClient odjfsClient, IListDocumentParser parser)
        {
            _odjfsClient = odjfsClient;
            _parser = parser;
        }

        public async Task<IEnumerable<ChildCareStub>> Scrape()
        {
            // fetch the contents
            ClientResponse response = await _odjfsClient.GetListDocument();
            ValidateClientResponse(response);

            // extract the information from the HTML
            return _parser.Parse(response.Content);
        }

        public async Task<IEnumerable<ChildCareStub>> Scrape(County county)
        {
            // fetch the contents
            ClientResponse response = await _odjfsClient.GetListDocument(county);
            ValidateClientResponse(response);

            // extract the information from the HTML
            return _parser.Parse(response.Content, county);
        }

        private void ValidateClientResponse(ClientResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var exception = new ScraperException("A status code that is not 200 was returned when getting the list document.");
                Logger.ErrorException(string.Format("RequestUri: '{0}', StatusCode: '{1}'", response.RequestUri, response.StatusCode), exception);
                throw exception;
            }
        }
    }
}