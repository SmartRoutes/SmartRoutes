using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Model.Odjfs.ChildCareStubs;

namespace SmartRoutes.OdjfsScraper.Parsers
{
    public interface IChildCareParser
    {
        ChildCare Parse(ChildCareStub childCareStub, byte[] bytes);
        ChildCare Parse(ChildCare childCare, byte[] bytes);
    }
}