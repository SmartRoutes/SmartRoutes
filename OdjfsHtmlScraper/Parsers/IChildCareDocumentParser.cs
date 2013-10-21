namespace OdjfsHtmlScraper.Parsers
{
    public interface IChildCareDocumentParser<in T>
    {
        void Parse(T childCare, byte[] bytes);
    }
}