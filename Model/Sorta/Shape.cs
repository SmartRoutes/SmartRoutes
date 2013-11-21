using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartRoutes.Model.Sorta
{
    public class Shape
    {
        private ICollection<ShapePoint> _shapePoints;

        private ICollection<Trip> _trips;

        public Shape()
        {
            _shapePoints = new Collection<ShapePoint>();
            _trips = new Collection<Trip>();
        }

        public int Id { get; set; }

        #region Navigation Properties

        public virtual ICollection<Trip> Trips
        {
            get { return _trips; }
            set { _trips = value; }
        }

        public virtual ICollection<ShapePoint> ShapePoints
        {
            get { return _shapePoints; }
            set { _shapePoints = value; }
        }

        #endregion
    }
}