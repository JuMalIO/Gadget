using System.Drawing;

namespace Gadget.Extensions
{
    public static class ImageExtensions
    {
        public static Image Resize(this Image image, int x, int y)
        {
            if (image == null)
                return null;

            if (image.Width > x || image.Height > y)
            {
                if (image.Width > image.Height)
                {
                    y = (int)((double)x * (double)((double)image.Height / (double)image.Width));
                }
                else
                {
                    x = (int)((double)y * (double)((double)image.Width / (double)image.Height));
                }
                return (Image)(new Bitmap(image, new Size(x, y)));
            }

            return image;
        }
    }
}
