using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SmartRoutes.Database;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Heap;
using SmartRoutes.Model;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Graph
{
    public class Graph : IGraph
    {
        private readonly IGraphBuilder Builder;
        private GtfsCollection Collection;
        private Destination[] Destinations;
        public INode[] GraphNodes { get; private set; }

        public Graph(IGraphBuilder Builder, IFibonacciHeap<INode, TimeSpan> Queue)
        {
            this.Builder = Builder;
            GetGtfsEntities();
            GetDestinations();
            GraphNodes = Builder.BuildGraph(Collection.StopTimes, Destinations);
        }

        public void GetDestinations()
        {
            using (var ctx = new Entities())
            {
                Destinations = (from c in ctx.Destinations select c).ToArray();
            }
        }

        public void GetGtfsEntities()
        {
            Collection = new GtfsCollection();

            using (var ctx = new Entities())
            {
                Collection.StopTimes = (from e in ctx.StopTimes select e).ToList();
                Collection.Stops = (from e in ctx.Stops select e).Include(s => s.CloseStops).ToList();
                Collection.Routes = (from e in ctx.Routes select e).ToList();
                Collection.Shapes = (from e in ctx.Shapes select e).ToList();
                Collection.ShapePoints = (from e in ctx.ShapePoints select e).ToList();
                Collection.Blocks = (from e in ctx.Blocks select e).ToList();
                Collection.Agencies = (from e in ctx.Agencies select e).ToList();
                Collection.Archive = ctx.GtfsArchives.OrderBy(e => e.LoadedOn).FirstOrDefault();
                Collection.Trips = (from e in ctx.Trips select e).ToList();
                Collection.ServiceExceptions = (from e in ctx.ServiceExceptions select e).ToList();
                Collection.Services = (from e in ctx.Services select e).ToList();
                Collection.ContainsEntities = true;
            }
        }

        public Stop closestMetroStop(ILocation location)
        {
            double minDistance = double.MaxValue;
            Stop closestStop = null;

            foreach (var stop in Collection.Stops)
            {
                double Distance = location.GetL1DistanceInFeet(stop);

                if (Distance < minDistance)
                {
                    minDistance = Distance;
                    closestStop = stop;
                }
            }

            if (closestStop == null)
            {
                throw new Exception("Closest stop to given location not found.");
            }

            return closestStop;
        }

        public IGtfsNode closestMetroNode(ILocation location, DateTime Time, TimeDirection Direction)
        {
            Stop closestStop = closestMetroStop(location);

            // retrieve metronodes corresponding to this stop
            List<IGtfsNode> nodes = null;
            if (!Builder.StopToNodes.TryGetValue(closestStop.Id, out nodes))
            {
                throw new Exception("Failed to find metro nodes associated with closest stop.");
            }

            double distance = closestStop.GetL1DistanceInFeet(location);
            double walkingTime = distance / Builder.Settings.WalkingFeetPerSecond;
            
            // sort nodes by increasing time;
            var nodesArray = nodes.ToArray();
            Array.Sort(nodesArray, new Comparers.ComparerForTransferSorting());
            IGtfsNode returnNode = null;

            if (Direction == TimeDirection.Forwards)
            {
                DateTime TimeThreshhold = Time + TimeSpan.FromSeconds(walkingTime);
                foreach (var node in nodesArray)
                {
                    if (node.Time >= TimeThreshhold)
                    {
                        returnNode = node;
                        break;
                    }
                }
            }
            else
            {
                DateTime TimeThreshhold = Time - TimeSpan.FromSeconds(walkingTime);
                for (int i = nodesArray.Count() - 1; i >= 0; i--)
                {
                    if (nodesArray[i].Time <= TimeThreshhold)
                    {
                        returnNode = nodesArray[i];
                        break;
                    }
                }
            }

            if (returnNode == null)
            {
                throw new Exception("Failed to find nearby metro node.");
            }

            return returnNode;
        }

        public List<IGtfsNode> GetChildCareNeighbors(IDestinationNode childCareNode, TimeDirection Direction)
        {
            List<NodeBase> UniqueNodeBases = new List<NodeBase>();
            List<IGtfsNode> ReturnNodes = new List<IGtfsNode>();

            var current = childCareNode;
            bool done = false;
            while (!done)
            {
                var neighbors = (Direction == TimeDirection.Backwards)
                    ? current.TimeBackwardNeighbors
                    : current.TimeForwardNeighbors;

                foreach (var neighbor in neighbors.OfType<IGtfsNode>())
                {
                    if (!UniqueNodeBases.Contains(neighbor.BaseNode))
                    {
                        UniqueNodeBases.Add(neighbor.BaseNode);
                        ReturnNodes.Add(neighbor);
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

            return ReturnNodes;
        }
    }
}
