using System;

namespace Model.Odjfs
{
    public class DayCamp : ChildCare
    {
        #region HTML

        public string Owner { get; set; }
        public string RegistrationStatus { get; set; }
        public string RegistrationBeginDate { get; set; }
        public string RegistrationEndDate { get; set; }
        public string EffectiveDates { get; set; }

        #endregion
    }
}