using System.Threading.Tasks;
using Model.Sorta;

namespace SortaCsvScraper.Scrapers
{
    public interface IArchiveScraper
    {
        Task<Archive> Scrape();
    }
}