using System.Collections.Generic;
using SmartRoutes.Model.Odjfs;
using SmartRoutes.Model.Odjfs.ChildCareStubs;

namespace SmartRoutes.OdjfsScraper.Parsers
{
    public interface IListParser
    {
        IEnumerable<ChildCareStub> Parse(byte[] bytes);
        IEnumerable<ChildCareStub> Parse(byte[] bytes, County county);
    }
}