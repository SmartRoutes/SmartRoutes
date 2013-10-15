using System;

namespace Model.Odjfs
{
    public abstract class DetailedChildCare : ChildCare
    {
        #region HTML

        public bool ProviderAgreement { get; set; }
        public string Administrator { get; set; }
        public string CenterStatus { get; set; }
        public DateTime InitialApplicationDate { get; set; }
        public DateTime LicenseBeginDate { get; set; }
        public DateTime LicenseEndDate { get; set; }

        public int? SutqRating { get; set; }

        public bool Infants { get; set; }
        public bool YoungToddlers { get; set; }
        public bool OlderToddlers { get; set; }
        public bool Preschoolers { get; set; }
        public bool Gradeschoolers { get; set; }

        public bool ChildCareFoodProgram { get; set; }

        public bool Naeyc { get; set; }
        public bool Necpa { get; set; }
        public bool Naccp { get; set; }
        public bool Coa { get; set; }
        public bool Acsi { get; set; }

        public DateTime? MondayBegin { get; set; }
        public DateTime? MondayEnd { get; set; }

        public DateTime? TuesdayBegin { get; set; }
        public DateTime? TuesdayEnd { get; set; }

        public DateTime? WednesdayBegin { get; set; }
        public DateTime? WednesdayEnd { get; set; }

        public DateTime? ThursdayBegin { get; set; }
        public DateTime? ThursdayEnd { get; set; }

        public DateTime? FridayBegin { get; set; }
        public DateTime? FridayEnd { get; set; }

        public DateTime? SaturdayBegin { get; set; }
        public DateTime? SaturdayEnd { get; set; }

        public DateTime? SundayBegin { get; set; }
        public DateTime? SundayEnd { get; set; }

        #endregion
    }
}