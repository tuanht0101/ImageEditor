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
    class EdgeDetection
    {
        public static unsafe Bitmap MinusBlur(Bitmap src, int amount, EdgeHandling edgeHandling, Color? edgeColor = null)
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
                    int value = (srcScan0[byteValue] - resScan0[byteValue]) * amount;
                    value = value < 0 ? 0 : value > 255 ? 255 : value;
                    resScan0[byteValue] = (byte)value;
                } else
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
                                                              {-1,  4, -1},
                                                              { 0, -1,  0} }
                                         : new double[3, 3] { {-1, -1, -1},
                                                              {-1,  8, -1},
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

            Convolution.Do(srcScan0, resScan0, width, height, kernel, edgeHandling, edgeColor);

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }


        public static unsafe Bitmap Sobel(Bitmap src, int type, int thresholdPercent, EdgeHandling edgeHandling, Color? edgeColor = null)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            int width = src.Width;
            int height = src.Height;
            int bytes = width * height * 4;

            Bitmap res = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            byte[] resXBytes = new byte[bytes];
            byte[] resYBytes = new byte[bytes];

            double[,] kernelX, kernelY;

            // 0: Sobel, 1: Sobel–Feldman, 2: Scharr
            switch (type)
            {
                default:
                    kernelX = new double[3, 3] { { 1,  0, -1},
                                                 { 2,  0, -2},
                                                 { 1,  0, -1} };
                    kernelY = new double[3, 3] { { 1,  2,  1},
                                                 { 0,  0,  0},
                                                 {-1, -2, -1} };
                    break;
                case 1:
                    kernelX = new double[3, 3] { { 3,  0, -3},
                                                 {10,  0, -10},
                                                 { 3,  0, -3} };
                    kernelY = new double[3, 3] { { 3,  10,  3},
                                                 { 0,   0,  0},
                                                 {-3, -10, -3} };
                    break;
                case 2:
                    kernelX = new double[3, 3] { { 47,  0, -47},
                                                 {162,  0, -162},
                                                 { 47,  0, -47} };
                    kernelY = new double[3, 3] { { 47,  162,  47},
                                                 {  0,    0,   0},
                                                 {-47, -162, -47} };
                    break;
            }

            fixed(byte* ptr = &resXBytes[0])
            {
                Convolution.Do(srcScan0, ptr, width, height, kernelX, edgeHandling, edgeColor);
            }
            fixed (byte* ptr = &resYBytes[0])
            {
                Convolution.Do(srcScan0, ptr, width, height, kernelX, edgeHandling, edgeColor);
            }

            double thresholdLimit = thresholdPercent / 100d * 255;
            Parallel.For(0, height, parallelOptions, (y) =>
            {
                for (int x = 0; x < width; x++)
                {
                    int px = (x + y * width) * 4;
                    double valueB = Math.Sqrt(Math.Pow(resXBytes[px + 0], 2) + Math.Pow(resYBytes[px + 0], 2));
                    double valueG = Math.Sqrt(Math.Pow(resXBytes[px + 1], 2) + Math.Pow(resYBytes[px + 1], 2));
                    double valueR = Math.Sqrt(Math.Pow(resXBytes[px + 2], 2) + Math.Pow(resYBytes[px + 2], 2));
                    if (valueB <= thresholdLimit) valueB = 0;
                    if (valueG <= thresholdLimit) valueG = 0;
                    if (valueR <= thresholdLimit) valueR = 0;
                    resScan0[px + 0] = (byte)Math.Round(valueB);
                    resScan0[px + 1] = (byte)Math.Round(valueG);
                    resScan0[px + 2] = (byte)Math.Round(valueR);
                    resScan0[px + 3] = srcScan0[px + 3];
                }
            });


            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }
    }
}
