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
    class Blur
    {
        public static unsafe Bitmap BoxBlur (Bitmap bitmap, int radius)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int kernelSize = 2 * radius + 1;
            int kernelCount = kernelSize * kernelSize;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Bitmap resBmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = resBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            byte* scan0 = (byte*)data.Scan0;
            byte* resScan0 = (byte*)resData.Scan0;


            Parallel.For(0, height, (y) =>
            {
                for (int x = 0; x < width; x++)
                {
                    int sumB = 0;
                    int sumG = 0;
                    int sumR = 0;

                    for (int yOffset = -radius; yOffset <= radius; yOffset++)
                    {
                        if (y + yOffset < 0 || y + yOffset >= height) continue;
                        for (int xOffset = -radius; xOffset <= radius; xOffset++)
                        {
                            if (x + xOffset < 0 || x + xOffset >= width) continue;
                            int coor = (x + xOffset + (y + yOffset) * width) * 4;
                            sumB += scan0[coor];
                            sumG += scan0[coor + 1];
                            sumR += scan0[coor + 2];
                        }
                    }

                    int des = (x + y * width) * 4;
                    resScan0[des] = (byte)Math.Round((double)(sumB / kernelCount));
                    resScan0[des + 1] = (byte)Math.Round((double)(sumG / kernelCount));
                    resScan0[des + 2] = (byte)Math.Round((double)(sumR / kernelCount));
                    resScan0[des + 3] = scan0[des + 3];
                }
            });

            bitmap.UnlockBits(data);
            resBmp.UnlockBits(resData);
            return resBmp;
        }


        public static unsafe Bitmap GaussianBlur(Bitmap bitmap, int radius, double sigma)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int kernelSize = 2 * radius + 1;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Bitmap resBmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = resBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            byte* scan0 = (byte*)data.Scan0;
            byte* resScan0 = (byte*)resData.Scan0;

            double[,] kernel = new double[kernelSize, kernelSize];

            double sigmaSqr = 2 * sigma * sigma;
            double math = 1 / (sigmaSqr * Math.PI);
            double kernelSum = 0;

            Parallel.For(0, kernelSize, (y) =>
            {
                Parallel.For(0, kernelSize, (x) =>
                {
                    kernelSum += kernel[y, x] = math * Math.Exp( -(Math.Pow(x - radius, 2) + Math.Pow(y - radius, 2)) / sigmaSqr);
                });
            });

            Parallel.For(0, height, (y) =>
            {
                Parallel.For(0, width, (x) =>
                {
                    double sumB = 0;
                    double sumG = 0;
                    double sumR = 0;

                    for (int yOffset = -radius; yOffset <= radius; yOffset++)
                    {
                        if (y + yOffset < 0 || y + yOffset >= height) continue;
                        for (int xOffset = -radius; xOffset <= radius; xOffset++)
                        {
                            if (x + xOffset < 0 || x + xOffset >= width) continue;
                            int coor = (x + xOffset + (y + yOffset) * width) * 4;
                            int xKernel = xOffset + radius;
                            int yKernel = yOffset + radius;

                            sumB += scan0[coor + 0] * kernel[yKernel, xKernel];
                            sumG += scan0[coor + 1] * kernel[yKernel, xKernel];
                            sumR += scan0[coor + 2] * kernel[yKernel, xKernel];
                        }
                    }

                    double calcB = sumB / kernelSum;
                    double calcG = sumG / kernelSum;
                    double calcR = sumR / kernelSum;

                    if (calcB > 255) calcB = 255;
                    if (calcB < 0) calcB = 0;
                    if (calcG > 255) calcG = 255;
                    if (calcG < 0) calcG = 0;
                    if (calcR > 255) calcR = 255;
                    if (calcR < 0) calcR = 0;

                    int des = (x + y * width) * 4;
                    resScan0[des + 0] = (byte)Math.Round(calcB);
                    resScan0[des + 1] = (byte)Math.Round(calcG);
                    resScan0[des + 2] = (byte)Math.Round(calcR);
                    resScan0[des + 3] = scan0[des + 3];

                    
                });
            });

            bitmap.UnlockBits(data);
            resBmp.UnlockBits(resData);
            return resBmp;
        }
    }
}
