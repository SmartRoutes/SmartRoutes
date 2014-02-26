using System;
using System.Collections.Generic;
using System.Linq;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using SmartRoutes.Models;

namespace SmartRoutes.Support
{
    // This stuff should probably go in a database eventually,
    // but as there is still debate about database issues, this
    // is just done inline for now.
    public static class ResourceModels
    {
        #region Accreditations

        private static readonly IEnumerable<Accrediation> AccreditationData = new[]
        {
            new Accrediation
            {
                Model = new AccreditationModel(Resources.NAEYCName, Resources.NAEYCDescription, new Uri(Resources.NAEYCURL)),
                Validate = dcc => dcc.Naeyc
            },
            new Accrediation
            {
                Model = new AccreditationModel(Resources.NECPAName, Resources.NECPADescription, new Uri(Resources.NECPAURL)),
                Validate = dcc => dcc.Necpa
            },
            new Accrediation
            {
                Model = new AccreditationModel(Resources.NACCPName, Resources.NACCPDescription, new Uri(Resources.NACCPURL)),
                Validate = dcc => dcc.Naccp
            },
            new Accrediation
            {
                Model = new AccreditationModel(Resources.NAFCCName, Resources.NAFCCDescription, new Uri(Resources.NAFCCURL)),
                Validate = dcc => dcc.Nafcc
            },
            new Accrediation
            {
                Model = new AccreditationModel(Resources.COAName, Resources.COADescription, new Uri(Resources.COAURL)),
                Validate = dcc => dcc.Coa
            },
            new Accrediation
            {
                Model = new AccreditationModel(Resources.ACSIName, Resources.ACSIDescription, new Uri(Resources.ACSIURL)),
                Validate = dcc => dcc.Acsi
            },
            new Accrediation
            {
                Model = new AccreditationModel(Resources.CCFPName, Resources.CCFPDescription, new Uri(Resources.CCFPURL)),
                Validate = dcc => dcc.ChildCareFoodProgram
            }
        };

        public static readonly IReadOnlyDictionary<string, Func<DetailedChildCare, bool>> AccreditationValidators = AccreditationData
            .ToDictionary(a => a.Model.Name, a => a.Validate);

        public static readonly IEnumerable<AccreditationModel> AccreditationModels = AccreditationData
            .Select(a => a.Model)
            .ToArray();

        #endregion

        #region Service Types

        private static readonly IEnumerable<ServiceType> ServiceTypeData = new[]
        {
            new ServiceType
            {
                Model = new ServiceTypeModel(Resources.TypeAHomeName, Resources.TypeAHomeDescription),
                Validate = cc => cc is TypeAHome
            },
            new ServiceType
            {
                Model = new ServiceTypeModel(Resources.LicensedCenterName, Resources.LicensedCenterDescription),
                Validate = cc => cc is LicensedCenter
            },
            new ServiceType
            {
                Model = new ServiceTypeModel(Resources.DayCampName, Resources.DayCampDescription),
                Validate = cc => cc is DayCamp
            }
        };

        public static readonly IReadOnlyDictionary<string, Func<ChildCare, bool>> ServiceTypeValidators = ServiceTypeData
            .ToDictionary(a => a.Model.Name, a => a.Validate);

        public static readonly IEnumerable<ServiceTypeModel> ServiceTypeModels = ServiceTypeData
            .Select(a => a.Model)
            .ToArray();

        #endregion

        #region Age Group

        private static readonly IEnumerable<AgeGroup> AgeGroupData = new[]
        {
            new AgeGroup
            {
                Name = Resources.AgeGroupInfantName,
                Validate = dcc => dcc.Infants
            },
            
            new AgeGroup
            {
                Name = Resources.AgeGroupYoungToddlerName,
                Validate = dcc => dcc.YoungToddlers
            },
            
            new AgeGroup
            {
                Name = Resources.AgeGroupOlderToddlerName,
                Validate = dcc => dcc.OlderToddlers
            },
            
            new AgeGroup
            {
                Name = Resources.AgeGroupPreschoolerName,
                Validate = dcc => dcc.Preschoolers
            },
            
            new AgeGroup
            {
                Name = Resources.AgeGroupSchoolAgeName,
                Validate = dcc => dcc.Gradeschoolers
            }
        };

        public static readonly IReadOnlyDictionary<string, Func<DetailedChildCare, bool>> AgeGroupValidators = AgeGroupData
            .ToDictionary(a => a.Name, a => a.Validate);

        public static readonly IEnumerable<string> AgeGroupNames = AgeGroupData
            .Select(a => a.Name)
            .ToArray();

        #endregion
    }
}