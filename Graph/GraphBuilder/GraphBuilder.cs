using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Odjfs.ChildCares;
using SortaScraper.Support;
using Model.Sorta;
using Model.Odjfs;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using NLog;
using Graph.Node;

namespace Graph
{
    public class GraphBuilder : IGraphBuilder
    {
        private readonly IMetroNode _metroNodeMaker;
        private readonly IChildcareNode _childCareNodeMaker;
        private Logger Logger = LogManager.GetCurrentClassLogger();
        private static double MaxFeetBetweenTransfers = 1000;
        private static double WalkingFeetPerSecond = 1.5;

        public GraphBuilder(IChildcareNode childCareNodeMaker, IMetroNode metroNodeMaker) 
        {
            try
            {
                _metroNodeMaker = metroNodeMaker;
                _childCareNodeMaker = childCareNodeMaker;
                Logger.Trace("GraphBuilder object created.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during GraphBuilder object creation.", e);
                throw e;
            }
        }

        public INode[] BuildGraph(EntityCollection collection)
        {
            try
            {
                Logger.Trace("Creating new graph.");

                var MetroNodes = CreateMetroNodes(collection);
                ConnectTrips(MetroNodes);
                //ConnectTransfers(MetroNodes);
                ExperimentalConnectTransfers(MetroNodes, collection);
                //var ChildcareNodes = CreateChildcareNodes(ChildcareCollection);
                //var Graph = CombineNodes(MetroNodes, ChildcareNodes);
                Logger.Trace("Graph created successfully.");
                return MetroNodes;
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during creation of graph.", e);
                Console.WriteLine(e);
                throw e;
            }
        }

        private IMetroNode[] CreateMetroNodes(EntityCollection collection)
        {
            Logger.Trace("Creating Metro Nodes.");

            try
            {
                var MetroNodes = (from stopTime in collection.StopTimes
                                  select _metroNodeMaker.CreateNode(stopTime))
                                 .ToArray();

                Logger.Trace("Metro Nodes created successfully.");
                return MetroNodes;
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during creation of Metro nodes.", e);
                throw e;
            }
        }

        private void ConnectTrips(IMetroNode[] MetroNodes)
        {
            Logger.Trace("Connecting Metro Trips.");
            try
            {
                Array.Sort(MetroNodes, new Comparers.ComparerForTripSorting());

                for (int i = 1; i < MetroNodes.Count(); i++)
                {
                    IMetroNode node = MetroNodes[i];
                    IMetroNode previousNode = MetroNodes[i-1];

                    if (node.TripID == previousNode.TripID)
                    {
                        node.DownwindNeighbors.Add(previousNode);
                        previousNode.UpwindNeighbors.Add(node);
                    }
                }
                Logger.Trace("Metro Trips connected successfully.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during connection of Metro Trips.", e);
                throw e;
            }
        }

        private void ConnectTransfers(IMetroNode[] MetroNodes)
        {
            Logger.Trace("Connecting Metro Transfers.");
            try
            {
                Array.Sort(MetroNodes, new Comparers.ComparerForTransferSorting());

                for (int i = 1; i < MetroNodes.Count(); i++)
                {
                    IMetroNode node = MetroNodes[i];
                    IMetroNode previousNode = MetroNodes[i-1];

                    if (node.StopID == previousNode.StopID)
                    {
                        node.DownwindNeighbors.Add(previousNode);
                        previousNode.UpwindNeighbors.Add(node);
                    }
                }
                Logger.Trace("Metro Transfers connected successfully.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during connection of Metro Transfers.", e);
                throw e;
            }
        }

        // connect transfers to stops within a given distance
        private void ExperimentalConnectTransfers(IMetroNode[] MetroNodes, EntityCollection collection)
        {
            // this sorting assures Nodes which have same stopID are sorted by ascending Time
            Array.Sort(MetroNodes, new Comparers.ComparerForTransferSorting());

            Dictionary<int, List<int>> StopToNearest; // from StopID to list of StopID's of nearest Stops
            Dictionary<int, List<IMetroNode>> StopToNodes; // from StopID to set of Nodes with given StopID

            StopToNearest = new Dictionary<int, List<int>>();
            StopToNodes = new Dictionary<int, List<IMetroNode>>();

            var enumerator1 = collection.Stops.GetEnumerator();

            while (enumerator1.MoveNext())
            {
                var Stop1 = enumerator1.Current;
                var NearestList = new List<int>();

                // associate Id's of closest stops with this stop
                var enumerator2 = collection.Stops.GetEnumerator();

                while (enumerator2.MoveNext())
                {
                    var Stop2 = enumerator2.Current;

                    if (GetDistanceInFeet(Stop1, Stop2) < MaxFeetBetweenTransfers)
                    {
                        NearestList.Add(Stop2.Id);
                    }
                }

                StopToNearest.Add(Stop1.Id, NearestList);

                // associate MetroNodes which contain this stop with this stop
                var NodeList = new List<IMetroNode>();

                for (int i = 0; i < MetroNodes.Count(); i++)
                {
                    if (MetroNodes[i].StopID == Stop1.Id)
                    {
                        NodeList.Add(MetroNodes[i]);
                    }
                }

                StopToNodes.Add(Stop1.Id, NodeList);
            }

            // loop through MetroNodes and connect transfers
            foreach (var node1 in MetroNodes)
            {
                // obtain stopID's of nodes in transfer distance
                List<int> NearestIDs = null;
                if (!StopToNearest.TryGetValue(node1.StopID, out NearestIDs))
                {
                    continue;
                }

                foreach (var ID in NearestIDs)
                {
                    List<IMetroNode> Nodes = null;
                    if (!StopToNodes.TryGetValue(ID, out Nodes))
                    {
                        continue;
                    }

                    // calculate walking time, will be same for all nodes in list, so use first
                    double WalkingTime = GetDistanceInFeet(node1, Nodes.First()) / WalkingFeetPerSecond;
                    DateTime MinTime = node1.Time + new TimeSpan(0, 0, (int)Math.Ceiling(WalkingTime));

                    // thanks to sorting, these nodes are iterated in ascending time
                    foreach (var node2 in Nodes)
                    {
                        if (node2.Time > MinTime)
                        {
                            node1.UpwindNeighbors.Add(node2);
                            node2.DownwindNeighbors.Add(node1);
                            break;
                        }
                    }
                }
            }
        }

        private IChildcareNode[] CreateChildcareNodes(IEnumerable<ChildCare> ChildcareCollection)
        {
            return null;
        }

        private INode[] CombineNodes(IMetroNode[] MetroNodes, IChildcareNode[] ChildcareNodes)
        {
            return MetroNodes;
        }

        private double GetDistanceInFeet(double Lat1, double Lon1, double Lat2, double Lon2)
        {
            double R = 20092000.0; // radius of earth in feet
            double dx = R * Math.Cos(Lat1) * (Lon2 - Lon1);
            double dy = R * (Lat2 - Lat1);
            return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        }

        private double GetDistanceInFeet(INode Node1, INode Node2)
        {
            return GetDistanceInFeet(Node1.Latitude, Node1.Longitude, Node2.Latitude, Node2.Longitude);
        }

        private double GetDistanceInFeet(Stop Stop1, Stop Stop2)
        {
            return GetDistanceInFeet(Stop1.Latitude, Stop1.Longitude, Stop2.Latitude, Stop2.Longitude);
        }
    }
}
