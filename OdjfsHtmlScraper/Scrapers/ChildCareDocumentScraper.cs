using System;
using System.Threading.Tasks;
using Model.Odjfs;
using OdjfsHtmlScraper.Support;
using Scraper;

namespace OdjfsHtmlScraper.Scrapers
{
    public class ChildCareDocumentScraper : IScraper<ChildCare, ChildCare>
    {
        private readonly IClient _client;

        public ChildCareDocumentScraper(IClient client)
        {
            _client = client;
        }

        public Task<ChildCare> Scrape(ChildCare childCare)
        {
            throw new NotImplementedException();
        }
    }
}