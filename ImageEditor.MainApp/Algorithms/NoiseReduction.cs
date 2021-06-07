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
    class NoiseReduction
    {
        public static unsafe Bitmap Average(Bitmap src, int radius, int threshold, EdgeHandling edgeHandling, Color? edgeColor = null)
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
                Parallel.For(0, width, parallelOptions, (x) =>
                {
                    double sumB = 0;
                    double sumG = 0;
                    double sumR = 0;
                    int kernelCount = 0;

                    for (int yOffset = -radius; yOffset <= radius; yOffset++)
                    {
                        if (y + yOffset < 0 || y + yOffset >= height) continue;
                        for (int xOffset = -radius; xOffset <= radius; xOffset++)
                        {
                            if (x + xOffset < 0 || x + xOffset >= width) continue;
                            int pxSrc = (x + xOffset + (y + yOffset) * width) * 4;

                            sumB += srcScan0[pxSrc + 0];
                            sumG += srcScan0[pxSrc + 1];
                            sumR += srcScan0[pxSrc + 2];
                            kernelCount++;
                        }
                    }

                    byte avgB = (byte)Math.Round(sumB / kernelCount);
                    byte avgG = (byte)Math.Round(sumG / kernelCount);
                    byte avgR = (byte)Math.Round(sumR / kernelCount);

                    int px = (x + y * width) * 4;
                    resScan0[px + 0] = srcScan0[px + 0] - avgB > threshold ? avgB : srcScan0[px + 0];
                    resScan0[px + 1] = srcScan0[px + 1] - avgG > threshold ? avgG : srcScan0[px + 1];
                    resScan0[px + 2] = srcScan0[px + 2] - avgR > threshold ? avgR : srcScan0[px + 2];
                    resScan0[px + 3] = srcScan0[px + 3];
                });
            });


            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }


        public static unsafe Bitmap Median(Bitmap src, int radius, EdgeHandling edgeHandling, Color? edgeColor = null)
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
                    List<byte> bs = new List<byte>();
                    List<byte> gs = new List<byte>();
                    List<byte> rs = new List<byte>();

                    for (int yOffset = -radius; yOffset <= radius; yOffset++)
                    {
                        if (y + yOffset < 0 || y + yOffset >= height) continue;
                        for (int xOffset = -radius; xOffset <= radius; xOffset++)
                        {
                            if (x + xOffset < 0 || x + xOffset >= width) continue;
                            int pxSrc = (x + xOffset + (y + yOffset) * width) * 4;

                            bs.Add(srcScan0[pxSrc + 0]);
                            gs.Add(srcScan0[pxSrc + 1]);
                            rs.Add(srcScan0[pxSrc + 2]);
                        }
                    }

                    bs.Sort();
                    gs.Sort();
                    rs.Sort();

                    int px = (x + y * width) * 4;
                    resScan0[px + 0] = bs[bs.Count / 2];
                    resScan0[px + 1] = gs[gs.Count / 2];
                    resScan0[px + 2] = rs[rs.Count / 2];
                    resScan0[px + 3] = srcScan0[px + 3];
                }
            });


            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }
    }
}
