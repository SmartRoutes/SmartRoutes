using System;
using System.IO;
using System.Threading.Tasks;
using NLog;
using SmartRoutes.Model;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader;
using SmartRoutes.SrdsReader.Support;

namespace SmartRoutes.SrdsReader.Readers
{
    public class SrdsCollectionReader : ISrdsCollectionReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IEntityCollectionParser<SrdsArchive, SrdsCollection> _srdsCollectionParser;

        public SrdsCollectionReader(IEntityCollectionParser<SrdsArchive, SrdsCollection> srdsCollectionParser)
        {
            _srdsCollectionParser = srdsCollectionParser;
        }

        public async Task<SrdsCollection> Read(string filePath, SrdsArchive currentArchive)
        {
            // read the whole file into memory
            Logger.Trace("Reading the archive bytes from {0}.", filePath);
            byte[] zipFileBytes;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                zipFileBytes = memoryStream.ToArray();
            }

            Logger.Trace("The newest archive has {0} bytes ({1} megabytes).", zipFileBytes.LongLength, Math.Round(zipFileBytes.LongLength/(1024.0*1024.0), 2));

            // see if the file archive has changed
            var newestSrdsArchive = new SrdsArchive
            {
                LoadedOn = DateTime.Now,
                Hash = zipFileBytes.GetSha256Hash()
            };
            Logger.Trace("The newest archive has the following hash: {0}", newestSrdsArchive.Hash);

            if (currentArchive != null &&
                currentArchive.Hash == newestSrdsArchive.Hash)
            {
                Logger.Trace("The newest archive has the same hash, but a different ETag from the previous.");
                return new SrdsCollection
                {
                    Archive = newestSrdsArchive,
                    ContainsEntities = false
                };
            }

            // parse the entities
            Logger.Trace("The newest archive is different. Parsing the newest archive.");
            SrdsCollection entities = _srdsCollectionParser.Parse(zipFileBytes);
            entities.Archive = newestSrdsArchive;
            entities.ContainsEntities = true;

            return entities;
        }
    }
}