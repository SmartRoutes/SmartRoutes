using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Graph.Test
{
    [TestClass]
    public class GraphBuilderTest
    {
        [TestMethod]
        public void Trips_One()
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
        public void Trips_Two_NoOverlappingTimes()
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
        public void Trips_Two_OverlappingTimes()
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

        [TestMethod]
        public void Transfers_SameStop()
        {
            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);

            var stop = new Stop {Id = 1};
            stop.CloseStops.Add(stop);

            var time1 = new DateTime(1970, 1, 1, 10, 0, 0);
            DateTime time2 = time1.AddMinutes(1);
            var stopTime1 = new StopTime
            {
                ArrivalTime = time1,
                Stop = stop,
                StopId = stop.Id,
                TripId = 1,
                Trip = new Trip { ShapeId = 0 }
            };
            var stopTime2 = new StopTime
            {
                ArrivalTime = time2,
                Stop = stop,
                StopId = stop.Id,
                TripId = 2,
                Trip = new Trip { ShapeId = 1 }
            };
            IEnumerable<StopTime> stoptimes = new[] {stopTime1, stopTime2};

            // ACT
            INode[] nodes = graphBuilder.BuildGraph(stoptimes, Enumerable.Empty<ChildCare>());

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            Assert.AreEqual(1, subgraphs.Length);
            Assert.AreEqual(subgraphs[0][0].Time, time1);
            Assert.AreEqual(subgraphs[0][1].Time, time2);
        }

        [TestMethod]
        public void Transfers_DifferentStop()
        {
            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);

            var stop1 = new Stop {Id = 1, Latitude = 0.0, Longitude = 0.00001};
            stop1.CloseStops.Add(stop1);
            var stop2 = new Stop {Id = 2, Latitude = 0.0, Longitude = 0.00002};
            stop2.CloseStops.Add(stop2);
            stop1.CloseStops.Add(stop2);
            stop2.CloseStops.Add(stop1);

            var time1 = new DateTime(1970, 1, 1, 10, 0, 0);
            DateTime time2 = time1.AddMinutes(10);
            var stopTime1 = new StopTime
            {
                ArrivalTime = time1,
                Stop = stop1,
                StopId = stop1.Id,
                TripId = 1,
                Trip = new Trip { ShapeId = 0 }
            };
            var stopTime2 = new StopTime
            {
                ArrivalTime = time2,
                Stop = stop2,
                StopId = stop2.Id,
                TripId = 2,
                Trip = new Trip { ShapeId = 1 }
            };
            IEnumerable<StopTime> stoptimes = new[] {stopTime1, stopTime2};

            // ACT
            INode[] nodes = graphBuilder.BuildGraph(stoptimes, Enumerable.Empty<ChildCare>());

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            Assert.AreEqual(1, subgraphs.Length);
            Assert.AreEqual(subgraphs[0][0].Time, time1);
            Assert.AreEqual(subgraphs[0][1].Time, time2);
        }

        [TestMethod]
        public void Transfers_DifferentStop_NotEnoughTime()
        {
            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);

            var stop1 = new Stop {Id = 1, Latitude = 0.0, Longitude = 0.00001};
            stop1.CloseStops.Add(stop1);
            var stop2 = new Stop {Id = 2, Latitude = 0.0, Longitude = 0.00002};
            stop2.CloseStops.Add(stop2);
            stop1.CloseStops.Add(stop2);
            stop2.CloseStops.Add(stop1);

            var time1 = new DateTime(1970, 1, 1, 10, 0, 0);
            DateTime time2 = time1; // the same instant
            var stopTime1 = new StopTime
            {
                ArrivalTime = time1,
                Stop = stop1,
                StopId = stop1.Id,
                TripId = 1,
                Trip = new Trip { ShapeId = 0 }
            };
            var stopTime2 = new StopTime
            {
                ArrivalTime = time2,
                Stop = stop2,
                StopId = stop2.Id,
                TripId = 2,
                Trip = new Trip { ShapeId = 1 }
            };
            IEnumerable<StopTime> stoptimes = new[] {stopTime1, stopTime2};

            // ACT
            INode[] nodes = graphBuilder.BuildGraph(stoptimes, Enumerable.Empty<ChildCare>());

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            Assert.AreEqual(2, subgraphs.Length);
        }

        [TestMethod]
        public void ChildCares_One()
        {
            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);

            var stop = new Stop {Id = 1, Latitude = 0.0, Longitude = 0.00001};
            var time = new DateTime(1970, 1, 1, 10, 0, 0);
            var stopTime = new StopTime
            {
                ArrivalTime = time,
                Stop = stop,
                StopId = stop.Id,
                TripId = 1,
                Trip = new Trip { ShapeId = 0 }
            };

            var childCare = new LicensedCenter { Id = 1, Latitude = 0.0, Longitude = 0.00002 };

            // ACT
            INode[] nodes = graphBuilder.BuildGraph(new[] {stopTime}, new[] {childCare});

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();

            Assert.AreEqual(1, subgraphs.Length);
            INode[] subgraph = subgraphs[0];

            VerifyChildCare(subgraph, time);
        }

        [TestMethod]
        public void ChildCares_One_NotCloseEnough()
        {
            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);

            var stop = new Stop { Id = 1, Latitude = 0.0, Longitude = 0.00001 };
            var time = new DateTime(1970, 1, 1, 10, 0, 0);
            var stopTime = new StopTime
            {
                ArrivalTime = time,
                Stop = stop,
                StopId = stop.Id,
                TripId = 1,
                Trip = new Trip { ShapeId = 0 }
            };

            var childCare = new LicensedCenter { Id = 1, Latitude = 0.0, Longitude = 1.0 };

            // ACT
            INode[] nodes = graphBuilder.BuildGraph(new[] { stopTime }, new[] { childCare });

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();

            Assert.AreEqual(1, subgraphs.Length);
            INode[] subgraph = subgraphs[0];

            Assert.AreEqual(1, subgraph.Length);
            INode node = subgraph[0]; // the bus stop
            Assert.AreEqual(node.Time, time);
        }

        [TestMethod]
        public void ChildCares_Two()
        {
            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);

            var stop1 = new Stop { Id = 1, Latitude = 0.0, Longitude = 0.00001 };
            var stop2 = new Stop { Id = 2, Latitude = 0.0, Longitude = 0.00002 };
            var time1 = new DateTime(1970, 1, 1, 10, 0, 0);
            var time2 = new DateTime(1970, 1, 1, 16, 0, 0);
            var stopTime1 = new StopTime
            {
                ArrivalTime = time1,
                Stop = stop1,
                StopId = stop1.Id,
                TripId = 1,
                Trip = new Trip { ShapeId = 0 }
            };
            var stopTime2 = new StopTime
            {
                ArrivalTime = time2,
                Stop = stop2,
                StopId = stop2.Id,
                TripId = 2,
                Trip = new Trip { ShapeId = 0 }
            };

            var childCare = new LicensedCenter { Id = 1, Latitude = 0.0, Longitude = 0.000015 };

            // ACT
            INode[] nodes = graphBuilder.BuildGraph(new[] { stopTime1, stopTime2 }, new[] { childCare });

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes)
                .OrderBy(n => n.First().Time)
                .ToArray();

            Assert.AreEqual(2, subgraphs.Length);
            INode[] subgraph1 = subgraphs[0];
            INode[] subgraph2 = subgraphs[1];

            VerifyChildCare(subgraph1, time1);
            VerifyChildCare(subgraph2, time2);
        }

        private static void VerifyChildCare(INode[] nodes, DateTime time)
        {
            Assert.AreEqual(3, nodes.Length);
            INode node1 = nodes[0]; // coming from the child care
            INode node2 = nodes[1]; // the bus stop
            INode node3 = nodes[2]; // going to the child care

            Assert.IsTrue(node1.Time < time);
            Assert.AreEqual(node2.Time, time);
            Assert.IsTrue(node3.Time > node2.Time);
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
                    Assert.IsTrue(node.TimeBackwardNeighbors.Contains(nodes[i - 1]));
                }
                Assert.AreEqual(trip[i].ArrivalTime, node.Time);
                if (i < trip.Length - 1)
                {
                    Assert.IsTrue(node.TimeForwardNeighbors.Contains(nodes[i + 1]));
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
                    Sequence = i,
                    Stop = stop,
                    StopId = stop.Id,
                    TripId = tripId,
                    Trip = new Trip { ShapeId = tripId }
                };
                stopTimes.Add(stopTime);

                // increment time
                time = time.AddMinutes(5);
            }

            stopTimes.Add(new StopTime
            {
                ArrivalTime = time,
                Sequence = stopTimeCount,
                Stop = stopTimes[0].Stop,
                StopId = stopTimes[0].StopId,
                TripId = tripId,
                Trip = new Trip { ShapeId = tripId }
            });

            return stopTimes.ToArray();
        }

        private static IEnumerable<INode[]> GetSortedDisconnectedSubgraphs(IEnumerable<INode> node)
        {
            // use depth-first search to explore the entire graph, finding disconnected subgraphs
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
                    foreach (INode neighbor in current.TimeForwardNeighbors.Concat(current.TimeBackwardNeighbors))
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