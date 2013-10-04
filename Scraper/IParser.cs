namespace Scraper
{
    public interface IParser<in TIn, out TOut>
    {
        TOut Parse(TIn input);
    }
}