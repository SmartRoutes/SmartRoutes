using System;
using System.Net;

namespace OdjfsStatusChecker
{
    [Serializable]
    public class DocumentStatus : IEquatable<DocumentStatus>
    {
        public DateTime DateTime { get; set; }
        public string Hash { get; set; }
        public Document Document { get; set; }
        public byte[] Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public bool Equals(DocumentStatus other)
        {
            return other != null && Hash == other.Hash && StatusCode == other.StatusCode;
        }
    }
}