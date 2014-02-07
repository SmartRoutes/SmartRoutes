using System;

namespace SmartRoutes.Demo.OdjfsDatabase.Model
{
    public class DetailedChildCare : ChildCare
    {
        private readonly OdjfsScraper.Model.ChildCares.DetailedChildCare _model;

        public DetailedChildCare(OdjfsScraper.Model.ChildCares.DetailedChildCare model) : base(model)
        {
            _model = model;
        }

        public DateTime? SundayEnd
        {
            get { return _model.SundayEnd; }
        }

        public DateTime? SundayBegin
        {
            get { return _model.SundayBegin; }
        }

        public bool SundayReported
        {
            get { return _model.SundayReported; }
        }

        public DateTime? SaturdayBegin
        {
            get { return _model.SaturdayBegin; }
        }

        public bool SaturdayReported
        {
            get { return _model.SaturdayReported; }
        }

        public DateTime? SaturdayEnd
        {
            get { return _model.SaturdayEnd; }
        }

        public DateTime? FridayEnd
        {
            get { return _model.FridayEnd; }
        }

        public DateTime? FridayBegin
        {
            get { return _model.FridayBegin; }
        }

        public bool FridayReported
        {
            get { return _model.FridayReported; }
        }

        public DateTime? ThursdayEnd
        {
            get { return _model.ThursdayEnd; }
        }

        public DateTime? ThursdayBegin
        {
            get { return _model.ThursdayBegin; }
        }

        public bool ThursdayReported
        {
            get { return _model.ThursdayReported; }
        }

        public DateTime? WednesdayEnd
        {
            get { return _model.WednesdayEnd; }
        }

        public DateTime? TuesdayEnd
        {
            get { return _model.TuesdayEnd; }
        }

        public DateTime? TuesdayBegin
        {
            get { return _model.TuesdayBegin; }
        }

        public bool TuesdayReported
        {
            get { return _model.TuesdayReported; }
        }

        public bool WednesdayReported
        {
            get { return _model.WednesdayReported; }
        }

        public DateTime? WednesdayBegin
        {
            get { return _model.WednesdayBegin; }
        }

        public DateTime? MondayEnd
        {
            get { return _model.MondayEnd; }
        }

        public DateTime? MondayBegin
        {
            get { return _model.MondayBegin; }
        }

        public bool MondayReported
        {
            get { return _model.MondayReported; }
        }

        public bool Acsi
        {
            get { return _model.Acsi; }
        }

        public bool Coa
        {
            get { return _model.Coa; }
        }

        public bool Nafcc
        {
            get { return _model.Nafcc; }
        }

        public bool Naccp
        {
            get { return _model.Naccp; }
        }

        public bool Naeyc
        {
            get { return _model.Naeyc; }
        }

        public bool Necpa
        {
            get { return _model.Necpa; }
        }

        public bool ChildCareFoodProgram
        {
            get { return _model.ChildCareFoodProgram; }
        }

        public bool Gradeschoolers
        {
            get { return _model.Gradeschoolers; }
        }

        public bool Preschoolers
        {
            get { return _model.Preschoolers; }
        }

        public bool YoungToddlers
        {
            get { return _model.YoungToddlers; }
        }

        public bool Infants
        {
            get { return _model.Infants; }
        }

        public bool OlderToddlers
        {
            get { return _model.OlderToddlers; }
        }

        public int? SutqRating
        {
            get { return _model.SutqRating; }
        }

        public string LicenseBeginDate
        {
            get { return _model.LicenseBeginDate; }
        }

        public string InitialApplicationDate
        {
            get { return _model.InitialApplicationDate; }
        }

        public string CenterStatus
        {
            get { return _model.CenterStatus; }
        }

        public string LicenseExpirationDate
        {
            get { return _model.LicenseExpirationDate; }
        }

        public string Administrators
        {
            get { return _model.Administrators; }
        }

        public string ProviderAgreement
        {
            get { return _model.ProviderAgreement; }
        }
    }
}