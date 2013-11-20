using System;
using System.Linq;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace OdjfsScraper.Test.Parsers.Support
{
    public class BaseDetailedChildCareHomeTemplate<T> : BaseChildCareTemplate<T> where T : DetailedChildCare
    {
        public BaseDetailedChildCareHomeTemplate(T childCare) : base(childCare)
        {
            childCare.ProviderAgreement = "ProviderAgreement";
            childCare.Administrators = "Adminitrators";
            childCare.CenterStatus = "CenterStatus";
            Model.InitialApplicationDate = "InitialApplicationDate";
            Model.LicenseBeginDate = "LicenseBeginDate";
            Model.LicenseExpirationDate = "LicenseExpirationDate";

            Model.SutqRating = 5;

            Model.Infants = true;
            Model.YoungToddlers = false;
            Model.OlderToddlers = true;
            Model.Preschoolers = false;
            Model.Gradeschoolers = true;

            Model.ChildCareFoodProgram = false;

            Model.Naeyc = true;
            Model.Necpa = false;
            Model.Naccp = true;
            Model.Nafcc = false;
            Model.Coa = true;
            Model.Acsi = false;

            Model.MondayReported = true;
            Model.MondayBegin = new DateTime(1970, 1, 1, 1, 0, 0);
            Model.MondayEnd = new DateTime(1970, 1, 1, 15, 0, 0);

            Model.TuesdayReported = true;
            Model.TuesdayBegin = new DateTime(1970, 1, 1, 2, 0, 0);
            Model.TuesdayEnd = new DateTime(1970, 1, 1, 16, 0, 0);

            Model.WednesdayReported = true;
            Model.WednesdayBegin = new DateTime(1970, 1, 1, 3, 0, 0);
            Model.WednesdayEnd = new DateTime(1970, 1, 1, 17, 0, 0);

            Model.ThursdayReported = true;
            Model.ThursdayBegin = new DateTime(1970, 1, 1, 4, 0, 0);
            Model.ThursdayEnd = new DateTime(1970, 1, 1, 18, 0, 0);

            Model.FridayReported = true;
            Model.FridayBegin = new DateTime(1970, 1, 1, 5, 0, 0);
            Model.FridayEnd = new DateTime(1970, 1, 1, 19, 0, 0);

            Model.SaturdayReported = true;
            Model.SaturdayBegin = new DateTime(1970, 1, 1, 6, 0, 0);
            Model.SaturdayEnd = new DateTime(1970, 1, 1, 20, 0, 0);

            Model.SundayReported = true;
            Model.SundayBegin = new DateTime(1970, 1, 1, 7, 0, 0);
            Model.SundayEnd = new DateTime(1970, 1, 1, 21, 0, 0);
        }

        public string CheckBooleanToString(bool value)
        {
            return value ? "<img src=\"Images/GreenMark.jpg\">" : "<img src=\"Images/EmptyBox.jpg\">";
        }

        public string DayTimesToString(bool reported, DateTime? beginTime, DateTime? endTime)
        {
            if (!reported)
            {
                return "NOT REPORTED";
            }
            if (!beginTime.HasValue || !endTime.HasValue)
            {
                return "CLOSED";
            }
            return string.Format("{0} to {1}", beginTime.Value.ToString("hh:mm tt"), endTime.Value.ToString("hh:mm tt"));
        }

        public string SutqRatingToString(int? sutqRating)
        {
            if (!sutqRating.HasValue)
            {
                return string.Empty;
            }
            return string.Concat(Enumerable.Repeat("<img src=\"smallredstar2.gif\">", sutqRating.Value));
        }
    }
}