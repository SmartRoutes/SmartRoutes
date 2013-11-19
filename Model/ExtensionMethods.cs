using System;

namespace Model
{
    public static class ExtensionMethods
    {
        private static double GetL1DistanceInFeet(double Lat1, double Lon1, double Lat2, double Lon2)
        {
            double R = 20092000.0; // radius of earth in feet
            double dx = R*Math.Cos(Lat1)*(Lon2 - Lon1);
            double dy = R*(Lat2 - Lat1);
            return Math.Abs(dx) + Math.Abs(dy);
        }

        public static double GetL1DistanceInFeet(this ILocation a, ILocation b)
        {
            return GetL1DistanceInFeet(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        public static double GetL1DistanceInFeet(this ILocation a, INullableLocation b)
        {
            return GetL1DistanceInFeet(a.Latitude, a.Longitude, b.Latitude.Value, b.Longitude.Value);
        }

        public static double GetL1DistanceInFeet(this INullableLocation a, ILocation b)
        {
            return GetL1DistanceInFeet(a.Latitude.Value, a.Longitude.Value, b.Latitude, b.Longitude);
        }

        public static double GetL1DistanceInFeet(this INullableLocation a, INullableLocation b)
        {
            return GetL1DistanceInFeet(a.Latitude.Value, a.Longitude.Value, b.Latitude.Value, b.Longitude.Value);
        }
    }
}