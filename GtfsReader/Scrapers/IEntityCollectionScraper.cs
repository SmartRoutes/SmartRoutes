using System.Threading.Tasks;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.SortaScraper.Support;

namespace SmartRoutes.SortaScraper.Scrapers
{
    public interface IEntityCollectionScraper
    {
        Task<EntityCollection> Scrape(Archive currentArchive);
    }
}