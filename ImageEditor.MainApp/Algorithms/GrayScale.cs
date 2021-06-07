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
        public static unsafe Bitmap Process(Bitmap src, double redRatio, double greenRatio, double blueRatio)
        {
            int width = src.Width;
            int height = src.Height;

            Bitmap res = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            int pxs = src.Width * src.Height;
            for (int px = 0; px < pxs; px++)
            {
                int pxByte = px * 4;
                byte gray = (byte)Math.Round(srcScan0[pxByte] * blueRatio + srcScan0[pxByte + 1] * greenRatio + srcScan0[pxByte + 2] * redRatio);
                resScan0[pxByte + 0] = gray;
                resScan0[pxByte + 1] = gray;
                resScan0[pxByte + 2] = gray;
                resScan0[pxByte + 3] = srcScan0[pxByte + 3];
            }

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }
    }
}
