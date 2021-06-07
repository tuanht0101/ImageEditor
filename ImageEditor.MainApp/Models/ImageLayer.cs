using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor.MainApp.Models
{
    public class ImageLayer : INotifyPropertyChanged, IDisposable
    {
        #region IMPLEMENTATIONS
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region CONSTRUCTORS
        public ImageLayer()
        {

        }

        public ImageLayer(string name, string fileName)
        {
            Name = name;
            using (Bitmap original = new Bitmap(fileName),
                clone = original.Clone(new Rectangle(0, 0, original.Width, original.Height), PixelFormat.Format32bppArgb))
            {
                OriginalBitmap = new Bitmap(clone);
                RenderedBitmap = new Bitmap(clone);
            }
        }
        #endregion


        #region PROPERTIES
        public string Name { get; set; }
        private Bitmap originalBitmap;
        public Bitmap OriginalBitmap
        {
            get => originalBitmap;
            set
            {
                if (originalBitmap != null)
                    originalBitmap.Dispose();
                originalBitmap = value;
                OnPropertyChanged();
            }
        }
        private Bitmap renderedBitmap;
        public Bitmap RenderedBitmap
        {
            get => renderedBitmap;
            set
            {
                if (renderedBitmap != null)
                    renderedBitmap.Dispose();
                renderedBitmap = value;
                OnPropertyChanged();
            }
        }

        private int xOffset = 0;
        public int XOffset
        {
            get => xOffset;
            set
            {
                xOffset = value;
                OnPropertyChanged();
            }
        }

        private int yOffset = 0;
        public int YOffset
        {
            get => yOffset;
            set
            {
                yOffset = value;
                OnPropertyChanged();
            }
        }

        private bool isVisible = true;
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                OnPropertyChanged();
            }
        } 
        #endregion

        public void Dispose()
        {
            OriginalBitmap = null;
            RenderedBitmap = null;
        }
    }
}
