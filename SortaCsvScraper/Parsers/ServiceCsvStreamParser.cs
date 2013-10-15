using System;
using System.Collections.Generic;
using System.Globalization;
using Model.Sorta;
using Scraper;

namespace SortaCsvScraper.Parsers
{
    public class ServiceCsvStreamParser : CsvStreamParser<Service>
    {
        protected override Service ConstructItem(IDictionary<string, string> values)
        {
            return new Service
            {
                Id = int.Parse(values["service_id"]),
                Monday = int.Parse(values["monday"]) != 0,
                Tuesday = int.Parse(values["tuesday"]) != 0,
                Wednesday = int.Parse(values["wednesday"]) != 0,
                Thursday = int.Parse(values["thursday"]) != 0,
                Friday = int.Parse(values["friday"]) != 0,
                Saturday = int.Parse(values["saturday"]) != 0,
                Sunday = int.Parse(values["sunday"]) != 0,
                StartDate = ParseDate(values["start_date"]),
                EndDate = ParseDate(values["end_date"])
            };
        }

        private DateTime ParseDate(string input)
        {
            return DateTime.ParseExact(input, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
    }
}