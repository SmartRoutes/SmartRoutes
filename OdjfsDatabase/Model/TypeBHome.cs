namespace SmartRoutes.Demo.OdjfsDatabase.Model
{
    public class DayCamp : ChildCare
    {
        private readonly OdjfsScraper.Model.ChildCares.DayCamp _model;

        public DayCamp(OdjfsScraper.Model.ChildCares.DayCamp model) : base(model)
        {
            _model = model;
        }

        public string RegistrationEndDate
        {
            get { return _model.RegistrationEndDate; }
        }

        public string RegistrationBeginDate
        {
            get { return _model.RegistrationBeginDate; }
        }

        public string RegistrationStatus
        {
            get { return _model.RegistrationStatus; }
        }

        public string Owner
        {
            get { return _model.Owner; }
        }
    }
}