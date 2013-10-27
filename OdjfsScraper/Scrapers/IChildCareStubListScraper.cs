using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;

namespace OdjfsScraper.Scrapers
{
    public interface IChildCareStubListScraper
    {
        Task<IEnumerable<ChildCareStub>> Scrape();
        Task<IEnumerable<ChildCareStub>> Scrape(County county);
    }
}