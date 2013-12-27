using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NLog;

namespace SmartRoutes.Reader.Parsers.Srds
{
    public class StringParser : IStringParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly IDictionary<Type, Func<string, object>> TypeToStringParser;
        private static readonly IDictionary<Type, Func<string, object>> TypeToNullableStringParser;

        private readonly IDictionary<string, Func<string, object>> _typeNameToStringParser = new Dictionary<string, Func<string, object>>();
        private readonly IDictionary<string, Type> _types = new Dictionary<string, Type>();

        static StringParser()
        {
            // initialize the dictionary of different string parsers
            Type thisType = typeof (StringParser);
            MethodInfo[] stringParserInfos = thisType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(m =>
                    m.Name.StartsWith("Parse") &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters().First().ParameterType == typeof (string))
                .ToArray();

            TypeToStringParser = new Dictionary<Type, Func<string, object>>();
            TypeToNullableStringParser = new Dictionary<Type, Func<string, object>>();
            foreach (MethodInfo info in stringParserInfos)
            {
                MethodInfo localInfo = info;
                Func<string, object> stringParser = s => Parse(localInfo, s);
                TypeToStringParser[info.ReturnType] = stringParser;
                if (info.ReturnType.IsValueType)
                {
                    Func<string, object> nullableStringParser = s => ParseNullable(stringParser, s);
                    TypeToNullableStringParser[info.ReturnType] = nullableStringParser;
                }
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

        private static object Parse(MethodInfo info, string value)
        {
            try
            {
                return info.Invoke(null, new object[] {value});
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }
                throw;
            }
        }

        private static object ParseNullable(Func<string, object> stringParser, string value)
        {
            if (value == string.Empty)
            {
                return null;
            }
            return stringParser(value);
        }

        private Func<string, object> GetStringParser(string typeName)
        {
            // check the cache
            if (_typeNameToStringParser.ContainsKey(typeName))
            {
                return _typeNameToStringParser[typeName];
            }

            // check for nullable flag
            bool nullable = typeName.EndsWith("?");
            string underlyingTypeName;
            if (nullable)
            {
                underlyingTypeName = typeName.Substring(0, typeName.Length - 1);
            }
            else
            {
                underlyingTypeName = typeName;
            }

            // get the type
            Type type;
            if (!_types.ContainsKey(underlyingTypeName))
            {
                type = Type.GetType(underlyingTypeName);
                _types[underlyingTypeName] = type;
            }
            else
            {
                type = _types[underlyingTypeName];
            }
            if (type == null)
            {
                var exception = new ArgumentException("The provided type name is not valid.");
                Logger.ErrorException(string.Format("TypeName: {0}", underlyingTypeName), exception);
                throw exception;
            }

            // get the parser
            Func<string, object> stringParser;
            if (nullable)
            {
                if (!TypeToNullableStringParser.TryGetValue(type, out stringParser))
                {
                    ThrowUnsupportedType(typeName);
                }
            }
            else
            {
                if (!TypeToStringParser.TryGetValue(type, out stringParser))
                {
                    ThrowUnsupportedType(typeName);
                }
            }

            // cache the result
            _typeNameToStringParser[typeName] = stringParser;
            return stringParser;
        }

        private static void ThrowUnsupportedType(string typeName)
        {
            var exception = new ArgumentException("The provided type name does not have an associated string parser.");
            Logger.ErrorException(string.Format("TypeName: {0}", typeName), exception);
            throw exception;
        }

        private static string ParseString(string value)
        {
            return value;
        }

        private static bool ParseBoolean(string value)
        {
            return Convert.ToBoolean(value);
        }

        private static int ParseInt32(string value)
        {
            return Convert.ToInt32(value);
        }

        private static double ParseDouble(string value)
        {
            return Convert.ToDouble(value);
        }

        private static float ParseFloat(string value)
        {
            return Convert.ToSingle(value);
        }

        private static DateTime ParseDateTime(string value)
        {
            return DateTime.ParseExact(value, "R", CultureInfo.InvariantCulture);
        }
    }
}