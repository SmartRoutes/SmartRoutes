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
        public void ConnectedTrip()
        {
            const int stopTimeCount = 5;

            // ARRANGE
            IMetroNode metroNodeMaker = new MetroNode();
            IGraphBuilder graphBuilder = new GraphBuilder(metroNodeMaker);
            StopTime[] stopTimes = GetStopTimeCircuit(stopTimeCount);

            // ACT
            INode[] nodes = graphBuilder
                .BuildGraph(stopTimes, Enumerable.Empty<ChildCare>())
                .OrderBy(n => n.Time)
                .ToArray();

            // ASSERT
            Assert.IsNotNull(nodes);
            Assert.AreEqual(stopTimeCount, nodes.Length);

            for (int i = 0; i < stopTimeCount; i++)
            {
                INode node = nodes[i];
                if (i > 0)
                {
                    Assert.IsTrue(node.DownwindNeighbors.Contains(nodes[i - 1]));
                }
                Assert.AreEqual(stopTimes[i].ArrivalTime, node.Time);
                if (i < stopTimeCount - 1)
                {
                    Assert.IsTrue(node.UpwindNeighbors.Contains(nodes[i + 1]));
                }
            }
        }

        public static StopTime[] GetStopTimeCircuit(int stopTimeCount)
        {
            if (stopTimeCount < 2)
            {
                throw new ArgumentException("The number of stop times must be greater than 1.", "stopTimeCount");
            }

            var time = new DateTime(1970, 1, 1, 8, 0, 0);
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
                    StopId = stop.Id
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
                StopId = stopTimes[0].StopId
            });

            return stopTimes.ToArray();
        }
    }
}