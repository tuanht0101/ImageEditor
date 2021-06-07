using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageEditor.MainApp.Algorithms
{
    public class Convolution
    {
        public enum EdgeHandling
        {
            Extend, Wrap, Mirror, Crop, Constant
        }

        public static Func<int, int> EdgeHandler(int length, EdgeHandling edgeHandling)
        {
            switch (edgeHandling)
            {
                case EdgeHandling.Extend:
                    return new Func<int, int>(
                        (offset) => offset < 0 ? 0 : offset >= length ? length - 1 : offset
                        );

                case EdgeHandling.Wrap:
                    return new Func<int, int>(
                        (offset) => offset < 0 ? length - 1 : offset >= length ? 0 : offset
                        );

                case EdgeHandling.Mirror:
                    return new Func<int, int>(
                        (offset) => offset < 0 ? -1 - offset : offset >= length ? length - (offset - (length - 1)) : offset
                        );
                default:
                    return new Func<int, int>((offset) => offset);
            }
        }

        public static unsafe void Do(byte* src, byte* dst, int width, int height, double[,] kernel, EdgeHandling edgeHandling, Color? edgeColor = null)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            if (edgeColor == null) edgeColor = Color.FromArgb(255, 0, 0, 0);

            int radius = (kernel.GetLength(0) - 1) / 2;

            if (edgeHandling != EdgeHandling.Crop && edgeHandling != EdgeHandling.Constant)
            {
                Func<int, int> xNormalize = EdgeHandler(width, edgeHandling);
                Func<int, int> yNormalize = EdgeHandler(height, edgeHandling);

                Parallel.For(0, height, parallelOptions, (y) =>
                {
                    Parallel.For(0, width, parallelOptions, (x) =>
                    {
                        double sumB = 0;
                        double sumG = 0;
                        double sumR = 0;

                        for (int yOffset = -radius; yOffset <= radius; yOffset++)
                        {
                            for (int xOffset = -radius; xOffset <= radius; xOffset++)
                            {
                                int xSrc = xNormalize.Invoke(x + xOffset);
                                int ySrc = yNormalize.Invoke(y + yOffset);
                                int pxSrc = (xSrc + ySrc * width) * 4;
                                int xKernel = xOffset + radius;
                                int yKernel = yOffset + radius;

                                sumB += src[pxSrc + 0] * kernel[yKernel, xKernel];
                                sumG += src[pxSrc + 1] * kernel[yKernel, xKernel];
                                sumR += src[pxSrc + 2] * kernel[yKernel, xKernel];
                            }
                        }

                        sumB = sumB < 0 ? 0 : sumB > 255 ? 255 : sumB;
                        sumG = sumG < 0 ? 0 : sumG > 255 ? 255 : sumG;
                        sumR = sumR < 0 ? 0 : sumR > 255 ? 255 : sumR;

                        int px = (x + y * width) * 4;

                        dst[px + 0] = (byte)Math.Round(sumB);
                        dst[px + 1] = (byte)Math.Round(sumG);
                        dst[px + 2] = (byte)Math.Round(sumR);
                        dst[px + 3] = src[px + 3];
                    });
                });
            }
            else if (edgeHandling == EdgeHandling.Crop)
            {
                Parallel.For(0, height, parallelOptions, (y) =>
                {
                    Parallel.For(0, width, parallelOptions, (x) =>
                    {
                        int px = (x + y * width) * 4;

                        if (y < radius || y >= height - radius || x < radius || x >= width - radius)
                        {
                            dst[px + 0] = edgeColor.Value.B;
                            dst[px + 1] = edgeColor.Value.G;
                            dst[px + 2] = edgeColor.Value.R;
                            dst[px + 3] = edgeColor.Value.A;
                        }
                        else
                        {
                            double sumB = 0;
                            double sumG = 0;
                            double sumR = 0;

                            for (int yOffset = -radius; yOffset <= radius; yOffset++)
                            {
                                for (int xOffset = -radius; xOffset <= radius; xOffset++)
                                {
                                    int pxSrc = (x + xOffset + (y + yOffset) * width) * 4;
                                    int xKernel = xOffset + radius;
                                    int yKernel = yOffset + radius;

                                    sumB += src[pxSrc + 0] * kernel[yKernel, xKernel];
                                    sumG += src[pxSrc + 1] * kernel[yKernel, xKernel];
                                    sumR += src[pxSrc + 2] * kernel[yKernel, xKernel];
                                }
                            }

                            sumB = sumB < 0 ? 0 : sumB > 255 ? 255 : sumB;
                            sumG = sumG < 0 ? 0 : sumG > 255 ? 255 : sumG;
                            sumR = sumR < 0 ? 0 : sumR > 255 ? 255 : sumR;

                            dst[px + 0] = (byte)Math.Round(sumB);
                            dst[px + 1] = (byte)Math.Round(sumG);
                            dst[px + 2] = (byte)Math.Round(sumR);
                            dst[px + 3] = src[px + 3];
                        }
                    });
                });
            }
            else
            {
                Parallel.For(0, height, parallelOptions, (y) =>
                {
                    Parallel.For(0, width, parallelOptions, (x) =>
                    {
                        double sumB = 0;
                        double sumG = 0;
                        double sumR = 0;

                        for (int yOffset = -radius; yOffset <= radius; yOffset++)
                        {
                            for (int xOffset = -radius; xOffset <= radius; xOffset++)
                            {
                                int xSrc = x + xOffset;
                                int ySrc = y + yOffset;
                                int xKernel = xOffset + radius;
                                int yKernel = yOffset + radius;

                                if (ySrc < 0 || ySrc >= height || xSrc < 0 || xSrc >= width)
                                {
                                    sumB += edgeColor.Value.B * kernel[yKernel, xKernel];
                                    sumG += edgeColor.Value.G * kernel[yKernel, xKernel];
                                    sumR += edgeColor.Value.R * kernel[yKernel, xKernel];
                                    continue;
                                }

                                int pxSrc = (xSrc + ySrc * width) * 4;

                                sumB += src[pxSrc + 0] * kernel[yKernel, xKernel];
                                sumG += src[pxSrc + 1] * kernel[yKernel, xKernel];
                                sumR += src[pxSrc + 2] * kernel[yKernel, xKernel];
                            }
                        }

                        sumB = sumB < 0 ? 0 : sumB > 255 ? 255 : sumB;
                        sumG = sumG < 0 ? 0 : sumG > 255 ? 255 : sumG;
                        sumR = sumR < 0 ? 0 : sumR > 255 ? 255 : sumR;

                        int px = (x + y * width) * 4;

                        dst[px + 0] = (byte)Math.Round(sumB);
                        dst[px + 1] = (byte)Math.Round(sumG);
                        dst[px + 2] = (byte)Math.Round(sumR);
                        dst[px + 3] = src[px + 3];
                    });
                });
            }
        }
    }
}
