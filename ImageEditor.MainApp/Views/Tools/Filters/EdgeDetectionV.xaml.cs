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

namespace ImageEditor.MainApp.Views.Tools.Filters
{
    /// <summary>
    /// Interaction logic for EdgeDetectionV.xaml
    /// </summary>
    public partial class EdgeDetectionV : UserControl
    {
        public EdgeDetectionV()
        {
            RevertCommand = new RevertCommand(
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            MBPreviewCommand = new PreviewCommand(
                () => MBValidate(),
                (b) => Algorithms.EdgeDetection.MinusBlur(b, MBAmount, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            MBApplyCommand = new ApplyCommand(
                () => MBValidate(),
                (b) => Algorithms.EdgeDetection.MinusBlur(b, MBAmount, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            LPreviewCommand = new PreviewCommand(
                () => true,
                (b) => Algorithms.EdgeDetection.Laplacian(b, LTypeSelectedIndex, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            LApplyCommand = new ApplyCommand(
                () => true,
                (b) => Algorithms.EdgeDetection.Laplacian(b, LTypeSelectedIndex, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            LoGPreviewCommand = new PreviewCommand(
                () => LoGValidate(),
                (b) => Algorithms.EdgeDetection.LaplacianOfGaussian(b, LoGRadius, LoGSigma, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            LoGApplyCommand = new ApplyCommand(
                () => LoGValidate(),
                (b) => Algorithms.EdgeDetection.LaplacianOfGaussian(b, LoGRadius, LoGSigma, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            SPreviewCommand = new PreviewCommand(
                () => true,
                (b) => Algorithms.EdgeDetection.Sobel(b, STypeSelectedIndex, SThresholdPercent, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            SApplyCommand = new ApplyCommand(
                () => true,
                (b) => Algorithms.EdgeDetection.Sobel(b, STypeSelectedIndex, SThresholdPercent, Algorithms.Helper.EdgeHandling),
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
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(EdgeDetectionV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(EdgeDetectionV));
        #endregion


        public ICommand RevertCommand { get; }

        #region MINUS BLUR
        public ICommand MBPreviewCommand { get; }
        public ICommand MBApplyCommand { get; }
        public int MBAmount { get; set; }
        private bool MBValidate() => !Validation.GetHasError(MBAmountTBox);
        #endregion


        #region LAPLACIAN
        public ICommand LPreviewCommand { get; }
        public ICommand LApplyCommand { get; }
        public int LTypeSelectedIndex { get; set; }
        #endregion


        #region LAPLACIAN OF GAUSSIAN
        public ICommand LoGPreviewCommand { get; }
        public ICommand LoGApplyCommand { get; }
        public int LoGRadius { get; set; }
        public double LoGSigma { get; set; }
        private bool LoGValidate() => !(Validation.GetHasError(LoGRadiusTBox) || Validation.GetHasError(LoGSigmaTBox));
        #endregion


        #region SOBEL VARIANTS
        public ICommand SPreviewCommand { get; }
        public ICommand SApplyCommand { get; }
        public int STypeSelectedIndex { get; set; }
        public int SThresholdPercent { get; set; }
        #endregion
    }
}
