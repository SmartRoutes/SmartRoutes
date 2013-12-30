using System;
using System.Data.Entity;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.Database;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.ArchiveLoader
{
    public class GtfsArchiveLoader : ArchiveLoader<GtfsArchive, GtfsCollection>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GtfsArchiveLoader(IEntityCollectionReader<GtfsArchive, GtfsCollection> reader, IEntityCollectionDownloader<GtfsArchive, GtfsCollection> downloader) : base(reader, downloader)
        {
        }

        protected override Func<Entities, IDbSet<GtfsArchive>> GetArchiveDbSetGetter()
        {
            return ctx => ctx.GtfsArchives;
        }

        protected override async Task AddCollection(GtfsCollection collection)
        {
            await Persist(collection.Agencies);
            await Persist(collection.Services);
            await Persist(collection.ServiceExceptions);
            await Persist(collection.Routes);
            await Persist(collection.Shapes);
            await Persist(collection.ShapePoints);
            await Persist(collection.Blocks);
            await Persist(collection.Trips);
            await Persist(collection.Stops);
            await Persist(collection.StopTimes);
        }

        protected override Task TruncateAsync(Entities ctx)
        {
            return ctx.TruncateGtfsAsync();
        }
    }
}