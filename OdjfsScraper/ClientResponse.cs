using System;
using System.Net;

namespace OdjfsScraper
{
    [Serializable]
    public class ClientResponse
    {
        public string RequestUri { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public byte[] Content { get; set; }
    }
}