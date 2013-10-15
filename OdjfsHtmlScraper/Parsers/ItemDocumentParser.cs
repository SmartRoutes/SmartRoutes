using CsQuery;
using Model.Odjfs;
using Scraper;

namespace OdjfsHtmlScraper.Parsers
{
    public abstract class ItemDocumentParser<T> : IParser<CQ, T> where T : ChildCare
    {
        public abstract T Parse(CQ input);
    }
}
