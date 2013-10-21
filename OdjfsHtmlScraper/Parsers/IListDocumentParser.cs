using System.Collections.Generic;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Parsers
{
    public interface IListDocumentParser
    {
        IEnumerable<ChildCare> Parse(byte[] bytes);
    }
}