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
    /// Interaction logic for CropV.xaml
    /// </summary>
    public partial class CropV : UserControl, INotifyPropertyChanged
    {
        #region IMPLEMENTATIONS
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion

        public CropV()
        {
            RevertCommand = new RevertCommand(
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            PreviewCommand = new PreviewCommand(
                () => Validate(),
                (b) => Algorithms.Crop.Process(b, X1, Y1, X2, Y2),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            ApplyCommand = new ApplyCommand(
                () => Validate(),
                (b) => Algorithms.Crop.Process(b, X1, Y1, X2, Y2),
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
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(CropV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(CropV)); 
        #endregion


        public ICommand RevertCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand ApplyCommand { get; }


        #region PROPERTIES
        private int x1;
        public int X1
        {
            get => x1;
            set
            {
                x1 = value >= SelectedLayer.OriginalBitmap.Width ? SelectedLayer.OriginalBitmap.Width - 1 : value;
                OnPropertyChanged();
                OnPropertyChanged("WidthRatio");
                OnPropertyChanged("HeightRatio");
            }
        }

        private int x2;
        public int X2
        {
            get => x2;
            set
            {
                x2 = value >= SelectedLayer.OriginalBitmap.Width ? SelectedLayer.OriginalBitmap.Width - 1 : value;
                OnPropertyChanged();
                OnPropertyChanged("WidthRatio");
                OnPropertyChanged("HeightRatio");
            }
        }

        private int y1;
        public int Y1
        {
            get => y1;
            set
            {
                y1 = value >= SelectedLayer.OriginalBitmap.Height ? SelectedLayer.OriginalBitmap.Height - 1 : value;
                OnPropertyChanged();
                OnPropertyChanged("WidthRatio");
                OnPropertyChanged("HeightRatio");
            }
        }

        private int y2;
        public int Y2
        {
            get => y2;
            set
            {
                y2 = value >= SelectedLayer.OriginalBitmap.Height ? SelectedLayer.OriginalBitmap.Height - 1 : value;
                OnPropertyChanged();
                OnPropertyChanged("WidthRatio");
                OnPropertyChanged("HeightRatio");
            }
        }

        public int WidthRatio
        {
            get
            {
                int w = Math.Abs(X2 - X1) + 1;
                int h = Math.Abs(Y2 - Y1) + 1;
                int gcd = GCD(w, h);
                if (gcd == 0) return 0;
                return w / gcd;
            }
        }

        public int HeightRatio
        {
            get
            {
                int w = Math.Abs(X2 - X1) + 1;
                int h = Math.Abs(Y2 - Y1) + 1;
                int gcd = GCD(w, h);
                if (gcd == 0) return 0;
                return h / gcd;
            }
        } 
        #endregion


        private int GCD(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }

        private void SelectedLayerUpdate(object sender, DataTransferEventArgs e)
        {
            if (SelectedLayer == null) return;
            X1 = Y1 = 0;
            X2 = SelectedLayer.OriginalBitmap.Width - 1;
            Y2 = SelectedLayer.OriginalBitmap.Height - 1;
        }

        public bool Validate()
        {
            return !(
                Validation.GetHasError(X1TBox) ||
                Validation.GetHasError(Y1TBox) ||
                Validation.GetHasError(X2TBox) ||
                Validation.GetHasError(Y2TBox)
                );
        }
    }
}
