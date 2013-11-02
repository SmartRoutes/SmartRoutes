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
        private readonly IEntityCollectionScraper _scraper;
        private EntityCollection _collection;
        private DataChecker _dataChecker;
        private INode[] GraphNodes;

        public Graph(IGraphBuilder builder, IEntityCollectionScraper scraper)
        {
            Console.WriteLine("Graph Constructor starting.");
            _builder = builder;
            _scraper = scraper;
            _dataChecker = new DataChecker(scraper);
            Console.WriteLine("Calling Data Checker.");
            //_dataChecker.UpdateDatabase().Wait();
            updateSortaEntities();

            // temporary untill entities are properly stitched
            //foreach (StopTime entry in _collection.StopTimes)
            //{
            //    entry.Stop = _collection.Stops.
            //        Single<Stop>(s => s.Id == entry.StopId);
            //}

            //foreach (StopTime entry in _collection.StopTimes)
            //{
            //    entry.Trip = _collection.Trips.
            //        Single<Trip>(s => s.Id == entry.TripId);
            //}

            //foreach (Trip entry in _collection.Trips)
            //{
            //    entry.Shape = _collection.Shapes.
            //        Single<Shape>(s => s.Id == entry.ShapeId);
            //}

            _builder.BuildGraph(_collection);
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
