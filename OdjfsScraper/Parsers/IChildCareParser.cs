using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;

namespace OdjfsScraper.Parsers
{
    public interface IChildCareParser
    {
        ChildCare Parse(ChildCareStub childCareStub, byte[] bytes);
        void Parse(ChildCare childCare, byte[] bytes);
    }
}