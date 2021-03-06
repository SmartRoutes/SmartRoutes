﻿using System.IO;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.Model;
using SmartRoutes.Reader.Parsers;

namespace SmartRoutes.Reader.Readers
{
    public class EntityCollectionReader<TArchive, TCollection> : BaseEntityCollectionReader<TArchive, TCollection>, IEntityCollectionReader<TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public EntityCollectionReader(IEntityCollectionParser<TArchive, TCollection> parser) : base(parser)
        {
        }

        public async Task<TCollection> Read(string filePath, TArchive currentArchive)
        {
            // read the whole file into memory
            Logger.Trace("Reading the archive bytes from {0}.", filePath);
            byte[] bytes;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                bytes = memoryStream.ToArray();
            }

            return Parse(bytes, currentArchive);
        }
    }
}