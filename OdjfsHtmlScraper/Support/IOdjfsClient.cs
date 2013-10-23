using System.Threading.Tasks;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Support
{
    public interface IOdjfsClient
    {
        Task<byte[]> GetChildCareDocument(ChildCare childCare);
        Task<byte[]> GetListDocument();
    }
}