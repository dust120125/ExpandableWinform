using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SImage = System.Drawing.Image;
using System.Drawing.Imaging;

namespace Dust.Image
{
    public class ImageTransparentSimulator
    {

        private const string SIZE_NO_MATCH = "The size of background image must be same with origin image.";
        private const int bytesPerPixel = 4;

        public static SImage getTransparentImage(SImage image, SImage background, float opacity)
        {
            if (!image.Size.Equals(background.Size)) throw new ImageSizeNoMatchException(SIZE_NO_MATCH);

            if ((image.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed
                || (background.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
            {
                // Cannot modify an image with indexed colors
                return image;
            }

            Bitmap bmp = (Bitmap)image.Clone();
            Bitmap bg = (Bitmap)background.Clone();

            float u_opacity = 1f - opacity;

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            BitmapData bgData = bg.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            IntPtr bgPtr = bgData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            // This code is specific to a bitmap with 32 bits per pixels 
            // (32 bits = 4 bytes, 3 for RGB and 1 byte for alpha).
            int numBytes = bmp.Width * bmp.Height * bytesPerPixel;
            byte[] argbValues = new byte[numBytes];
            byte[] bgArgbValues = new byte[numBytes];

            // Copy the ARGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);
            System.Runtime.InteropServices.Marshal.Copy(bgPtr, bgArgbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // RGB values for all pixels in the the bitmap.
            for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                int pos;

                pos = counter + 0; //B
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bgArgbValues[pos] * u_opacity);

                pos = counter + 1; //G
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bgArgbValues[pos] * u_opacity);

                pos = counter + 2; //R
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bgArgbValues[pos] * u_opacity);
            }

            // Copy the ARGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static SImage getTransparentImage(SImage image, Color backColor, float opacity)
        {

            if ((image.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
            {
                // Cannot modify an image with indexed colors
                return image;
            }

            Bitmap bmp = (Bitmap)image.Clone();
            byte br = backColor.R;
            byte bg = backColor.G;
            byte bb = backColor.B;

            float u_opacity = 1f - opacity;

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            // This code is specific to a bitmap with 32 bits per pixels 
            // (32 bits = 4 bytes, 3 for RGB and 1 byte for alpha).
            int numBytes = bmp.Width * bmp.Height * bytesPerPixel;
            byte[] argbValues = new byte[numBytes];

            // Copy the ARGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // RGB values for all pixels in the the bitmap.
            for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                int pos;

                pos = counter + 0; //B
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bb * u_opacity);

                pos = counter + 1; //G
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bg * u_opacity);

                pos = counter + 2; //R
                argbValues[pos] = (byte)(argbValues[pos] * opacity + br * u_opacity);
            }

            // Copy the ARGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }


    [Serializable]
    public class ImageSizeNoMatchException : Exception
    {
        public ImageSizeNoMatchException() { }
        public ImageSizeNoMatchException(string message) : base(message) { }
        public ImageSizeNoMatchException(string message, Exception inner) : base(message, inner) { }
        protected ImageSizeNoMatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
