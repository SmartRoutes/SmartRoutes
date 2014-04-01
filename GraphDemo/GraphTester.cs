using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartRoutes.Demo.OdjfsDatabase;
using SmartRoutes.Graph;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader.Parsers.Gtfs;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.GraphDemo
{
    public partial class GraphTester : Form
    {
        bool initialized = false;

        public GraphTester()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ResultsBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void BuildGraphBtn_Click(object sender, EventArgs e)
        {
            if (!initialized)
            {
                initialized = true;

                ResultsBox.Text = "Fetching the SORTA data from the web.\r\n"; ResultsBox.Refresh();
                
                DateTime tic = DateTime.Now;
                var gtfsParser = new GtfsCollectionParser(
                    new AgencyCsvStreamParser(),
                    new RouteCsvStreamParser(),
                    new ServiceCsvStreamParser(),
                    new ServiceExceptionCsvStreamParser(),
                    new ShapePointCsvStreamParser(),
                    new StopTimeCsvStreamParser(),
                    new StopCsvStreamParser(),
                    new TripCsvStreamParser());
                var gtfsFetcher = new EntityCollectionDownloader<GtfsArchive, GtfsCollection>(gtfsParser);
                var gtfsCollection = gtfsFetcher.Download(new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip"), null).Result;
                
                DateTime toc = DateTime.Now;

                ResultsBox.Text += String.Format("GTFS data fetched in {0} milliseconds.\r\n", (toc - tic).TotalMilliseconds);
                ResultsBox.Text += "Fetching the ODJFS data from the database\r\n.";
                ResultsBox.Refresh();

                tic = DateTime.Now;
                var odjfsDatabase = new OdjfsDatabase("OdjfsDatabase");
                var childCares = odjfsDatabase.GetChildCares().Result;
                toc = DateTime.Now;
                ResultsBox.Text += String.Format("Destination data fetched in {0} milliseconds.\r\n", (toc - tic).TotalMilliseconds);
                ResultsBox.Text += "Creating Graph.";
                ResultsBox.Refresh();

                tic = DateTime.Now;
                var graphBuilder = new GraphBuilder();
                var graph = graphBuilder.BuildGraph(gtfsCollection.StopTimes, childCares, GraphBuilderSettings.Default);
                toc = DateTime.Now;

                ResultsBox.Text += String.Format("Graph created in {0} milliseconds.\r\n", (toc - tic).TotalMilliseconds);
                ResultsBox.Refresh();
            }
        }
    }
}
