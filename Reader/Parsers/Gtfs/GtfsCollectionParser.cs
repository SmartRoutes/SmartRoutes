using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Reader.Parsers.Gtfs
{
    public class GtfsCollectionParser : EntityCollectionParser<GtfsArchive, GtfsCollection>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICsvStreamParser<Agency> _agencyParser;
        private readonly ICsvStreamParser<Route> _routeParser;
        private readonly ICsvStreamParser<ServiceException> _serviceExceptionParser;
        private readonly ICsvStreamParser<Service> _serviceParser;
        private readonly ICsvStreamParser<ShapePoint> _shapePointParser;
        private readonly ICsvStreamParser<Stop> _stopParser;
        private readonly ICsvStreamParser<StopTime> _stopTimeParser;
        private readonly ICsvStreamParser<Trip> _tripParser;

        public GtfsCollectionParser(ICsvStreamParser<Agency> agencyParser, ICsvStreamParser<Route> routeParser, ICsvStreamParser<Service> serviceParser, ICsvStreamParser<ServiceException> serviceExceptionParser, ICsvStreamParser<ShapePoint> shapePointParser, ICsvStreamParser<StopTime> stopTimeParser, ICsvStreamParser<Stop> stopParser, ICsvStreamParser<Trip> tripParser)
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

        protected override GtfsCollection Parse(IDictionary<string, Func<Stream>> streams)
        {
            // initialize
            var collection = new GtfsCollection();

            // populate
            collection.Agencies = _agencyParser.Parse(GetStream(streams, "agency.txt")).ToArray();

            collection.Services = _serviceParser.Parse(GetStream(streams, "calendar.txt")).ToArray();

            collection.ServiceExceptions = _serviceExceptionParser.Parse(GetStream(streams, "calendar_dates.txt")).ToArray();

            collection.Routes = _routeParser.Parse(GetStream(streams, "routes.txt")).ToArray();

            collection.ShapePoints = _shapePointParser.Parse(GetStream(streams, "shapes.txt")).ToArray();
            collection.Shapes = collection
                .ShapePoints
                .Select(s => s.ShapeId)
                .Distinct()
                .Select(i => new Shape {Id = i})
                .ToArray();

            collection.StopTimes = _stopTimeParser.Parse(GetStream(streams, "stop_times.txt")).ToArray();

            collection.Stops = _stopParser.Parse(GetStream(streams, "stops.txt")).ToArray();

            collection.Trips = _tripParser.Parse(GetStream(streams, "trips.txt")).ToArray();
            collection.Blocks = collection
                .Trips
                .Where(t => t.BlockId.HasValue)
                .Select(t => t.BlockId.Value)
                .Distinct()
                .Select(i => new Block {Id = i})
                .ToArray();


            // stitch up
            Logger.Trace("Associating GtfsCollection entities.");
            Associate(collection);

            return collection;
        }

        private static void Associate(GtfsCollection collection)
        {
            // Route.Agency
            IDictionary<string, Agency> agencies = collection.Agencies.ToDictionary(a => a.Id);
            foreach (Route route in collection.Routes)
            {
                route.Agency = agencies[route.AgencyId];
                route.Agency.Routes.Add(route);
            }

            // ServiceException.Service
            IDictionary<int, Service> services = collection.Services.ToDictionary(s => s.Id);
            for (int i = 0; i < collection.ServiceExceptions.Length; i++)
            {
                ServiceException serviceException = collection.ServiceExceptions[i];
                serviceException.Id = i + 1;
                serviceException.Service = services[serviceException.ServiceId];
                serviceException.Service.ServiceExceptions.Add(serviceException);
            }

            // ShapePoint.Shape
            IDictionary<int, Shape> shapes = collection.Shapes.ToDictionary(s => s.Id);
            for (int i = 0; i < collection.ShapePoints.Length; i++)
            {
                ShapePoint shapePoint = collection.ShapePoints[i];
                shapePoint.Id = i + 1;
                shapePoint.Shape = shapes[shapePoint.ShapeId];
                shapePoint.Shape.ShapePoints.Add(shapePoint);
            }

            // Stop.ParentStop
            IDictionary<int, Stop> stops = collection.Stops.ToDictionary(s => s.Id);
            foreach (Stop stop in collection.Stops)
            {
                if (stop.ParentId.HasValue)
                {
                    stop.ParentStop = stops[stop.ParentId.Value];
                    stop.ParentStop.ChildStops.Add(stop);
                }
            }

            // StopTime.Trip, StopTime.Stop
            IDictionary<int, Trip> trips = collection.Trips.ToDictionary(t => t.Id);
            for (int i = 0; i < collection.StopTimes.Length; i++)
            {
                StopTime stopTime = collection.StopTimes[i];
                stopTime.Id = i + 1;
                stopTime.Trip = trips[stopTime.TripId];
                stopTime.Trip.StopTimes.Add(stopTime);

                stopTime.Stop = stops[stopTime.StopId];
                stopTime.Stop.StopTimes.Add(stopTime);
            }

            // Trip.Route, Trip.Service, Trip.Block, Trip.Shape
            IDictionary<int, Route> routes = collection.Routes.ToDictionary(r => r.Id);
            IDictionary<int, Block> blocks = collection.Blocks.ToDictionary(b => b.Id);
            foreach (Trip trip in collection.Trips)
            {
                trip.Route = routes[trip.RouteId];
                trip.Route.Trips.Add(trip);

                trip.Service = services[trip.ServiceId];
                trip.Service.Trips.Add(trip);

                if (trip.BlockId.HasValue)
                {
                    trip.Block = blocks[trip.BlockId.Value];
                    trip.Block.Trips.Add(trip);
                }

                if (trip.ShapeId.HasValue)
                {
                    trip.Shape = shapes[trip.ShapeId.Value];
                    trip.Shape.Trips.Add(trip);
                }
            }
        }
    }
}