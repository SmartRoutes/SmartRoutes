using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRoutes.Model.Odjfs;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Model.Odjfs.ChildCareStubs;

namespace SmartRoutes.OdjfsScraper.Scrapers
{
    public interface IChildCareStubListScraper
    {
        Task<IEnumerable<ChildCareStub>> Scrape();
        Task<IEnumerable<ChildCareStub>> Scrape(County county);
    }
}