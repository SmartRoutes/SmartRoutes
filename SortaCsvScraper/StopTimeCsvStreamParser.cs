using System;
using System.Collections.Generic;
using System.Globalization;
using Model.Sorta;
using Scraper;

namespace SortaCsvScraper
{
    public class StopTimeCsvStreamParser : CsvStreamParser<StopTime>
    {
        protected override StopTime ConstructItem(IDictionary<string, string> values)
        {
            return new StopTime
            {
                TripId = int.Parse(values["trip_id"]),
                ArrivalTime = ParseTime(values["arrival_time"]),
                DepartureTime = ParseTime(values["departure_time"]),
                StopId = int.Parse(values["stop_id"]),
                Sequence = int.Parse(values["stop_sequence"]),
                Headsign = values["stop_headsign"],
                PickupType = ParseNullableInt(values["pickup_type"]),
                DropOffType = ParseNullableInt(values["drop_off_type"]),
                ShapeDistanceTraveled = ParseNullableDouble(values["shape_dist_traveled"])
            };
        }

        private DateTime ParseTime(string input)
        {
            return DateTime.ParseExact(input, "ggmmss", CultureInfo.InvariantCulture);
        }
    }
}