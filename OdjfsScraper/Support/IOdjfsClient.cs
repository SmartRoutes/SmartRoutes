using System.Threading.Tasks;
using Model.Odjfs;

namespace OdjfsScraper.Support
{
    public interface IOdjfsClient
    {
        Task<byte[]> GetChildCareDocument(ChildCare childCare);
        Task<byte[]> GetListDocument();
    }
}