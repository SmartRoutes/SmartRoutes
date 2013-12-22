using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Database;
using SmartRoutes.GtfsReader.Support;
using SmartRoutes.GtfsReader.Parsers;

namespace SmartRoutes.Graph
{
    class DatabaseLoader
    {
        private readonly IEntityCollectionParser _parser;

        public DatabaseLoader(IEntityCollectionParser parser)
        {
            _parser = parser;
        }

        public async Task loadDatabaseFromFile(Byte[] fileBytes)
        {
            Console.WriteLine("Parsing entity collection from file");
            EntityCollection collection = _parser.Parse(fileBytes);

            using (var ctx = new Entities())
            {
                ctx.Truncate();

                using (var inserter = new FastInserter<Entities>(ctx, 1000))
                {
                    Console.WriteLine(String.Format("Adding {0} new Agency records.", collection.Agencies.Count()));
                    await inserter.AddRangeAsync(collection.Agencies);

                    Console.WriteLine(String.Format("Adding {0} new Service records.", collection.Services.Count()));
                    await inserter.AddRangeAsync(collection.Services);

                    Console.WriteLine(String.Format("Adding {0} new ServiceException records.", collection.ServiceExceptions.Count()));
                    await inserter.AddRangeAsync(collection.ServiceExceptions);

                    Console.WriteLine(String.Format("Adding {0} new Route records.", collection.Routes.Count()));
                    await inserter.AddRangeAsync(collection.Routes);

                    Console.WriteLine(String.Format("Adding {0} new Shape records.", collection.Shapes.Count()));
                    await inserter.AddRangeAsync(collection.Shapes);

                    Console.WriteLine(String.Format("Adding {0} new ShapePoint records.", collection.ShapePoints.Count()));
                    await inserter.AddRangeAsync(collection.ShapePoints);

                    Console.WriteLine(String.Format("Adding {0} new Block records.", collection.Blocks.Count()));
                    await inserter.AddRangeAsync(collection.Blocks);

                    Console.WriteLine(String.Format("Adding {0} new Trip records.", collection.Trips.Count()));
                    await inserter.AddRangeAsync(collection.Trips);

                    Console.WriteLine(String.Format("Adding {0} new Stop records.", collection.Stops.Count()));
                    await inserter.AddRangeAsync(collection.Stops);

                    Console.WriteLine(String.Format("Adding {0} new StopTime records.", collection.StopTimes.Count()));
                    await inserter.AddRangeAsync(collection.StopTimes);

                    //Console.WriteLine("Adding the new Archive record.");
                    //await inserter.AddAsync(collection.Archive);
                }
            }
            return;
        }
    }
}
