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
            await Persist(ctx => ctx.Agencies, collection.Agencies, true);
            await Persist(ctx => ctx.Services, collection.Services, true);
            await Persist(ctx => ctx.ServiceExceptions, collection.ServiceExceptions, true);
            await Persist(ctx => ctx.Routes, collection.Routes, true);
            await Persist(ctx => ctx.Shapes, collection.Shapes, true);
            await Persist(ctx => ctx.ShapePoints, collection.ShapePoints, true);
            await Persist(ctx => ctx.Blocks, collection.Blocks, true);
            await Persist(ctx => ctx.Trips, collection.Trips, true);
            await Persist(ctx => ctx.Stops, collection.Stops, false);
            await Persist(ctx => ctx.StopTimes, collection.StopTimes, true);
        }
    }
}