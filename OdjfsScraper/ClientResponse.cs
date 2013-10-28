using System.Net;

namespace OdjfsScraper
{
    public class ClientResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public byte[] Content { get; set; }
    }
}