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
using Model.Sorta;

namespace Graph
{
    public class Graph : IGraph
    {
        private readonly IGraphBuilder _builder;
        private EntityCollection _collection;
        private INode[] GraphNodes;

        public Graph(IGraphBuilder builder)
        {
            Console.WriteLine("Graph Constructor starting.");
            _builder = builder;
            Console.WriteLine("Calling Data Checker.");
            updateSortaEntities();

            GraphNodes = _builder.BuildGraph(_collection);
        }

        public void updateSortaEntities()
        {
            Console.WriteLine("updateSortaEntities called.");
            using (var ctx = new SortaEntities())
            {
                _collection = new EntityCollection();
                _collection.StopTimes = (from e in ctx.StopTimes select e).ToList();
                _collection.Stops = (from e in ctx.Stops select e).ToList();
                _collection.Routes = (from e in ctx.Routes select e).ToList();
                _collection.Shapes = (from e in ctx.Shapes select e).ToList();
                _collection.ShapePoints = (from e in ctx.ShapePoints select e).ToList();
                _collection.Blocks = (from e in ctx.Blocks select e).ToList();
                _collection.Agencies = (from e in ctx.Agencies select e).ToList();
                _collection.Archive = ctx.Archives.OrderBy(e => e.DownloadedOn).FirstOrDefault();
                _collection.Trips = (from e in ctx.Trips select e).ToList();
                _collection.ServiceException = (from e in ctx.ServiceException select e).ToList();
                _collection.Services = (from e in ctx.Services select e).ToList();
                _collection.ContainsEntities = true;
            }
        }
    }
}
