using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Model.Odjfs;
using Scraper;

namespace OdjfsScraper.Support
{
    public class OdjfsClient : IOdjfsClient
    {
        private readonly ScraperClient _scraperClient;

        protected OdjfsClient()
        {
            _scraperClient = new ScraperClient();
        }

        public async Task<byte[]> GetChildCareDocument(ChildCare childCare)
        {
            // create the URL
            var requestUri = new Uri(string.Format("http://www.odjfs.state.oh.us/cdc/results2.asp?provider_number={0}", childCare.ExternalUrlId));

            // fetch the bytes
            byte[] bytes = await GetBytes(requestUri);

            // execute the implementation-specific code
            await HandleChildCareDocumentBytes(childCare, bytes);

            return bytes;
        }

        public async Task<byte[]> GetListDocument()
        {
            // create the URL
            var requestUri = new Uri("http://www.odjfs.state.oh.us/cdc/results1.asp?Zip=45224&Printable=Y&ShowAllPages=Y");

            // fetch the bytes
            byte[] bytes = await GetBytes(requestUri);

            // execute the implementation-specific code
            await HandleListDocumentBytes(bytes);

            return bytes;
        }

        private async Task<byte[]> GetBytes(Uri requestUri)
        {
            // get the response bytes
            return await _scraperClient.GetByteArrayAsync(requestUri);
        }

        protected virtual Task HandleChildCareDocumentBytes(ChildCare childCare, byte[] bytes)
        {
            return Task.FromResult(0);
        }

        protected virtual Task HandleListDocumentBytes(byte[] bytes)
        {
            return Task.FromResult(0);
        }
    }
}