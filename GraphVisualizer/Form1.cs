using System;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using SmartRoutes.GtfsReader.Parsers;
using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.Graph;
using SmartRoutes.Graph.Node;
using SmartRoutes.Graph.Comparers;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader;

namespace SmartRoutes.GraphVisualizer
{
    public partial class Form1 : Form 
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e)
        {
            IKernel kernel = new StandardKernel(new GraphModule());

            kernel.Bind(c => c
                .FromAssemblyContaining(typeof(GtfsCollection))
                .SelectAllClasses()
                .BindAllInterfaces());

            var graph = kernel.Get<IGraph>();

            var Nodes = graph.GraphNodes;

            Array.Sort(Nodes, new ComparerForDisplay());

            var scene = new ILScene();
            var plotCube = new ILPlotCube(twoDMode: false);

            plotCube.Axes.XAxis.Label.Text = "Latitude";
            plotCube.Axes.YAxis.Label.Text = "Longitude";
            plotCube.Axes.ZAxis.Label.Text = "Time";
            
            for (int i = 0; i < 1000 && i < Nodes.Length; i++)
            {
                INode node = Nodes[i];
                foreach (INode neighbor in node.TimeForwardNeighbors)
                {               
                    float[] linePoints = 
                    { 
                        (float)node.Latitude,
                        (float)node.Longitude,
                        (float)(node.Time - (new DateTime(1970,1,1))).TotalSeconds / 3600,
                        (float)neighbor.Latitude, 
                        (float)neighbor.Longitude, 
                        (float)(neighbor.Time - (new DateTime(1970,1,1))).TotalSeconds / 3600
                    };

                    int dimensions = 3, numPoints = 2;
                    ILArray<float> line = ILMath.array<float>(linePoints, dimensions, numPoints);
                    ILLinePlot linePlot = new ILLinePlot(line, markerStyle: MarkerStyle.Dot);
                    plotCube.Add(linePlot);
                }
            }

            scene.Add(plotCube);

            ilPanel1.Scene = scene; 
        }
    }
}
