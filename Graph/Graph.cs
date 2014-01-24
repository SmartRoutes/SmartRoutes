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

        // returns numStops closest metro stops to given location having unique BaseNode properties
        public IEnumerable<Stop> GetClosestGtfsStops(ILocation location, int numStops)
        {
            var minDistances = new double [numStops];
            var closestStops = new Stop [numStops];
            for (int i = 0; i < numStops; i++)
            {
                minDistances[i] = double.MaxValue;
                closestStops[i] = null;
            }

            foreach (var stop in _stops)
            {
                double distance = location.GetL1DistanceInFeet(stop);
                if (distance > _settings.MaxFeetFromDestinationToStop) continue;
                var currentStop = stop;

                for (int i = 0; i < numStops; i++)
                {
                    if (distance < minDistances[i])
                    {
                        var tempDistance = minDistances[i];
                        var tempStop = closestStops[i];
                        minDistances[i] = distance;
                        closestStops[i] = currentStop;
                        distance = tempDistance;
                        currentStop = tempStop;
                    }
                }
            }

            return closestStops.Where(s => s != null);
        }

        public IEnumerable<IGtfsNode> GetClosestGtfsNodes(ILocation location, DateTime time, TimeDirection direction, int numNodes)
        {
            var closestStops = GetClosestGtfsStops(location, numNodes);
            var returnNodes = new List<IGtfsNode>();

            foreach (var closestStop in closestStops)
            {
                // retrieve metronodes corresponding to this stop
                List<IGtfsNode> nodes;
                if (!_stopToNodes.TryGetValue(closestStop.Id, out nodes))
                {
                    throw new Exception("Failed to find metro nodes associated with closest stop.");
                }

                double distance = closestStop.GetL1DistanceInFeet(location);
                double walkingTime = distance / _settings.WalkingFeetPerSecond;

                // sort nodes by increasing time;
                var nodesArray = nodes.ToArray();
                Array.Sort(nodesArray, new Comparers.ComparerForTransferSorting());
                IGtfsNode returnNode = null;

                if (direction == TimeDirection.Forwards)
                {
                    DateTime timeThreshhold = time + TimeSpan.FromSeconds(walkingTime);
                    foreach (var node in nodesArray)
                    {
                        if (node.Time >= timeThreshhold)
                        {
                            returnNode = node;
                            break;
                        }
                    }
                }
                else
                {
                    DateTime timeThreshhold = time - TimeSpan.FromSeconds(walkingTime);
                    for (int i = nodesArray.Count() - 1; i >= 0; i--)
                    {
                        if (nodesArray[i].Time <= timeThreshhold)
                        {
                            returnNode = nodesArray[i];
                            break;
                        }
                    }
                }

                if (returnNode != null)
                {
                    returnNodes.Add(returnNode);
                }
            }

            return returnNodes;
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
