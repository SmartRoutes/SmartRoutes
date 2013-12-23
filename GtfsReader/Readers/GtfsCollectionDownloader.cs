using System;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.GtfsReader.Support;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader;

namespace SmartRoutes.GtfsReader.Readers
{
    public class GtfsCollectionDownloader : IEntityCollectionDownloader<GtfsArchive, GtfsCollection>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ScraperClient _scraperClient;
        private readonly IEntityCollectionParser<GtfsArchive, GtfsCollection> _gtfsCollectionParser;

        public GtfsCollectionDownloader(IEntityCollectionParser<GtfsArchive, GtfsCollection> gtfsCollectionParser)
        {
            _scraperClient = new ScraperClient();
            _gtfsCollectionParser = gtfsCollectionParser;
        }

        public async Task<GtfsCollection> Download(Uri uri, GtfsArchive currentArchive)
        {
            // does the content indicate a change?
            Logger.Trace("Fetching the newest archive bytes.");
            var response = await _scraperClient.GetAsync(uri, HttpCompletionOption.ResponseContentRead);
            ClientResponse clientResponse = await ClientResponse.Create(uri, response);
            var newestGtfsArchive = new GtfsArchive { LoadedOn = DateTime.Now };

            // get the archive contents
            Logger.Trace("The newest archive has {0} bytes ({1} megabytes).", clientResponse.Content.LongLength, Math.Round(clientResponse.Content.LongLength / (1024.0 * 1024.0), 2));

            newestGtfsArchive.Hash = clientResponse.Content.GetSha256Hash();
            Logger.Trace("The newest archive has the following hash: {0}", newestGtfsArchive.Hash);

            if (currentArchive != null &&
                currentArchive.Hash == newestGtfsArchive.Hash)
            {
                Logger.Trace("The newest archive has the same hash, but a different ETag from the previous.");
                return new GtfsCollection
                {
                    Archive = newestGtfsArchive,
                    ContainsEntities = false
                };
            }

            // parse the entities
            Logger.Trace("The newest archive is different. Parsing the newest archive.");
            GtfsCollection entities = _gtfsCollectionParser.Parse(clientResponse.Content);
            entities.Archive = newestGtfsArchive;
            entities.ContainsEntities = true;

            return entities;
        }
    }
}