using ImageEditor.MainApp.Models;
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
    class Blur
    {
        public const int GAUSSIAN_RADIUS = 3;
        public const double GAUSSIAN_SIGMA = 0.84089642;

        public static unsafe Bitmap BoxBlur(Bitmap src, int radius, EdgeHandling edgeHandling, Color? edgeColor = null)
        {
            int width = src.Width;
            int height = src.Height;

            Bitmap res = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;
            
            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            int kernelSize = 2 * radius + 1;
            double kernelValue = 1d / (kernelSize * kernelSize);
            double[,] kernel = new double[kernelSize, kernelSize];
            for (int y = 0; y < kernelSize; y++)
            {
                for (int x = 0; x < kernelSize; x++)
                {
                    kernel[y, x] = kernelValue;
                }
            }

            Convolution.Do(srcScan0, resScan0, width, height, kernel, edgeHandling, edgeColor);

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }

        public static unsafe Bitmap GaussianBlur(Bitmap src, int radius, double sigma, EdgeHandling edgeHandling, Color? edgeColor = null)
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
            double sigmaSqr2 = 2 * sigma * sigma;
            double math = 1 / (sigmaSqr2 * Math.PI);
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    kernel[y + radius, x + radius] = math * Math.Exp(-(x * x + y * y) / sigmaSqr2);
                }
            }

            Convolution.Do(srcScan0, resScan0, width, height, kernel, edgeHandling, edgeColor);

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }
    }
}
