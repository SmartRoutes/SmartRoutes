using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NLog;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Reader.Parsers.Srds
{
    public class SrdsCollectionParser : EntityCollectionParser<SrdsArchive, SrdsCollection>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

            collection.AttributeValues = collection.Destinations.SelectMany(d => d.AttributeValues).ToArray();

            // stitch up
            Logger.Trace("Associating SrdsCollection entities.");
            Associate(collection);

            return collection;
        }

        private static void Associate(SrdsCollection collection)
        {
            for (int i = 0; i < collection.AttributeKeys.Length; i++)
            {
                collection.AttributeKeys[i].Id = i + 1;
            }

            for (int i = 0; i < collection.Destinations.Length; i++)
            {
                Destination destination = collection.Destinations[i];
                destination.Id = i + 1;
            }

            for (int i = 0; i < collection.AttributeValues.Length; i++)
            {
                AttributeValue value = collection.AttributeValues[i];
                value.Id = i + 1;
                value.AttributeKeyId = value.AttributeKey.Id;
                value.DestinationId = value.Destination.Id;
            }
        }
    }
}