using System.Threading.Tasks;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;

namespace OdjfsScraper.Support
{
    public interface IOdjfsClient
    {
        Task<ClientResponse> GetChildCareDocument(ChildCareStub childCareStub);
        Task<ClientResponse> GetChildCareDocument(ChildCare childCare);
        Task<ClientResponse> GetListDocument();
        Task<ClientResponse> GetListDocument(County county);
    }
}