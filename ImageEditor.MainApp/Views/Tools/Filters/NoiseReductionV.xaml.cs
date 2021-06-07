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
using static ImageEditor.MainApp.Algorithms.Convolution;

namespace ImageEditor.MainApp.Views.Tools.Filters
{
    /// <summary>
    /// Interaction logic for NoiseReductionV.xaml
    /// </summary>
    public partial class NoiseReductionV : UserControl
    {
        public NoiseReductionV()
        {
            RevertCommand = new RevertCommand(
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            APreviewCommand = new PreviewCommand(
                () => AValidate(),
                (b) => Algorithms.NoiseReduction.Average(b, ARadius, AThreshold, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            AApplyCommand = new ApplyCommand(
                () => AValidate(),
                (b) => Algorithms.NoiseReduction.Average(b, ARadius, AThreshold, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            MPreviewCommand = new PreviewCommand(
                () => MValidate(),
                (b) => Algorithms.NoiseReduction.Median(b, MRadius, Algorithms.Helper.EdgeHandling),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            MApplyCommand = new ApplyCommand(
                () => MValidate(),
                (b) => Algorithms.NoiseReduction.Median(b, MRadius, Algorithms.Helper.EdgeHandling),
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
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(NoiseReductionV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(NoiseReductionV));
        #endregion


        public ICommand RevertCommand { get; }

        #region AVERAGE
        public ICommand APreviewCommand { get; }
        public ICommand AApplyCommand { get; }
        public int ARadius { get; set; }
        public int AThreshold { get; set; }
        public bool AValidate() => !(Validation.GetHasError(ARadiusTBox) || Validation.GetHasError(AThresholdTBox));
        #endregion

        #region MEDIAN
        public ICommand MPreviewCommand { get; }
        public ICommand MApplyCommand { get; }
        public int MRadius { get; set; }
        public bool MValidate() => !Validation.GetHasError(MRadiusTBox);
        #endregion
    }
}
