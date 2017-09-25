using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.UI;
using System.Windows.Media;
using System.Windows;

namespace ArcGISApp1
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapViewModel : INotifyPropertyChanged
    {
        public MapViewModel()
        { 
            _scene = new Scene(Basemap.CreateImagery());
            _grapchicsOverlays = new GraphicsOverlayCollection();
            _tempOverlay = new GraphicsOverlay();
            graphicsOverlay = new GraphicsOverlay();
            var elevationSource = new ArcGISTiledElevationSource(new System.Uri("http://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer"));
            var sceneSurface = new Surface();
            sceneSurface.ElevationSources.Add(elevationSource);
            _scene.BaseSurface = sceneSurface;
            _grapchicsOverlays.Add(_tempOverlay);
            _grapchicsOverlays.Add(graphicsOverlay);
            _lineTextBlock = "Line Length: 0.00 meters";
            _areaTextBlock = "Area Size: 0.00 square meters";
            _loadCloseVisibility = Visibility.Hidden;
        }
        private Scene _scene;
        private String _lineTextBlock;
        private String _areaTextBlock;
        private GraphicsOverlayCollection _grapchicsOverlays;
        private GraphicsOverlay graphicsOverlay;
        private GraphicsOverlay _tempOverlay;
        private Visibility _loadCloseVisibility;
      
        public void Line(Polyline temp,string type)
        {
            var lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.FromArgb(255, 255, 0, 0), 2.0);
            var boatTripGraphic = new Graphic(temp, lineSymbol);
            if (type == "line")
            {
                _tempOverlay.Graphics.Clear();
                graphicsOverlay.Graphics.Add(boatTripGraphic);
            }
            else { 
                _tempOverlay.Graphics.Add(boatTripGraphic);
            }
        }

        public void Area(Polygon temp)
        {
            _tempOverlay.Graphics.Clear();
            var lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.FromArgb(255, 255, 0, 0), 1.0);
            var fillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, Color.FromArgb(155, 255, 255, 0), lineSymbol);
            var boatTripGraphic = new Graphic(temp, fillSymbol);
            graphicsOverlay.Graphics.Add(boatTripGraphic);
        }

        public void Clear()
        {
            _tempOverlay.Graphics.Clear();
            graphicsOverlay.Graphics.Clear();
        }
        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Scene Scene
        {
            get { return _scene; }
            set { _scene = value; OnPropertyChanged(); }
        }

        public String LineTextBlock {
            get { return _lineTextBlock; }
            set { _lineTextBlock = value; OnPropertyChanged("LineTextBlock"); }
        }
        public String AreaTextBlock
        {
            get { return _areaTextBlock; }
            set { _areaTextBlock = value; OnPropertyChanged("AreaTextBlock"); }
        }
        public GraphicsOverlayCollection GraphicsOverlays
        {
            get { return _grapchicsOverlays; }
            set { _grapchicsOverlays = value; OnPropertyChanged(); }
        }
        
        public Visibility LoadCloseVisibility
        {
            get { return _loadCloseVisibility; }
            set
            {
                _loadCloseVisibility = value;
                OnPropertyChanged("LoadCloseVisibility");
            }
        }

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }
      
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
