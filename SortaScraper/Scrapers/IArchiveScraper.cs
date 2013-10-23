using System.Threading.Tasks;
using Model.Sorta;

namespace SortaScraper.Scrapers
{
    public interface IArchiveScraper
    {
        Task<Archive> Scrape();
    }
}