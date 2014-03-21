using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyGeocoder.Support;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using Location = SmartRoutes.Model.Location;

namespace SmartRoutes.Support
{
    public static class ExtensionMethods
    {
        public static async Task<ILocation> GetLocationOrNull(this ISimpleGeocoder geocoder, IDictionary<string, ILocation> destinations, string request)
        {
            // try to get a previous response
            ILocation location;
            if (destinations.TryGetValue(request, out location))
            {
                return location;
            }

            // geocode the address and save the result to the dictionary
            Response response = await geocoder.GeocodeAsync(request).ConfigureAwait(false);
            PolyGeocoder.Support.Location geocodedLocation = response.Locations.FirstOrDefault();
            if (geocodedLocation != null)
            {
                location = new Location
                {
                    Latitude = geocodedLocation.Latitude,
                    Longitude = geocodedLocation.Longitude
                };
            }
            else
            {
                return null;
            }
            destinations[request] = location;

            return location;
        }

        public static Location GetLocation<T>(this T location) where T : ILocation
        {
            return new Location
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }

        public static IEnumerable<Stop> GetStopsBetween(this StopTime start, StopTime end)
        {
            // validate the parameters
            if (start.TripId != end.TripId)
            {
                throw new ArgumentException(string.Format(
                    "The TripId '{0}' on the provided end StopTime does not match the start StopTime's TripId '{1}'.",
                    start.TripId,
                    end.TripId));
            }
            if (start.Trip == null || start.Trip.Shape == null)
            {
                return Enumerable.Empty<Stop>();
            }

            // pull out the shape points associated with these stop times, and make sure they are in order
            return start
                .Trip
                .StopTimes
                .OrderBy(s => s.Sequence)
                .Select(s => s.Stop)
                .ToArray()
                .GetLocationsBetween(start.Stop, end.Stop);
        }

        public static IEnumerable<ShapePoint> GetShapePointsBetween(this StopTime start, StopTime end)
        {
            // validate the parameters
            if (start.TripId != end.TripId)
            {
                throw new ArgumentException(string.Format(
                    "The TripId '{0}' on the provided end StopTime does not match the start StopTime's TripId '{1}'.",
                    start.TripId,
                    end.TripId));
            }
            if (start.Trip == null || start.Trip.Shape == null)
            {
                return Enumerable.Empty<ShapePoint>();
            }

            return start
                .Trip
                .Shape
                .ShapePoints
                .OrderBy(s => s.Sequence)
                .ToArray()
                .GetLocationsBetween(start.Stop, end.Stop);
        }

        private static IEnumerable<T> GetLocationsBetween<T>(this IList<T> points, ILocation start, ILocation end) where T : ILocation
        {
            // find the closest shape point to both the start and end stop
            int closestToStart = -1;
            double minDistanceToStart = double.MaxValue;
            int closestToEnd = -1;
            double minDistanceToEnd = double.MaxValue;

            for (int i = 0; i < points.Count; i++)
            {
                double distanceToStart = points[i].GetL1DistanceInFeet(start);
                if (distanceToStart < minDistanceToStart)
                {
                    minDistanceToStart = distanceToStart;
                    closestToStart = i;
                }

                double distanceToEnd = points[i].GetL1DistanceInFeet(end);
                if (distanceToEnd < minDistanceToEnd)
                {
                    minDistanceToEnd = distanceToEnd;
                    closestToEnd = i;
                }
            }

            // make sure closestToStart has the lower sequence number
            if (closestToStart > closestToEnd)
            {
                int temp = closestToStart;
                closestToStart = closestToEnd;
                closestToEnd = temp;
            }

            return points
                .Skip(closestToStart - 1)
                .Take(closestToEnd - closestToStart)
                .ToArray();
        }
    }
}