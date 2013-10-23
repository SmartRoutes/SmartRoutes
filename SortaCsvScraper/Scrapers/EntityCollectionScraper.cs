using System.Threading.Tasks;
using SortaCsvScraper.Parsers;
using SortaCsvScraper.Support;

namespace SortaCsvScraper.Scrapers
{
    public class EntityCollectionScraper : IEntityCollectionScraper
    {
        private readonly IEntityCollectionParser _parser;
        private readonly ISortaClient _sortaClient;

        public EntityCollectionScraper(ISortaClient sortaClient, IEntityCollectionParser parser)
        {
            _sortaClient = sortaClient;
            _parser = parser;
        }

        public async Task<EntityCollection> Scrape()
        {
            // get the zip bytes
            byte[] archiveBytes = await _sortaClient.GetArchiveBytes();

            // parse the entities
            return _parser.Parse(archiveBytes);
        }
    }
}