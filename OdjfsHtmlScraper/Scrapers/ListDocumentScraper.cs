using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Odjfs;
using OdjfsHtmlScraper.Parsers;
using OdjfsHtmlScraper.Support;

namespace OdjfsHtmlScraper.Scrapers
{
    public class ListDocumentScraper : IListDocumentScraper
    {
        private readonly IClient _client;
        private readonly IListDocumentParser _parser;

        public ListDocumentScraper(IClient client, IListDocumentParser parser)
        {
            _client = client;
            _parser = parser;
        }

        public async Task<IEnumerable<ChildCare>> Scrape()
        {
            // fetch the contents
            byte[] bytes = await _client.GetListDocument();

            // extract the information from the HTML
            return _parser.Parse(bytes);
        }
    }
}