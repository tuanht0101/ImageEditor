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

namespace ImageEditor.MainApp.Views.Tools
{
    /// <summary>
    /// Interaction logic for ScaleV.xaml
    /// </summary>
    public partial class ScaleV : UserControl, INotifyPropertyChanged
    {
        #region IMPLEMENTATIONS
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion

        public ScaleV()
        {
            RevertCommand = new RevertCommand(
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            PreviewCommand = new PreviewCommand(
                () => Validate(),
                (b) =>
                {
                    switch(Algorithms.Helper.ResamplingType)
                    {
                        default:
                            return Algorithms.Resample.Nearest(b, desiredWidth, desiredHeight);
                        case Algorithms.Resample.Type.Bilinear:
                            return Algorithms.Resample.Bilinear(b, desiredWidth, desiredHeight);
                    }
                },
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            ApplyCommand = new ApplyCommand(
                () => Validate(),
                (b) =>
                {
                    switch (Algorithms.Helper.ResamplingType)
                    {
                        default:
                            return Algorithms.Resample.Nearest(b, desiredWidth, desiredHeight);
                        case Algorithms.Resample.Type.Bilinear:
                            return Algorithms.Resample.Bilinear(b, desiredWidth, desiredHeight);
                    }
                },
                () => IsProcessingEnabled = true,
                () =>
                {
                    IsProcessingEnabled = false;
                    SelectedLayerUpdate(null, null);
                });

            InitializeComponent();
        }


        #region DEPENDENCY PROPERTIES
        public ImageLayer SelectedLayer
        {
            get { return (ImageLayer)GetValue(SelectedLayerProperty); }
            set { SetValue(SelectedLayerProperty, value); }
        }
        public static readonly DependencyProperty SelectedLayerProperty =
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(ScaleV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(ScaleV)); 
        #endregion


        public ICommand RevertCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand ApplyCommand { get; }


        #region PROPERTIES
        public int ResampleSelectedIndex
        {
            get => (int)Algorithms.Helper.ResamplingType;
            set => Algorithms.Helper.ResamplingType = (Algorithms.Resample.Type)value;
        }

        public string SourceRatio
        {
            get
            {
                if (SelectedLayer == null) return $"NaN";
                int w = SelectedLayer.OriginalBitmap.Width;
                int h = SelectedLayer.OriginalBitmap.Height;
                int gcd = Utilities.GCD(w, h);
                return $"{w / gcd} : {h / gcd}";
            }
        }

        public bool IsRatioLocked { get; set; }

        private int desiredWidth;
        public int DesiredWidth
        {
            get => desiredWidth;
            set
            {
                desiredWidth = value;
                OnPropertyChanged();
                if (IsRatioLocked)
                {
                    desiredHeight = (int)Math.Round((double)value / SelectedLayer.OriginalBitmap.Width * SelectedLayer.OriginalBitmap.Height);
                    OnPropertyChanged("DesiredHeight");
                    OnPropertyChanged("DesiredRatio");
                }
                OnPropertyChanged("DesiredRatio");
            }
        }

        private int desiredHeight;
        public int DesiredHeight
        {
            get => desiredHeight;
            set
            {
                desiredHeight = value;
                OnPropertyChanged();
                if (IsRatioLocked)
                {
                    desiredWidth = (int)Math.Round((double)value * SelectedLayer.OriginalBitmap.Width / SelectedLayer.OriginalBitmap.Height);
                    OnPropertyChanged("DesiredWidth");
                    OnPropertyChanged("DesiredRatio");
                }
                OnPropertyChanged("DesiredRatio");
            }
        }

        public string DesiredRatio
        {
            get
            {
                int gcd = Utilities.GCD(DesiredWidth, DesiredHeight);
                if (gcd == 0) return $"NaN";
                return $"{DesiredWidth / gcd} : {DesiredHeight / gcd}";
            }
        }

        #endregion

        public bool Validate() => !(Validation.GetHasError(DWTBox) || Validation.GetHasError(DHTBox));

        private void SelectedLayerUpdate(object sender, DataTransferEventArgs e)
        {
            if (SelectedLayer == null)
            {
                desiredWidth = 0;
                desiredHeight = 0;
                OnPropertyChanged("DesiredHeight");
                OnPropertyChanged("DesiredWidth");
                OnPropertyChanged("DesiredRatio");
            }
            else
            {
                DesiredWidth = SelectedLayer.OriginalBitmap.Width;
                DesiredHeight = SelectedLayer.OriginalBitmap.Height;
                OnPropertyChanged("SourceRatio");
            }
        }
    }
}
