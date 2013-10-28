using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;
using NLog;
using Scraper;

namespace OdjfsScraper.Support
{
    public class OdjfsClient : IOdjfsClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotFound)
            {
                var exception = new ScraperException("A status code that is not 200 or 404 was returned when getting a child care document from a stub.");
                Logger.ErrorException(string.Format(
                    "Type: '{0}', ExternalUrlId: '{1}', StatusCode: '{2}'",
                    childCareStub.GetType(),
                    childCareStub.ExternalUrlId,
                    response.StatusCode), exception);
                throw exception;
            }

            return response;
        }

        public async Task<ClientResponse> GetChildCareDocument(ChildCare childCare)
        {
            // get the document
            ClientResponse response = await GetChildCareDocument(childCare.ExternalUrlId);

            // execute implementation-specific code
            await HandleChildCareDocumentResponse(childCare, response);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotFound)
            {
                var exception = new ScraperException("A status code that is not 200 or 404 was returned when getting a child care document.");
                Logger.ErrorException(string.Format(
                    "Type: '{0}', ExternalUrlId: '{1}', StatusCode: '{2}'",
                    childCare.GetType(),
                    childCare.ExternalUrlId,
                    response.StatusCode), exception);
                throw exception;
            }

            return response;
        }

        public async Task<ClientResponse> GetListDocument()
        {
            return await GetListDocument(null);
        }

        public async Task<ClientResponse> GetListDocument(County county)
        {
            // create the query parameter
            string countyQueryParameter = county == null ? string.Empty : string.Format("County={0}&", county.Name);

            // create the URL
            var requestUri = new Uri(string.Format("http://www.odjfs.state.oh.us/cdc/results1.asp?{0}Printable=Y&ShowAllPages=Y", countyQueryParameter));

            // fetch the bytes
            ClientResponse response = await GetResponse(requestUri);

            // execute the implementation-specific code
            await HandleListDocumentResponse(county, response);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var exception = new ScraperException("A status code that is not 200 was returned when getting the list document.");
                Logger.ErrorException(string.Format("RequestUri: '{0}', StatusCode: '{1}'", requestUri, response.StatusCode), exception);
                throw exception;
            }

            return response;
        }

        private async Task<ClientResponse> GetChildCareDocument(string externalUrlId)
        {
            // create the URL
            var requestUri = new Uri(string.Format("http://www.odjfs.state.oh.us/cdc/results2.asp?provider_number={0}", externalUrlId));

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