using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace Scraper
{
    public abstract class CsvStreamParser<TOut> : IParser<Stream, IEnumerable<TOut>>
    {
        public IEnumerable<TOut> Parse(Stream csvStream)
        {
            // use the .NET library that parses CSVs
            var parser = new TextFieldParser(csvStream);
            parser.HasFieldsEnclosedInQuotes = true;
            parser.TextFieldType = FieldType.Delimited;
            parser.CommentTokens = new string[0];
            parser.Delimiters = new[] {","};

            // get the first line, which is column headings
            string[] keys = parser.ReadFields();
            if (keys == null)
            {
                yield break;
            }

            // parse each line
            string[] values;
            while ((values = parser.ReadFields()) != null)
            {
                // trim and turn empty strings to null
                values = values
                    .Select(v => string.IsNullOrWhiteSpace(v) ? null : v.Trim())
                    .ToArray();

                // associate values with headings
                // TODO: assumes that headings are unique
                // TODO: assumes the number of keys and values is the same
                IDictionary<string, string> valueDictionary = Enumerable
                    .Range(0, keys.Length)
                    .ToDictionary(i => keys[i], i => values[i]);

                yield return ConstructItem(valueDictionary);
            }
        }

        protected int? ParseNullableInt(string value)
        {
            return value != null ? int.Parse(value) : (int?) null;
        }

        protected double? ParseNullableDouble(string value)
        {
            return value != null ? double.Parse(value) : (double?) null;
        }

        protected abstract TOut ConstructItem(IDictionary<string, string> values);
    }
}