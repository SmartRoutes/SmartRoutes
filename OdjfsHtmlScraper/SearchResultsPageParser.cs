using System.Collections.Generic;
using System.Linq;
using CsQuery;
using Model.Odjfs;
using NLog;
using Scraper;

namespace OdjfsHtmlScraper
{
    public class SearchResultsPageParser : IParser<CQ, IEnumerable<ChildCare>>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IParser<IDomElement, ChildCare> _searchResultsRowParser;

        public SearchResultsPageParser(IParser<IDomElement, ChildCare> searchResultsRowParser)
        {
            _searchResultsRowParser = searchResultsRowParser;
        }

        public IEnumerable<ChildCare> Parse(CQ document)
        {
            // select the table
            CQ table = document["table"];
            if (table.Length != 1)
            {
                var exception = new ParserException("Exactly one table on the search results page is expected.");
                Logger.ErrorException(string.Format("Expected: 1, Actual: {0}, HTML:\n{1}", table.Length, document.Document.OuterHTML), exception);
                throw exception;
            }

            // select all of the rows in the table
            CQ rows = table["tr"];

            // parse all but the first row (which is the header row)
            return rows.Elements.Skip(1).Select(row => _searchResultsRowParser.Parse(row));
        }
    }
}