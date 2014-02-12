using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class BusinessHours
    {
        public BusinessHours()
        {

        }

        public enum WeekDay
        {
            Monday = 0,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        }

        public WeekDay Day
        {
            get;
            set;
        }

        public string OpeningTime
        {
            get;
            set;
        }

        public string ClosingTime
        {
            get;
            set;
        }
    }
}