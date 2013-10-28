﻿using System;
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

        public async Task<byte[]> GetChildCareDocument(ChildCareStub childCareStub)
        {
            byte[] bytes = await GetChildCareDocument(childCareStub.ExternalUrlId);
            await HandleChildCareDocumentBytes(childCareStub, bytes);
            return bytes;
        }

        public async Task<byte[]> GetChildCareDocument(ChildCare childCare)
        {
            byte[] bytes = await GetChildCareDocument(childCare.ExternalUrlId);
            await HandleChildCareDocumentBytes(childCare, bytes);
            return bytes;
        }

        public async Task<byte[]> GetListDocument()
        {
            return await GetListDocument(null);
        }

        public async Task<byte[]> GetListDocument(County county)
        {
            // create the query parameter
            string countyQueryParameter = county == null ? string.Empty : string.Format("County={0}&", county.Name);

            // create the URL
            var requestUri = new Uri(string.Format("http://www.odjfs.state.oh.us/cdc/results1.asp?{0}Printable=Y&ShowAllPages=Y", countyQueryParameter));

            // fetch the bytes
            byte[] bytes = await GetBytes(requestUri);

            // execute the implementation-specific code
            await HandleListDocumentBytes(county, bytes);

            return bytes;
        }

        private async Task<byte[]> GetChildCareDocument(string externalUrlId)
        {
            // create the URL
            var requestUri = new Uri(string.Format("http://www.odjfs.state.oh.us/cdc/results2.asp?provider_number={0}", externalUrlId));

            // fetch the bytes
            byte[] bytes = await GetBytes(requestUri);

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

        protected virtual Task HandleChildCareDocumentBytes(ChildCareStub childCareStub, byte[] bytes)
        {
            return Task.FromResult(0);
        }

        protected virtual Task HandleListDocumentBytes(County county, byte[] bytes)
        {
            return Task.FromResult(0);
        }
    }
}