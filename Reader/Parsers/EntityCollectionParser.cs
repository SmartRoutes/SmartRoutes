using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using NLog;
using SmartRoutes.Model;
using SmartRoutes.Reader.Support;

namespace SmartRoutes.Reader.Parsers
{
    public abstract class EntityCollectionParser<TArchive, TCollection> : IEntityCollectionParser<TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TCollection Parse(byte[] zipFileBytes)
        {
            // extract files from the zip
            Logger.Trace("Parsing the zip file.");
            using (ZipFile zipFile = ZipFile.Read(new MemoryStream(zipFileBytes)))
            {
                IDictionary<string, Func<Stream>> streams = zipFile
                    .Entries
                    .Where(z => !z.IsDirectory)
                    .ToDictionary(z => z.FileName, z => new Func<Stream>(z.OpenReader));

                return Parse(streams);
            }
        }

        protected abstract TCollection Parse(IDictionary<string, Func<Stream>> streams);

        protected static Stream GetStream(IDictionary<string, Func<Stream>> streams, string fileName)
        {
            Func<Stream> getStream;
            if (!streams.TryGetValue(fileName, out getStream))
            {
                Exception exception = new ArgumentException("The required file was not found.");
                Logger.ErrorException(string.Format("MissingFileName: {0}, AllFileNames: {1}",
                    fileName,
                    string.Join(", ", streams.Keys)),
                    exception);
                throw exception;
            }

            return getStream();
        }
    }
}