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
    class GrayScale
    {
        public static unsafe void Process(Bitmap bitmap, double redRatio, double greenRatio, double blueRatio)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* scan0 = (byte*)bmpData.Scan0;
            int pixels = bitmap.Height * bitmap.Width;

            for (int pixel = 0; pixel < pixels; pixel++)
            {
                byte gray = (byte)Math.Round(scan0[pixel * 4 + 0] * blueRatio + scan0[pixel * 4 + 1] * greenRatio + scan0[pixel * 4 + 2] * redRatio);
                scan0[pixel * 4 + 0] = gray;
                scan0[pixel * 4 + 1] = gray;
                scan0[pixel * 4 + 2] = gray;
            }

            bitmap.UnlockBits(bmpData);
        }
    }
}
