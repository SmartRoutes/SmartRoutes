using System.Collections.Generic;
using System.Threading.Tasks;
using CsQuery;
using Model.Odjfs;
using OdjfsHtmlScraper.Support;
using Scraper;

namespace OdjfsHtmlScraper.Scrapers
{
    public class ListDocumentScraper : IScraper<IEnumerable<ChildCare>>
    {
        private readonly IClient _client;
        private readonly IParser<CQ, IEnumerable<ChildCare>> _parser;

        public ListDocumentScraper(IClient client, IParser<CQ, IEnumerable<ChildCare>> parser)
        {
            _client = client;
            _parser = parser;
        }

        public async Task<IEnumerable<ChildCare>> Scrape()
        {
            // fetch and parse the HTML
            CQ document = await _client.GetListDocument();

            // extract the information from the HTML
            return _parser.Parse(document);
        }
    }
}