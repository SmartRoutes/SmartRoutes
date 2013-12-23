using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader;
using SmartRoutes.SrdsReader.Readers;

namespace SmartRoutes.SrdsReader.Parsers
{
    public class SrdsCollectionParser : EntityCollectionParser<SrdsCollection>
    {
        private readonly ICsvStreamParser<AttributeKey> _attributeKeyParser;
        private readonly IDestinationCsvStreamParser _destinationParser;

        public SrdsCollectionParser(ICsvStreamParser<AttributeKey> attributeKeyParser, IDestinationCsvStreamParser destinationParser)
        {
            _attributeKeyParser = attributeKeyParser;
            _destinationParser = destinationParser;
        }

        protected override SrdsCollection Parse(IDictionary<string, Func<Stream>> streams)
        {
            // initialize
            var collection = new SrdsCollection();

            // populate
            collection.AttributeKeys = _attributeKeyParser.Parse(GetStream(streams, "AttributeKeys.csv")).ToArray();

            _destinationParser.AttachAttributeKeys(collection.AttributeKeys);
            collection.Destinations = _destinationParser.Parse(GetStream(streams, "Destinations.csv")).ToArray();

            return collection;
        }
    }
}