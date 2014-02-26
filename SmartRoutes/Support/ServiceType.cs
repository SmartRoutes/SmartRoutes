using System;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using SmartRoutes.Models;

namespace SmartRoutes.Support
{
    public class ServiceType
    {
        public ServiceTypeModel Model { get; set; }
        public Func<ChildCare, bool> Validate { get; set; }
    }
}