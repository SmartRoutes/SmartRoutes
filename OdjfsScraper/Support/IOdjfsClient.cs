using System.Threading.Tasks;
using Model.Odjfs;
using Model.Odjfs.ChildCares;

namespace OdjfsScraper.Support
{
    public interface IOdjfsClient
    {
        Task<byte[]> GetChildCareDocument(ChildCare childCare);
        Task<byte[]> GetListDocument();
        Task<byte[]> GetListDocument(County county);
    }
}