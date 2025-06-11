using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FusionCammy.App.Utils
{
    public static class ImageHelper
    {
        public static WriteableBitmap ConvertBgrRawDataToBitmap(byte[] rawData, int width, int height)
        {
            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), rawData, width * 3, 0);
            return bitmap;
        }
    }
}
