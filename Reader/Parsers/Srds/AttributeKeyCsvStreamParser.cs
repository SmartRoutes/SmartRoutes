﻿using System.Collections.Generic;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.Parsers;

namespace SmartRoutes.Reader.Parsers.Srds
{
    public class AttributeKeyCsvStreamParser : CsvStreamParser<AttributeKey>
    {
        protected override AttributeKey ConstructItem(IDictionary<string, string> values)
        {
            return new AttributeKey
            {
                Name = values["Name"],
                TypeName = values["TypeName"]
            };
        }
    }
}