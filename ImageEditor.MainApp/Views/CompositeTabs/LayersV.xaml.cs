using ImageEditor.MainApp.Models;
using ImageEditor.MainApp.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ImageEditor.MainApp.Views.CompositeTabs
{
    /// <summary>
    /// Interaction logic for LayersV.xaml
    /// </summary>
    public partial class LayersV : UserControl
    {
        public LayersV()
        {
            InitializeComponent();
        }

        #region DEPENDENCY PROPERTIES
        public ObservableCollection<ImageLayer> ImageLayers
        {
            get { return (ObservableCollection<ImageLayer>)GetValue(ImageLayersProperty); }
            set { SetValue(ImageLayersProperty, value); }
        }
        public static readonly DependencyProperty ImageLayersProperty =
            DependencyProperty.Register("ImageLayers", typeof(ObservableCollection<ImageLayer>), typeof(LayersV));

        public ImageLayer SelectedLayer
        {
            get { return (ImageLayer)GetValue(SelectedLayerProperty); }
            set { SetValue(SelectedLayerProperty, value); }
        }
        public static readonly DependencyProperty SelectedLayerProperty =
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(LayersV));

        public int SelectedLayerIndex
        {
            get { return (int)GetValue(SelectedLayerIndexProperty); }
            set { SetValue(SelectedLayerIndexProperty, value); }
        }
        public static readonly DependencyProperty SelectedLayerIndexProperty =
            DependencyProperty.Register("SelectedLayerIndex", typeof(int), typeof(LayersV));

        public bool IsHighlightRendered
        {
            get { return (bool)GetValue(IsHighlightRenderedProperty); }
            set { SetValue(IsHighlightRenderedProperty, value); }
        }
        public static readonly DependencyProperty IsHighlightRenderedProperty =
            DependencyProperty.Register("IsHighlightRendered", typeof(bool), typeof(LayersV));
        #endregion


        #region CUSTOM EVENTS
        public event RoutedEventHandler SelectedLayerChanged
        {
            add { AddHandler(SelectedLayerChangedEvent, value); }
            remove { RemoveHandler(SelectedLayerChangedEvent, value); }
        }
        public static readonly RoutedEvent SelectedLayerChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedLayerChanged", RoutingStrategy.Bubble, typeof(EventHandler), typeof(LayersV)
            );

        private void RaiseSelectedLayerChanged(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SelectedLayerChangedEvent));
        }
        #endregion
    }
}
