using System.Threading.Tasks;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Model.Odjfs.ChildCareStubs;

namespace OdjfsScraper.Scrapers
{
    public interface IChildCareScraper
    {
        Task<ChildCare> Scrape(ChildCareStub childCareStub);
        Task<ChildCare> Scrape(ChildCare childCare);
    }
}