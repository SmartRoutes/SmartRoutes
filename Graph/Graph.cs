using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SmartRoutes.Database;
using SmartRoutes.Database.Contexts;
using SmartRoutes.Graph.Node;
using SmartRoutes.SortaScraper.Support;
using SmartRoutes.SortaScraper.Scrapers;
using SmartRoutes.Model.Sorta;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Heap;

namespace SmartRoutes.Graph
{
    public class Graph : IGraph
    {
        private readonly IGraphBuilder Builder;
        private EntityCollection Collection;
        private ChildCare[] ChildCares;
        public INode[] GraphNodes { get; private set; }

        public Graph(IGraphBuilder Builder, IFibonacciHeap<INode, TimeSpan> Queue)
        {
            this.Builder = Builder;
            GetSortaEntities();
            GetChildCares();
            GraphNodes = Builder.BuildGraph(Collection.StopTimes, ChildCares);
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
    }
}
