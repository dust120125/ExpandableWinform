using System;
using System.Drawing;
using System.Drawing.Imaging;
using SImage = System.Drawing.Image;

namespace Dust.Image
{
    public class ImageSizeConverter
    {
        public enum ScaleOption { Height, Width, None }

        public static Color _borderColor = Color.LightGray;
        public static Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                borderBrush = new SolidBrush(_borderColor);
            }
        }

        private static SolidBrush borderBrush = new SolidBrush(Color.LightGray);

        public static SImage ConvertImageSize(SImage image, int width, int height)
        {
            return ConvertImageSize(image, width, height, _borderColor);
        }

        public static SImage ConvertImageSize(SImage image, int width, int height, Color borderColor)
        {
            if (width == 0 || height == 0) return image;
            Bitmap bmp = new Bitmap(width, height);

            double widthZoom = (double)width / image.Width;
            double heightZoom = (double)height / image.Height;
            double scale = Math.Min(widthZoom, heightZoom);

            ScaleOption scaleOption;
            if (widthZoom == heightZoom)
            {
                scaleOption = ScaleOption.None;
            }
            else if (widthZoom < heightZoom)
            {
                scaleOption = ScaleOption.Width;
            }
            else
            {
                scaleOption = ScaleOption.Height;
            }

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                if (scaleOption == ScaleOption.None)
                {
                    gfx.DrawImage(image, 0, 0, width, height);
                }
                else
                {
                    DrawScaledImage(image, gfx, width, height, scale, scaleOption, borderColor);
                }
            }

            return bmp;
        }

        private static void DrawScaledImage(SImage image, Graphics graphics, int width, int height, double scale, ScaleOption scaleOption, Color borderColor)
        {
            SolidBrush brush = new SolidBrush(borderColor);
            graphics.FillRectangle(brush, 0, 0, width, height);
            if (scaleOption == ScaleOption.Width)
            {
                int h = (int)(image.Height * scale);
                int y = (height - h) / 2;                
                graphics.DrawImage(image, 0, y, width, h);
            }
            else if (scaleOption == ScaleOption.Height)
            {
                int w = (int)(image.Width * scale);
                int x = (width - w) / 2;
                graphics.DrawImage(image, x, 0, w, height);
            }
        }

        public static Color GetAvgColor(SImage image)
        {
            Bitmap bmp = (Bitmap)image.Clone();

            Rectangle rec = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rec, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            IntPtr ptr = bmpData.Scan0;

            int totalPixel = bmp.Width * bmp.Height;
            int numBytes = totalPixel * 4;

            byte[] argbValues = new byte[numBytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

            long totalR = 0, totalG = 0, totalB = 0;
            byte avgR, avgG, avgB;

            for(int i = 0; i < argbValues.Length; i += 4)
            {
                int pos;

                pos = i + 0; //B
                totalB += argbValues[pos];

                pos = i + 1; //G
                totalG += argbValues[pos];

                pos = i + 2; //R
                totalR += argbValues[pos];
            }

            avgR = (byte)(totalR / totalPixel);
            avgG = (byte)(totalG / totalPixel);
            avgB = (byte)(totalB / totalPixel);

            return Color.FromArgb(avgR, avgG, avgB);
        }

    }



}
