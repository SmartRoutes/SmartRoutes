using System;
using System.Collections.Generic;
using System.Globalization;
using Model.Sorta;
using Scraper;

namespace SortaCsvScraper
{
    public class ServiceCsvStreamParser : CsvStreamParser<Service>
    {
        protected override Service ConstructItem(IDictionary<string, string> values)
        {
            return new Service
            {
                Id = int.Parse(values["service_id"]),
                Monday = bool.Parse(values["monday"]),
                Tuesday = bool.Parse(values["tuesday"]),
                Wednesday = bool.Parse(values["wednesday"]),
                Thursday = bool.Parse(values["thursday"]),
                Friday = bool.Parse(values["friday"]),
                Saturday = bool.Parse(values["saturday"]),
                Sunday = bool.Parse(values["sunday"]),
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