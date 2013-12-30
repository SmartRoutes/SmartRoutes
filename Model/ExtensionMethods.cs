using System;

namespace SmartRoutes.Model
{
    public static class ExtensionMethods
    {
        private static double GetL1DistanceInFeet(double Lat1, double Lon1, double Lat2, double Lon2)
        {
            double R = 20925524.9; // radius of earth in feet
            double dx = R * Math.Cos(Lat1) * (Lon2 - Lon1) * Math.PI / 180;
            double dy = R * (Lat2 - Lat1) * Math.PI / 180;
            return Math.Abs(dx) + Math.Abs(dy);
        }

        public static double GetL1DistanceInFeet(this ILocation a, ILocation b)
        {
            return GetL1DistanceInFeet(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }
    }
}