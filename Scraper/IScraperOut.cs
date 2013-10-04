using System.Threading.Tasks;

namespace Scraper
{
    public interface IScraper<TOut>
    {
        Task<TOut> Scrape();
    }
}