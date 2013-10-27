using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;
using OdjfsScraper.Parsers;
using OdjfsScraper.Support;

namespace OdjfsScraper.Scrapers
{
    public class ChildCareStubListScraper : IChildCareStubListScraper
    {
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
            byte[] bytes = await _odjfsClient.GetListDocument();

            // extract the information from the HTML
            return _parser.Parse(bytes);
        }

        public async Task<IEnumerable<ChildCareStub>> Scrape(County county)
        {
            // fetch the contents
            byte[] bytes = await _odjfsClient.GetListDocument(county);

            // extract the information from the HTML
            return _parser.Parse(bytes, county);
        }
    }
}