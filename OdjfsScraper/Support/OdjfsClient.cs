using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;
using Scraper;

namespace OdjfsScraper.Support
{
    public class OdjfsClient : IOdjfsClient
    {
        private readonly ScraperClient _scraperClient;

        public OdjfsClient()
        {
            _scraperClient = new ScraperClient();
        }

        public async Task<ClientResponse> GetChildCareDocument(ChildCareStub childCareStub)
        {
            // geth the document
            ClientResponse response = await GetChildCareDocument(childCareStub.ExternalUrlId);

            // execute implementation-specific code
            await HandleChildCareDocumentResponse(childCareStub, response);

            return response;
        }

        public async Task<ClientResponse> GetChildCareDocument(ChildCare childCare)
        {
            // get the document
            ClientResponse response = await GetChildCareDocument(childCare.ExternalUrlId);

            // execute implementation-specific code
            await HandleChildCareDocumentResponse(childCare, response);

            return response;
        }

        public async Task<ClientResponse> GetListDocument()
        {
            return await GetListDocument(null, 0);
        }

        public async Task<ClientResponse> GetListDocument(int zipCode)
        {
            return await GetListDocument(null, zipCode);
        }

        public async Task<ClientResponse> GetListDocument(County county)
        {
            return await GetListDocument(county, 0);
        }

        public async Task<ClientResponse> GetListDocument(County county, int zipCode)
        {
            // create the query parameter
            string countyQueryParameter = county == null ? string.Empty : string.Format("County={0}&", county.Name);
            string zipCodeQueryParameter = zipCode == 0 ? string.Empty : string.Format("Zip={0}&", zipCode);

            // create the URL
            var requestUri = new Uri(string.Format("http://www.odjfs.state.oh.us/cdc/results1.asp?{0}{1}Printable=Y&ShowAllPages=Y", countyQueryParameter, zipCodeQueryParameter));

            // fetch the bytes
            ClientResponse response = await GetResponse(requestUri);

            // execute the implementation-specific code
            await HandleListDocumentResponse(county, response);

            return response;
        }

        private async Task<ClientResponse> GetChildCareDocument(string externalUrlId)
        {
            // create the URL
            var requestUri = new Uri(string.Format("http://www.odjfs.state.oh.us/cdc/results2.asp?provider_number={0}&Printable=Y", externalUrlId));

            // fetch the bytes
            ClientResponse response = await GetResponse(requestUri);

            // 404 errors are masked as 500 errors...
            if (response.StatusCode == HttpStatusCode.InternalServerError && Encoding.UTF8.GetString(response.Content).Contains("error '800a0bcd'"))
            {
                response.StatusCode = HttpStatusCode.NotFound;
            }

            return response;
        }

        private async Task<ClientResponse> GetResponse(Uri requestUri)
        {
            // get the response bytes
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            HttpResponseMessage response = await _scraperClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            return new ClientResponse
            {
                RequestUri = requestUri.ToString(),
                StatusCode = response.StatusCode,
                Content = bytes
            };
        }

        protected virtual Task HandleChildCareDocumentResponse(ChildCare childCare, ClientResponse response)
        {
            return Task.FromResult(0);
        }

        protected virtual Task HandleChildCareDocumentResponse(ChildCareStub childCareStub, ClientResponse response)
        {
            return Task.FromResult(0);
        }

        protected virtual Task HandleListDocumentResponse(County county, ClientResponse response)
        {
            return Task.FromResult(0);
        }
    }
}