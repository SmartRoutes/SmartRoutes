using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.Sorta
{
    public class Block
    {
        private ICollection<Trip> _trips;

        public Block()
        {
            _trips = new Collection<Trip>();
        }

        public int Id { get; set; }

        #region Navigation Properties

        public virtual ICollection<Trip> Trips
        {
            get { return _trips; }
            set { _trips = value; }
        }

        #endregion
    }
}