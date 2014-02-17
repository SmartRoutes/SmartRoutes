using System;
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

        public virtual DateTime? SundayEnd
        {
            get { return null; }
        }

        public virtual DateTime? SundayBegin
        {
            get { return null; }
        }

        public virtual bool SundayReported
        {
            get { return false; }
        }

        public virtual DateTime? SaturdayBegin
        {
            get { return null; }
        }

        public virtual bool SaturdayReported
        {
            get { return false; }
        }

        public virtual DateTime? SaturdayEnd
        {
            get { return null; }
        }

        public virtual DateTime? FridayEnd
        {
            get { return null; }
        }

        public virtual DateTime? FridayBegin
        {
            get { return null; }
        }

        public virtual bool FridayReported
        {
            get { return false; }
        }

        public virtual DateTime? ThursdayEnd
        {
            get { return null; }
        }

        public virtual DateTime? ThursdayBegin
        {
            get { return null; }
        }

        public virtual bool ThursdayReported
        {
            get { return false; }
        }

        public virtual DateTime? WednesdayEnd
        {
            get { return null; }
        }

        public virtual DateTime? TuesdayEnd
        {
            get { return null; }
        }

        public virtual DateTime? TuesdayBegin
        {
            get { return null; }
        }

        public virtual bool TuesdayReported
        {
            get { return false; }
        }

        public virtual bool WednesdayReported
        {
            get { return false; }
        }

        public virtual DateTime? WednesdayBegin
        {
            get { return null; }
        }

        public virtual DateTime? MondayEnd
        {
            get { return null; }
        }

        public virtual DateTime? MondayBegin
        {
            get { return null; }
        }

        public virtual bool MondayReported
        {
            get { return false; }
        }
    }
}