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
    /// Interaction logic for HueSatV.xaml
    /// </summary>
    public partial class HueSatV : UserControl, INotifyPropertyChanged
    {
        #region IMPLEMENTATION
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion

        public HueSatV()
        {
            RevertCommand = new RevertCommand(
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            PreviewCommand = new PreviewCommand(
                () => Validate(),
                (b) => Algorithms.ColorAdjustment.HSL(b, Hue, Saturation, Lightness),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            ApplyCommand = new ApplyCommand(
                () => Validate(),
                (b) => Algorithms.ColorAdjustment.HSL(b, Hue, Saturation, Lightness),
                () => IsProcessingEnabled = true,
                () =>
                {
                    IsProcessingEnabled = false;
                    Hue = 0;
                    Saturation = 0;
                    Lightness = 0;
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
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(HueSatV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(HueSatV));
        #endregion


        public ICommand RevertCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand ApplyCommand { get; }

        private int hue = 0;
        public int Hue
        {
            get => hue;
            set
            {
                hue = value;
                OnPropertyChanged();
            }
        }
        private int saturation = 0;
        public int Saturation
        {
            get => saturation;
            set
            {
                saturation = value;
                OnPropertyChanged();
            }
        }
        private int lightness = 0;
        public int Lightness
        {
            get => lightness;
            set
            {
                lightness = value;
                OnPropertyChanged();
            }
        }

        private bool Validate() => !(Validation.GetHasError(HueTBox) ||
            Validation.GetHasError(SaturationTBox) ||
            Validation.GetHasError(LightnessTBox));
    }
}
