using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Model.Sorta;

namespace SmartRoutes.Graph.Test
{
    [TestClass]
    public class GraphBuilderTest
    {
        [TestMethod]
        public void OneTrip()
        {
            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);
            StopTime[] trip = GetStopTimeCircuit(new DateTime(1970, 1, 1, 10, 0, 0), 1, 5);

            // ACT
            INode[] nodes = graphBuilder.BuildGraph(trip, Enumerable.Empty<ChildCare>());

            // VERIFY
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            VerifyCircuit(trip, subgraphs[0]);
        }

        [TestMethod]
        public void TwoTrips_NoOverlappingTimes()
        {
            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);
            StopTime[] trip1 = GetStopTimeCircuit(new DateTime(1970, 1, 1, 10, 0, 0), 1, 5);
            StopTime[] trip2 = GetStopTimeCircuit(new DateTime(1970, 1, 1, 18, 0, 0), 2, 7);

            // ACT
            INode[] nodes = graphBuilder.BuildGraph(trip1.Concat(trip2), Enumerable.Empty<ChildCare>());

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            VerifyCircuit(trip1, subgraphs[0]);
            VerifyCircuit(trip2, subgraphs[1]);
        }

        [TestMethod]
        public void TwoTrips_OverlappingTimes()
        {
            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);
            StopTime[] trip1 = GetStopTimeCircuit(new DateTime(1970, 1, 1, 10, 0, 0), 1, 5);
            StopTime[] trip2 = GetStopTimeCircuit(new DateTime(1970, 1, 1, 10, 2, 0), 2, 7);

            // ACT
            INode[] nodes = graphBuilder.BuildGraph(trip1.Concat(trip2), Enumerable.Empty<ChildCare>());

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            VerifyCircuit(trip1, subgraphs[0]);
            VerifyCircuit(trip2, subgraphs[1]);
        }

        private static void VerifyCircuit(StopTime[] trip, INode[] nodes)
        {
            // ASSERT
            Assert.IsNotNull(nodes);
            Assert.AreEqual(trip.Length, nodes.Length);

            for (int i = 0; i < trip.Length; i++)
            {
                INode node = nodes[i];
                if (i > 0)
                {
                    Assert.IsTrue(node.DownwindNeighbors.Contains(nodes[i - 1]));
                }
                Assert.AreEqual(trip[i].ArrivalTime, node.Time);
                if (i < trip.Length - 1)
                {
                    Assert.IsTrue(node.UpwindNeighbors.Contains(nodes[i + 1]));
                }
            }
        }

        private static StopTime[] GetStopTimeCircuit(DateTime startTime, int tripId, int stopTimeCount)
        {
            if (stopTimeCount < 2)
            {
                throw new ArgumentException("The number of stop times must be greater than 1.", "stopTimeCount");
            }

            DateTime time = startTime;
            IList<StopTime> stopTimes = new List<StopTime>();
            for (int i = 1; i <= stopTimeCount - 1; i++)
            {
                var stop = new Stop {Id = i};
                var stopTime = new StopTime
                {
                    ArrivalTime = time,
                    DepartureTime = time,
                    Sequence = i,
                    Stop = stop,
                    StopId = stop.Id,
                    TripId = tripId
                };
                stopTimes.Add(stopTime);

                // increment time
                time = time.AddMinutes(5);
            }

            stopTimes.Add(new StopTime
            {
                ArrivalTime = time,
                DepartureTime = time,
                Sequence = stopTimeCount,
                Stop = stopTimes[0].Stop,
                StopId = stopTimes[0].StopId,
                TripId = tripId
            });

            return stopTimes.ToArray();
        }

        private static IEnumerable<INode[]> GetSortedDisconnectedSubgraphs(IEnumerable<INode> node)
        {
            ISet<INode> unvisitedNodes = new HashSet<INode>(node);
            while (unvisitedNodes.Count > 0)
            {
                var stack = new Stack<INode>();
                ISet<INode> visistedNodes = new HashSet<INode>();
                stack.Push(unvisitedNodes.First());
                while (stack.Count > 0)
                {
                    INode current = stack.Pop();
                    if (visistedNodes.Contains(current))
                    {
                        continue;
                    }
                    unvisitedNodes.Remove(current);
                    visistedNodes.Add(current);
                    foreach (INode neighbor in current.DownwindNeighbors.Concat(current.UpwindNeighbors))
                    {
                        stack.Push(neighbor);
                    }
                }

                // visisted nodes not contains a subgraph
                yield return visistedNodes.OrderBy(n => n.Time).ToArray();
            }
        }
    }
}