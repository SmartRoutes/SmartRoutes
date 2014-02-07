namespace SmartRoutes.Demo.OdjfsDatabase.Model
{
    public class TypeBHome : ChildCare
    {
        private readonly OdjfsScraper.Model.ChildCares.TypeBHome _model;

        public TypeBHome(OdjfsScraper.Model.ChildCares.TypeBHome model) : base(model)
        {
            _model = model;
        }

        public string CertificationBeginDate
        {
            get { return _model.CertificationBeginDate; }
        }

        public string CertificationExpirationDate
        {
            get { return _model.CertificationExpirationDate; }
        }
    }
}