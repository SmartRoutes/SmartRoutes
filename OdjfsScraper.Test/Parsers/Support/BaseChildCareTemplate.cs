using System;
using SmartRoutes.Model.Odjfs;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.OdjfsScraper.Test.Parsers.Support
{
    public abstract class BaseChildCareTemplate<T> where T : ChildCare
    {
        protected BaseChildCareTemplate()
        {
            Model = Activator.CreateInstance<T>();
            Model.ExternalId = "ExternalId";
            Model.Name = "Name";
            Model.Address = "Address";
            Model.City = "City";
            Model.State = "State";
            Model.ZipCode = 99999;
            Model.County = new County {Name = "County"};
            Model.PhoneNumber = "PhoneNumber";
        }

        public T Model { get; private set; }
    }
}