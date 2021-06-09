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
