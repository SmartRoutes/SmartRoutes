using System.Threading.Tasks;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;

namespace OdjfsScraper.Scrapers
{
    public interface IChildCareScraper
    {
        Task<ChildCare> Scrape(ChildCareStub childCareStub);
        Task<ChildCare> Scrape(ChildCare childCare);
    }
}