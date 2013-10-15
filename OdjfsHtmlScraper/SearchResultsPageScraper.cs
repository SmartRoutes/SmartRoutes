using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using Model.Odjfs;
using Scraper;

namespace OdjfsHtmlScraper
{
    public class SearchResultsPageScraper : IScraper<CQ, IEnumerable<ChildCare>>
    {
        public Task<IEnumerable<ChildCare>> Scrape(CQ input)
        {
            throw new NotImplementedException();
        }
    }
}
