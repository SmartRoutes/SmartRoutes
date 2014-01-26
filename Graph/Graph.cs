using System;
using System.Collections.Generic;
using System.Linq;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model;

namespace SmartRoutes.Graph
{
    public class Graph : IGraph
    {
        private readonly Stop[] _stops;
        public INode[] GraphNodes { get; private set; }
        private readonly GraphBuilderSettings _settings;
        private readonly IDictionary<int, List<IGtfsNode>> _stopToNodes;

        public Graph(IEnumerable<Stop> stops, IDictionary<int, List<IGtfsNode>> stopToNodes, IEnumerable<INode> nodes, GraphBuilderSettings settings)
        {
            _stops = stops.ToArray();
            _stopToNodes = stopToNodes;
            GraphNodes = nodes.ToArray();
            _settings = settings;
        }

        public IEnumerable<IGtfsNode> GetClosestGtfsNodes(ILocation location, DateTime time, TimeDirection direction)
        {
            Func<Stop, double> stopDistanceOrdering = s => s.GetL1DistanceInFeet(location);
            Func<Stop, bool> stopDistanceFilter = s => s.GetL1DistanceInFeet(location) < _settings.MaxFeetFromDestinationToStop;

            Func<IGtfsNode, TimeSpan> walkingTime = n =>
            {
                double distance = n.GetL1DistanceInFeet(location);
                return TimeSpan.FromSeconds(distance / _settings.WalkingFeetPerSecond);
            };

            Func<IGtfsNode, bool> nodeTimeFilter;
            Func<IGtfsNode, TimeSpan> nodeTimeOrdering;

            if (direction == TimeDirection.Forwards)
            {
                nodeTimeFilter = n => n.Time - walkingTime(n) >= time;
                nodeTimeOrdering = n => n.Time - time;
            }
            else
            {
                nodeTimeFilter = n => n.Time + walkingTime(n) <= time;
                nodeTimeOrdering = n => time - n.Time;
            }

            var closeStops = _stops.Where(stopDistanceFilter).OrderBy(stopDistanceOrdering);

            var closeNodes = closeStops.SelectMany(s => {
                List<IGtfsNode> relatedNodes = null;
                _stopToNodes.TryGetValue(s.Id, out relatedNodes);
                return relatedNodes;
            }).Where(nodeTimeFilter).OrderBy(nodeTimeOrdering);

            IEnumerable<IGtfsNode> orderedCloseNodes;
            if (direction == TimeDirection.Forwards)
            {
                orderedCloseNodes = closeNodes.OrderBy(n => n.Time + walkingTime(n));
            }
            else
            {
                orderedCloseNodes = closeNodes.OrderByDescending(n => n.Time - walkingTime(n));
            }

            // return the closest node from each trip
            var returnVal = orderedCloseNodes.GroupBy(n => new Tuple<int, int?>(n.RouteId, n.BlockId)).Select(g => g.First());
            return returnVal.ToArray();
        }

        public IEnumerable<IGtfsNode> GetDestinationNeighbors(IDestinationNode destinationNode, TimeDirection direction)
        {
            var uniqueNodeBases = new List<NodeBase>();
            var returnNodes = new List<IGtfsNode>();

            var current = destinationNode;
            bool done = false;
            while (!done)
            {
                var neighbors = (direction == TimeDirection.Backwards)
                    ? current.TimeBackwardNeighbors
                    : current.TimeForwardNeighbors;

                foreach (var neighbor in neighbors.OfType<IGtfsNode>())
                {
                    if (!uniqueNodeBases.Contains(neighbor.BaseNode))
                    {
                        uniqueNodeBases.Add(neighbor.BaseNode);
                        returnNodes.Add(neighbor);
                    }
                }

                done = true;

                foreach (var neighbor in neighbors.OfType<IDestinationNode>())
                {
                    if (neighbor.BaseNode == current.BaseNode)
                    {
                        done = false;
                        current = neighbor;
                        break;
                    }
                }
            }

            return returnNodes;
        }
    }
}
