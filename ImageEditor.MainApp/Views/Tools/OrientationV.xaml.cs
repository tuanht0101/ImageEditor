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

namespace ImageEditor.MainApp.Views.Tools
{
    /// <summary>
    /// Interaction logic for OrientationV.xaml
    /// </summary>
    public partial class OrientationV : UserControl
    {
        public OrientationV()
        {
            RotateLeftCommand = new ApplyCommand(
                () => true,
                (b) => Algorithms.Orientation.Rotate(b, true),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            RotateRightCommand = new ApplyCommand(
                () => true,
                (b) => Algorithms.Orientation.Rotate(b),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            FlipHorizontalCommand = new ApplyCommand(
                () => true,
                (b) => Algorithms.Orientation.Flip(b),
                () => IsProcessingEnabled = true,
                () => IsProcessingEnabled = false
                );

            FlipVerticalCommand = new ApplyCommand(
                () => true,
                (b) => Algorithms.Orientation.Flip(b, true),
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
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(OrientationV));

        public bool IsProcessingEnabled
        {
            get { return (bool)GetValue(IsProcessingEnabledProperty); }
            set { SetValue(IsProcessingEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsProcessingEnabledProperty =
            DependencyProperty.Register("IsProcessingEnabled", typeof(bool), typeof(OrientationV));
        #endregion

        public ICommand RotateLeftCommand { get; }
        public ICommand RotateRightCommand { get; }
        public ICommand FlipHorizontalCommand { get; }
        public ICommand FlipVerticalCommand { get; }
    }
}
