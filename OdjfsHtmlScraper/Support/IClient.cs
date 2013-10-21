using System.Threading.Tasks;
using CsQuery;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Support
{
    public interface IClient
    {
        Task<CQ> GetChildCareDocument(ChildCare childCare);
        Task<CQ> GetListDocument();
    }
}