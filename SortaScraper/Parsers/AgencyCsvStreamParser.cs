using System.Collections.Generic;
using SmartRoutes.Model.Sorta;
using Scraper;

namespace SortaScraper.Parsers
{
    public class AgencyCsvStreamParser : CsvStreamParser<Agency>
    {
        protected override Agency ConstructItem(IDictionary<string, string> values)
        {
            return new Agency
            {
                Id = values["agency_id"],
                Name = values["agency_name"],
                Url = values["agency_url"],
                Timezone = values["agency_timezone"],
                Language = values["agency_lang"],
                Phone = values["agency_phone"],
                FareUrl = values["agency_fare_url"]
            };
        }
    }
}