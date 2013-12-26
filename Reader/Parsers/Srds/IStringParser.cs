namespace SmartRoutes.Reader.Parsers.Srds
{
    public interface IStringParser
    {
        bool SupportsParsing(string typeName);
        object Parse(string typeName, string value);
    }
}