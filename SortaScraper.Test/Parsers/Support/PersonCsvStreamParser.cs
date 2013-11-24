using System.Collections.Generic;
using SmartRoutes.Scraper;

namespace SmartRoutes.SortaScraper.Test.Parsers
{
    public class PersonCsvStreamParser : CsvStreamParser<Person>
    {
        protected override Person ConstructItem(IDictionary<string, string> values)
        {
            return new Person
            {
                Name = values["Name"],
                FavoriteColor = values["FavoriteColor"]
            };
        }
    }
}