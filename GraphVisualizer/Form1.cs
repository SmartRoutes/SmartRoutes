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
using SortaScraper.Parsers;
using SortaScraper.Support;
using Ninject;
using Ninject.Extensions.Conventions;
using Model.Sorta;
using Graph;
using Graph.Node;
using System.IO;
using Model.Odjfs;

namespace GraphVisualizer
{
    public partial class Form1 : Form 
    {
        private static Byte[] zipFileBytes = File.ReadAllBytes("C:\\Users\\alcaz0r\\Documents\\School\\CS Senior Design\\streetsmartz\\Sandbox\\sorta\\google_transit_info.zip");

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

            var collection = kernel.Get<IEntityCollectionParser>().Parse(zipFileBytes);

            foreach (StopTime entry in collection.StopTimes)
            {
                entry.Stop = collection.Stops.
                    Single<Stop>(s => s.Id == entry.StopId);
            }

            foreach (StopTime entry in collection.StopTimes)
            {
                entry.Trip = collection.Trips.
                    Single<Trip>(s => s.Id == entry.TripId);
            }

            foreach (Trip entry in collection.Trips)
            {
                entry.Shape = collection.Shapes.
                    Single<Shape>(s => s.Id == entry.ShapeId);
            }
            
            var builder = kernel.Get<IGraphBuilder>();
            INode[] MetroNodes = builder.BuildGraph(collection);

            var scene = new ILScene();
            var plotCube = new ILPlotCube(twoDMode: false);

            plotCube.Axes.XAxis.Label.Text = "Latitude";
            plotCube.Axes.YAxis.Label.Text = "Longitude";
            plotCube.Axes.ZAxis.Label.Text = "Time";
            
            for (int i = 0; i < 500; i++)
            {
                INode node = MetroNodes[i];
                foreach (INode neighbor in node.DownwindNeighbors)
                {               
                    float[] linePoints = 
                    { 
                        (float)node.Latitude(),
                        (float)node.Longitude(),
                        (float)(node.Time() - (new DateTime(1970,1,1))).TotalSeconds / 3600,
                        (float)neighbor.Latitude(), 
                        (float)neighbor.Longitude(), 
                        (float)(neighbor.Time() - (new DateTime(1970,1,1))).TotalSeconds / 3600
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
