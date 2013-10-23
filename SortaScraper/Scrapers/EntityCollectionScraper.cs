using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Model.Sorta;
using Scraper;
using SortaScraper.Parsers;
using SortaScraper.Support;

namespace SortaScraper.Scrapers
{
    public class EntityCollectionScraper : IEntityCollectionScraper
    {
        private readonly ISortaClient _sortaClient;
        private readonly IEntityCollectionParser _entityCollectionParser;
        private readonly IArchiveParser _archiveParser;

        public EntityCollectionScraper(ISortaClient sortaClient, IArchiveParser archiveParser, IEntityCollectionParser entityCollectionParser)
        {
            _sortaClient = sortaClient;
            _archiveParser = archiveParser;
            _entityCollectionParser = entityCollectionParser;
        }

        public async Task<EntityCollection> Scrape(Archive currentArchive)
        {
            // do the headers indicate a change?
            HttpResponseHeaders headers = await _sortaClient.GetArchiveHeaders();
            Archive newestArchive = _archiveParser.Parse(headers);

            if (currentArchive != null &&
                newestArchive.ETag == currentArchive.ETag &&
                newestArchive.LastModified == currentArchive.LastModified)
            {
                return null;
            }

            // does the content indicate a change?
            HttpResponseMessage response = await _sortaClient.GetArchiveContent();
            newestArchive = _archiveParser.Parse(response.Headers);
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            newestArchive.Hash = bytes.GetSha256Hash();

            if (currentArchive != null &&
                currentArchive.Hash == newestArchive.Hash)
            {
                return new EntityCollection
                {
                    Archives = new[] {newestArchive}
                };
            }

            // parse the entities
            EntityCollection entities = _entityCollectionParser.Parse(bytes);
            entities.Archives = new[] {newestArchive};
            return entities;
        }
    }
}