using System;
using System.Net;

namespace OdjfsScraper
{
    public class ClientResponse
    {
        public string RequestUri { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public byte[] Content { get; set; }
    }
}