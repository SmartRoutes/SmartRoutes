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
            Logger.Trace("ArchiveLoader<{0}, {1}> is now starting.", typeof (TArchive).Name, typeof (TCollection).Name);
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
                Logger.Trace("Fetching the latest {0} record.", typeof (TArchive).Name);
                currentArchive = await dbSet
                    .OrderByDescending(a => a.LoadedOn)
                    .FirstOrDefaultAsync();
                if (currentArchive == null)
                {
                    Logger.Trace("No {0} record was found.", typeof (TArchive).Name);
                }
                else
                {
                    Logger.Trace("A {0} record was found, which was loaded on {1}.", typeof (TArchive).Name, currentArchive.LoadedOn);
                }
            }
            else
            {
                Logger.Trace("The current {0} record is being ignored, due to the force parameter.", typeof (TArchive).Name);
            }

            // get the collection, by some means
            Logger.Trace("Fetching the {0}.", typeof (TCollection).Name);
            TCollection collection = await collectionGetter(currentArchive);

            if (!collection.ContainsEntities)
            {
                Logger.Trace("The current {0} that is loaded in the database is current.", typeof (TCollection).Name);
                return;
            }

            Logger.Trace("The {0} has changed. The tables for this collection will be truncated and re-populated.", typeof (TCollection).Name);
            if (currentArchive != null)
            {
                dbSet.Remove(currentArchive);
                currentArchive = null;
                await _ctx.SaveChangesAsync();
            }

            // clear out the tables
            Logger.Trace("Truncating the tables.");
            await _ctx.TruncateAsync();

            // persist the new collection
            Logger.Trace("Adding the {0} data.", typeof (TCollection).Name);
            await AddCollection(collection);

            // persist the Archive
            dbSet.Add(collection.Archive);
            await _ctx.SaveChangesAsync();

            _ctx.Dispose();

            Logger.Trace("ArchiveLoader<{0}, {1}> is now complete.", typeof (TArchive).Name, typeof (TCollection).Name);
        }

        protected async Task Persist<T>(Func<Entities, IDbSet<T>> getDbSet, IEnumerable<T> entities, bool intermediateSaves) where T : class
        {
            // enumerate the entities
            T[] entityArray = entities.ToArray();

            Logger.Trace("Persisting {0} {1} entities from a {2}{3}.",
                entityArray.Length,
                typeof (T).Name,
                typeof (TCollection).Name,
                intermediateSaves ? " (with intermediate saves)" : string.Empty);

            // get the DbSet
            IDbSet<T> dbSet = getDbSet(_ctx);

            int i = 1;
            foreach (T entity in entityArray)
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