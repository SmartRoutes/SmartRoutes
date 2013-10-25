using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Model.Sorta;
using NLog;
using Scraper;
using SortaScraper.Parsers;
using SortaScraper.Support;

namespace SortaScraper.Scrapers
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
                HttpResponseHeaders headers = await _sortaClient.GetArchiveHeaders();
                newestArchive = _archiveParser.Parse(headers);

                if (newestArchive.ETag == currentArchive.ETag)
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
            HttpResponseMessage response = await _sortaClient.GetArchiveContent();
            newestArchive = _archiveParser.Parse(response.Headers);

            // get the archive contents
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            Logger.Trace("The newest archive has {0} bytes ({1} megabytes).", bytes.LongLength, Math.Round(bytes.LongLength/(1024.0*1024.0), 2));

            newestArchive.Hash = bytes.GetSha256Hash();
            Logger.Trace("The newest archive has the follow SHA-2 (SHA-256) hash: {0}", newestArchive.Hash);

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
            EntityCollection entities = _entityCollectionParser.Parse(bytes);
            entities.Archive = newestArchive;
            entities.ContainsEntities = true;

            return entities;
        }
    }
}