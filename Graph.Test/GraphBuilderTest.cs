using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Graph.Test
{
    [TestClass]
    public class GraphBuilderTest
    {
        [TestMethod]
        public void Trips_One()
        {
            // ARRANGE
            StopTime[] trip = GetStopTimeCircuit(new DateTime(1970, 1, 1, 10, 0, 0), 1, 5);

            // ACT
            INode[] nodes = BuildGraph(trip, Enumerable.Empty<IDestination>());

            // VERIFY
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            VerifyCircuit(trip, subgraphs[0]);
        }

        [TestMethod]
        public void Trips_Two_NoOverlappingTimes()
        {
            // ARRANGE
            StopTime[] trip1 = GetStopTimeCircuit(new DateTime(1970, 1, 1, 10, 0, 0), 1, 5);
            StopTime[] trip2 = GetStopTimeCircuit(new DateTime(1970, 1, 1, 18, 0, 0), 2, 7);

            // ACT
            // disable transfers
            GraphBuilderSettings settings = GraphBuilderSettings.Default;
            settings.MaxFeetBetweenTransfers = -1;
            INode[] nodes = BuildGraph(trip1.Concat(trip2), Enumerable.Empty<IDestination>(), settings);

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            VerifyCircuit(trip1, subgraphs[0]);
            VerifyCircuit(trip2, subgraphs[1]);
        }

        [TestMethod]
        public void Trips_Two_OverlappingTimes()
        {
            // ARRANGE
            StopTime[] trip1 = GetStopTimeCircuit(new DateTime(1970, 1, 1, 10, 0, 0), 1, 5);
            StopTime[] trip2 = GetStopTimeCircuit(new DateTime(1970, 1, 1, 10, 2, 0), 2, 7);

            // ACT
            // disable transfers
            GraphBuilderSettings settings = GraphBuilderSettings.Default;
            settings.MaxFeetBetweenTransfers = -1;
            INode[] nodes = BuildGraph(trip1.Concat(trip2), Enumerable.Empty<IDestination>(), settings);

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            VerifyCircuit(trip1, subgraphs[0]);
            VerifyCircuit(trip2, subgraphs[1]);
        }

        [TestMethod]
        public void Transfers_SameStop()
        {
            // ARRANGE
            var stop = new Stop {Id = 1};

            var time1 = new DateTime(1970, 1, 1, 10, 0, 0);
            DateTime time2 = time1.AddMinutes(1);
            var stopTime1 = new StopTime
            {
                ArrivalTime = time1,
                Stop = stop,
                StopId = stop.Id,
                TripId = 1,
                Trip = new Trip { BlockId = 0 }
            };
            var stopTime2 = new StopTime
            {
                ArrivalTime = time2,
                Stop = stop,
                StopId = stop.Id,
                TripId = 2,
                Trip = new Trip { BlockId = 1 }
            };
            IEnumerable<StopTime> stoptimes = new[] {stopTime1, stopTime2};

            // ACT
            INode[] nodes = BuildGraph(stoptimes, Enumerable.Empty<IDestination>());

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
            var stop1 = new Stop {Id = 1, Latitude = 0.0, Longitude = 0.00001};
            var stop2 = new Stop {Id = 2, Latitude = 0.0, Longitude = 0.00002};

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
            INode[] nodes = BuildGraph(stoptimes, Enumerable.Empty<IDestination>());

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
            var stop1 = new Stop {Id = 1, Latitude = 0.0, Longitude = 0.00001};
            var stop2 = new Stop {Id = 2, Latitude = 0.0, Longitude = 0.00002};

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
            INode[] nodes = BuildGraph(stoptimes, Enumerable.Empty<IDestination>());

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();
            Assert.AreEqual(2, subgraphs.Length);
        }

        [TestMethod]
        public void Destinations_One()
        {
            // ARRANGE
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

            var destination = new Destination { Latitude = 0.0, Longitude = 0.00002 };

            // ACT
            INode[] nodes = BuildGraph(new[] {stopTime}, new[] {destination});

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();

            Assert.AreEqual(1, subgraphs.Length);
            INode[] subgraph = subgraphs[0];

            VerifyDestination(subgraph, time);
        }

        [TestMethod]
        public void Destinations_One_NotCloseEnough()
        {
            // ARRANGE
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

            var destination = new Destination { Latitude = 0.0, Longitude = 1.0 };

            // ACT
            INode[] nodes = BuildGraph(new[] {stopTime}, new[] {destination});

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes).ToArray();

            Assert.AreEqual(1, subgraphs.Length);
            INode[] subgraph = subgraphs[0];

            Assert.AreEqual(1, subgraph.Length);
            INode node = subgraph[0]; // the bus stop
            Assert.AreEqual(node.Time, time);
        }

        [TestMethod]
        public void Destinations_Two()
        {
            // ARRANGE
            var stop1 = new Stop { Id = 1, Latitude = 0.0, Longitude = 1.00000 };
            var stop2 = new Stop { Id = 2, Latitude = 0.0, Longitude = 2.00000 };
            var time1 = new DateTime(1970, 1, 1, 10, 0, 0);
            var time2 = new DateTime(1970, 1, 1, 16, 0, 0);
            var stopTime1 = new StopTime
            {
                ArrivalTime = time1,
                Stop = stop1,
                StopId = stop1.Id,
                TripId = 1,
                Trip = new Trip()
            };
            var stopTime2 = new StopTime
            {
                ArrivalTime = time2,
                Stop = stop2,
                StopId = stop2.Id,
                TripId = 2,
                Trip = new Trip()
            };

            var destinationA = new Destination { Latitude = 0.0, Longitude = 1.00001 };
            var destinationB = new Destination { Latitude = 0.0, Longitude = 2.00001 };

            // ACT
            INode[] nodes = BuildGraph(new[] { stopTime1, stopTime2 }, new[] { destinationA, destinationB });

            // ASSERT
            INode[][] subgraphs = GetSortedDisconnectedSubgraphs(nodes)
                .OrderBy(n => n.First().Time)
                .ToArray();

            Assert.AreEqual(2, subgraphs.Length);
            INode[] subgraph1 = subgraphs[0];
            INode[] subgraph2 = subgraphs[1];

            VerifyDestination(subgraph1, time1);
            VerifyDestination(subgraph2, time2);
        }

        private static INode[] BuildGraph(IEnumerable<StopTime> stopTimes, IEnumerable<IDestination> destinations, GraphBuilderSettings graphBuilderSettings)
        {
            IGraphBuilder graphBuilder = new GraphBuilder();
            IGraph graph = graphBuilder.BuildGraph(stopTimes, destinations, graphBuilderSettings);
            return graph.GraphNodes;
        }

        private static INode[] BuildGraph(IEnumerable<StopTime> stopTimes, IEnumerable<IDestination> destinations)
        {
            return BuildGraph(stopTimes, destinations, GraphBuilderSettings.Default);
        }

        private static void VerifyDestination(INode[] nodes, DateTime time)
        {
            Assert.AreEqual(3, nodes.Length);
            INode node1 = nodes[0]; // coming from the destination
            INode node2 = nodes[1]; // the bus stop
            INode node3 = nodes[2]; // going to the destination

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

        private static IEnumerable<INode[]> GetSortedDisconnectedSubgraphs(IEnumerable<INode> nodes)
        {
            // use depth-first search to explore the entire graph, finding disconnected subgraphs
            ISet<INode> unvisitedNodes = new HashSet<INode>(nodes);
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