using System;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.Model;
using SmartRoutes.Reader.Parsers;
using SmartRoutes.Reader.Support;

namespace SmartRoutes.Reader.Readers
{
    public class EntityCollectionDownloader<TArchive, TCollection> : BaseEntityCollectionReader<TArchive, TCollection>, IEntityCollectionDownloader<TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ScraperClient _scraperClient;

        public EntityCollectionDownloader(IEntityCollectionParser<TArchive, TCollection> parser) : base(parser)
        {
            _scraperClient = new ScraperClient();
        }

        public async Task<TCollection> Download(Uri uri, TArchive currentArchive)
        {
            // get the bytes
            Logger.Trace("Downloading the newest archive bytes from {0}.", uri);
            HttpResponseMessage response = await _scraperClient.GetAsync(uri, HttpCompletionOption.ResponseContentRead);
            ClientResponse clientResponse = await ClientResponse.Create(uri, response);

            // parse the bytes
            return Parse(clientResponse.Content, currentArchive);
        }
    }
}