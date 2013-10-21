using System.Threading.Tasks;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Scrapers
{
    public interface IChildCareDocumentScraper<in T> where T : ChildCare
    {
        Task Scrape(T childCare);
    }
}