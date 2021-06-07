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
        public static unsafe Bitmap Process(Bitmap src, int x1, int y1, int x2, int y2)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            int xMin = Math.Min(x1, x2);
            int xMax = Math.Max(x1, x2);
            int yMin = Math.Min(y1, y2);
            int yMax = Math.Max(y1, y2);
            int srcWidth = src.Width;
            int srcHeight = src.Height;
            int dstWidth = xMax - xMin + 1;
            int dstHeight = yMax - yMin + 1;


            Bitmap res = new Bitmap(dstWidth, dstHeight, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, dstWidth, dstHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;
            
            BitmapData srcData = src.LockBits(new Rectangle(0, 0, srcWidth, srcHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;


            Parallel.For(yMin, yMax + 1, parallelOptions, (y) =>
            {
                for (int x = xMin; x < xMax + 1; x++)
                {
                    int pxSrc = (x + y * srcWidth) * 4;
                    int pxDst = (x - xMin + (y - yMin) * dstWidth) * 4;
                    resScan0[pxDst + 0] = srcScan0[pxSrc + 0];
                    resScan0[pxDst + 1] = srcScan0[pxSrc + 1];
                    resScan0[pxDst + 2] = srcScan0[pxSrc + 2];
                    resScan0[pxDst + 3] = srcScan0[pxSrc + 3];
                }
            });

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }
    }
}
