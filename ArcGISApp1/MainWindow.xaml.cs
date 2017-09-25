using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcGISApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _mapViewModel = this.FindResource("MapViewModel") as MapViewModel;
            EditButton.Click += OnEditClicked;
            LineButton.Click += OnLineClicked;
            AreaButton.Click += OnAreaClicked;        
            CloseButton.Click += OnCloseClicked;
            ClearButton.Click += OnClearClicked;
            var snowdonCamera = new Camera(53.06, -4.04, 1289, 295, 71, 0);
            MySceneView.SetViewpointCamera(snowdonCamera);
        }

        private MapViewModel _mapViewModel;
        List<MapPoint> mapPoint_list = new List<MapPoint>();
        public bool edit = false;
        public string type = "default";

        private void OnEditClicked(object sender, RoutedEventArgs e)
        {
            if (LineButton.IsEnabled == false)
            {
                edit = true;
                LineButton.IsEnabled = true;
                AreaButton.IsEnabled = true;
                ClearButton.IsEnabled = true;
            }
            else {
                edit = false;
                LineButton.IsEnabled = false;
                AreaButton.IsEnabled = false;
                ClearButton.IsEnabled = false;
                mapPoint_list.Clear();
            }
        }
        private void OnLineClicked(object sender, RoutedEventArgs e) {
            type = "line";
        }

        private void OnAreaClicked(object sender, RoutedEventArgs e)
        {
            type = "area";
        }
     
        private void OnClearClicked(object sender, RoutedEventArgs e)
        {
            mapPoint_list.Clear();
            _mapViewModel.ClearAll();
            LineText.Text = "Line Length: 0.00 meters";
            AreaText.Text = "Area Size: 0.00 square meters";
        }
        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            BorderContentText.Visibility = Visibility.Hidden;
        }
        private  void MySceneView_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (type == "line" && mapPoint_list.Count != 0)
            {
                BorderContentText.Visibility = Visibility.Visible;
                var boatPositions = new PolylineBuilder(SpatialReferences.Wgs84);
                for (var i = 0; i < mapPoint_list.Count; i++)
                {
                    boatPositions.AddPoint(new MapPoint(mapPoint_list[i].X, mapPoint_list[i].Y));
                }
                var boatRoute = boatPositions.ToGeometry();
                _mapViewModel.Line(boatRoute, "line");
                LineText.Text = "Line Length: " + GeometryEngine.LengthGeodetic(boatRoute).ToString("N2") + " meters";
            }
            else if (type == "area" && mapPoint_list.Count !=0)
            {
                BorderContentText.Visibility = Visibility.Visible;
                var boatPositions = new PolygonBuilder(SpatialReferences.Wgs84);
                for (var i = 0; i < mapPoint_list.Count; i++)
                {
                    boatPositions.AddPoint(new MapPoint(mapPoint_list[i].X, mapPoint_list[i].Y));
                }
                var boatRoute = boatPositions.ToGeometry();
                _mapViewModel.Area(boatRoute);
                AreaText.Text = "Area Size: " + GeometryEngine.AreaGeodetic(boatRoute).ToString("N2") + "  square meters";
            }
            mapPoint_list.Clear();
        }

        private async void MySceneView_MouseLeftDown(object sender, MouseEventArgs e)
        {
            if (edit == true && (type=="line" || type =="area"))
            {
                if (MySceneView.GetCurrentViewpoint(ViewpointType.BoundingGeometry) == null)
                    return;
                _mapViewModel.Clear(type);
                System.Windows.Point screenPoint = e.GetPosition(MySceneView);
                MapPoint mapPoint = await MySceneView.ScreenToLocationAsync(screenPoint);
                if (mapPoint.X != 0 && mapPoint.Y != 0 && mapPoint.Z != 0)
                {
                    mapPoint = GeometryEngine.NormalizeCentralMeridian(mapPoint) as MapPoint;
                    mapPoint_list.Add(new MapPoint(mapPoint.X, mapPoint.Y));
                    if (mapPoint_list.Count > 1)
                    {
                        var boatPositions = new PolylineBuilder(SpatialReferences.Wgs84);
                        boatPositions.AddPoint(new MapPoint(mapPoint_list[mapPoint_list.Count - 2].X, mapPoint_list[mapPoint_list.Count - 2].Y));
                        boatPositions.AddPoint(new MapPoint(mapPoint_list[mapPoint_list.Count - 1].X, mapPoint_list[mapPoint_list.Count - 1].Y));
                        var boatRoute = boatPositions.ToGeometry();
                        _mapViewModel.Line(boatRoute, "temp"); ;
                    }
                }
            }
        }
    }
}
