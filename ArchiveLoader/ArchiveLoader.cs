using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.Database;
using SmartRoutes.Model;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.ArchiveLoader
{
    public abstract class ArchiveLoader<TArchive, TCollection> : IDisposable, IArchiveLoader<TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        private const int AddsPerRefresh = 1000;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEntityCollectionDownloader<TArchive, TCollection> _downloader;
        private readonly IEntityCollectionReader<TArchive, TCollection> _reader;
        private Entities _ctx;

        protected ArchiveLoader(IEntityCollectionReader<TArchive, TCollection> reader, IEntityCollectionDownloader<TArchive, TCollection> downloader)
        {
            _reader = reader;
            _downloader = downloader;
        }

        public async Task Download(Uri uri, bool force)
        {
            await Load(archive => _downloader.Download(uri, archive), force);
        }

        public async Task Read(string filePath, bool force)
        {
            await Load(archive => _reader.Read(filePath, archive), force);
        }

        public void Dispose()
        {
            if (_ctx != null)
            {
                _ctx.Dispose();
            }
        }

        protected abstract Func<Entities, IDbSet<TArchive>> GetArchiveDbSetGetter();
        protected abstract Task AddCollection(TCollection collection);

        private async Task Load(Func<TArchive, Task<TCollection>> collectionGetter, bool force)
        {
            _ctx = new Entities();

            // speed up adding
            _ctx.Configuration.AutoDetectChangesEnabled = false;
            _ctx.Configuration.ValidateOnSaveEnabled = false;

            // get the most recent archive
            IDbSet<TArchive> dbSet = GetArchiveDbSetGetter()(_ctx);
            TArchive currentArchive = null;
            if (!force)
            {
                // fetch the newest
                currentArchive = await dbSet
                    .OrderByDescending(a => a.LoadedOn)
                    .FirstOrDefaultAsync();
            }

            // get the collection, by some means
            TCollection collection = await collectionGetter(currentArchive);

            if (!collection.ContainsEntities)
            {
                return;
            }

            if (currentArchive != null)
            {
                dbSet.Remove(currentArchive);
                currentArchive = null;
                await _ctx.SaveChangesAsync();
            }

            // clear out the tables
            await _ctx.TruncateAsync();

            // persist the new collection
            await AddCollection(collection);

            // persist the Archive
            dbSet.Add(collection.Archive);
            await _ctx.SaveChangesAsync();

            _ctx.Dispose();
        }

        protected async Task Persist<T>(Func<Entities, IDbSet<T>> getDbSet, IEnumerable<T> entities, bool intermediateSaves) where T : class
        {
            // get the DbSet
            IDbSet<T> dbSet = getDbSet(_ctx);

            int i = 1;
            foreach (T entity in entities)
            {
                dbSet.Add(entity);
                if (i%AddsPerRefresh == 0 && intermediateSaves)
                {
                    await _ctx.SaveChangesAsync();
                }
            }

            await _ctx.SaveChangesAsync();
        }
    }
}