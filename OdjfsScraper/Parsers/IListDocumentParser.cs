using System.Collections.Generic;
using Model.Odjfs;

namespace OdjfsScraper.Parsers
{
    public interface IListDocumentParser
    {
        IEnumerable<ChildCareStub> Parse(byte[] bytes);
    }
}