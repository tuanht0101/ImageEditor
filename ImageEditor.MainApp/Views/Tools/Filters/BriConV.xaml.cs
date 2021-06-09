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
    /// Interaction logic for BriConV.xaml
    /// </summary>
    public partial class BriConV : UserControl, INotifyPropertyChanged
    {
        #region IMPLEMENTATION
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion

        public BriConV()
        {
            RevertCommand = new RevertCommand(
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            PreviewCommand = new PreviewCommand(
                () => Validate(),
                (b) => Algorithms.ColorAdjustment.BrightnessContrast(b, Brightness, Contrast),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            ApplyCommand = new ApplyCommand(
                () => Validate(),
                (b) => Algorithms.ColorAdjustment.BrightnessContrast(b, Brightness, Contrast),
                () => IsProcessingEnabled = true,
                () =>
                {
                    IsProcessingEnabled = false;
                    Brightness = 100;
                    Contrast = 0;
                }
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
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(BriConV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(BriConV));
        #endregion


        public ICommand RevertCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand ApplyCommand { get; }

        private int brightness = 100;
        public int Brightness
        {
            get => brightness;
            set
            {
                brightness = value;
                OnPropertyChanged();
            }
        }
        private int contrast = 0;
        public int Contrast
        {
            get => contrast;
            set
            {
                contrast = value;
                OnPropertyChanged();
            }
        }

        private bool Validate() => !(Validation.GetHasError(BrightnessTBox) ||
            Validation.GetHasError(ContrastTBox));
    }
}
