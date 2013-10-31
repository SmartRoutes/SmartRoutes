using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Model.Odjfs.ChildCares;
using NLog;
using OdjfsScraper;
using OdjfsScraper.Support;
using Scraper;

namespace OdjfsStatusChecker
{
    internal class Program
    {
        private const string PreviousDetailsDocumentStatusPath = @"PreviousDetailsDocumentStatus.xml";
        private const string PreviousListDocumentStatusPath = @"PreviousListDocumentStatus.xml";
        private static readonly XmlSerializer XmlSerializer = new XmlSerializer(typeof (DocumentStatus));
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            try
            {
                Logger.Trace("OdjfsUptimeChecker is now starting.");
                
                IOdjfsClient client = new OdjfsClient();

                bool checkingDetailsDocument = true;
                while (true)
                {
                    if (checkingDetailsDocument)
                    {
                        DocumentStatus current = GetCurrentDetailsDocumentStatus(client);
                        if (HasDocumentStatusChanged(PreviousDetailsDocumentStatusPath, current))
                        {
                            Logger.Trace("The details document has changed. HttpStatusCode: {0}, Hash: {1}.", current.ClientResponse.StatusCode, current.Hash);
                        }
                    }
                    else
                    {
                        DocumentStatus current = GetCurrentListDocumentStatus(client);
                        if (HasDocumentStatusChanged(PreviousListDocumentStatusPath, current))
                        {
                            Logger.Trace("The list document has changed. HttpStatusCode: {0}, Hash: {1}.", current.ClientResponse.StatusCode, current.Hash);
                        }
                    }

                    checkingDetailsDocument = !checkingDetailsDocument;

                    // wait for 5 seconds
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException("An exception has forced the OdjfsUptimeChecker to terminate.", e);
            }
        }

        private static bool HasDocumentStatusChanged(string path, DocumentStatus current)
        {
            // get the previous status, if any
            DocumentStatus previous = GetPreviousDocumentStatus(path);

            if (current.Equals(previous))
            {
                return false;
            }

            // backup the previous record, since it will be overwritten
            if (previous != null)
            {
                string directory = Path.GetDirectoryName(path);
                string prefix = Path.GetFileNameWithoutExtension(path);
                string timestamp = (previous.DateTime - new DateTime(1970, 1, 1)).TotalSeconds.ToString(CultureInfo.InvariantCulture);
                string extension = Path.GetExtension(path);
                string name = string.Format("{0}_{1}{2}", prefix, timestamp, extension);
                string backupPath = Path.Combine(directory, name);

                // move the old file
                File.Move(path, backupPath);
            }

            // write the new file
            SetPreviousDocumentStatus(path, current);

            return true;
        }

        private static DocumentStatus GetPreviousDocumentStatus(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (var inputStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return XmlSerializer.Deserialize(inputStream) as DocumentStatus;
            }
        }

        private static void SetPreviousDocumentStatus(string path, DocumentStatus documentStatus)
        {
            string directory = Path.GetDirectoryName(path) ?? string.Empty;
            if (!Directory.Exists(directory) && directory != string.Empty)
            {
                Directory.CreateDirectory(directory);
            }

            using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer.Serialize(outputStream, documentStatus);
            }
        }

        private static DocumentStatus GetCurrentDetailsDocumentStatus(IOdjfsClient client)
        {
            DateTime dateTime = DateTime.Now;
            ClientResponse clientResponse = client.GetChildCareDocument(new LicensedCenter {ExternalUrlId = "CDCSFJQMQINKNININI"}).Result;

            return new DocumentStatus
            {
                ClientResponse = clientResponse,
                DateTime = dateTime,
                Hash = clientResponse.Content.GetSha256Hash()
            };
        }

        private static DocumentStatus GetCurrentListDocumentStatus(IOdjfsClient client)
        {
            DateTime dateTime = DateTime.Now;
            ClientResponse clientResponse = client.GetListDocument(45224).Result;

            return new DocumentStatus
            {
                ClientResponse = clientResponse,
                DateTime = dateTime,
                Hash = clientResponse.Content.GetSha256Hash()
            };
        }
    }
}