using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model describing the business hours for a particular day.
    /// </summary>
    public class BusinessHoursModel
    {
        public BusinessHoursModel()
        {

        }

        /// <summary>
        /// Enumeration for the days of the week.
        /// </summary>
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

        /// <summary>
        /// Indicates the day of the week the hours refer to.
        /// </summary>
        public WeekDay Day
        {
            get;
            set;
        }

        /// <summary>
        /// The opening time for the business.
        /// </summary>
        public string OpeningTime
        {
            get;
            set;
        }

        /// <summary>
        /// The closing time for the business.
        /// </summary>
        public string ClosingTime
        {
            get;
            set;
        }
    }
}