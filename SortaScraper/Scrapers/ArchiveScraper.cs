using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Model.Sorta;
using SortaScraper.Support;

namespace SortaScraper.Scrapers
{
    public class ArchiveScraper : IArchiveScraper
    {
        private readonly ISortaClient _sortaClient;

        public ArchiveScraper(ISortaClient sortaClient)
        {
            _sortaClient = sortaClient;
        }

        public async Task<Archive> Scrape()
        {
            // the output instance
            // TODO: UTC or local time?
            var archive = new Archive {DownloadedOn = DateTime.Now};

            // get the headers 
            HttpResponseHeaders headers = await _sortaClient.GetArchiveHeaders();

            // parse the headers
            // TODO: handle weak ETags?
            archive.ETag = headers.ETag.Tag;
            archive.LastModified = headers.GetValues("Last-Modified").FirstOrDefault();

            return archive;
        }
    }
}