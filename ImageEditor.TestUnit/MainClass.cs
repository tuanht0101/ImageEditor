using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor.TestUnit
{
    public class MainClass
    {
        #region DO NOT CHANGE
        // Original Imported Bitmap
        private readonly Bitmap originalBitmap;
        public Bitmap OriginalBitmap { get => originalBitmap; }
        #endregion

        public MainClass(Bitmap originalBitmap)
        {
            #region DO NOT CHANGE
            this.originalBitmap = originalBitmap; 
            #endregion
        }

        /// <summary>
        /// Treat this method like main().
        /// </summary>
        /// <remarks>Used to render your work, it will show on UI.</remarks>
        /// <returns>Bitmap</returns>
        public Bitmap Process()
        {
            Bitmap bitmap = new Bitmap(200, 200);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color;
                    if (x % 2 == 1)
                    {
                        if (y % 2 == 1)
                        {
                            color = Color.FromArgb(255, 0, 0);
                        }
                        else
                        {
                            color = Color.FromArgb(0, 255, 0);
                        }
                    }
                    else
                    {
                        if (y % 2 == 1)
                        {
                            color = Color.FromArgb(0, 0, 255);
                        }
                        else
                        {
                            color = Color.FromArgb(255, 0, 255);
                        }
                    }

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }
    }
}
