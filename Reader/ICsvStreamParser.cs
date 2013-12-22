using System.Collections.Generic;
using System.IO;

namespace SmartRoutes.Reader
{
    public interface ICsvStreamParser<out TOut>
    {
        IEnumerable<TOut> Parse(Stream csvStream);
    }
}