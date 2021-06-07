using ImageEditor.MainApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageEditor.MainApp.ViewModels
{
    class LayersVM : BaseVM
    {
        public LayersVM()
        {
            DuplicateLayerCommand = new RelayCommand<object>(
               (p) => SelectedImageLayer != null,
               (p) =>
               {
                   ImageLayers.Add(new ImageLayer()
                   {
                       Name = $"Copy of {SelectedImageLayer.Name}",
                       OriginalBitmap = new Bitmap(SelectedImageLayer.OriginalBitmap),
                       RenderedBitmap = new Bitmap(SelectedImageLayer.OriginalBitmap),
                       XOffset = SelectedImageLayer.XOffset,
                       YOffset = SelectedImageLayer.YOffset,
                       IsVisible = SelectedImageLayer.IsVisible
                   });
               }
            );

            UnselectLayerCommand = new RelayCommand<object>(
               (p) => SelectedImageLayer != null,
               (p) =>
               {
                   SelectedImageLayer = null;
                   SelectedLayerIndex = -1;
               }
            );

            AddLayerCommand = new RelayCommand<object>(
               (p) => true,
               (p) =>
               {
                   if (Utilities.OpenImageFileDialog() is string fileName)
                   {
                       List<int> ints = new List<int>();
                       foreach (ImageLayer layer in ImageLayers)
                       {
                           if (Regex.IsMatch(layer.Name, @"^Layer \d*$"))
                               ints.Add(int.Parse(layer.Name.Split(new string[] { "Layer " }, StringSplitOptions.RemoveEmptyEntries)[0]));
                       }

                       int iter = 0;
                       while (ints.Contains(iter)) iter++;
                       ImageLayers.Add(new ImageLayer($"Layer {iter}", fileName));
                   }
               }
            );

            DeleteLayerCommand = new RelayCommand<object>(
                (p) => SelectedImageLayer != null,
                (p) =>
                {
                    SelectedImageLayer.Dispose();
                    ImageLayers.Remove(SelectedImageLayer);
                    if (triggerGC++ > 2)
                    {
                        GC.Collect();
                        triggerGC = 0;
                    }
                }
            );

            LayerMoveUpCommand = new RelayCommand<object>(
                (p) => ImageLayers.IndexOf(SelectedImageLayer) > 0 && SelectedImageLayer != null,
                (p) =>
                {
                    int index = ImageLayers.IndexOf(SelectedImageLayer);
                    ImageLayers.Move(index, index - 1);
                }
            );

            LayerMoveDownCommand = new RelayCommand<object>(
                (p) => ImageLayers.IndexOf(SelectedImageLayer) < ImageLayers.Count - 1 && SelectedImageLayer != null,
                (p) =>
                {
                    int index = ImageLayers.IndexOf(SelectedImageLayer);
                    ImageLayers.Move(index, index + 1);
                }
            );
        }

        public ObservableCollection<ImageLayer> ImageLayers { get; } = new ObservableCollection<ImageLayer>();

        private ImageLayer selectedImageLayer;
        public ImageLayer SelectedImageLayer
        {
            get => selectedImageLayer;
            set
            {
                selectedImageLayer = value;
                OnPropertyChanged();
            }
        }

        private int selectedLayerIndex = -1;
        public int SelectedLayerIndex
        {
            get => selectedLayerIndex;
            set
            {
                selectedLayerIndex = value;
                OnPropertyChanged();
            }
        }

        private bool isHighlightRendered;
        public bool IsHighlightRendered
        {
            get => isHighlightRendered;
            set
            {
                isHighlightRendered = value;
                OnPropertyChanged();
            }
        }


        int triggerGC = 0;
        public ICommand DuplicateLayerCommand { get; }
        public ICommand UnselectLayerCommand { get; }
        public ICommand AddLayerCommand { get; }
        public ICommand DeleteLayerCommand { get; }
        public ICommand LayerMoveUpCommand { get; }
        public ICommand LayerMoveDownCommand { get; }
    }
}
