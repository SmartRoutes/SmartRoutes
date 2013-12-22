using System;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.GtfsReader.Parsers;
using SmartRoutes.GtfsReader.Support;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader;

namespace SmartRoutes.GtfsReader.Scrapers
{
    public class EntityCollectionScraper : IEntityCollectionScraper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IArchiveParser _archiveParser;
        private readonly IEntityCollectionParser _entityCollectionParser;
        private readonly ISortaClient _sortaClient;

        public EntityCollectionScraper(ISortaClient sortaClient, IArchiveParser archiveParser, IEntityCollectionParser entityCollectionParser)
        {
            _sortaClient = sortaClient;
            _archiveParser = archiveParser;
            _entityCollectionParser = entityCollectionParser;
        }

        public async Task<EntityCollection> Scrape(Archive currentArchive)
        {
            Archive newestArchive;

            if (currentArchive != null)
            {
                Logger.Trace("An Archive instance was provided. Checking the newest headers.");
                // do the headers indicate a change?
                ClientResponseHeaders headers = await _sortaClient.GetArchiveHeaders();
                newestArchive = _archiveParser.Parse(headers);

                if (newestArchive.ETag != null && newestArchive.ETag == currentArchive.ETag)
                {
                    Logger.Trace("The ETag has not changed.");
                    return null;
                }
                Logger.Trace("The ETag has changed.");
            }
            else
            {
                Logger.Trace("No Archive instance was provided.");
            }

            // does the content indicate a change?
            Logger.Trace("Fetching the newest archive bytes.");
            ClientResponse response = await _sortaClient.GetArchiveContent();
            newestArchive = _archiveParser.Parse(response.Headers);

            // get the archive contents
            Logger.Trace("The newest archive has {0} bytes ({1} megabytes).", response.Content.LongLength, Math.Round(response.Content.LongLength/(1024.0*1024.0), 2));

            newestArchive.Hash = response.Content.GetSha256Hash();
            Logger.Trace("The newest archive has the following hash: {0}", newestArchive.Hash);

            if (currentArchive != null &&
                currentArchive.Hash == newestArchive.Hash)
            {
                Logger.Trace("The newest archive has the same hash, but a different ETag from the previous.");
                return new EntityCollection
                {
                    Archive = newestArchive,
                    ContainsEntities = false
                };
            }

            // parse the entities
            Logger.Trace("The newest archive is different. Parsing the newest archive.");
            EntityCollection entities = _entityCollectionParser.Parse(response.Content);
            entities.Archive = newestArchive;
            entities.ContainsEntities = true;

            return entities;
        }
    }
}