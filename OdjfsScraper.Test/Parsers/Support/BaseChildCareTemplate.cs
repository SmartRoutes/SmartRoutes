using RazorEngine.Templating;
using SmartRoutes.Model.Odjfs;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace OdjfsScraper.Test.Parsers.Support
{
    public abstract class BaseChildCareTemplate<T> : TemplateBase<T> where T : ChildCare
    {
        protected BaseChildCareTemplate(T childCare)
        {
            Model = childCare;
            Model.ExternalId = "ExternalId";
            Model.Name = "Name";
            Model.Address = "Address";
            Model.City = "City";
            Model.State = "State";
            Model.ZipCode = 99999;
            Model.County = new County { Name = "County" };
            Model.PhoneNumber = "PhoneNumber";
        }
    }
}