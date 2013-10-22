namespace Model.Odjfs
{
    public class TypeBHome : ChildCare
    {
        public const string Discriminator = "TypeBHome";

        public TypeBHome()
        {
            ChildCareType = Discriminator;
        }

        #region HTML

        public string CertificationBeginDate { get; set; }
        public string CertificationEndDate { get; set; }

        #endregion
    }
}