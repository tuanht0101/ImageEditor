using ImageEditor.MainApp.Models;
using ImageEditor.MainApp.ViewModels;
using ImageEditor.MainApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ImageEditor.MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ResetZoom = new RelayCommand<object>(
                (i) => true,
                (i) => CompositeGridScale.ScaleX = 1
                );

            CenterLayer = new RelayCommand<ImageLayer>(
                (i) => i != null,
                (i) =>
                {
                    i.XOffset = (CanvasV.CanvasWidth - i.RenderedBitmap.Width) / 2;
                    i.YOffset = (CanvasV.CanvasHeight - i.RenderedBitmap.Height) / 2;
                });

            ResetLayerOffset = new RelayCommand<ImageLayer>(
                (i) => i != null,
                (i) =>
                {
                    i.XOffset = 0;
                    i.YOffset = 0;
                });

            InitializeComponent();

            Binding selectedLayerBinding = new Binding
            {
                ElementName = "LayersV",
                Path = new PropertyPath("SelectedLayer"),
                Mode = BindingMode.OneWay
            };
            SetBinding(SelectedLayerProperty, selectedLayerBinding);
        }

        public ICommand ResetZoom { get; }
        public ICommand CenterLayer { get; }
        public ICommand ResetLayerOffset { get; }

        private Point prevMouseMovePoint;
        private void Renderer_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = Mouse.GetPosition(this);
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (OrientationToolLBI.IsSelected)
                {
                    ImageLayer selectedLayer = LayersV.SelectedLayer;
                    if (selectedLayer != null)
                    {
                        double scale = CompositeGridScale.ScaleX;
                        double mouseXOffset = (currentPoint.X - prevMouseMovePoint.X) / scale;
                        double mouseYOffset = (currentPoint.Y - prevMouseMovePoint.Y) / scale;

                        if (Math.Abs(mouseXOffset) < 1)
                        {
                            currentPoint.X = prevMouseMovePoint.X;
                            mouseXOffset = 0;
                        }
                        if (Math.Abs(mouseYOffset) < 1)
                        {
                            currentPoint.Y = prevMouseMovePoint.Y;
                            mouseYOffset = 0;
                        }

                        selectedLayer.XOffset += (int)Math.Round(mouseXOffset);
                        selectedLayer.YOffset += (int)Math.Round(mouseYOffset);
                    }
                }
                
            }

            if (Mouse.MiddleButton == MouseButtonState.Pressed)
            {
                CompositeGridTrans.X += currentPoint.X - prevMouseMovePoint.X;
                CompositeGridTrans.Y += currentPoint.Y - prevMouseMovePoint.Y;
            }
            prevMouseMovePoint = currentPoint;
        }


        private void Renderer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                double scale = CompositeGridScale.ScaleX;
                if (Math.Round(scale, 1) != 0.10)
                {
                    if (scale < 0.20)
                        CompositeGridScale.ScaleX = 0.10;
                    else
                        CompositeGridScale.ScaleX -= 0.10;

                    CompositeGridTrans.X += CanvasV.CanvasWidth * 0.10 / 2;
                    CompositeGridTrans.Y += CanvasV.CanvasHeight * 0.10 / 2;
                }
            }
            else if (e.Delta > 0)
            {
                CompositeGridScale.ScaleX += 0.10;
                CompositeGridTrans.X -= CanvasV.CanvasWidth * 0.10 / 2;
                CompositeGridTrans.Y -= CanvasV.CanvasHeight * 0.10 / 2;
            }
            ReshapeHighlight();
            e.Handled = true;
        }



        private void ReshapeCanvasHighlight(object sender = null, EventArgs e = null)
        {
            double scale = CompositeGridScale.ScaleX;
            Point offset = CompositeGrid.TranslatePoint(new Point(-1, -1), RendererSV);
            CanvasHighlightBorderScale.ScaleX = scale;
            CanvasHighlightBorderScale.ScaleY = scale;
            CanvasHighlightBorderTrans.X = offset.X;
            CanvasHighlightBorderTrans.Y = offset.Y;
        }

        private void ReshapeSelectedHighlight(object sender = null, EventArgs e = null)
        {
            ImageLayer selectedLayer = LayersV.SelectedLayer;
            if (selectedLayer != null)
            {
                double scale = CompositeGridScale.ScaleX;
                Point offset = CompositeGrid.TranslatePoint(new Point(-1, -1), RendererSV);
                SelectedHighlightTrans.X = offset.X + selectedLayer.XOffset * scale;
                SelectedHighlightTrans.Y = offset.Y + selectedLayer.YOffset * scale;
                SelectedHighlightScale.ScaleX = scale;
                SelectedHighlightScale.ScaleY = scale;
            }
        }

        private void ReshapeHighlight(object sender = null, EventArgs e = null)
        {
            ReshapeCanvasHighlight();
            ReshapeSelectedHighlight();
        }


        private void Renderer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                CanvasHighlightBorder.Visibility = Visibility.Visible;
                ReshapeCanvasHighlight();
            }
        }

        private void Renderer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                CanvasHighlightBorder.Visibility = Visibility.Collapsed;
            }
        }

        private bool isCropMouseDown;
        private void Renderer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CropToolLBI.IsSelected && e.LeftButton == MouseButtonState.Pressed && LayersV.SelectedLayer != null)
            {
                Point current = Mouse.GetPosition(CompositeGrid);
                ImageLayer selected = LayersV.SelectedLayer;
                double cropX = current.X + selected.XOffset;
                double cropY = current.Y + selected.YOffset;
                if (cropX < 0) cropX = 0;
                if (cropY < 0) cropY = 0;
                CropVTools.X1 = (int)Math.Round(cropX);
                CropVTools.Y1 = (int)Math.Round(cropY);
                isCropMouseDown = true;
            }
        }

        private void Renderer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isCropMouseDown && CropToolLBI.IsSelected && e.LeftButton == MouseButtonState.Released)
            {
                Point current = Mouse.GetPosition(CompositeGrid);
                ImageLayer selected = LayersV.SelectedLayer;
                CropVTools.X2 = (int)Math.Round(current.X + selected.XOffset);
                CropVTools.Y2 = (int)Math.Round(current.Y + selected.YOffset);
                isCropMouseDown = false;
            }
        }

        private void ZoomLevelTBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void CenterCanvas(object sender = null, EventArgs e = null)
        {
            CompositeGridTrans.X = (EditorBorder.ActualWidth - CanvasV.CanvasWidth * CompositeGridScale.ScaleX) / 2;
            CompositeGridTrans.Y = (EditorBorder.ActualHeight - CanvasV.CanvasHeight * CompositeGridScale.ScaleX) / 2;
        }

        private void DisableMouseWheelScroll(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void NewCanvasDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            CenterCanvas();
            ReshapeHighlight();
        }


        public ImageLayer SelectedLayer
        {
            get { return (ImageLayer)GetValue(SelectedLayerProperty); }
            set { SetValue(SelectedLayerProperty, value); }
        }
        public static readonly DependencyProperty SelectedLayerProperty =
            DependencyProperty.Register("SelectedLayer", typeof(ImageLayer), typeof(MainWindow));
    }
}
