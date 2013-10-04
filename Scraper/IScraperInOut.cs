using System.Threading.Tasks;

namespace Scraper
{
    public interface IScraper<in TIn, TOut>
    {
        Task<TOut> Scrape(TIn input);
    }
}