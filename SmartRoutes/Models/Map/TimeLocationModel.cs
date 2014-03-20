using System;
using SmartRoutes.Model;

namespace SmartRoutes.Models.Map
{
    public class TimeLocationModel : LocationModel
    {
        public TimeLocationModel()
        {
        }

        public TimeLocationModel(ILocation location, DateTime time) : base(location)
        {
            Time = time;
        }

        public DateTime Time { get; set; }
    }
}