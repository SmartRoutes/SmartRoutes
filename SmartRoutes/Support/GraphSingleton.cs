using System;
using System.Collections.Generic;
using System.Linq;
using SmartRoutes.Demo.OdjfsDatabase;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using SmartRoutes.Graph;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader.Parsers.Gtfs;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.Support
{
    public sealed class GraphSingleton
    {
        private static readonly Lazy<GraphSingleton> LazyInstance = new Lazy<GraphSingleton>(() => new GraphSingleton());

        private readonly Dictionary<int, ChildCare> _childCares;
        private readonly IDictionary<int, StopTime> _stopTimes;

        private GraphSingleton()
        {
            // get Metro models
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
            GtfsCollection gtfsCollection = gtfsFetcher
                .Download(new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip"), null)
                .Result;
            _stopTimes = gtfsCollection.StopTimes.ToDictionary(st => st.Id);

            // get child care models
            var odjfsDatabase = new OdjfsDatabase("OdjfsDatabase");
            ChildCare[] childCares = odjfsDatabase.GetChildCares().Result.ToArray();
            _childCares = childCares.ToDictionary(cc => cc.Id);

            // build the graph
            var graphBuilder = new GraphBuilder();
            Graph = graphBuilder.BuildGraph(gtfsCollection.StopTimes, childCares, GraphBuilderSettings.Default);
        }

        public IGraph Graph { get; private set; }

        public static GraphSingleton Instance
        {
            get { return LazyInstance.Value; }
        }

        public ChildCare GetChildCare(int childCareId)
        {
            ChildCare childCare;
            _childCares.TryGetValue(childCareId, out childCare);
            return childCare;
        }

        public StopTime GetStopTime(int stopTimeId)
        {
            StopTime stopTime;
            _stopTimes.TryGetValue(stopTimeId, out stopTime);
            return stopTime;
        }
    }
}