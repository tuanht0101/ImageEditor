using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor.MainApp.Algorithms
{
    class Resample
    {
        public enum Type
        {
            Nearest, Bilinear
        }

        public static unsafe Bitmap Nearest(Bitmap src, int desiredWidth, int desiredHeight)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            int width = src.Width;
            int height = src.Height;

            Bitmap res = new Bitmap(desiredWidth, desiredHeight, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, desiredWidth, desiredHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;


            double widthRatio = (width - 1) / (double)(desiredWidth - 1);
            double heightRatio = (height - 1) / (double)(desiredHeight - 1);

            Parallel.For(0, desiredHeight, parallelOptions, (y) =>
            {
                for (int x = 0; x < desiredWidth; x++)
                {
                    int pxSrc = (int)(Math.Round(x * widthRatio) + Math.Round(y * heightRatio) * width) * 4;
                    int pxRes = (x + y * desiredWidth) * 4;
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


        public static unsafe Bitmap Bilinear(Bitmap src, int desiredWidth, int desiredHeight)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            int width = src.Width;
            int height = src.Height;

            Bitmap res = new Bitmap(desiredWidth, desiredHeight, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, desiredWidth, desiredHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;


            double widthRatio = (width - 1) / (double)(desiredWidth - 1);
            double heightRatio = (height - 1) / (double)(desiredHeight - 1);

            Parallel.For(0, desiredHeight, parallelOptions, (y) =>
            {
                for (int x = 0; x < desiredWidth; x++)
                {
                    double xProportion = x * widthRatio;
                    double yProportion = y * heightRatio;
                    int x1 = (int)Math.Floor(xProportion);
                    int y1 = (int)Math.Floor(yProportion);
                    int x2 = (int)Math.Ceiling(xProportion);
                    int y2 = (int)Math.Ceiling(yProportion);
                    double x1x = xProportion % 1;
                    double xx2 = 1 - x1x;
                    double y1y = yProportion % 1;
                    double yy2 = 1 - y1y;
                    Func<int, byte> cal = new Func<int, byte>(
                        (i) =>
                        {
                            int val = (int)Math.Round(srcScan0[(x1 + y1 * width) * 4 + i] * x1x * y1y +
                                                      srcScan0[(x2 + y1 * width) * 4 + i] * xx2 * y1y +
                                                      srcScan0[(x1 + y2 * width) * 4 + i] * x1x * yy2 +
                                                      srcScan0[(x2 + y2 * width) * 4 + i] * xx2 * yy2);
                            val = val > 255 ? 255 : val;
                            return (byte)val;
                        });
                    int pxRes = (x + y * desiredWidth) * 4;
                    resScan0[pxRes + 0] = cal.Invoke(0);
                    resScan0[pxRes + 1] = cal.Invoke(1);
                    resScan0[pxRes + 2] = cal.Invoke(2);
                    resScan0[pxRes + 3] = cal.Invoke(3);
                }
            });

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }
    }
}
