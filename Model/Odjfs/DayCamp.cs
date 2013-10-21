namespace Model.Odjfs
{
    public class DayCamp : ChildCare
    {
        public DayCamp()
        {
            ChildCareType = "DayCamp";
        }

        #region HTML

        public string Owner { get; set; }
        public string RegistrationStatus { get; set; }
        public string RegistrationBeginDate { get; set; }
        public string RegistrationEndDate { get; set; }
        public string EffectiveDates { get; set; }

        #endregion
    }
}