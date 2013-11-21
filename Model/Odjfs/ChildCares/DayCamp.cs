﻿namespace SmartRoutes.Model.Odjfs.ChildCares
{
    public class DayCamp : ChildCare
    {
        public const string Discriminator = "DayCamp";

        public DayCamp()
        {
            ChildCareType = Discriminator;
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