#region
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using MIConvexHull;
using System.Linq;
using System.Diagnostics;
#endregion
namespace Triangulacja_Delaunay_z_ograniczeniami
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>
        private List<Vertex> Vertices;
        public int NumberOfVertices = 50;
        VoronoiMesh<Vertex, Cell, VoronoiEdge<Vertex, Cell>> voronoiMesh;

        public MainWindow()
        {
            InitializeComponent();
            //this.Title += string.Format(" ({0} punktów)", NumberOfVertices);
            TxtPktCount.Text = string.Format("Ilość punktów: {0}", NumberOfVertices);
            btnFindDelaunay.IsEnabled = false;
        }

        void ShowVertices(List<Vertex> vertices)
        {
            for (var i = 0; i < vertices.Count; i++)
            {
                drawingCanvas.Children.Add(vertices[i]);
            }
        }

        void MakeRandom(int n, List<Vertex> vertices)
        {
            var r = new Random();
            var sizeX = drawingCanvas.ActualWidth;
            var sizeY = drawingCanvas.ActualHeight;
            for (var i = 0; i < n; i++)
            {
                var vi = new Vertex(sizeX * r.NextDouble(), sizeY * r.NextDouble());
                vertices.Add(vi);
            }
        }

        void Create(List<Vertex> vertices, bool translate)
        {

            drawingCanvas.Children.Clear();
            ShowVertices(vertices);

            var config = !translate
                ? new TriangulationComputationConfig()
                : new TriangulationComputationConfig
                {
                    PointTranslationType = PointTranslationType.TranslateInternal,
                    PlaneDistanceTolerance = 0.00001,
                // the translation radius should be lower than PlaneDistanceTolerance / 2
                PointTranslationGenerator = TriangulationComputationConfig.RandomShiftByRadius(0.000001, 0)
                };

            try
            {
                voronoiMesh = VoronoiMesh.Create<Vertex, Cell>(vertices, config);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return;
            }
            txtBlkTimer.Text = string.Format("{0} faces", voronoiMesh.Vertices.Count());

            Vertices = vertices;

            btnFindDelaunay.IsEnabled = true;
        }

        private void btnMakePoints_Click(object sender, RoutedEventArgs e)
        {
            var vs = new List<Vertex>();
            MakeRandom(NumberOfVertices, vs);
            Create(vs, false);
        }

        private void btnFindDelaunay_Click(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Children.Clear();

            btnFindDelaunay.IsEnabled = false;

            foreach (var cell in voronoiMesh.Vertices)
            {
                drawingCanvas.Children.Add(cell.Visual);
            }

            ShowVertices(Vertices);
        }

        static int IsLeft(Point a, Point b, Point c)
        {
            return ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) > 0 ? 1 : -1;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(NumberOfVertices < 901 || NumberOfVertices > 1)
            NumberOfVertices += 10;

            TxtPktCount.Text = string.Format("Ilość punktów: {0}", NumberOfVertices);
        }

        private void btnSub_Click(object sender, RoutedEventArgs e)
        {
            if (NumberOfVertices < 901 || NumberOfVertices > 1)
                NumberOfVertices -= 10;
            TxtPktCount.Text = string.Format("Ilość punktów: {0}", NumberOfVertices);
        }
    }
}
