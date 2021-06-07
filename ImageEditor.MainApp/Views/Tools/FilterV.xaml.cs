using ImageEditor.MainApp.Models;
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

namespace ImageEditor.MainApp.Views.Tools
{
    /// <summary>
    /// Interaction logic for FilterV.xaml
    /// </summary>
    public partial class FilterV : UserControl
    {
        public FilterV()
        {
            InitializeComponent();
        }

        #region DEPENDENCY PROPERTIES
        public ImageLayer SelectedLayer
        {
            get { return (ImageLayer)GetValue(SelectedLayerProperty); }
            set { SetValue(SelectedLayerProperty, value); }
        }
        public static readonly DependencyProperty SelectedLayerProperty =
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(FilterV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(FilterV));
        #endregion

        public int EdgeHandingSelectedIndex
        {
            get => (int)Algorithms.Helper.EdgeHandling;
            set => Algorithms.Helper.EdgeHandling = (Algorithms.Convolution.EdgeHandling)value;
        }
    }
}
