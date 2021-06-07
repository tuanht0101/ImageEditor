using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor.MainApp.Algorithms
{
    class Orientation
    {
        public static unsafe Bitmap Rotate(Bitmap src, bool rotateLeft = false)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            int width = src.Width;
            int height = src.Height;

            Bitmap res = new Bitmap(height, width, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, height, width), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            Parallel.For(0, height, parallelOptions, (y) =>
            {
                for(int x = 0; x < width; x++)
                {
                    int pxSrc = (x + y * width) * 4;
                    int pxRes = rotateLeft ? (y + (width - 1 - x) * height) * 4 : (height - 1 - y + x * height) * 4;
                    resScan0[pxRes + 0] = srcScan0[pxSrc + 0];
                    resScan0[pxRes + 1] = srcScan0[pxSrc + 1];
                    resScan0[pxRes + 2] = srcScan0[pxSrc + 2];
                    resScan0[pxRes + 3] = srcScan0[pxSrc + 3];
                }
            });

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }

        public static unsafe Bitmap Flip(Bitmap src, bool flipVertical = false)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            int width = src.Width;
            int height = src.Height;

            Bitmap res = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            Parallel.For(0, height, parallelOptions, (y) =>
            {
                for (int x = 0; x < width; x++)
                {
                    int pxSrc = (x + y * width) * 4;
                    int pxRes = flipVertical ? (x + (height - 1 - y) * width) * 4 : (width - 1 - x + y * width) * 4;
                    resScan0[pxRes + 0] = srcScan0[pxSrc + 0];
                    resScan0[pxRes + 1] = srcScan0[pxSrc + 1];
                    resScan0[pxRes + 2] = srcScan0[pxSrc + 2];
                    resScan0[pxRes + 3] = srcScan0[pxSrc + 3];
                }
            });

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }
    }
}
