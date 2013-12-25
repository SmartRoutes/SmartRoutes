using System.Collections.Generic;
using SmartRoutes.Reader.Parsers;

namespace SmartRoutes.Reader.UnitTests.Parsers.TestSupport
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