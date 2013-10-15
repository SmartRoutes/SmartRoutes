using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsQuery;
using Model.Odjfs;
using Scraper;

namespace OdjfsHtmlScraper.Scrapers
{
    public class ListDocumentScraper : IScraper<CQ, IEnumerable<ChildCare>>
    {
        public Task<IEnumerable<ChildCare>> Scrape(CQ input)
        {
            throw new NotImplementedException();
        }
    }
}
