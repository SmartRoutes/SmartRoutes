using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.GtfsReader.Support;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader;

namespace SmartRoutes.GtfsReader.Readers
{
    public class GtfsCollectionReader : IEntityCollectionReader<GtfsArchive, GtfsCollection>
    {
        public Task<GtfsCollection> Read(string filePath, GtfsArchive currentArchive)
        {
            throw new NotImplementedException();
        }
    }
}
