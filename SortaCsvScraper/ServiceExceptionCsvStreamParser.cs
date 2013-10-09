using System;
using System.Collections.Generic;
using System.Globalization;
using Model.Sorta;
using Scraper;

namespace SortaCsvScraper
{
    public class ServiceExceptionCsvStreamParser : CsvStreamParser<ServiceException>
    {
        protected override ServiceException ConstructItem(IDictionary<string, string> values)
        {
            return new ServiceException
            {
                ServiceId = int.Parse(values["service_id"]),
                Date = ParseDate(values["date"]),
                ServiceExemptionTypeId = int.Parse(values["exception_type"])
            };
        }

        private DateTime ParseDate(string input)
        {
            return DateTime.ParseExact(input, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
    }
}