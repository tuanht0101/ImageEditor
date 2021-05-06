using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
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

namespace ImageEditor.TestUnit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private Bitmap _bmpOriginal = new Bitmap($"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\Assets\\404NotFound.jpg");
        public Bitmap BmpOriginal
        {
            get => _bmpOriginal;
            set
            {
                _bmpOriginal = value;
                OnPropertyChanged();
            }
        }
        private double BmpOriginalZoom = 1;

        private Bitmap _bmpProcessed = new Bitmap($"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\Assets\\404NotFound.jpg");
        public Bitmap BmpProcessed
        {
            get => _bmpProcessed;
            set
            {
                _bmpProcessed = value;
                OnPropertyChanged();
            }
        }
        private double BmpProcessedZoom = 1;

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                BmpOriginal = new Bitmap(openFileDialog.FileName);
            }
        }

        private void processButton_Click(object sender, RoutedEventArgs e)
        {
            MainClass mainClass = new MainClass(BmpOriginal);
            BmpProcessed = mainClass.Process();
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                BmpProcessed.Save(dialog.FileName, ImageFormat.Png);
            }
        }

        private void SVOg_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                BmpOriginalZoom += 0.2;
                ImageOg.Width = BmpOriginal.Width * BmpOriginalZoom;
                ImageOg.Height = BmpOriginal.Height * BmpOriginalZoom;
            }
            else
            {
                BmpOriginalZoom -= 0.2;
                if (BmpOriginalZoom < 0.2) BmpOriginalZoom = 0.2;
                ImageOg.Width = BmpOriginal.Width * BmpOriginalZoom;
                ImageOg.Height = BmpOriginal.Height * BmpOriginalZoom;
            }
            e.Handled = true;
        }

        private void SVP_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                BmpProcessedZoom += 0.2;
                ImageP.Width = BmpProcessed.Width * BmpProcessedZoom;
                ImageP.Height = BmpProcessed.Height * BmpProcessedZoom;
            }
            else
            {
                BmpProcessedZoom -= 0.2;
                if (BmpProcessedZoom < 0.2) BmpProcessedZoom = 0.2;
                ImageP.Width = BmpProcessed.Width * BmpProcessedZoom;
                ImageP.Height = BmpProcessed.Height * BmpProcessedZoom;
            }
            e.Handled = true;
        }
    }
}
