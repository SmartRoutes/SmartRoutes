using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Scrapers
{
    public interface IListDocumentScraper
    {
        Task<IEnumerable<ChildCare>> Scrape();
    }
}