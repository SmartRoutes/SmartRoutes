using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using SmartRoutes.Model.Srds;
using SmartRoutes.Scraper;

namespace SmartRoutes.SrdsReader.Parsers
{
    public class DestinationCsvStreamParser : CsvStreamParser<Destination>, IDestinationCsvStreamParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ISet<string> ReservedKeys = new HashSet<string> {"Id", "Name", "Latitude", "Longitude"};
        private readonly IStringParser _stringParser;

        private ISet<string> _attributeKeyNames;
        private IDictionary<string, AttributeKey> _attributeKeys;

        public DestinationCsvStreamParser(IStringParser stringParser)
        {
            _stringParser = stringParser;
            AttachAttributeKeys(Enumerable.Empty<AttributeKey>());
        }

        public void AttachAttributeKeys(IEnumerable<AttributeKey> attributeKeys)
        {
            // attribute key state
            _attributeKeyNames = new HashSet<string>();
            _attributeKeys = new Dictionary<string, AttributeKey>();

            // validate the attribute keys
            IEnumerable<IGrouping<string, AttributeKey>> attributeKeyGroups = attributeKeys
                .GroupBy(a => a.Name);
            foreach (var g in attributeKeyGroups)
            {
                string attributeKeyName = g.Key;

                // check for reserved name
                if (ReservedKeys.Contains(attributeKeyName))
                {
                    var exception = new ArgumentException("A reserved AttributeKey name was used.", "attributeKeys");
                    Logger.ErrorException(string.Format("AttributeKeyName: {0}", attributeKeyName), exception);
                    throw exception;
                }

                // check for duplicates
                List<AttributeKey> groupAttributeKeys = g.ToList();
                if (groupAttributeKeys.Count > 1)
                {
                    var exception = new ArgumentException("Duplicate AttributeKey names are not allowed.", "attributeKeys");
                    Logger.ErrorException(string.Format("AttributeKeyName: {0}, Count: {1}", attributeKeyName, groupAttributeKeys.Count), exception);
                    throw exception;
                }
                _attributeKeyNames.Add(attributeKeyName);
                _attributeKeys[attributeKeyName] = groupAttributeKeys[0];
            }

            // validate the attribute key type names
            foreach (AttributeKey attributeKey in _attributeKeys.Values)
            {
                if (!_stringParser.SupportsParsing(attributeKey.TypeName))
                {
                    var exception = new ArgumentException("An AttributeKey TypeName does not have a supported string parser.", "attributeKeys");
                    Logger.ErrorException(string.Format("AttributeKey.Name: {0}, AttributeKey.TypeName: {1}",
                        attributeKey.Name,
                        attributeKey.TypeName),
                        exception);
                    throw exception;
                }
            }
        }

        protected override Destination ConstructItem(IDictionary<string, string> values)
        {
            var destination = new Destination
            {
                Name = values["Name"],
                Latitude = double.Parse(values["Latitude"]),
                Longitude = double.Parse(values["Longitude"])
            };

            foreach (string attributeKeyName in values.Keys.Intersect(_attributeKeyNames))
            {
                AttributeKey attributeKey = _attributeKeys[attributeKeyName];
                string value = values[attributeKeyName];

                // create the child AttributeValue, with a parsed object value
                destination.AttributeValues.Add(new AttributeValue
                {
                    AttributeKey = attributeKey,
                    Value = _stringParser.Parse(attributeKey.TypeName, value)
                });
            }

            return destination;
        }
    }
}