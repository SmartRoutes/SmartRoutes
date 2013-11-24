using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using SmartRoutes.SortaScraper.Parsers;
using SmartRoutes.SortaScraper.Support;
using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.Model.Sorta;
using SmartRoutes.Graph;
using SmartRoutes.Graph.Node;
using SmartRoutes.Graph.Comparers;
using System.IO;
using SmartRoutes.Model.Odjfs;

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
                .FromAssemblyContaining(typeof(IEntityCollectionParser))
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
                foreach (INode neighbor in node.DownwindNeighbors)
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
