using System;
using System.Collections.Generic;
using System.Linq;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.OdjfsScraper.Test.Parsers.Support
{
    public class TypeAHomeTemplate : DetailedChildCareHomeTemplate<TypeAHome>
    {
        public TypeAHomeTemplate() : base(Enumerable.Empty<KeyValuePair<string, Func<TypeAHome, string>>>())
        {
        }
    }
}