using System;
using System.Threading.Tasks;
using Model.Odjfs;
using NLog;
using OdjfsHtmlScraper.Parsers;
using OdjfsHtmlScraper.Support;

namespace OdjfsHtmlScraper.Scrapers
{
    public class ChildCareDocumentScraper<T> : IChildCareDocumentScraper<T> where T : ChildCare
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IClient _client;
        private readonly IChildCareDocumentParser<T> _parser;

        public ChildCareDocumentScraper(IClient client, IChildCareDocumentParser<T> parser)
        {
            _client = client;
            _parser = parser;
        }

        public async Task Scrape(T childCare)
        {
            // make sure we have a URL ID
            if (string.IsNullOrWhiteSpace(childCare.ExternalUrlId))
            {
                var exception = new ArgumentNullException("childCare", "The provided child care has a null external URL ID.");
                Logger.ErrorException(string.Format(
                    "Type: '{0}', ExternalUrlId: '{1}'",
                    typeof (T),
                    childCare.ExternalUrlId), exception);
                throw exception;
            }

            // fetch the contents
            byte[] bytes = await _client.GetChildCareDocument(childCare);

            // extract the child care information
            _parser.Parse(childCare, bytes);
        }
    }
}