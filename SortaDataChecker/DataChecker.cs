using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.Database;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.GtfsReader.Readers;
using SmartRoutes.GtfsReader.Support;

namespace SmartRoutes.SortaDataChecker
{
    public class DataChecker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IGtfsCollectionDownloader _downloader;

        public DataChecker(IGtfsCollectionDownloader downloader)
        {
            _downloader = downloader;
        }

        public async Task UpdateDatabase(bool force)
        {
            Logger.Trace("Initializing SortaEntities.");
            using (var ctx = new Entities())
            {
                Archive currentArchive = null;
                if (!force)
                {
                    // get the current archive from the database (if any)
                    Logger.Trace("Getting the current Archive instance (if any).");
                    currentArchive = await ctx.Archives
                        .OrderByDescending(a => a.LoadedOn)
                        .FirstOrDefaultAsync();
                }
                if (currentArchive == null)
                {
                    Logger.Trace(force ? "Any existing archive record has been ignored." : "No current archive is recorded.");
                }
                else
                {
                    Logger.Trace("The current archive was downloaded on {0}.", currentArchive.LoadedOn);
                }

                Logger.Trace("Fetching the entity collection.");
                GtfsCollection gtfsCollection = await _downloader.Download(currentArchive);

                // handled matching ETag and LastModified headers
                if (gtfsCollection == null)
                {
                    Logger.Trace("No entity collection was returned.");
                    return;
                }

                // handle hash match
                if (!gtfsCollection.ContainsEntities)
                {
                    Logger.Trace("No entities were returned except for a new Archive instance.");
                    ctx.Archives.Add(gtfsCollection.Archive);
                    Logger.Trace("Saving the new Archive record.");
                    await ctx.SaveChangesAsync();
                    return;
                }

                // truncate the old data
                Logger.Trace("Truncating all of the old entities.");
                if (currentArchive != null)
                {
                    ctx.Archives.Remove(currentArchive);
                    await ctx.SaveChangesAsync();
                }
                ctx.Truncate();

                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.ValidateOnSaveEnabled = false;

                using (var inserter = new FastInserter<Entities>(ctx, 1000))
                {
                    //  persist all of the new entities
                    Logger.Trace("Adding {0} new Agency records.", gtfsCollection.Agencies.Count());
                    await inserter.AddRangeAsync(gtfsCollection.Agencies);

                    Logger.Trace("Adding {0} new Service records.", gtfsCollection.Services.Count());
                    await inserter.AddRangeAsync(gtfsCollection.Services);

                    Logger.Trace("Adding {0} new ServiceException records.", gtfsCollection.ServiceExceptions.Count());
                    await inserter.AddRangeAsync(gtfsCollection.ServiceExceptions);

                    Logger.Trace("Adding {0} new Route records.", gtfsCollection.Routes.Count());
                    await inserter.AddRangeAsync(gtfsCollection.Routes);

                    Logger.Trace("Adding {0} new Shape records.", gtfsCollection.Shapes.Count());
                    await inserter.AddRangeAsync(gtfsCollection.Shapes);

                    Logger.Trace("Adding {0} new ShapePoint records.", gtfsCollection.ShapePoints.Count());
                    await inserter.AddRangeAsync(gtfsCollection.ShapePoints);

                    Logger.Trace("Adding {0} new Block records.", gtfsCollection.Blocks.Count());
                    await inserter.AddRangeAsync(gtfsCollection.Blocks);

                    Logger.Trace("Adding {0} new Trip records.", gtfsCollection.Trips.Count());
                    await inserter.AddRangeAsync(gtfsCollection.Trips);

                    // the stops must be inserted in a single transaction, because the table is self-referential
                    Logger.Trace("Adding {0} new Stop records.", gtfsCollection.Stops.Count());
                    foreach (Stop stop in gtfsCollection.Stops)
                    {
                        ctx.Stops.Add(stop);
                    }
                    ctx.SaveChanges();

                    Logger.Trace("Adding {0} new StopTime records.", gtfsCollection.StopTimes.Count());
                    await inserter.AddRangeAsync(gtfsCollection.StopTimes);

                    Logger.Trace("Adding the new Archive record.");
                    await inserter.AddAsync(gtfsCollection.Archive);
                }
            }
        }
    }
}