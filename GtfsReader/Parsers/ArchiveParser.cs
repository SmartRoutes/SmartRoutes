using System;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Scraper;

namespace SmartRoutes.SortaScraper.Parsers
{
    public class ArchiveParser : IArchiveParser
    {
        public Archive Parse(ClientResponseHeaders headers)
        {
            // the output instance
            var archive = new Archive {DownloadedOn = DateTime.Now};

            // parse the headers
            string[] values;
            if (headers.TryGetValue("ETag", out values) && values.Length == 1)
            {
                archive.ETag = values[0];
            }

            return archive;
        }
    }
}