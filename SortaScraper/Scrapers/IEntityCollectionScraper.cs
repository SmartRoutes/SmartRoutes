using System.Threading.Tasks;
using SmartRoutes.Model.Sorta;
using SortaScraper.Support;

namespace SortaScraper.Scrapers
{
    public interface IEntityCollectionScraper
    {
        Task<EntityCollection> Scrape(Archive currentArchive);
    }
}