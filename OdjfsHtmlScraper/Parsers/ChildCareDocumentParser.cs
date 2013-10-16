using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CsQuery;
using CsQuery.Implementation;
using Model.Odjfs;
using NLog;
using Scraper;

namespace OdjfsHtmlScraper.Parsers
{
    public abstract class ChildCareDocumentParser<T> : IParser<CQ, T> where T : ChildCare
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public T Parse(CQ document)
        {
            // parse the first and second tables
            string[][] detailArrays = GetFirstTableDetailArrays(document)
                .Concat(GetSecondTableDetails(document))
                .Select(t => new[] {t[0], t[1] != string.Empty ? t[1] : null}) // coalesce empty strings to null 
                .ToArray();

            // make sure the keys are unique
            string[] keys = detailArrays.Select(tokens => tokens[0]).ToArray();
            string[] uniqueKeys = keys.Distinct().ToArray();
            if (keys.Length != uniqueKeys.Length)
            {
                var exception = new ParserException("All keys in the first details table must be unique.");
                Logger.ErrorException(string.Format("Type: '{0}', OriginalKeys: {1}", typeof (T).Name, string.Join(", ", keys)), exception);
                throw exception;
            }

            // create the dictionary
            IDictionary<string, string> details = detailArrays.ToDictionary(t => t[0], t => t[1]);

            // generate the concrete object using the child implementation
            return GetChildCareInstance(details);
        }

        protected abstract T PopulateFields(T childCare, IDictionary<string, string> details);

        private T GetChildCareInstance(IDictionary<string, string> details)
        {
            var childCare = Activator.CreateInstance<T>();

            // fill in fields shared by all subclasses
            childCare.Type = details["Type"];
            childCare.ExternalId = details["Number"];
            childCare.Name = details["Name"];
            // Address is excluded because not all addresses are available on the ODJFS website
            // childCare.Address = details["Address"]
            childCare.City = details["City"];
            childCare.State = details["State"];
            childCare.ZipCode = int.Parse(details["Zip"]);
            childCare.County = details["County"];
            childCare.PhoneNumber = details["Phone"];

            return PopulateFields(childCare, details);
        }

        private IEnumerable<string[]> GetFirstTableDetailArrays(CQ document)
        {
            // get the table
            IDomElement table = document["#PageContent table:first"].FirstElement();
            if (table == null)
            {
                var exception = new ParserException("No Program Details table was found.");
                Logger.ErrorException(string.Format("Type: '{0}'", typeof (T).Name), exception);
                throw exception;
            }

            // replace all of the images with text
            ReplaceImagesWithText(table, new Dictionary<string, string>
                                         {
                                             {"smallredstar2.gif", "*"},
                                             {"http://jfs.ohio.gov/_assets/images/web_graphics/common/spacer.gif", string.Empty},
                                         });

            // get all of the text fields in the first details table
            string[][] detailArrays = table
                .GetDescendentElements() //                                                        1. get all descendent elements
                .Where(e => e.NodeName == "TR") //                                                 2. exclude non-row elements
                .Where(row => row.GetDescendentElements().All(child => child.NodeName != "TR")) // 3. exclude elements that do not themselves have child TR elements
                .Select(GetInnerText) //                                                           4. extract all of the text from the row
                .Where(text => text != string.Empty) //                                            5. exclude empty rows
                .Select(text => text.Split(new[] {':'}, 2)) //                                     6. split by the first colon
                .Select(tokens => tokens.Select(s => s.Trim()).ToArray()) //                       7. trim the tokens
                .ToArray(); //                                                                     8. convert to an array

            // make sure that there every detail is a 2-tuple (it was delimited by a colon)
            foreach (var detailArray in detailArrays)
            {
                if (detailArray.Length == 1)
                {
                    var exception = new ParserException("A detail was not colon seperated.");
                    Logger.ErrorException(string.Format("Type: '{0}', String: '{1}'", typeof (T).Name, detailArray[0]), exception);
                    throw exception;
                }
            }

            return detailArrays;
        }

        private IEnumerable<string[]> GetSecondTableDetails(CQ document)
        {
            // get the table
            IDomElement table = document["#providerinfo"].FirstElement();
            if (table == null)
            {
                return Enumerable.Empty<string[]>();
            }

            // replace all of the images with text
            ReplaceImagesWithText(table, new Dictionary<string, string>
                                         {
                                             {"Images/EmptyBox.jpg", "false"},
                                             {"Images/GreenMark.jpg", "true"},
                                         });

            // get the useful rows in the table                     
            return table
                .GetDescendentElements() //                                                   1. get all descendent elements
                .Where(e => e.NodeName == "TR") //                                            2. exclude non-row elements
                .Select(row => row.ChildElements.Where(e => e.NodeName == "TD").ToArray()) // 3. get all child cells
                .Where(cells => cells.Length == 6) //                                         4. exclude rows without 6 cells
                .SelectMany(GetTokensFromCells) //                                            5. re-arrange the text in the cells
                .Where(t => t[0] != string.Empty && t[1] != string.Empty); //                 6. exclude empty token pairs
        }

        private string GetInnerText(IDomElement e)
        {
            // get the text content
            string text = e.TextContent;

            // decode the entities
            text = WebUtility.HtmlDecode(text);

            // collapse the whitespace
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private IEnumerable<string[]> GetTokensFromCells(IDomElement[] cells)
        {
            // get the text from each cell
            string[] tokens = cells.Select(GetInnerText).ToArray();

            // arrange the tokens as key value pairs
            return new[]
                   {
                       new[] {tokens[1], tokens[0]},
                       new[] {tokens[3], tokens[2]},
                       new[] {tokens[4], tokens[5]}
                   };
        }

        private void ReplaceImagesWithText(IDomElement parent, IDictionary<string, string> replacements)
        {
            // replace all of the images with text
            IDomElement[] images = parent
                .GetDescendentElements()
                .Where(e => e.NodeName == "IMG")
                .ToArray();
            foreach (IDomElement image in images)
            {
                string text;
                if (!replacements.TryGetValue(image.GetAttribute("src"), out text))
                {
                    text = "unknown";
                }
                image.ParentNode.InsertAfter(new DomText(text), image);
                image.Remove();
            }
        }

        protected DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
    }
}