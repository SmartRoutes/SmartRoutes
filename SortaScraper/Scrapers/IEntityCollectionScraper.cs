using System.Threading.Tasks;
using SortaScraper.Support;

namespace SortaScraper.Scrapers
{
    public interface IEntityCollectionScraper
    {
        Task<EntityCollection> Scrape();
    }
}