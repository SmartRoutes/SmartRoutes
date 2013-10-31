using System;
using OdjfsScraper;

namespace OdjfsStatusChecker
{
    [Serializable]
    public class DocumentStatus : IEquatable<DocumentStatus>
    {
        public DateTime DateTime { get; set; }
        public string Hash { get; set; }
        public ClientResponse ClientResponse { get; set; }

        public bool Equals(DocumentStatus other)
        {
            return other != null &&
                   Hash == other.Hash &&
                   ((ClientResponse == null &&
                     other.ClientResponse == null) ||
                    (ClientResponse != null &&
                     other.ClientResponse != null &&
                     ClientResponse.StatusCode == other.ClientResponse.StatusCode));
        }
    }
}