using System;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.GtfsReader.Support;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader;

namespace SmartRoutes.GtfsReader.Readers
{
    public class GtfsCollectionReader : IGtfsCollectionReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEntityCollectionParser<GtfsArchive, GtfsCollection> _gtfsCollectionParser;
        private readonly IGtfsClient _gtfsClient;

        public GtfsCollectionReader(IGtfsClient gtfsClient, IEntityCollectionParser<GtfsArchive, GtfsCollection> gtfsCollectionParser)
        {
            _gtfsClient = gtfsClient;
            _gtfsCollectionParser = gtfsCollectionParser;
        }

        public async Task<GtfsCollection> Download(GtfsArchive currentArchive)
        {
            // does the content indicate a change?
            Logger.Trace("Fetching the newest archive bytes.");
            ClientResponse response = await _gtfsClient.GetArchiveContent();
            var newestGtfsArchive = new GtfsArchive {LoadedOn = DateTime.Now};

            // get the archive contents
            Logger.Trace("The newest archive has {0} bytes ({1} megabytes).", response.Content.LongLength, Math.Round(response.Content.LongLength/(1024.0*1024.0), 2));

            newestGtfsArchive.Hash = response.Content.GetSha256Hash();
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
            GtfsCollection entities = _gtfsCollectionParser.Parse(response.Content);
            entities.Archive = newestGtfsArchive;
            entities.ContainsEntities = true;

            return entities;
        }
    }
}