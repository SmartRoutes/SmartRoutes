namespace OdjfsHtmlScraper.Parsers
{
    public interface IChildCareParser<in T>
    {
        void Parse(T childCare, byte[] bytes);
    }
}