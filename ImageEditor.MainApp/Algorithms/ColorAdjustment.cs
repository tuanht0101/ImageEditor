using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor.MainApp.Algorithms
{
    class ColorAdjustment
    {
        public static unsafe Bitmap HSL(Bitmap src, int hueOffset, int saturationPercent, int lightnessPercent)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            int width = src.Width;
            int height = src.Height;
            double saturation = saturationPercent / 100d;
            double lightness = lightnessPercent / 100d;

            Bitmap res = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            int pxs = width * height;
            for (int px = 0; px < pxs; px++)
            {
                int pxRes = px * 4;
                HSL hsl = new HSL(srcScan0[pxRes + 2], srcScan0[pxRes + 1], srcScan0[pxRes]);
                hsl.Hue += hueOffset;
                hsl.Saturation += hsl.Saturation * saturation;
                hsl.Lightness += hsl.Lightness * lightness;
                Color color = hsl.ToRGB();
                resScan0[pxRes + 0] = color.B;
                resScan0[pxRes + 1] = color.G;
                resScan0[pxRes + 2] = color.R;
                resScan0[pxRes + 3] = srcScan0[pxRes + 3];
            }

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }


        public static unsafe Bitmap BrightnessContrast(Bitmap src, int brightnessPercent, int contrastOffset)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Helper.MaxAllocatedProcessors
            };

            int width = src.Width;
            int height = src.Height;
            double brightness = brightnessPercent / 100d;

            Bitmap res = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData resData = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* resScan0 = (byte*)resData.Scan0;

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* srcScan0 = (byte*)srcData.Scan0;

            int pxs = width * height;
            for (int px = 0; px < pxs; px++)
            {
                int pxRes = px * 4;
                double r = Math.Round(srcScan0[pxRes + 0] * brightness + contrastOffset);
                double g = Math.Round(srcScan0[pxRes + 1] * brightness + contrastOffset);
                double b = Math.Round(srcScan0[pxRes + 2] * brightness + contrastOffset);
                r = r < 0 ? 0 : r > 255 ? 255 : r;
                g = g < 0 ? 0 : g > 255 ? 255 : g;
                b = b < 0 ? 0 : b > 255 ? 255 : b;
                resScan0[pxRes + 0] = (byte)r;
                resScan0[pxRes + 1] = (byte)g;
                resScan0[pxRes + 2] = (byte)b;
                resScan0[pxRes + 3] = srcScan0[pxRes + 3];
            }

            src.UnlockBits(srcData);
            res.UnlockBits(resData);

            return res;
        }
    }


    //struct HSV
    //{
    //    public HSV(int red, int green, int blue)
    //    {
    //        int[] channels = new int[3] { red, green, blue };
    //        int max = channels.Max();
    //        int min = channels.Min();
    //        double chroma = max - min;

    //        hue = 0;
    //        saturation = 0;
    //        value = max / 255d;

    //        if (!chroma.Equals(0))
    //        {
    //            if (max == red)
    //                Hue = (green - blue) / chroma % 6 * 60;
    //            else if (max == green)
    //                Hue = ((blue - red) / chroma + 2) * 60;
    //            else
    //                Hue = ((red - green) / chroma + 4) * 60;
    //        }

    //        if (max != 0)
    //            Saturation = chroma / max;
    //    }

    //    double hue;
    //    public double Hue
    //    {
    //        get => hue;
    //        set
    //        {
    //            if (value < 0)
    //                hue = value + 360;
    //            else if (value >= 360)
    //                hue = value - 360;
    //            else
    //                hue = value;
    //        }
    //    }

    //    double saturation;
    //    public double Saturation
    //    {
    //        get => saturation;
    //        set
    //        {
    //            if (value < 0)
    //                saturation = 0;
    //            else if (value > 1)
    //                saturation = 1;
    //            else
    //                saturation = value;
    //        }
    //    }

    //    double value;
    //    public double Value
    //    {
    //        get => value;
    //        set
    //        {
    //            if (value < 0)
    //                this.value = 0;
    //            else if (value > 1)
    //                this.value = 1;
    //            else
    //                this.value = value;
    //        }
    //    }

    //    public Color ToRGB()
    //    {
    //        double chroma = Value * Saturation;
    //        double hueDiv = Hue / 60;
    //        double kR = (5 + hueDiv) % 6;
    //        double kG = (3 + hueDiv) % 6;
    //        double kB = (1 + hueDiv) % 6;
    //        int r = (int)Math.Round((Value - chroma * Math.Max(0, Math.Min(Math.Min(kR, 4 - kR), 1))) * 255);
    //        int g = (int)Math.Round((Value - chroma * Math.Max(0, Math.Min(Math.Min(kG, 4 - kG), 1))) * 255);
    //        int b = (int)Math.Round((Value - chroma * Math.Max(0, Math.Min(Math.Min(kB, 4 - kB), 1))) * 255);
    //        return Color.FromArgb(r, g, b);
    //    }
    //}

    struct HSL
    {
        public HSL(int red, int green, int blue)
        {
            int[] channels = new int[3] { red, green, blue };
            int max = channels.Max();
            int min = channels.Min();
            double chroma = max - min;

            hue = 0;
            saturation = 0;
            lightness = (max + min) / 2 / 255d;

            if (!chroma.Equals(0))
            {
                if (max == red)
                    Hue = (green - blue) / chroma % 6 * 60;
                else if (max == green)
                    Hue = ((blue - red) / chroma + 2) * 60;
                else
                    Hue = ((red - green) / chroma + 4) * 60;
            }

            if (lightness != 0 && lightness != 1)
                Saturation = (chroma / 255) / (1 - Math.Abs(2 * lightness - 1));
        }

        double hue;
        public double Hue
        {
            get => hue;
            set
            {
                if (value < 0)
                    hue = value + 360;
                else if (value >= 360)
                    hue = value - 360;
                else
                    hue = value;
            }
        }

        double saturation;
        public double Saturation
        {
            get => saturation;
            set
            {
                if (value < 0)
                    saturation = 0;
                else if (value > 1)
                    saturation = 1;
                else
                    saturation = value;
            }
        }

        double lightness;
        public double Lightness
        {
            get => lightness;
            set
            {
                if (value < 0)
                    lightness = 0;
                else if (value > 1)
                    lightness = 1;
                else
                    lightness = value;
            }
        }

        public Color ToRGB()
        {
            double a = saturation * Math.Min(lightness, 1 - lightness);
            double hueDiv = Hue / 30;
            double kR = (0 + hueDiv) % 12;
            double kG = (8 + hueDiv) % 12;
            double kB = (4 + hueDiv) % 12;
            int r = (int)Math.Round((lightness - a * Math.Max(-1, Math.Min(Math.Min(kR - 3, 9 - kR), 1))) * 255);
            int g = (int)Math.Round((Lightness - a * Math.Max(-1, Math.Min(Math.Min(kG - 3, 9 - kG), 1))) * 255);
            int b = (int)Math.Round((Lightness - a * Math.Max(-1, Math.Min(Math.Min(kB - 3, 9 - kB), 1))) * 255);
            return Color.FromArgb(r, g, b);
        }
    }
}
