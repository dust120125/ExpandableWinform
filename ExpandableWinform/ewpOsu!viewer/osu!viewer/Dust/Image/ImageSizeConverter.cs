using System;
using System.Drawing;
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
                    DrawScaledImage(image, gfx, width, height, scale, scaleOption);
                }
            }

            return bmp;
        }

        private static void DrawScaledImage(SImage image, Graphics graphics, int width, int height, double scale, ScaleOption scaleOption)
        {
            graphics.FillRectangle(borderBrush, 0, 0, width, height);
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
    }



}
