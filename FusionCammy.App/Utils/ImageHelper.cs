using OpenCvSharp;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;

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

        public static void OverlayDecorationSimple(Mat frame, Mat decoration, Point center)
        {
            decoration = decoration.Clone().Resize(new OpenCvSharp.Size(50, 50));

            int x = center.X - decoration.Width / 2;
            int y = center.Y - decoration.Height / 2;

            if (x < 0 || y < 0 || x + decoration.Width > frame.Width || y + decoration.Height > frame.Height)
                return;

            var roi = new Rect(x, y, decoration.Width, decoration.Height);

            Mat roiMat = new Mat(frame, roi);
            decoration.CopyTo(roiMat);
        }
    }
}
