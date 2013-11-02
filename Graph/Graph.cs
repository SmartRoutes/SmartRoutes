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
        private EntityCollection _entities;
        private DataChecker _dataChecker;
        private INode[] GraphNodes;

        public Graph(IGraphBuilder builder, IEntityCollectionScraper scraper)
        {
            Console.WriteLine("Graph Constructor starting.");
            _builder = builder;
            _scraper = scraper;
            _dataChecker = new DataChecker(scraper);
            Console.WriteLine("Calling Data Checker.");
            _dataChecker.UpdateDatabase().Wait();

            updateSortaEntities().Wait();
            _builder.BuildGraph(_entities);
        }

        public async Task updateSortaEntities()
        {
            Console.WriteLine("updateSortaEntities called.");
            using (var ctx = new SortaEntities())
            {

                Archive currentArchive = await ctx.Archives
                    .OrderByDescending(a => a.DownloadedOn)
                    .FirstOrDefaultAsync();

                Console.WriteLine("Archive updated.");

                _entities = await _scraper.Scrape(currentArchive);
            }
        }
    }
}
