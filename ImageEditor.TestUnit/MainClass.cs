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
            // Create Bitmap with Size of 100 x 100
            // SetPixel() set x y of bitmap with Color.FromArgb(alpha, red, green, blue)
            // GetPixel() get Color from x y
            Bitmap bmpR = new Bitmap(100, 100);
            for (int x = 0; x < bmpR.Width; x++)
            {
                for (int y = 0; y < bmpR.Height; y++)
                {
                    bmpR.SetPixel(x, y, Color.FromArgb(127, 255, 0, 0));
                }
            }

            Bitmap bmpG = new Bitmap(100, 100);
            for (int x = 0; x < bmpG.Width; x++)
            {
                for (int y = 0; y < bmpG.Height; y++)
                {
                    bmpG.SetPixel(x, y, Color.FromArgb(127, 0, 255, 0));
                }
            }

            // Dont care about this
            using (var g = Graphics.FromImage(originalBitmap))
            {
                g.DrawImage(bmpR, 0, 0);
                g.DrawImage(bmpG, 0, 0);
            }

            return originalBitmap;
        }
    }
}
