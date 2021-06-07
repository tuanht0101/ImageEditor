using ImageEditor.MainApp.Models;
using ImageEditor.MainApp.Views.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ImageEditor
{
    public static class Utilities
    {
        /// <summary>
        /// Only for PixelFormat.Format32bppArgb with order BGRA
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="color"></param>
        public static unsafe void FillColor(Bitmap bitmap, Color color)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            byte* ptr = (byte*)bmpData.Scan0;

            int pixels = bitmap.Width * bitmap.Height;
            Parallel.For(0, pixels, (pixel) =>
            {
                int coor = pixel * 4;
                ptr[coor + 0] = color.B;
                ptr[coor + 1] = color.G;
                ptr[coor + 2] = color.R;
                ptr[coor + 3] = color.A;
            });

            bitmap.UnlockBits(bmpData);
        }

        public static void FillRenderTransparent(Bitmap bitmap, int boxSize = 20)
        {
            Color lightGray = Color.LightGray;
            byte[] grayBytes = new byte[4] {lightGray.B, lightGray.G, lightGray.R, lightGray.A};
            byte[] whiteBytes = new byte[4] {255, 255, 255, 255};

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;

            int byteCount = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] newBytes = new byte[byteCount];

            int boxesX = bitmap.Width / 20;
            int boxesY = bitmap.Height / 20;
            int pixelsLeftoverX = bitmap.Width % 20;
            int pixelsLeftoverY = bitmap.Height % 20;
            bool isInverted = false;

            

            for (int boxY = 0; boxY < boxesY; boxY++)
            {
                for (int line = 0; line < 20; line++)
                {
                    bool isInvertedLine = isInverted;
                    for (int boxX = 0; boxX < boxesX; boxX++)
                    {
                        for (int pixel = 0; pixel < 20; pixel++)
                        {
                            bool isInvertedCurrent = isInverted ? !isInvertedLine : isInvertedLine;
                            byte[] color = isInvertedCurrent ? whiteBytes : grayBytes;
                            int pixelCoor = pixel * 4 + boxX * 20 + line * Math.Abs(bmpData.Stride);
                            newBytes[0 + pixelCoor] = color[0];
                            newBytes[1 + pixelCoor] = color[1];
                            newBytes[2 + pixelCoor] = color[2];
                            newBytes[3 + pixelCoor] = color[3];
                        }
                        isInvertedLine = !isInvertedLine;
                    }
                    for (int pixel = 0; pixel < pixelsLeftoverX; pixel++)
                    {
                        bool isInvertedCurrent = isInverted ? !isInvertedLine : isInvertedLine;
                        byte[] color = isInvertedCurrent ? whiteBytes : grayBytes;
                        int pixelCoor = pixel * 4 + boxesX * 20 + line * Math.Abs(bmpData.Stride);
                        newBytes[0 + pixelCoor] = color[0];
                        newBytes[1 + pixelCoor] = color[1];
                        newBytes[2 + pixelCoor] = color[2];
                        newBytes[3 + pixelCoor] = color[3];
                    }
                }
                isInverted = !isInverted;
            }
            for (int line = 0; line < pixelsLeftoverY; line++)
            {
                bool isInvertedLine = isInverted;
                for (int boxX = 0; boxX < boxesX; boxX++)
                {
                    for (int pixel = 0; pixel < 20; pixel++)
                    {
                        bool isInvertedCurrent = isInverted ? !isInvertedLine : isInvertedLine;
                        byte[] color = isInvertedCurrent ? whiteBytes : grayBytes;
                        int pixelCoor = pixel * 4 + boxX * 20 + line * Math.Abs(bmpData.Stride);
                        newBytes[0 + pixelCoor] = color[0];
                        newBytes[1 + pixelCoor] = color[1];
                        newBytes[2 + pixelCoor] = color[2];
                        newBytes[3 + pixelCoor] = color[3];
                    }
                    isInvertedLine = !isInvertedLine;
                }
                for (int pixel = 0; pixel < pixelsLeftoverX; pixel++)
                {
                    bool isInvertedCurrent = isInverted ? !isInvertedLine : isInvertedLine;
                    byte[] color = isInvertedCurrent ? whiteBytes : grayBytes;
                    int pixelCoor = pixel * 4 + boxesX * 20 + line * Math.Abs(bmpData.Stride);
                    newBytes[0 + pixelCoor] = color[0];
                    newBytes[1 + pixelCoor] = color[1];
                    newBytes[2 + pixelCoor] = color[2];
                    newBytes[3 + pixelCoor] = color[3];
                }
            }


            Marshal.Copy(newBytes, 0, ptr, byteCount);

            bitmap.UnlockBits(bmpData);
        }

        public static string OpenImageFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Image File (*.png;*.jpg;*.bmp;*.tiff;*.tif)|*.png;*.jpg;*.bmp;*.tiff;*.tif",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        public static int GCD(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }
    }
}
