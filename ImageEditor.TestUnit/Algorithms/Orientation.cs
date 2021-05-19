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
        public static unsafe Bitmap RotateLeft(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Bitmap resBmp = new Bitmap(height, width);
            BitmapData resData = resBmp.LockBits(new Rectangle(0, 0, height, width), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            byte* scan0 = (byte*)data.Scan0;
            byte* resScan0 = (byte*)resData.Scan0;

            Parallel.For(0, height, (y) =>
            {
                for (int x = 0; x < width; x++)
                {
                    int src = (x + y * width) * 4;
                    int des = (y + (width - 1 - x) * height) * 4;
                    resScan0[des] = scan0[src];
                    resScan0[des + 1] = scan0[src + 1];
                    resScan0[des + 2] = scan0[src + 2];
                    resScan0[des + 3] = scan0[src + 3];
                }
            }
            );

            bitmap.UnlockBits(data);
            resBmp.UnlockBits(resData);
            return resBmp;
        }


        public static unsafe Bitmap RotateRight(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Bitmap resBmp = new Bitmap(height, width);
            BitmapData resData = resBmp.LockBits(new Rectangle(0, 0, height, width), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            byte* scan0 = (byte*)data.Scan0;
            byte* resScan0 = (byte*)resData.Scan0;

            Parallel.For(0, height, (y) =>
            {
                for (int x = 0; x < width; x++)
                {
                    int src = (x + y * width) * 4;
                    int des = (height - 1 - y + x * height) * 4;
                    resScan0[des] = scan0[src];
                    resScan0[des + 1] = scan0[src + 1];
                    resScan0[des + 2] = scan0[src + 2];
                    resScan0[des + 3] = scan0[src + 3];
                }
            }
            );

            bitmap.UnlockBits(data);
            resBmp.UnlockBits(resData);
            return resBmp;
        }


        public static unsafe Bitmap FlipHorizontal(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Bitmap resBmp = new Bitmap(width, height);
            BitmapData resData = resBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            byte* scan0 = (byte*)data.Scan0;
            byte* resScan0 = (byte*)resData.Scan0;

            Parallel.For(0, height, (y) =>
            {
                for (int x = 0; x < width; x++)
                {
                    int src = (x + y * width) * 4;
                    int des = (width - 1 - x + y * width) * 4;
                    resScan0[des] = scan0[src];
                    resScan0[des + 1] = scan0[src + 1];
                    resScan0[des + 2] = scan0[src + 2];
                    resScan0[des + 3] = scan0[src + 3];
                }
            }
            );

            bitmap.UnlockBits(data);
            resBmp.UnlockBits(resData);
            return resBmp;
        }


        public static unsafe Bitmap FlipVertical(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Bitmap resBmp = new Bitmap(width, height);
            BitmapData resData = resBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            byte* scan0 = (byte*)data.Scan0;
            byte* resScan0 = (byte*)resData.Scan0;

            Parallel.For(0, height, (y) =>
            {
                for (int x = 0; x < width; x++)
                {
                    int src = (x + y * width) * 4;
                    int des = (x + (height - 1 - y) * width) * 4;
                    resScan0[des] = scan0[src];
                    resScan0[des + 1] = scan0[src + 1];
                    resScan0[des + 2] = scan0[src + 2];
                    resScan0[des + 3] = scan0[src + 3];
                }
            }
            );

            bitmap.UnlockBits(data);
            resBmp.UnlockBits(resData);
            return resBmp;
        }
    }
}
