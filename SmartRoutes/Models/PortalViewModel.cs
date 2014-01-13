using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class PortalViewModel
    {
        public PortalViewModel(string id)
        {
            this.ID = id;
        }

        public string ID
        {
            get;
            set;
        }

        public IEnumerable<PortalButtonModel> ButtonModels
        {
            get
            {
                return this.buttonModels;
            }
        }

        public void AddButtonModel(PortalButtonModel buttonModel)
        {
            this.buttonModels.Add(buttonModel);
        }

        private List<PortalButtonModel> buttonModels = new List<PortalButtonModel>();
    }
}