﻿using System.Collections.Generic;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader.Parsers;

namespace SmartRoutes.Reader.Parsers.Gtfs
{
    public class ShapePointCsvStreamParser : CsvStreamParser<ShapePoint>
    {
        protected override ShapePoint ConstructItem(IDictionary<string, string> values)
        {
            return new ShapePoint
            {
                ShapeId = int.Parse(values["shape_id"]),
                Latitude = double.Parse(values["shape_pt_lat"]),
                Longitude = double.Parse(values["shape_pt_lon"]),
                Sequence = int.Parse(values["shape_pt_sequence"]),
                DistanceTraveled = ParseNullableDouble(values["shape_dist_traveled"])
            };
        }
    }
}