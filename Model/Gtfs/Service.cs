using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartRoutes.Model.Gtfs
{
    public class Service
    {
        private ICollection<ServiceException> _serviceExceptions;

        private ICollection<Trip> _trips;

        public Service()
        {
            _serviceExceptions = new Collection<ServiceException>();
            _trips = new Collection<Trip>();
        }

        #region Navigation Properties

        public virtual ICollection<Trip> Trips
        {
            get { return _trips; }
            set { _trips = value; }
        }

        public virtual ICollection<ServiceException> ServiceExceptions
        {
            get { return _serviceExceptions; }
            set { _serviceExceptions = value; }
        }

        #endregion

        #region CSV

        public int Id { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        #endregion
    }
}