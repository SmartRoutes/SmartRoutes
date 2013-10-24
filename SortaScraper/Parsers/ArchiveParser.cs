using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Model.Sorta;

namespace SortaScraper.Parsers
{
    public class ArchiveParser : IArchiveParser
    {
        public Archive Parse(HttpResponseHeaders headers)
        {
            // the output instance
            // TODO: UTC or local time?
            var archive = new Archive {DownloadedOn = DateTime.Now};

            // parse the headers
            archive.ETag = headers.ETag.Tag;

            return archive;
        }
    }
}