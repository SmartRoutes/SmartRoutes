using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.Database;
using SmartRoutes.Database.Data;
using SmartRoutes.Model;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.ArchiveLoader
{
    public abstract class ArchiveLoader<TArchive, TCollection> : IArchiveLoader<TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEntityCollectionDownloader<TArchive, TCollection> _downloader;
        private readonly IEntityCollectionReader<TArchive, TCollection> _reader;

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

        protected abstract Func<Entities, IDbSet<TArchive>> GetArchiveDbSetGetter();
        protected abstract Task AddCollection(TCollection collection);

        protected virtual void Configure<TEntity>(RecordDataReaderConfiguration<TEntity> configuration) where TEntity : class
        {
        }

        private async Task Load(Func<TArchive, Task<TCollection>> collectionGetter, bool force)
        {
            Logger.Trace("ArchiveLoader<{0}, {1}> is now starting.", typeof (TArchive).Name, typeof (TCollection).Name);

            TCollection collection;
            using (var ctx = new Entities())
            {
                // get the most recent archive
                IDbSet<TArchive> dbSet = GetArchiveDbSetGetter()(ctx);
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
                collection = await collectionGetter(currentArchive);

                if (!collection.ContainsEntities)
                {
                    Logger.Trace("The current {0} that is loaded in the database is current.", typeof (TCollection).Name);
                    return;
                }

                Logger.Trace("The tables for this collection will be truncated and re-populated.", typeof (TCollection).Name);
                if (currentArchive != null)
                {
                    dbSet.Remove(currentArchive);
                    await ctx.SaveChangesAsync();
                }

                // clear out the tables
                Logger.Trace("Truncating the tables.");
                await ctx.TruncateAsync();
            }

            // persist the new collection
            Logger.Trace("Adding the {0} data.", typeof (TCollection).Name);
            await AddCollection(collection);

            // persist the Archive
            using (var ctx = new Entities())
            {
                IDbSet<TArchive> dbSet = GetArchiveDbSetGetter()(ctx);
                dbSet.Add(collection.Archive);
                await ctx.SaveChangesAsync();
            }

            Logger.Trace("ArchiveLoader<{0}, {1}> is now complete.", typeof (TArchive).Name, typeof (TCollection).Name);
        }

        protected async Task Persist<T>(T[] entities) where T : class
        {
            Logger.Trace("Persisting {0} {1} instances.",
                entities.Count(),
                typeof (T).Name,
                typeof (TCollection).Name);

            using (var ctx = new Entities())
            {
                // connect to the database
                var connection = ctx.Database.Connection as SqlConnection;
                if (connection == null)
                {
                    var exception = new InvalidOperationException("The configured database connection does not resolve to a valid SqlConnection.");
                    Logger.ErrorException(string.Format("ConnectionType: {0}", ctx.Database.Connection.GetType()), exception);
                    throw exception;
                }
                connection.Open();

                using (var sbc = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default | SqlBulkCopyOptions.KeepIdentity, null))
                {
                    sbc.DestinationTableName = Entities.GetTableName(typeof (T));

                    // configure
                    var reader = new RecordDataReader<T>(entities);
                    Configure(reader.Configuration);

                    // execute
                    await sbc.WriteToServerAsync(reader);
                }
            }
        }
    }
}