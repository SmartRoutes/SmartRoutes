using System.Threading.Tasks;
using SmartRoutes.Model.Odjfs;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Model.Odjfs.ChildCareStubs;

namespace OdjfsScraper.Support
{
    public interface IOdjfsClient
    {
        Task<ClientResponse> GetChildCareDocument(ChildCareStub childCareStub);
        Task<ClientResponse> GetChildCareDocument(ChildCare childCare);
        Task<ClientResponse> GetListDocument();
        Task<ClientResponse> GetListDocument(int zipCode);
        Task<ClientResponse> GetListDocument(County county);
        Task<ClientResponse> GetListDocument(County county, int zipCode);
    }
}