using System.Collections.Generic;
using System.IO;

namespace SmartRoutes.Scraper
{
    public interface ICsvStreamParser<out TOut>
    {
        IEnumerable<TOut> Parse(Stream csvStream);
    }
}