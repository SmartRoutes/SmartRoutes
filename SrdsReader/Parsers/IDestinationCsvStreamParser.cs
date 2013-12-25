using System.Collections.Generic;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.Parsers;

namespace SmartRoutes.SrdsReader.Parsers
{
    public interface IDestinationCsvStreamParser : ICsvStreamParser<Destination>
    {
        void AttachAttributeKeys(IEnumerable<AttributeKey> attributeKeys);
    }
}