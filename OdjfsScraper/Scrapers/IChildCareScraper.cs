using System.Threading.Tasks;
using Model.Odjfs;

namespace OdjfsScraper.Scrapers
{
    public interface IChildCareScraper<in T> where T : ChildCare
    {
        Task Scrape(T childCare);
    }
}