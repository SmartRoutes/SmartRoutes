using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog;

namespace SmartRoutes.Reader.Parsers.Srds
{
    public class StringParser : IStringParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly IDictionary<Type, Func<string, object>> StringParsers;

        private readonly IDictionary<string, Type> _types = new Dictionary<string, Type>();

        static StringParser()
        {
            // initialize the dictionary of different string parsers
            Type thisType = typeof (StringParser);
            MethodInfo[] stringParsers = thisType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(m =>
                    m.Name.StartsWith("Parse") &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters().First().ParameterType == typeof (string))
                .ToArray();

            StringParsers = new Dictionary<Type, Func<string, object>>();
            foreach (MethodInfo stringParser in stringParsers)
            {
                MethodInfo localStringParser = stringParser;
                StringParsers[stringParser.ReturnType] = s => localStringParser.Invoke(null, new object[] {s});
            }
        }

        public bool SupportsParsing(string typeName)
        {
            try
            {
                GetStringParser(typeName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object Parse(string typeName, string value)
        {
            return GetStringParser(typeName)(value);
        }

        private Func<string, object> GetStringParser(string typeName)
        {
            // get the type
            Type type;
            if (!_types.ContainsKey(typeName))
            {
                type = Type.GetType(typeName);
                _types[typeName] = type;
            }
            else
            {
                type = _types[typeName];
            }

            // get the parser
            if (!StringParsers.ContainsKey(type))
            {
                var exception = new ArgumentException("The provided type name does not have an associated string parser.");
                Logger.ErrorException(string.Format("TypeName: {0}", typeName), exception);
                throw exception;
            }
            return StringParsers[type];
        }

        private static string ParseString(string value)
        {
            return value;
        }
    }
}