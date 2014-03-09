using System;
using System.Linq;
using SmartRoutes.Demo.OdjfsDatabase;
using SmartRoutes.Graph;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader.Parsers.Gtfs;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.GraphDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var form = new GraphTester();
            form.ShowDialog();
        }

        private static void SimpleTest()
        {
            /********************************************************************************************
            ******************* Fetch the data from the database and build the graph ********************
            ********************************************************************************************/

            Console.WriteLine("Fetching the SORTA data from the web.");
            DateTime tic = DateTime.Now;
            var gtfsParser = new GtfsCollectionParser(
                new AgencyCsvStreamParser(),
                new RouteCsvStreamParser(),
                new ServiceCsvStreamParser(),
                new ServiceExceptionCsvStreamParser(),
                new ShapePointCsvStreamParser(),
                new StopTimeCsvStreamParser(),
                new StopCsvStreamParser(),
                new TripCsvStreamParser());
            var gtfsFetcher = new EntityCollectionDownloader<GtfsArchive, GtfsCollection>(gtfsParser);
            var gtfsCollection = gtfsFetcher.Download(new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip"), null).Result;
            DateTime toc = DateTime.Now;
            Console.WriteLine("GTFS data fetched in {0} milliseconds.", (toc - tic).TotalMilliseconds);

            Console.WriteLine("Fetching the ODJFS data from the database.");
            tic = DateTime.Now;
            var odjfsDatabase = new OdjfsDatabase("OdjfsDatabase");
            var childCares = odjfsDatabase.GetChildCares().Result;
            toc = DateTime.Now;
            Console.WriteLine("Destination data fetched in {0} milliseconds.", (toc - tic).TotalMilliseconds);

            Console.WriteLine("Creating Graph.");
            tic = DateTime.Now;
            var graphBuilder = new GraphBuilder();
            var graph = graphBuilder.BuildGraph(gtfsCollection.StopTimes, childCares, GraphBuilderSettings.Default);
            toc = DateTime.Now;
            Console.WriteLine("Graph created in {0} milliseconds.", (toc - tic).TotalMilliseconds);
            Console.WriteLine("Finding route...");
            tic = DateTime.Now;

            /********************************************************************************************
            ******************** Example usage of Dijkstras for complete route **************************
            ********************************************************************************************/

            // starting at my address
            var homeLocation = new Location { Latitude = 39.122309, Longitude = -84.507639 };

            // ending at the college of engineering
            var workLocation = new Location { Latitude = 39.133292, Longitude = -84.515099 };

            // have to be at work by 10:30 am
            var atWorkBy = new DateTime(1970, 1, 1, 10, 30, 0);

            // child care selected by name, since properties are not implemented yet
            const string childCareName1 = "ANOINTED HANDS LEARNING CENTER";
            const string childCareName2 = "DIVINE DAY CARE CENTER, INC. II";

            Func<IDestination, bool> criteria1 = dest => dest.Name == childCareName1;
            Func<IDestination, bool> criteria2 = dest => dest.Name == childCareName2;

            var criterion = new[] { criteria1, criteria2 };

            var results = graph.Search(workLocation, homeLocation, atWorkBy, TimeDirection.Backwards, criterion);

            toc = DateTime.Now;
            Console.WriteLine("Route found in in {0} milliseconds.",
                (toc - tic).TotalMilliseconds);

            var result = results.First();
            Console.WriteLine("Displaying Route (Long Results.)");
            foreach (var current in result.LongResults)
            {
                var gtfsCurrent = current.Node as IGtfsNode;
                if (gtfsCurrent != null)
                {
                    Console.WriteLine("{0} -- {1} <Trip {2}> <Route {3}>", current.Node.Name, current.Node.Time, gtfsCurrent.TripId, gtfsCurrent.stopTime.Trip.Route.ShortName);
                }
                else
                {
                    Console.WriteLine("{0} -- {1}", current.Node.Name, current.Node.Time);
                }
            }

            Console.WriteLine("\n\nDisplaying Route (Short Results.)");
            foreach (var current in result.ShortResults)
            {
                var gtfsCurrent = current.Node as IGtfsNode;
                if (gtfsCurrent != null)
                {
                    Console.WriteLine("{0} -- {1} <Trip {2}> <Route {3}>", current.Node.Name, current.Node.Time, gtfsCurrent.TripId, gtfsCurrent.stopTime.Trip.Route.ShortName);
                }
                else
                {
                    Console.WriteLine("{0} -- {1}", current.Node.Name, current.Node.Time);
                }
            }

            Console.WriteLine("fin");
        }
    }
}
