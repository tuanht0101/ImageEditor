using ImageEditor.MainApp.Models;
using ImageEditor.MainApp.Views.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for ExportV.xaml
    /// </summary>
    public partial class ExportV : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExportV()
        {
            ExportCommand = new RelayCommand<object>(
                (p) =>
                {
                    if (!Directory.Exists(System.IO.Path.GetDirectoryName(OutputPath))) return false;
                    switch (ModeSelectedIndex)
                    {
                        case 0: return true;
                        case 1: return !(SpecificLayerCBox.SelectedIndex == -1);
                        default: return SelectedLayer != null;
                    }
                },
                (p) =>
                {
                    ContentControl dialogHost = ((MainWindow)Application.Current.MainWindow).DialogHost;
                    ImageLayer layer;
                    switch (ModeSelectedIndex)
                    {
                        case 0:
                            dialogHost.Content = Export(OutputPath, ImageLayers, CanvasWidth, CanvasHeight) ?
                            new NotificationDialog($"Exported All Layers to\n{System.IO.Path.GetFileName(OutputPath)}") :
                            new NotificationDialog("Export Failed!");
                            return;
                        case 1:
                            layer = (ImageLayer)SpecificLayerCBox.SelectedItem;
                            break;
                        default:
                            layer = SelectedLayer;
                            break;
                    }
                    dialogHost.Content = Export(OutputPath, layer) ?
                            new NotificationDialog($"Exported {layer.Name} to\n{System.IO.Path.GetFileName(OutputPath)}") :
                            new NotificationDialog("Export Failed!");
                }
            );

            BrowseCommand = new RelayCommand<object>(
                (p) => true,
                (p) =>
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        Filter = "PNG File (*.png)|*.png",
                        RestoreDirectory = true
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        OutputPath = saveFileDialog.FileName;
                    }
                }
            );
            InitializeComponent();
        }


        #region DEPENDENCY PROPERTIES
        public ObservableCollection<ImageLayer> ImageLayers
        {
            get { return (ObservableCollection<ImageLayer>)GetValue(ImageLayersProperty); }
            set { SetValue(ImageLayersProperty, value); }
        }
        public static readonly DependencyProperty ImageLayersProperty =
            DependencyProperty.Register("ImageLayers", typeof(ObservableCollection<ImageLayer>), typeof(ExportV));

        public ImageLayer SelectedLayer
        {
            get { return (ImageLayer)GetValue(SelectedLayerProperty); }
            set { SetValue(SelectedLayerProperty, value); }
        }
        public static readonly DependencyProperty SelectedLayerProperty =
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(ExportV));


        public int CanvasWidth
        {
            get { return (int)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(int), typeof(ExportV));

        public int CanvasHeight
        {
            get { return (int)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(int), typeof(ExportV));
        #endregion


        public ICommand BrowseCommand { get; }
        public ICommand ExportCommand { get; }


        private string outputPath;
        public string OutputPath
        {
            get => outputPath;
            set
            {
                outputPath = value;
                OnPropertyChanged();
            }
        }

        public int ModeSelectedIndex { get; set; }

        public ImageLayer SpecificLayerSelectedIndex { get; set; }

        public List<string> ModesTitle { get; } = new List<string>()
        {
            "All", "Specific Layer", "Selected Layer"
        };



        #region EXPORT METHODS
        private bool Export(string outputFileName, ImageLayer imageLayer)
        {
            try
            {
                imageLayer.OriginalBitmap.Save(outputFileName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool Export(string outputFileName, IEnumerable<ImageLayer> imageLayers, int width, int height)
        {
            try
            {
                Bitmap canvas = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    foreach (ImageLayer layer in imageLayers)
                    {
                        g.DrawImage(layer.OriginalBitmap, layer.XOffset, layer.YOffset, layer.OriginalBitmap.Width, layer.OriginalBitmap.Height);
                    }
                    g.Save();
                }
                canvas.Save(outputFileName);
                canvas.Dispose();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
