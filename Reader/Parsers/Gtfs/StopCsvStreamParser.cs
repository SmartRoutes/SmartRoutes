using System.Collections.Generic;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader.Parsers;

namespace SmartRoutes.Reader.Parsers.Gtfs
{
    public class StopCsvStreamParser : CsvStreamParser<Stop>
    {
        protected override Stop ConstructItem(IDictionary<string, string> values)
        {
            return new Stop
            {
                Id = int.Parse(values["stop_id"]),
                Code = values["stop_code"],
                Name = values["stop_name"],
                Latitude = double.Parse(values["stop_lat"]),
                Longitude = double.Parse(values["stop_lon"]),
                ZoneId = ParseNullableInt(values["zone_id"]),
                Url = values["stop_url"],
                TypeId = ParseNullableInt(values["location_type"]),
                ParentId = ParseNullableInt(values["location_type"]),
                Timezone = values["stop_url"],
                WheelchairBoarding = ParseNullableInt(values["wheelchair_boarding"])
            };
        }
    }
}