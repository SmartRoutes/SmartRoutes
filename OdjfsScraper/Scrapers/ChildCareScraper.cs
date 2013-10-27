using System;
using System.Threading.Tasks;
using Model.Odjfs.ChildCares;
using NLog;
using OdjfsScraper.Parsers;
using OdjfsScraper.Support;

namespace OdjfsScraper.Scrapers
{
    public class ChildCareScraper<T> : IChildCareScraper<T> where T : ChildCare
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IOdjfsClient _odjfsClient;
        private readonly IChildCareParser<T> _parser;

        public ChildCareScraper(IOdjfsClient odjfsClient, IChildCareParser<T> parser)
        {
            _odjfsClient = odjfsClient;
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
            byte[] bytes = await _odjfsClient.GetChildCareDocument(childCare);

            // extract the child care information
            _parser.Parse(childCare, bytes);
        }
    }
}