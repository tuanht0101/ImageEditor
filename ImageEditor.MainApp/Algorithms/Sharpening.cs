using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static ImageEditor.MainApp.Algorithms.Convolution;

namespace ImageEditor.MainApp.Algorithms
{
    class Sharpening
    {
        public static unsafe Bitmap UnsharpMasking(Bitmap src, int amount, EdgeHandling edgeHandling, Color? edgeColor = null)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            int width = src.Width;
            int height = src.Height;

            Bitmap res = Blur.GaussianBlur(src, Blur.GAUSSIAN_RADIUS, Blur.GAUSSIAN_SIGMA, edgeHandling, edgeColor);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            int bytes = width * height * 4;
            Parallel.For(0, bytes, parallelOptions, (byteValue) =>
            {
                if ((byteValue + 1) % 4 != 0)
                {
                    int value = srcScan0[byteValue] + (srcScan0[byteValue] - resScan0[byteValue]) * amount;
                    value = value < 0 ? 0 : value > 255 ? 255 : value;
                    resScan0[byteValue] = (byte)value;
                }
                else
                {
                    resScan0[byteValue] = srcScan0[byteValue];
                }
            });

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }


        public static unsafe Bitmap Laplacian(Bitmap src, int type, EdgeHandling edgeHandling, Color? edgeColor = null)
        {
            int width = src.Width;
            int height = src.Height;

            Bitmap res = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            double[,] kernel = type == 0 ? new double[3, 3] { { 0, -1,  0},
                                                              {-1,  5, -1},
                                                              { 0, -1,  0} }
                                         : new double[3, 3] { {-1, -1, -1},
                                                              {-1,  9, -1},
                                                              {-1, -1, -1} };

            Convolution.Do(srcScan0, resScan0, width, height, kernel, edgeHandling, edgeColor);

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }


        public static unsafe Bitmap LaplacianOfGaussian(Bitmap src, int radius, double sigma, EdgeHandling edgeHandling, Color? edgeColor = null)
        {
            int width = src.Width;
            int height = src.Height;

            Bitmap res = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            int kernelSize = 2 * radius + 1;
            double[,] kernel = new double[kernelSize, kernelSize];

            double sigmaSqr = sigma * sigma;
            double sigmaSS = sigmaSqr * sigmaSqr;
            double math1 = 2 / sigmaSqr;
            double math2 = 2 * sigmaSqr;
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    double xSyS = x * x + y * y;
                    kernel[y + radius, x + radius] = -(xSyS / sigmaSS - math1) * Math.Exp(-xSyS / math2);
                }
            }
            kernel[radius, radius] += 1;

            Convolution.Do(srcScan0, resScan0, width, height, kernel, edgeHandling, edgeColor);

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }
    }
}
