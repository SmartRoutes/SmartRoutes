using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Database;
using Database.Contexts;
using Graph.Node;
using SortaScraper.Support;
using SortaScraper.Scrapers;
using SortaDataChecker;
using SmartRoutes.Model.Sorta;
using SmartRoutes.Model.Odjfs.ChildCares;
using Heap;

namespace Graph
{
    internal enum NodeState
    {
        Open, Closed
    }

    public enum Direction 
    { 
        Upwind, Downwind 
    }

    public class NodeInfo
    {
        internal FibHeapHandle<INode, TimeSpan> handle;
        internal TimeSpan travelTime;
        internal NodeState state;
        public INode node;
        public NodeInfo parent;
    }

    public class Graph : IGraph
    {
        private readonly IGraphBuilder Builder;
        private EntityCollection Collection;
        private ChildCare[] ChildCares;
        private Dictionary<INode, NodeInfo> SearchInfo { get; set; }
        public INode[] GraphNodes { get; private set; }
        public IFibonacciHeap<INode, TimeSpan> Queue { get; set; }

        public Graph(IGraphBuilder Builder, IFibonacciHeap<INode, TimeSpan> Queue)
        {
            this.Queue = Queue;
            this.Builder = Builder;
            GetSortaEntities();
            GetChildCares();
            GraphNodes = Builder.BuildGraph(Collection, ChildCares);
        }

        public void GetChildCares()
        {
            using (var ctx = new OdjfsEntities())
            {
                ChildCares = (from c in ctx.ChildCares select c).ToArray();
            }
        }

        public void GetSortaEntities()
        {
            Collection = new EntityCollection();

            using (var ctx = new SortaEntities())
            {
                Collection.StopTimes = (from e in ctx.StopTimes select e).ToList();
                Collection.Stops = (from e in ctx.Stops select e).Include(s => s.CloseStops).ToList();
                Collection.Routes = (from e in ctx.Routes select e).ToList();
                Collection.Shapes = (from e in ctx.Shapes select e).ToList();
                Collection.ShapePoints = (from e in ctx.ShapePoints select e).ToList();
                Collection.Blocks = (from e in ctx.Blocks select e).ToList();
                Collection.Agencies = (from e in ctx.Agencies select e).ToList();
                Collection.Archive = ctx.Archives.OrderBy(e => e.DownloadedOn).FirstOrDefault();
                Collection.Trips = (from e in ctx.Trips select e).ToList();
                Collection.ServiceExceptions = (from e in ctx.ServiceException select e).ToList();
                Collection.Services = (from e in ctx.Services select e).ToList();
                Collection.ContainsEntities = true;
            }
        }

        public NodeInfo Dijkstras(ISet<INode> StartNodes, Func<INode, bool> GoalCheck, Direction direction)
        {
            SearchInfo = new Dictionary<INode, NodeInfo>();

            // assign search info to StartNodes and place them in queue
            foreach (var node in StartNodes)
            {
                var nodeInfo = new NodeInfo();
                nodeInfo.state = NodeState.Closed;
                nodeInfo.travelTime = new TimeSpan(0);
                nodeInfo.handle = Queue.Insert(node, nodeInfo.travelTime);
                SearchInfo.Add(node, nodeInfo);
            }

            while (!Queue.Empty())
            {
                INode current = Queue.DeleteMin();

                // get search info
                NodeInfo currentInfo = null;
                if (!SearchInfo.TryGetValue(current, out currentInfo))
                { 
                    throw new KeyNotFoundException("Node removed from heap did not have associated search info: ");
                }

                // check for completion
                if (GoalCheck(current))
                {
                    return currentInfo;
                }

                // loop through neighbors and handle business
                var Neighbors = (direction == Direction.Upwind) ?
                    current.UpwindNeighbors : current.DownwindNeighbors;

                foreach (var neighbor in Neighbors)
                {
                    NodeInfo neighborInfo = null;
                    if (!SearchInfo.TryGetValue(neighbor, out neighborInfo))
                    { 
                        // node is new, give it search info and place in queue
                        neighborInfo = new NodeInfo();
                        neighborInfo.node = neighbor;
                        neighborInfo.parent = currentInfo;
                        neighborInfo.state = NodeState.Open;
                        neighborInfo.travelTime = (direction == Direction.Upwind)
                            ? currentInfo.travelTime + (neighbor.Time - current.Time)
                            : currentInfo.travelTime + (current.Time - neighbor.Time);
                        neighborInfo.handle = Queue.Insert(neighbor, neighborInfo.travelTime);
                    }
                    else
                    { 
                        // neighbor is in queue, check state
                        if (neighborInfo.state == NodeState.Open)
                        {
                            // update neighborInfo if this route is better
                            TimeSpan newTravelTime = (direction == Direction.Upwind)
                                ? currentInfo.travelTime + (neighbor.Time - current.Time)
                                : currentInfo.travelTime + (current.Time - neighbor.Time);
                            if (newTravelTime < neighborInfo.travelTime)
                            {
                                // update search info and update queue for new key
                                neighborInfo.travelTime = newTravelTime;
                                neighborInfo.parent = currentInfo;
                                Queue.UpdateKey(neighborInfo.handle, newTravelTime);
                            }
                        }
                    }
                }

                // and we're done with current
                currentInfo.state = NodeState.Closed;
            }

            throw new Exception("Dijkstras did not reach a goal node.");
        }
    }
}
