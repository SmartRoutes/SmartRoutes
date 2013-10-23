using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Odjfs;

namespace OdjfsScraper.Scrapers
{
    public interface IChildCareStubListScraper
    {
        Task<IEnumerable<ChildCareStub>> Scrape();
    }
}