using System.Collections.Generic;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;

namespace OdjfsScraper.Parsers
{
    public interface IListDocumentParser
    {
        IEnumerable<ChildCareStub> Parse(byte[] bytes);
        IEnumerable<ChildCareStub> Parse(byte[] bytes, County county);
    }
}