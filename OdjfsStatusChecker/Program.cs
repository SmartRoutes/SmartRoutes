using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Xml.Serialization;
using NLog;
using Scraper;

namespace OdjfsStatusChecker
{
    public class Program
    {
        private const string CurrentDocumentStatusIdentifier = "current";
        private const string DocumentStatusesDirectory = @"Logs\DocumentStatuses";
        private static readonly XmlSerializer XmlSerializer = new XmlSerializer(typeof (DocumentStatus));
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Document[] Documents =
        {
            new Document
            {
                Name = "Invalid_Characters",
                RequestUri = "http://www.odjfs.state.oh.us/cdc/results2.asp?provider_number=CDCSFJRJPIPRNININI&Printable=Y"
            },
            new Document
            {
                Name = "Details",
                RequestUri = "http://www.odjfs.state.oh.us/cdc/results2.asp?provider_number=CDCSFJQMQINKNININI&Printable=Y"
            },
            new Document
            {
                Name = "Missing",
                RequestUri = "http://www.odjfs.state.oh.us/cdc/results2.asp?provider_number=CDCSFJUNRINKNININI_INVALID&Printable=Y"
            },
            new Document
            {
                Name = "List",
                RequestUri = "http://www.odjfs.state.oh.us/cdc/RESULTS1.ASP?Zip=45224&Printable=Y"
            },
            new Document
            {
                Name = "EmptyList",
                RequestUri = "http://www.odjfs.state.oh.us/cdc/RESULTS1.ASP?Zip=99999&Printable=Y"
            }
        };

        private static void Main(string[] args)
        {
            try
            {
                Logger.Trace("OdjfsUptimeChecker is now starting.");

                var client = new ScraperClient();

                int currentIndex = 0;
                while (currentIndex < Documents.Length)
                {
                    Document currentDocument = Documents[currentIndex];
                    currentIndex = (currentIndex + 1)%Documents.Length;

                    if (Directory.Exists(DocumentStatusesDirectory))
                    {
                        Directory.CreateDirectory(DocumentStatusesDirectory);
                    }

                    CheckIfDocumentStatusHasChanged(DocumentStatusesDirectory, client, currentDocument);

                    // wait for 5 seconds
                    Thread.Sleep(2000);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException("An exception has forced the OdjfsUptimeChecker to terminate.", e);
            }
        }

        private static void CheckIfDocumentStatusHasChanged(string directory, HttpClient client, Document document)
        {
            // get the previous status, if any
            DocumentStatus previous = GetPreviousDocumentStatus(directory, document);

            // get the current document status
            DocumentStatus current = GetCurrentDocumentStatus(client, document);

            // no IO if they are the same
            if (current.Equals(previous))
            {
                return;
            }

            // backup the previous if it exists
            if (previous != null)
            {
                BackupPreviousDocumentStatus(directory, previous);
            }

            // write the current
            SetPreviousDocumentStatus(directory, current);

            Logger.Trace("Document '{0}' has changed. HttpStatusCode: {1}. Hash: {2}", document.Name, current.StatusCode, current.Hash);
        }

        private static void BackupPreviousDocumentStatus(string directory, DocumentStatus status)
        {
            string previousIdentifier = (status.DateTime - new DateTime(1970, 1, 1)).TotalSeconds.ToString(CultureInfo.InvariantCulture);

            File.Move(
                status.Document.GetPath(directory, CurrentDocumentStatusIdentifier, "xml"),
                status.Document.GetPath(directory, previousIdentifier, "xml"));

            File.Move(
                status.Document.GetPath(directory, CurrentDocumentStatusIdentifier, "html"),
                status.Document.GetPath(directory, previousIdentifier, "html"));
        }

        private static void SetPreviousDocumentStatus(string directory, DocumentStatus status)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // write the XML
            using (var outputStream = new FileStream(status.Document.GetPath(directory, CurrentDocumentStatusIdentifier, "xml"), FileMode.Create, FileAccess.Write))
            {
                XmlSerializer.Serialize(outputStream, status);
            }

            // write the HTML
            File.WriteAllBytes(status.Document.GetPath(directory, CurrentDocumentStatusIdentifier, "html"), status.Content);
        }

        private static DocumentStatus GetPreviousDocumentStatus(string directory, Document document)
        {
            string path = document.GetPath(directory, CurrentDocumentStatusIdentifier, "xml");
            if (!File.Exists(path))
            {
                return null;
            }

            // read the XML
            using (var inputStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return XmlSerializer.Deserialize(inputStream) as DocumentStatus;
            }
        }

        private static DocumentStatus GetCurrentDocumentStatus(HttpClient client, Document document)
        {
            DateTime dateTime = DateTime.Now;
            var request = new HttpRequestMessage(HttpMethod.Get, document.RequestUri);
            HttpResponseMessage response = client.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
            byte[] content = response.Content.ReadAsByteArrayAsync().Result;

            return new DocumentStatus
            {
                Document = document,
                Content = content,
                StatusCode = response.StatusCode,
                DateTime = dateTime,
                Hash = content.GetSha256Hash()
            };
        }
    }
}