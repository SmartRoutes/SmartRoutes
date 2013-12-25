using System;
using NLog;
using SmartRoutes.Model;
using SmartRoutes.Reader.Parsers;
using SmartRoutes.Reader.Support;

namespace SmartRoutes.Reader.Readers
{
    public abstract class BaseEntityCollectionReader<TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEntityCollectionParser<TArchive, TCollection> _parser;

        protected BaseEntityCollectionReader(IEntityCollectionParser<TArchive, TCollection> parser)
        {
            _parser = parser;
        }

        protected TCollection Parse(byte[] bytes, TArchive currentArchive)
        {
            // create the new archive
            var newestArchive = Activator.CreateInstance<TArchive>();
            newestArchive.LoadedOn = DateTime.Now;

            // get the archive contents
            Logger.Trace("The newest archive has {0} bytes ({1} megabytes).", bytes.LongLength, Math.Round(bytes.LongLength/(1024.0*1024.0), 2));

            newestArchive.Hash = bytes.GetSha256Hash();
            Logger.Trace("The newest archive has the following hash: {0}", newestArchive.Hash);

            if (currentArchive != null && currentArchive.Hash == newestArchive.Hash)
            {
                Logger.Trace("The newest archive has the same hash as the one provided.");
                var collection = Activator.CreateInstance<TCollection>();
                collection.Archive = newestArchive;
                collection.ContainsEntities = false;
                return collection;
            }

            // parse the entities
            Logger.Trace("The newest archive is different. Parsing the newest archive.");
            TCollection entities = _parser.Parse(bytes);
            entities.Archive = newestArchive;
            entities.ContainsEntities = true;

            return entities;
        }
    }
}