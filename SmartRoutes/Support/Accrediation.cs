using System;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using SmartRoutes.Models;

namespace SmartRoutes.Support
{
    public class Accrediation
    {
        public AccreditationModel Model { get; set; }
        public Func<DetailedChildCare, bool> Validate { get; set; }
    }
}