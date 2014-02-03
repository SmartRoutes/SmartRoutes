using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class ServiceTypeModel : DetailedCheckboxModel
    {
        public ServiceTypeModel()
        {

        }

        public ServiceTypeModel(string name, string description)
            : base(name, description)
        {
        }
    }
}