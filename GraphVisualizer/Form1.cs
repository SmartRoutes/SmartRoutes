using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.Graph;
using SmartRoutes.Graph.Comparers;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.GraphVisualizer
{
    public partial class GraphVisualizerForm : Form
    {
        public GraphVisualizerForm()
        {
            InitializeComponent();
        }

        private void SetGraphButtonText(string text)
        {
            if (LoadGraphButton.InvokeRequired)
            {
                LoadGraphButton.Invoke(new StringDelegate(SetGraphButtonText), new object[] {text});
                return;
            }
            LoadGraphButton.Text = text;
        }

        private void SetFormEnabled(bool enabled)
        {
            if (LoadGraphButton.InvokeRequired)
            {
                LoadGraphButton.Invoke(new BooleanDelegate(SetFormEnabled), new object[] { enabled });
                return;
            }
            LoadGraphButton.Enabled = enabled;
            NodeCount.Enabled = enabled;
        }

        private void SetScene(ILScene scene)
        {
            if (Scene.InvokeRequired)
            {
                Scene.Invoke(new SceneDelegate(SetScene), new object[] {scene});
                return;
            }
            Scene.Scene = scene;
            Scene.Render(0);
        }

        private void LoadGraphClick(object sender, EventArgs e)
        {
            SetFormEnabled(false);
            var nodeCount = (int) NodeCount.Value;

            Task.Run(() =>
            {
                IKernel kernel = new StandardKernel(new GraphModule());

                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof (GtfsCollection), typeof (IEntityCollectionDownloader<,>))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                // build the graph
                SetGraphButtonText("Loading GTFS...");
                var gtfsFetcher = kernel.Get<IEntityCollectionDownloader<GtfsArchive, GtfsCollection>>();
                GtfsCollection gtfsCollection = gtfsFetcher.Download(new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip"), null).Result;

                SetGraphButtonText("Loading SRDS...");
                var srdsFetcher = kernel.Get<IEntityCollectionDownloader<SrdsArchive, SrdsCollection>>();
                SrdsCollection srdsCollection = srdsFetcher.Download(new Uri(SRDS_URL), null).Result;

                SetGraphButtonText("Building graph...");
                var graphBuilder = kernel.Get<IGraphBuilder>();
                IGraph graph = graphBuilder.BuildGraph(gtfsCollection.StopTimes, srdsCollection.Destinations, GraphBuilderSettings.Default);
                INode[] nodes = graph.GraphNodes;

                SetGraphButtonText("Displaying nodes...");
                Array.Sort(nodes, new ComparerForDisplay());

                var scene = new ILScene();
                var plotCube = new ILPlotCube(twoDMode: false);

                plotCube.Axes.XAxis.Label.Text = "Latitude";
                plotCube.Axes.YAxis.Label.Text = "Longitude";
                plotCube.Axes.ZAxis.Label.Text = "Time";

                for (int i = 0; i < nodeCount && i < nodes.Length; i++)
                {
                    INode node = nodes[i];
                    foreach (INode neighbor in node.TimeForwardNeighbors)
                    {
                        float[] linePoints =
                        {
                            (float) node.Latitude,
                            (float) node.Longitude,
                            (float) (node.Time - (new DateTime(1970, 1, 1))).TotalSeconds/3600,
                            (float) neighbor.Latitude,
                            (float) neighbor.Longitude,
                            (float) (neighbor.Time - (new DateTime(1970, 1, 1))).TotalSeconds/3600
                        };

                        ILArray<float> line = ILMath.array(linePoints, 3, 2);
                        var linePlot = new ILLinePlot(line, markerStyle: MarkerStyle.Dot);
                        plotCube.Add(linePlot);
                    }
                }

                scene.Add(plotCube);
                SetScene(scene);
                SetGraphButtonText("Load Graph");
                SetFormEnabled(true);
            });
        }

        private delegate void BooleanDelegate(bool b);

        private delegate void SceneDelegate(ILScene scene);

        private delegate void StringDelegate(string s);
    }
}