using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsQuery;
using Model.Odjfs;
using NLog;
using Scraper;

namespace OdjfsHtmlScraper.Parsers
{
    public class ListDocumentParser : IListDocumentParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<ChildCare> Parse(byte[] bytes)
        {
            // parse the HTML
            CQ document = CQ.Create(new MemoryStream(bytes));

            // select the table
            CQ table = document["table"];
            if (table.Length != 1)
            {
                var exception = new ParserException("Exactly one table on the search results page is expected.");
                Logger.ErrorException(string.Format("Expected: 1, Actual: {0}", table.Length), exception);
                throw exception;
            }

            // select all of the relevant rows in the table
            IEnumerable<IDomElement> rows = table["tr"]
                .Elements
                .Where((e, i) => i%2 == 0) // every other row is empty...
                .Skip(1); // the first two is for the header

            // parse the rows using the child parser
            return rows.Select(ParseRow);
        }

        private ChildCare ParseRow(IDomElement element)
        {
            // get all of the cells
            IDomElement[] cells = element.ChildElements.ToArray();
            if (cells.Length != 24)
            {
                var exception = new ParserException("Exactly 24 cells in each search result row is expected.");
                Logger.ErrorException(string.Format("Expected: 24, Actual: {0}, HTML:\n{1}", cells.Length, element.OuterHTML), exception);
                throw exception;
            }

            // TODO: parse the "Program Type Codes" key at the top of the page to make this more generic
            // get the type
            ChildCare childCare;
            string typeCode = cells[14].InnerText.Trim();
            switch (typeCode)
            {
                case "A":
                    childCare = new TypeAHome();
                    break;
                case "B":
                    childCare = new TypeBHome();
                    break;
                case "C":
                    childCare = new LicensedCenter();
                    break;
                case "D":
                    childCare = new DayCamp();
                    break;
                default:
                    var exception = new ParserException("An unexpected child care type code was found.");
                    Logger.ErrorException(string.Format("TypeCode: {0}, HTML:\n{1}", typeCode, element.OuterHTML), exception);
                    throw exception;
            }

            // get the link containing URL number
            var nameLink = (IHTMLAnchorElement) cells[2].FirstElementChild;

            // parse the URL number out of the URL
            Match match = Regex.Match(nameLink.Href, @"^results2\.asp\?provider_number=(?<ExternalUrlId>[A-Z]{18})$");
            if (!match.Success)
            {
                var exception = new ParserException("The child care link URL was not in the expected format.");
                Logger.ErrorException(string.Format("HREF: {0}, HTML:\n{1}", nameLink.Href, element.OuterHTML), exception);
                throw exception;
            }
            childCare.ExternalUrlId = match.Groups["ExternalUrlId"].Value;

            // parse out the name
            childCare.Name = nameLink.InnerText.Trim();

            // parse out the name
            childCare.City = cells[10].InnerText.Trim();

            // TODO: parse out the address and rating

            return childCare;
        }
    }
}