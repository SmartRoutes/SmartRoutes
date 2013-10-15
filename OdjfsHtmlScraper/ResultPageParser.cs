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
    public abstract class SearchResultsPageParser<T> : IParser<CQ, T> where T : ChildCare
    {
        public abstract T Parse(CQ input);
    }
}
