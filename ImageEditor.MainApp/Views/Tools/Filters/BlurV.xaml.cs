using ImageEditor.MainApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ImageEditor.MainApp.Views.Tools.Filters
{
    /// <summary>
    /// Interaction logic for BlurV.xaml
    /// </summary>
    public partial class BlurV : UserControl, INotifyPropertyChanged
    {
        #region IMPLEMENTATION
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion

        public BlurV()
        {
            RevertCommand = new RevertCommand(
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            BPreviewCommand = new PreviewCommand(
                () => BValidate(),
                (b) => Algorithms.Blur.BoxBlur(b, BRadius, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            BApplyCommand = new ApplyCommand(
                () => BValidate(),
                (b) => Algorithms.Blur.BoxBlur(b, BRadius, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            GPreviewCommand = new PreviewCommand(
                () => GValidate(),
                (b) => Algorithms.Blur.GaussianBlur(b, GRadius, GSigma, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            GApplyCommand = new ApplyCommand(
                () => GValidate(),
                (b) => Algorithms.Blur.GaussianBlur(b, GRadius, GSigma, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            InitializeComponent();
        }


        #region DEPENDENCY PROPERTIES
        public ImageLayer SelectedLayer
        {
            get { return (ImageLayer)GetValue(SelectedLayerProperty); }
            set { SetValue(SelectedLayerProperty, value); }
        }
        public static readonly DependencyProperty SelectedLayerProperty =
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(BlurV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(BlurV));
        #endregion


        public ICommand RevertCommand { get; }

        #region BOX BLUR
        public ICommand BPreviewCommand { get; }
        public ICommand BApplyCommand { get; }
        public int BRadius { get; set; }
        private bool BValidate() => !Validation.GetHasError(BRadiusTBox);
        #endregion


        #region GAUSSIAN BLUR
        public ICommand GPreviewCommand { get; }
        public ICommand GApplyCommand { get; }
        private int gRadius;
        public int GRadius
        {
            get => gRadius;
            set
            {
                gRadius = value;
                RecommendGRadius = "";
                RecommendGSigma = $"Recommend: {Math.Round(GRadius / 3d, 2)}";
                OnPropertyChanged(nameof(RecommendGRadius));
                OnPropertyChanged(nameof(RecommendGSigma));
            }
        }
        private double gSigma;
        public double GSigma
        {
            get => gSigma;
            set
            {
                gSigma = value;
                RecommendGRadius = $"Recommend: {Math.Ceiling(3 * GSigma)}";
                RecommendGSigma = "";
                OnPropertyChanged(nameof(RecommendGRadius));
                OnPropertyChanged(nameof(RecommendGSigma));
            }
        }
        public string RecommendGRadius { get; set; }
        public string RecommendGSigma { get; set; }
        private bool GValidate() => !(Validation.GetHasError(GRadiusTBox) || Validation.GetHasError(GSigmaTBox));
        #endregion
    }
}
