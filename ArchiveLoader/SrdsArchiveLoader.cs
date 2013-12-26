using System;
using System.Data.Entity;
using System.Threading.Tasks;
using SmartRoutes.Database;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.ArchiveLoader
{
    public class SrdsArchiveLoader : ArchiveLoader<SrdsArchive, SrdsCollection>
    {
        public SrdsArchiveLoader(IEntityCollectionReader<SrdsArchive, SrdsCollection> reader, IEntityCollectionDownloader<SrdsArchive, SrdsCollection> downloader)
            : base(reader, downloader)
        {
        }

        protected override Func<Entities, IDbSet<SrdsArchive>> GetArchiveDbSetGetter()
        {
            return ctx => ctx.SrdsArchives;
        }

        protected override async Task AddCollection(SrdsCollection collection)
        {
            await Persist(ctx => ctx.AttributeKeys, collection.AttributeKeys, true);
            await Persist(ctx => ctx.Destinations, collection.Destinations, true);
            await Persist(ctx => ctx.AttributeValues, collection.AttributeValues, true);
        }
    }
}