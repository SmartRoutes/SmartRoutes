using SmartRoutes.Model;

namespace SmartRoutes.Models.Map
{
    public class LocationModel
    {
        public LocationModel()
        {
        }

        public LocationModel(ILocation location)
        {
            Latitude = location.Latitude;
            Longitude = location.Longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}