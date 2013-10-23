using System.IO;
using System.Linq;
using Ionic.Zip;
using Model.Sorta;
using Scraper;
using SortaCsvScraper.Support;

namespace SortaCsvScraper.Parsers
{
    public class EntityCollectionParser : IEntityCollectionParser
    {
        private readonly ICsvStreamParser<Agency> _agencyParser;
        private readonly ICsvStreamParser<Route> _routeParser;
        private readonly ICsvStreamParser<ServiceException> _serviceExceptionParser;
        private readonly ICsvStreamParser<Service> _serviceParser;
        private readonly ICsvStreamParser<ShapePoint> _shapePointParser;
        private readonly ICsvStreamParser<Stop> _stopParser;
        private readonly ICsvStreamParser<StopTime> _stopTimeParser;
        private readonly ICsvStreamParser<Trip> _tripParser;

        public EntityCollectionParser(ICsvStreamParser<Agency> agencyParser, ICsvStreamParser<Route> routeParser, ICsvStreamParser<Service> serviceParser, ICsvStreamParser<ServiceException> serviceExceptionParser, ICsvStreamParser<ShapePoint> shapePointParser, ICsvStreamParser<StopTime> stopTimeParser, ICsvStreamParser<Stop> stopParser, ICsvStreamParser<Trip> tripParser)
        {
            _agencyParser = agencyParser;
            _routeParser = routeParser;
            _serviceParser = serviceParser;
            _serviceExceptionParser = serviceExceptionParser;
            _shapePointParser = shapePointParser;
            _stopTimeParser = stopTimeParser;
            _stopParser = stopParser;
            _tripParser = tripParser;
        }

        public EntityCollection Parse(byte[] bytes)
        {
            // initialize the collection
            var collection = new EntityCollection();

            // extract files from the zip
            using (ZipFile zipFile = ZipFile.Read(new MemoryStream(bytes)))
            {
                foreach (ZipEntry entry in zipFile.Entries)
                {
                    if (entry.IsDirectory)
                    {
                        continue;
                    }

                    switch (entry.FileName)
                    {
                        case "agency.txt":
                            collection.Agencies = _agencyParser.Parse(entry.InputStream).ToArray();
                            break;
                        case "calendar.txt":
                            collection.Services = _serviceParser.Parse(entry.InputStream).ToArray();
                            break;
                        case "calendar_dates.txt":
                            collection.ServiceException = _serviceExceptionParser.Parse(entry.InputStream).ToArray();
                            break;
                        case "routes.txt":
                            collection.Routes = _routeParser.Parse(entry.InputStream).ToArray();
                            break;
                        case "shapes.txt":
                            collection.ShapePoints = _shapePointParser.Parse(entry.InputStream).ToArray();
                            collection.Shapes = collection
                                .ShapePoints
                                .Select(s => s.ShapeId)
                                .Distinct()
                                .Select(i => new Shape {Id = i});
                            break;
                        case "stop_times.txt":
                            collection.StopTimes = _stopTimeParser.Parse(entry.InputStream).ToArray();
                            break;
                        case "stops.txt":
                            collection.Stops = _stopParser.Parse(entry.InputStream).ToArray();
                            break;
                        case "trips.txt":
                            collection.Trips = _tripParser.Parse(entry.InputStream).ToArray();
                            collection.Blocks = collection
                                .Trips
                                .Where(t => t.BlockId.HasValue)
                                .Select(t => t.BlockId.Value)
                                .Distinct()
                                .Select(i => new Block {Id = i});
                            break;
                    }
                }
            }

            return collection;
        }
    }
}