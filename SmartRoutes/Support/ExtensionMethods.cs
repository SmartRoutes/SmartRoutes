using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyGeocoder.Support;
using SmartRoutes.Model;
using Location = PolyGeocoder.Support.Location;

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
            Location geocodedLocation = response.Locations.FirstOrDefault();
            if (geocodedLocation != null)
            {
                location = new Model.Location
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
    }
}