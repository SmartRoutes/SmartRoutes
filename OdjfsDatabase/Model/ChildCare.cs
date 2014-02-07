using SmartRoutes.Model;

namespace SmartRoutes.Demo.OdjfsDatabase.Model
{
    public class ChildCare : IDestination
    {
        private readonly OdjfsScraper.Model.ChildCares.ChildCare _model;

        public ChildCare(OdjfsScraper.Model.ChildCares.ChildCare model)
        {
            _model = model;
        }

        public string CountyName
        {
            get { return _model.County.Name; }
        }

        public string ExternalUrlId
        {
            get { return _model.ExternalUrlId; }
        }

        public string City
        {
            get { return _model.City; }
        }

        public string Address
        {
            get { return _model.Address; }
        }

        public string State
        {
            get { return _model.State; }
        }

        public int ZipCode
        {
            get { return _model.ZipCode; }
        }

        public string PhoneNumber
        {
            get { return _model.PhoneNumber; }
        }

        public double Latitude
        {
            get { return _model.Latitude.Value; }
        }

        public double Longitude
        {
            get { return _model.Longitude.Value; }
        }

        public string Name
        {
            get { return _model.Name; }
        }
    }
}