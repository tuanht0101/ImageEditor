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

        private Bitmap _ogBmp;
        public Bitmap OgBmp
        {
            get => _ogBmp;
            set
            {
                _ogBmp = value;
                OnPropertyChanged();
            }
        }

        private Bitmap _processedBmp;
        public Bitmap ProcessedBmp
        {
            get => _processedBmp;
            set
            {
                _processedBmp = value;
                OnPropertyChanged();
            }
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                OgBmp = new Bitmap(openFileDialog.FileName);
            }
        }

        private void processButton_Click(object sender, RoutedEventArgs e)
        {
            MainClass mainClass = new MainClass(OgBmp);
            ProcessedBmp = mainClass.Process();
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                ProcessedBmp.Save(dialog.FileName, ImageFormat.Png);
            }
        }
    }
}
