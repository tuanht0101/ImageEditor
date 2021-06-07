using ImageEditor.MainApp.Models;
using ImageEditor.MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageEditor.MainApp.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for NewCanvasDialog.xaml
    /// </summary>
    public partial class NewCanvasDialog : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NewCanvasDialog()
        {
            Loaded += (s, e) => WidthTBox.Focus();
            ConfirmCommand = new RelayCommand<object>(
                (p) => !(Validation.GetHasError(WidthTBox) ||
                Validation.GetHasError(HeightTBox) ||
                !(System.IO.File.Exists(FileTBox.Text) || FileTBox.Text == string.Empty)),
                async (p) =>
                {
                    SetValue(CanvasWidthProperty, PreviewWidth);
                    SetValue(CanvasHeightProperty, PreviewHeight);
                    if (FileName != null) ImageLayers.Add(new ImageLayer("Layer 0", FileName));
                    unloadedStoryboard.Begin();
                    await Task.Delay(unloadedStoryboard.Duration.TimeSpan);
                    ((ContentControl)Parent).Content = null;
                }
                );

            InitializeComponent();
            unloadedStoryboard = FindResource("DialogUnloadedSb") as Storyboard;
        }

        #region DEPENDENCY PROPERTIES
        public ObservableCollection<ImageLayer> ImageLayers
        {
            get { return (ObservableCollection<ImageLayer>)GetValue(ImageLayersProperty); }
            set { SetValue(ImageLayersProperty, value); }
        }
        public static readonly DependencyProperty ImageLayersProperty =
            DependencyProperty.Register("ImageLayers", typeof(ObservableCollection<ImageLayer>), typeof(NewCanvasDialog));

        public ImageLayer SelectedLayer
        {
            get { return (ImageLayer)GetValue(SelectedLayerProperty); }
            set { SetValue(SelectedLayerProperty, value); }
        }
        public static readonly DependencyProperty SelectedLayerProperty =
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(NewCanvasDialog));


        public int CanvasWidth
        {
            get { return (int)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(int), typeof(NewCanvasDialog));

        public int CanvasHeight
        {
            get { return (int)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(int), typeof(NewCanvasDialog));
        #endregion

        #region PROPERTIES
        private readonly Storyboard unloadedStoryboard;

        public ICommand ConfirmCommand { get; }

        public string FileName { get; set; }

        private int previewWidth = 300;
        public int PreviewWidth
        {
            get => previewWidth;
            set
            {
                previewWidth = value;
                OnPropertyChanged();
            }
        }

        private int previewHeight = 300;
        public int PreviewHeight
        {
            get => previewHeight;
            set
            {
                previewHeight = value;
                OnPropertyChanged();
            }
        }
        #endregion


        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileName = Utilities.OpenImageFileDialog();
            if (fileName != null)
            {
                using (Bitmap bitmap = new Bitmap(fileName))
                {
                    FileName = fileName;
                    PreviewWidth = bitmap.Width;
                    PreviewHeight = bitmap.Height;
                    OnPropertyChanged("FileName");
                }
            }
        }
    }
}
