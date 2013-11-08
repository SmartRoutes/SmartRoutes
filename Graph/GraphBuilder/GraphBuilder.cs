using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Odjfs.ChildCares;
using SortaScraper.Support;
using Model.Sorta;
using Model.Odjfs;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using NLog;
using Graph.Node;

namespace Graph
{
    public class GraphBuilder : IGraphBuilder
    {
        private readonly IMetroNode _metroNodeMaker;
        private readonly IChildcareNode _childCareNodeMaker;
        private Logger Logger = LogManager.GetCurrentClassLogger();

        public GraphBuilder(IChildcareNode childCareNodeMaker, IMetroNode metroNodeMaker) 
        {
            try
            {
                _metroNodeMaker = metroNodeMaker;
                _childCareNodeMaker = childCareNodeMaker;
                Logger.Trace("GraphBuilder object created.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during GraphBuilder object creation.", e);
                throw e;
            }
        }

        public INode[] BuildGraph(EntityCollection collection)
        {
            try
            {
                Logger.Trace("Creating new graph.");

                var MetroNodes = CreateMetroNodes(collection);
                ConnectTrips(MetroNodes);
                ConnectTransfers(MetroNodes);
                //var ChildcareNodes = CreateChildcareNodes(ChildcareCollection);
                //var Graph = CombineNodes(MetroNodes, ChildcareNodes);
                Logger.Trace("Graph created successfully.");
                return MetroNodes;
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during creation of graph.", e);
                Console.WriteLine(e);
                throw e;
            }
        }

        private IMetroNode[] CreateMetroNodes(EntityCollection collection)
        {
            Logger.Trace("Creating Metro Nodes.");

            try
            {
                var MetroNodes = (from stopTime in collection.StopTimes
                                  select _metroNodeMaker.CreateNode(stopTime))
                                 .ToArray();

                Logger.Trace("Metro Nodes created successfully.");
                return MetroNodes;
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during creation of Metro nodes.", e);
                throw e;
            }
        }

        private void ConnectTrips(IMetroNode[] MetroNodes)
        {
            Logger.Trace("Connecting Metro Trips.");
            try
            {
                Array.Sort(MetroNodes, new Comparers.ComparerForTripSorting());

                for (int i = 1; i < MetroNodes.Count(); i++)
                {
                    IMetroNode node = MetroNodes[i];
                    IMetroNode previousNode = MetroNodes[i-1];

                    if (node.TripID == previousNode.TripID)
                    {
                        node.DownwindNeighbors.Add(previousNode);
                        previousNode.UpwindNeighbors.Add(node);
                    }
                }
                Logger.Trace("Metro Trips connected successfully.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during connection of Metro Trips.", e);
                throw e;
            }
        }

        private void ConnectTransfers(IMetroNode[] MetroNodes)
        {
            Logger.Trace("Connecting Metro Transfers.");
            try
            {
                Array.Sort(MetroNodes, new Comparers.ComparerForTransferSorting());

                for (int i = 1; i < MetroNodes.Count(); i++)
                {
                    IMetroNode node = MetroNodes[i];
                    IMetroNode previousNode = MetroNodes[i-1];

                    if (node.StopID == previousNode.StopID)
                    {
                        node.DownwindNeighbors.Add(previousNode);
                        previousNode.UpwindNeighbors.Add(node);
                    }
                }
                Logger.Trace("Metro Transfers connected successfully.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during connection of Metro Transfers.", e);
                throw e;
            }
        }

        private IChildcareNode[] CreateChildcareNodes(IEnumerable<ChildCare> ChildcareCollection)
        {
            return null;
        }

        private INode[] CombineNodes(IMetroNode[] MetroNodes, IChildcareNode[] ChildcareNodes)
        {
            return MetroNodes;
        }
    }
}
