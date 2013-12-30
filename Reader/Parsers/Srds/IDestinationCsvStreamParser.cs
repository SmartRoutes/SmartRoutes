using System.Collections.Generic;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.Parsers;

namespace SmartRoutes.Reader.Parsers.Srds
{
    public interface IDestinationCsvStreamParser : ICsvStreamParser<Destination>
    {
        void AttachAttributeKeys(IEnumerable<AttributeKey> attributeKeys);
    }
}