using System;
using System.Collections.Generic;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader.Parsers;

namespace SmartRoutes.GtfsReader.Parsers
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
            string[] pieces = input.Split(':');
            if (pieces.Length != 3)
            {
                throw new ArgumentException("The provided time did not have two colon as seperator charactors.");
            }
            var intPieces = new int[3];
            for (int i = 0; i < 3; i++) // <3
            {
                if (!int.TryParse(pieces[i], out intPieces[i]) || intPieces[i] < 0)
                {
                    throw new ArgumentException("The provided time did not have three positive integers.");
                }
            }

            return new DateTime(1970, 1, 1)
                .AddHours(intPieces[0])
                .AddMinutes(intPieces[1])
                .AddSeconds(intPieces[2]);
        }
    }
}