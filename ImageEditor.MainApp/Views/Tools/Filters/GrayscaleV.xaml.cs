using ImageEditor.MainApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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
    /// Interaction logic for GrayscaleV.xaml
    /// </summary>
    public partial class GrayscaleV : UserControl, INotifyPropertyChanged
    {
        #region IMPLEMENTATION
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion

        public GrayscaleV()
        {
            RevertCommand = new RevertCommand(
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            PreviewCommand = new PreviewCommand(
                () => Validate(),
                (b) => Algorithms.GrayScale.Process(b, RedRatio, GreenRatio, BlueRatio),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );
            ApplyCommand = new ApplyCommand(
                () => Validate(),
                (b) => Algorithms.GrayScale.Process(b, RedRatio, GreenRatio, BlueRatio),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            InitializeComponent();
        }

        public ImageLayer SelectedLayer
        {
            get { return (ImageLayer)GetValue(SelectedLayerProperty); }
            set { SetValue(SelectedLayerProperty, value); }
        }
        public static readonly DependencyProperty SelectedLayerProperty =
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(GrayscaleV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(GrayscaleV));


        public ObservableCollection<GrayScalePreset> Presets { get; } = new ObservableCollection<GrayScalePreset>()
        {
            new GrayScalePreset("Standard", 0.3, 0.59, 0.11),
            new GrayScalePreset("BT.601", 0.299, 0.587, 0.114),
            new GrayScalePreset("BT.709", 0.2126, 0.7152, 0.0722),
            new GrayScalePreset("BT.2627", 0.2627, 0.6780, 0.0593),
            new GrayScalePreset("Average", 0.33, 0.33, 0.33),
        };

        public ICommand RevertCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand ApplyCommand { get; }

        private GrayScalePreset selectedPreset;
        public GrayScalePreset SelectedPreset
        {
            get => selectedPreset;
            set
            {
                selectedPreset = value;
                RedRatio = value.RedRatio;
                GreenRatio = value.GreenRatio;
                BlueRatio = value.BlueRatio;
                OnPropertyChanged("RedRatio");
                OnPropertyChanged("GreenRatio");
                OnPropertyChanged("BlueRatio");
            }
        }

        public double RedRatio { get; set; }
        public double GreenRatio { get; set; }
        public double BlueRatio { get; set; }

        private bool Validate()
        {
            return !(
                Validation.GetHasError(RedRatioTBox) ||
                Validation.GetHasError(GreenRatioTBox) ||
                Validation.GetHasError(BlueRatioTBox)
                );
        }
    }

    public struct GrayScalePreset
    {
        public GrayScalePreset(string name, double redRatio, double greenRatio, double blueRatio)
        {
            Name = name;
            RedRatio = redRatio;
            GreenRatio = greenRatio;
            BlueRatio = blueRatio;
        }

        public string Name { get; }
        public double RedRatio { get; }
        public double GreenRatio { get; }
        public double BlueRatio { get; }
    }
}
