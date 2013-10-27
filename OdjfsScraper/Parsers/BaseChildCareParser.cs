using System.Collections.Generic;
using System.Linq;
using CsQuery;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using NLog;
using OdjfsScraper.Support;
using Scraper;

namespace OdjfsScraper.Parsers
{
    public class BaseChildCareParser<T> : AbstractChildCareParser<T> where T : ChildCare
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override IEnumerable<KeyValuePair<string, string>> GetDetailKeyValuePairs(CQ document)
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
            return table
                .GetDescendentElements() //                                                    1. get all descendent elements
                .Where(e => e.NodeName == "TR") //                                             2. exclude non-row elements
                .Where(r => r.GetDescendentElements().All(child => child.NodeName != "TR")) // 3. exclude elements that do not themselves have child TR elements
                .Select(r => r.GetCollapsedInnerText()) //                                     4. extract all of the text from the row
                .Select(ParseColonSeperatedString); //                                         5. Parse the colon seperated strings
        }

        protected override void PopulateFields(T childCare, IDictionary<string, string> details)
        {
            // fill in fields shared by all subclasses
            // TODO: verify Type string is the expected value per typeof(T)
            childCare.ExternalId = GetDetailString(details, "Number");
            childCare.Name = GetDetailString(details, "Name");
            childCare.City = GetDetailString(details, "City");
            childCare.State = GetDetailString(details, "State");
            childCare.ZipCode = int.Parse(GetDetailString(details, "Zip"));
            childCare.PhoneNumber = GetDetailString(details, "Phone");
            childCare.County = new County {Name = GetDetailString(details, "County")};
        }

        #region Helpers

        private static KeyValuePair<string, string> ParseColonSeperatedString(string input)
        {
            // make sure there is a colon and a key
            input = input.Trim();
            if (!input.Contains(":") || input.StartsWith(":"))
            {
                return default(KeyValuePair<string, string>);
            }

            // split by the colon
            string[] tokens = input.Split(':');
            string key = tokens[0].Trim();
            string value = tokens[1].Trim();

            // coalesce empty strings to null
            value = value == string.Empty ? null : value;

            return new KeyValuePair<string, string>(key, value);
        }

        #endregion
    }
}