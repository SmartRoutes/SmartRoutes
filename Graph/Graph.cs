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

        public Stop GetClosestGtfsStop(ILocation location)
        {
            double minDistance = double.MaxValue;
            Stop closestStop = null;

            foreach (var stop in _stops)
            {
                double distance = location.GetL1DistanceInFeet(stop);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestStop = stop;
                }
            }

            if (closestStop == null)
            {
                throw new Exception("Closest stop to given location not found.");
            }

            return closestStop;
        }

        public IGtfsNode GetClosestGtfsNode(ILocation location, DateTime time, TimeDirection direction)
        {
            Stop closestStop = GetClosestGtfsStop(location);

            // retrieve GTFS nodes corresponding to this stop
            List<IGtfsNode> nodes;
            if (!_stopToNodes.TryGetValue(closestStop.Id, out nodes))
            {
                throw new Exception("Failed to find GTFS nodes associated with closest stop.");
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

            if (returnNode == null)
            {
                throw new Exception("Failed to find nearby GTFS node.");
            }

            return returnNode;
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
