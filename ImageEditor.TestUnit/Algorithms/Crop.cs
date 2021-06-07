using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageEditor.MainApp.Algorithms
{
    class Crop
    {
        public static unsafe Bitmap Process(Bitmap bitmap, int x1, int y1, int x2, int y2)
        {
            int xMin = Math.Min(x1, x2);
            int xMax = Math.Max(x1, x2);
            int yMin = Math.Min(y1, y2);
            int yMax = Math.Max(y1, y2);
            int srcWidth = bitmap.Width;
            int desWidth = xMax - xMin;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Bitmap resBmp = new Bitmap(xMax - xMin, yMax - yMin, PixelFormat.Format32bppArgb);
            BitmapData resData = resBmp.LockBits(new Rectangle(0, 0, resBmp.Width, resBmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            byte* scan0 = (byte*)data.Scan0;
            byte* resScan0 = (byte*)resData.Scan0;


            Parallel.For(yMin, yMax, (y) =>
            {
                for (int x = xMin; x < xMax; x++)
                {
                    int src = (x + y * srcWidth) * 4;
                    int des = (x - xMin + (y - yMin) * desWidth) * 4;
                    resScan0[des] = scan0[src];
                    resScan0[des + 1] = scan0[src + 1];
                    resScan0[des + 2] = scan0[src + 2];
                    resScan0[des + 3] = scan0[src + 3];
                }
            });

            bitmap.UnlockBits(data);
            resBmp.UnlockBits(resData);
            return resBmp;
        }
    }
}
