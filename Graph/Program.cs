using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.Scraper;
using SmartRoutes.SortaScraper.Parsers;
using SmartRoutes.SortaScraper.Support;
using SmartRoutes.Model.Sorta;
using SmartRoutes.Model.Odjfs.ChildCares;
using Ninject.Modules;
using SmartRoutes.Graph.Node;
using System.IO;
using Ionic.Zip;
using SmartRoutes.SortaScraper.Scrapers;
using SmartRoutes.Heap;
using SmartRoutes.Database.Contexts;
using SmartRoutes.Model;

namespace SmartRoutes.Graph
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IKernel kernel = new StandardKernel(new GraphModule());

                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof(IEntityCollectionParser), typeof(IEntityCollectionScraper))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                Console.WriteLine("Creating Graph.");
                DateTime tic = DateTime.Now;

                var graph = kernel.Get<IGraph>();

                DateTime toc = DateTime.Now;

                Console.WriteLine("Graph created in {0} milliseconds.", (toc - tic).TotalMilliseconds);
                Console.WriteLine("Performing Dijkstras...");
                tic = DateTime.Now;

                int count = 0;

                Func<INode, bool> GoalCheck = node =>
                {
                    var check = node as ChildCareNode;
                    return check != null;
                };

                var Results = new List<NodeInfo>();

                var StartNodes = new INode[] { graph.GraphNodes[count] };
                Results = ExtensionMethods.Dijkstras(StartNodes, GoalCheck, Direction.Upwind);

                toc = DateTime.Now;
                Console.WriteLine("Dijkstra's completed in {0} milliseconds, {1} results found.", 
                    (toc - tic).TotalMilliseconds, Results.Count());

                List<NodeBase> UniqueChildCareBases = new List<NodeBase>();

                foreach (var node in graph.GraphNodes)
                {
                    var cnode = node as ChildCareNode;
                    if (cnode == null) continue;
                    if (!UniqueChildCareBases.Contains(cnode.BaseNode))
                    {
                        UniqueChildCareBases.Add(cnode.BaseNode);
                    }
                }

                Console.WriteLine("{0} unique child cares found in graph.", UniqueChildCareBases.Count());

            Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
