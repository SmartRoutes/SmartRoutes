using System;

namespace SmartRoutes.GeocoderComparisonLoader
{
    public class GeocodedData : Tuple<double, double, DateTime>
    {
        public GeocodedData(double latitude, double longitude, DateTime lastGeocodedOn) : base(latitude, longitude, lastGeocodedOn)
        {
        }

        public double Latitude
        {
            get { return Item1; }
        }

        public double Longitude
        {
            get { return Item2; }
        }

        public DateTime LastGeocodedOn
        {
            get { return Item3; }
        }
    }
}