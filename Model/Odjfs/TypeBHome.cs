namespace Model.Odjfs
{
    public class TypeBHome : ChildCare
    {
        public TypeBHome()
        {
            ChildCareType = "TypeBHome";
        }

        #region HTML

        public string CertificationBeginDate { get; set; }
        public string CertificationEndDate { get; set; }

        #endregion
    }
}