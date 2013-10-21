﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using CsQuery;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Support
{
    public class BaseClient : IClient
    {
        private readonly HttpClient _httpClient;

        protected BaseClient()
        {
            var handler = new WebRequestHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false,
                AllowPipelining = true
            };

            // get the version at runtime
            string version = ((AssemblyInformationalVersionAttribute) Assembly
                .GetAssembly(typeof (IClient))
                .GetCustomAttributes(typeof (AssemblyInformationalVersionAttribute), false)[0])
                .InformationalVersion;

            // construct a helpful user-agent
            string userAgent = string.Format("SmartRoutes/{0} (+http://goo.gl/Ol3VNR)", version);

            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        public async Task<CQ> GetChildCareDocument(ChildCare childCare)
        {
            // create the URL
            var requestUri = new Uri(string.Format("http://www.odjfs.state.oh.us/cdc/results2.asp?provider_number={0}", childCare.ExternalUrlId));

            // fetch the bytes
            byte[] bytes = await GetBytes(requestUri);

            // execute the implementation-specific code
            await HandleChildCareDocumentBytes(childCare, bytes);

            // parse the HTML
            return ParseBytes(bytes);
        }

        public async Task<CQ> GetListDocument()
        {
            // create the URL
            var requestUri = new Uri("http://www.odjfs.state.oh.us/cdc/results1.asp?Zip=45224&Printable=Y&ShowAllPages=Y");

            // fetch the bytes
            byte[] bytes = await GetBytes(requestUri);

            // execute the implementation-specific code
            await HandleListDocumentBytes(bytes);

            // parse the HTML
            return ParseBytes(bytes);
        }

        private async Task<byte[]> GetBytes(Uri requestUri)
        {
            // get the response bytes
            return await _httpClient.GetByteArrayAsync(requestUri);
        }

        protected virtual Task HandleChildCareDocumentBytes(ChildCare childCare, byte[] bytes)
        {
            return Task.FromResult(0);
        }

        protected virtual Task HandleListDocumentBytes(byte[] bytes)
        {
            return Task.FromResult(0);
        }

        private CQ ParseBytes(byte[] bytes)
        {
            // parse the response HTML
            return CQ.Create(new MemoryStream(bytes));
        }
    }
}