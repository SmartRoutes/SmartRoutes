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

        public override DateTime? SundayEnd
        {
            get { return _model.SundayEnd; }
        }

        public override DateTime? SundayBegin
        {
            get { return _model.SundayBegin; }
        }

        public override bool SundayReported
        {
            get { return _model.SundayReported; }
        }

        public override DateTime? SaturdayBegin
        {
            get { return _model.SaturdayBegin; }
        }

        public override bool SaturdayReported
        {
            get { return _model.SaturdayReported; }
        }

        public override DateTime? SaturdayEnd
        {
            get { return _model.SaturdayEnd; }
        }

        public override DateTime? FridayEnd
        {
            get { return _model.FridayEnd; }
        }

        public override DateTime? FridayBegin
        {
            get { return _model.FridayBegin; }
        }

        public override bool FridayReported
        {
            get { return _model.FridayReported; }
        }

        public override DateTime? ThursdayEnd
        {
            get { return _model.ThursdayEnd; }
        }

        public override DateTime? ThursdayBegin
        {
            get { return _model.ThursdayBegin; }
        }

        public override bool ThursdayReported
        {
            get { return _model.ThursdayReported; }
        }

        public override DateTime? WednesdayEnd
        {
            get { return _model.WednesdayEnd; }
        }

        public override DateTime? TuesdayEnd
        {
            get { return _model.TuesdayEnd; }
        }

        public override DateTime? TuesdayBegin
        {
            get { return _model.TuesdayBegin; }
        }

        public override bool TuesdayReported
        {
            get { return _model.TuesdayReported; }
        }

        public override bool WednesdayReported
        {
            get { return _model.WednesdayReported; }
        }

        public override DateTime? WednesdayBegin
        {
            get { return _model.WednesdayBegin; }
        }

        public override DateTime? MondayEnd
        {
            get { return _model.MondayEnd; }
        }

        public override DateTime? MondayBegin
        {
            get { return _model.MondayBegin; }
        }

        public override bool MondayReported
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