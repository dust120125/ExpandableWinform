using System;
using System.Drawing;
using System.Drawing.Imaging;
using SImage = System.Drawing.Image;

namespace Dust.Image
{
    public class ImageTransparentSimulator
    {

        private const string SIZE_NO_MATCH = "The size of background image must be same with origin image.";
        private const int bytesPerPixel = 4;

        public static SImage GetTransparentImage(SImage image, SImage background, float opacity)
        {
            return GetTransparentImage(image, background, opacity, out Color avgColor);
        }

        public static SImage GetTransparentImage(SImage image, SImage background, float opacity, out Color avgColor)
        {
            if (!image.Size.Equals(background.Size)) throw new ImageSizeNoMatchException(SIZE_NO_MATCH);

            avgColor = Color.Black;

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
            int totalPixel = bmp.Width * bmp.Height;
            int numBytes = totalPixel * bytesPerPixel;
            byte[] argbValues = new byte[numBytes];
            byte[] bgArgbValues = new byte[numBytes];

            // Copy the ARGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);
            System.Runtime.InteropServices.Marshal.Copy(bgPtr, bgArgbValues, 0, numBytes);

            long totalR = 0, totalG = 0, totalB = 0;
            byte avgR, avgG, avgB;

            // Manipulate the bitmap, such as changing the
            // RGB values for all pixels in the the bitmap.
            for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                int pos;                

                pos = counter + 0; //B
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bgArgbValues[pos] * u_opacity);
                totalB += argbValues[pos];

                pos = counter + 1; //G
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bgArgbValues[pos] * u_opacity);
                totalG += argbValues[pos];

                pos = counter + 2; //R
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bgArgbValues[pos] * u_opacity);
                totalR += argbValues[pos];
            }

            avgR = (byte)(totalR / totalPixel);
            avgG = (byte)(totalG / totalPixel);
            avgB = (byte)(totalB / totalPixel);
            avgColor = Color.FromArgb(avgR, avgG, avgB);

            // Copy the ARGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static SImage GetTransparentImage(SImage image, Color backColor, float opacity)
        {
            return GetTransparentImage(image, backColor, opacity, out Color avgColor);
        }

        public static SImage GetTransparentImage(SImage image, Color backColor, float opacity, out Color avgColor)
        {
            avgColor = Color.Black;

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
            int totalPixel = bmp.Width * bmp.Height;
            int numBytes = totalPixel * bytesPerPixel;
            byte[] argbValues = new byte[numBytes];

            // Copy the ARGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

            long totalR = 0, totalG = 0, totalB = 0;
            byte avgR, avgG, avgB;

            // Manipulate the bitmap, such as changing the
            // RGB values for all pixels in the the bitmap.
            for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                int pos;

                pos = counter + 0; //B
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bb * u_opacity);
                totalB += argbValues[pos];

                pos = counter + 1; //G
                argbValues[pos] = (byte)(argbValues[pos] * opacity + bg * u_opacity);
                totalG += argbValues[pos];

                pos = counter + 2; //R
                argbValues[pos] = (byte)(argbValues[pos] * opacity + br * u_opacity);
                totalR += argbValues[pos];
            }

            avgR = (byte)(totalR / totalPixel);
            avgG = (byte)(totalG / totalPixel);
            avgB = (byte)(totalB / totalPixel);
            avgColor = Color.FromArgb(avgR, avgG, avgB);

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
