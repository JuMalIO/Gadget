using System.Drawing;

namespace Gadget.Extensions
{
    public static class ImageExtensions
    {
        private static readonly float _scale;

        static ImageExtensions()
        {
            using (Bitmap b = new Bitmap(1, 1))
            {
                _scale = b.HorizontalResolution / 96.0f;
            }
        }

        public static int GetWidth(this Image image)
        {
            return (int)(image.Width * _scale);
        }

        public static int GetHeight(this Image image)
        {
            return (int)(image.Height * _scale);
        }

        public static Image Resize(this Image image, int x, int y)
        {
            if (image == null)
            {
                return null;
            }

            if (image.Width < x && image.Height < y)
            {
                return image;
            }
            
            if (image.Width > image.Height)
            {
                y = (int)(x * (image.Height / (double)image.Width));
            }
            else
            {
                x = (int)(y * (image.Width / (double)image.Height));
            }

            return new Bitmap(image, new Size(x, y));
        }
    }
}
