using System.Threading.Tasks;
using SortaCsvScraper.Support;

namespace SortaCsvScraper.Scrapers
{
    public interface IEntityCollectionScraper
    {
        Task<EntityCollection> Scrape();
    }
}