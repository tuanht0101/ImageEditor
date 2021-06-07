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

namespace ImageEditor.MainApp.Views.CompositeTabs
{
    /// <summary>
    /// Interaction logic for CanvasV.xaml
    /// </summary>
    public partial class CanvasV : UserControl
    {
        public CanvasV()
        {
            InitializeComponent();
        }


        private void ScaleBtn_Click(object sender, RoutedEventArgs e)
        {
            CanvasWidth = ImageLayers.Max(l => l.OriginalBitmap.Width);
            CanvasHeight = ImageLayers.Max(l => l.OriginalBitmap.Height);
        }


        #region DEPENDENCY PROPERTIES
        public int CanvasWidth
        {
            get { return (int)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(int), typeof(CanvasV));

        public int CanvasHeight
        {
            get { return (int)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(int), typeof(CanvasV));

        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(CanvasV));

        public bool IsRender
        {
            get { return (bool)GetValue(IsRenderProperty); }
            set { SetValue(IsRenderProperty, value); }
        }
        public static readonly DependencyProperty IsRenderProperty =
            DependencyProperty.Register("IsRender", typeof(bool), typeof(CanvasV));


        public IEnumerable<ImageLayer> ImageLayers
        {
            get { return (IEnumerable<ImageLayer>)GetValue(ImageLayersProperty); }
            set { SetValue(ImageLayersProperty, value); }
        }
        public static readonly DependencyProperty ImageLayersProperty =
            DependencyProperty.Register("ImageLayers", typeof(IEnumerable<ImageLayer>), typeof(CanvasV));
        #endregion
    }
}
