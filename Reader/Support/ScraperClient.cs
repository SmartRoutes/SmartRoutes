﻿using System.Net;
using System.Net.Http;
using System.Reflection;

namespace SmartRoutes.Reader.Support
{
    public class ScraperClient : HttpClient
    {
        public ScraperClient() : base(GetHttpMessageHandler())
        {
            DefaultRequestHeaders.Add("User-Agent", GetUserAgent());
        }

        private static HttpMessageHandler GetHttpMessageHandler()
        {
            return new WebRequestHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false,
                AllowPipelining = true
            };
        }

        public static string GetUserAgent()
        {
            // get the version at runtime
            string version = Assembly.GetExecutingAssembly().GetInformationalVersion();

            // construct a helpful user-agent
            return string.Format("SmartRoutes/{0} (+http://goo.gl/Ol3VNR)", version);
        }
    }
}