﻿using System;
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
        private GtfsCollection GtfsCollection;
        private SrdsCollection SrdsCollection;
        public INode[] GraphNodes { get; private set; }

        public Graph(IGraphBuilder Builder, IFibonacciHeap<INode, TimeSpan> Queue)
        {
            this.Builder = Builder;
            GetGtfsEntities();
            GetDestinations();
            GraphNodes = Builder.BuildGraph(GtfsCollection.StopTimes, SrdsCollection.Destinations);
        }

        public void GetDestinations()
        {
            SrdsCollection = new SrdsCollection();

            using (var ctx = new Entities())
            {
                SrdsCollection.AttributeValues = (from e in ctx.AttributeValues select e).ToArray();
                SrdsCollection.Destinations = (from e in ctx.Destinations select e).ToArray();
                SrdsCollection.AttributeKeys = (from e in ctx.AttributeKeys select e).ToArray();
            }
        }

        public void GetGtfsEntities()
        {
            GtfsCollection = new GtfsCollection();

            using (var ctx = new Entities())
            {
                GtfsCollection.StopTimes = (from e in ctx.StopTimes select e).ToArray();
                GtfsCollection.Stops = (from e in ctx.Stops select e).ToArray();
                GtfsCollection.Routes = (from e in ctx.Routes select e).ToArray();
                GtfsCollection.Shapes = (from e in ctx.Shapes select e).ToArray();
                GtfsCollection.ShapePoints = (from e in ctx.ShapePoints select e).ToArray();
                GtfsCollection.Blocks = (from e in ctx.Blocks select e).ToArray();
                GtfsCollection.Agencies = (from e in ctx.Agencies select e).ToArray();
                GtfsCollection.Archive = ctx.GtfsArchives.OrderBy(e => e.LoadedOn).FirstOrDefault();
                GtfsCollection.Trips = (from e in ctx.Trips select e).ToArray();
                GtfsCollection.ServiceExceptions = (from e in ctx.ServiceExceptions select e).ToArray();
                GtfsCollection.Services = (from e in ctx.Services select e).ToArray();
                GtfsCollection.ContainsEntities = true;
            }
        }

        // returns numStops closest metro stops to given location having unique BaseNode properties
        public IEnumerable<Stop> closestMetroStops(ILocation location, int numStops)
        {
            double [] minDistances = new double [numStops];
            Stop [] closestStops = new Stop [numStops];
            for (int i = 0; i < numStops; i++)
            {
                minDistances[i] = double.MaxValue;
                closestStops[i] = null;
            }

            foreach (var stop in GtfsCollection.Stops)
            {
                double distance = location.GetL1DistanceInFeet(stop);
                if (distance > Builder.Settings.MaxFeetFromChildCareToBusStop) continue;
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

        public IEnumerable<IGtfsNode> closestMetroNodes(ILocation location, DateTime Time, TimeDirection Direction, int numNodes)
        {
            var closestStops = closestMetroStops(location, numNodes);
            var returnNodes = new List<IGtfsNode>();

            foreach (var closestStop in closestStops)
            {
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

                if (returnNode != null)
                {
                    returnNodes.Add(returnNode);
                }
            }

            return returnNodes;
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
