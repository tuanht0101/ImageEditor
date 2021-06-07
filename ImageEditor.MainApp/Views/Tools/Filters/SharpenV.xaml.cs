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
    /// Interaction logic for SharpenV.xaml
    /// </summary>
    public partial class SharpenV : UserControl
    {
        public SharpenV()
        {
            RevertCommand = new RevertCommand(
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            UMPreviewCommand = new PreviewCommand(
                () => UMValidate(),
                (b) => Algorithms.Sharpening.UnsharpMasking(b, UMAmount, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            UMApplyCommand = new ApplyCommand(
                () => UMValidate(),
                (b) => Algorithms.Sharpening.UnsharpMasking(b, UMAmount, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            LPreviewCommand = new PreviewCommand(
                () => true,
                (b) => Algorithms.Sharpening.Laplacian(b, LTypeSelectedIndex, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            LApplyCommand = new ApplyCommand(
                () => true,
                (b) => Algorithms.Sharpening.Laplacian(b, LTypeSelectedIndex, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            LoGPreviewCommand = new PreviewCommand(
                () => LoGValidate(),
                (b) => Algorithms.Sharpening.LaplacianOfGaussian(b, LoGRadius, LoGSigma, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            LoGApplyCommand = new ApplyCommand(
                () => LoGValidate(),
                (b) => Algorithms.Sharpening.LaplacianOfGaussian(b, LoGRadius, LoGSigma, Algorithms.Helper.EdgeHandling),
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
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(SharpenV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(SharpenV));
        #endregion


        public ICommand RevertCommand { get; }

        #region UNSHARP MASKING
        public ICommand UMPreviewCommand { get; }
        public ICommand UMApplyCommand { get; }
        public int UMAmount { get; set; }
        private bool UMValidate() => !Validation.GetHasError(UMAmountTBox);
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
    }
}
