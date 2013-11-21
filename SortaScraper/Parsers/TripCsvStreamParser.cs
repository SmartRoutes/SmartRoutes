using System.Collections.Generic;
using SmartRoutes.Model.Sorta;
using SmartRoutes.Scraper;

namespace SmartRoutes.SortaScraper.Parsers
{
    public class TripCsvStreamParser : CsvStreamParser<Trip>
    {
        protected override Trip ConstructItem(IDictionary<string, string> values)
        {
            return new Trip
            {
                Id = int.Parse(values["trip_id"]),
                RouteId = int.Parse(values["route_id"]),
                ServiceId = int.Parse(values["service_id"]),
                Headsign = values["trip_headsign"],
                ShortName = values["trip_short_name"],
                DirectionId = ParseNullableInt(values["direction_id"]),
                BlockId = ParseNullableInt(values["block_id"]),
                ShapeId = ParseNullableInt(values["shape_id"])
            };
        }
    }
}