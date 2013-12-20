namespace SmartRoutes.SrdsReader.Parsers
{
    public interface IStringParser
    {
        bool SupportsParsing(string typeName);
        object Parse(string typeName, string value);
    }
}