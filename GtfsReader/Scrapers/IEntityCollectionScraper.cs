using System.Threading.Tasks;
using SmartRoutes.GtfsReader.Support;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.GtfsReader.Scrapers
{
    public interface IEntityCollectionScraper
    {
        Task<EntityCollection> Scrape(Archive currentArchive);
    }
}