using System;
using System.Data.Entity;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.Database;
using SmartRoutes.Database.Data;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.ArchiveLoader
{
    public class SrdsArchiveLoader : ArchiveLoader<SrdsArchive, SrdsCollection>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SrdsArchiveLoader(IEntityCollectionReader<SrdsArchive, SrdsCollection> reader, IEntityCollectionDownloader<SrdsArchive, SrdsCollection> downloader)
            : base(reader, downloader)
        {
        }

        protected override Func<Entities, IDbSet<SrdsArchive>> GetArchiveDbSetGetter()
        {
            return ctx => ctx.SrdsArchives;
        }

        protected override Task TruncateAsync(Entities ctx)
        {
            return ctx.TruncateSrdsAsync();
        }

        protected override void Configure<TEntity>(RecordDataReaderConfiguration<TEntity> c)
        {
            var avc = c as RecordDataReaderConfiguration<AttributeValue>;
            if (avc != null)
            {
                avc.Ignore(e => e.Value);
            }
        }

        protected override async Task AddCollection(SrdsCollection collection)
        {
            await Persist(collection.AttributeKeys);
            await Persist(collection.Destinations);
            await Persist(collection.AttributeValues);
        }
    }
}