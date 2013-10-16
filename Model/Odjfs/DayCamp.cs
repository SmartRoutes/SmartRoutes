using System;

namespace Model.Odjfs
{
    public class DayCamp : ChildCare
    {
        #region HTML

        public string Owner { get; set; }
        public string RegistrationStatus { get; set; }
        public DateTime RegistrationBeginDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }

        #endregion
    }
}