using OpenCvSharp;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;
using Size = OpenCvSharp.Size;

namespace FusionCammy.App.Utils
{
    public static class ImageHelper
    {
        public static void OverlayDecorationNoneAlpha(this Mat image, Mat decoration, Point center, Size? targetSize = null)
        {
            Mat resized = targetSize.HasValue ? decoration.Resize(targetSize.Value) : decoration;

            int x = center.X - resized.Width / 2;
            int y = center.Y - resized.Height / 2;

            if (x < 0 || y < 0 || x + resized.Width > image.Width || y + resized.Height > image.Height)
                return;

            var roi = new Rect(x, y, resized.Width, resized.Height);

            Mat roiMat = new Mat(image, roi);
            resized.CopyTo(roiMat);

            if (targetSize.HasValue)
                resized.Dispose();
        }

        public static void OverlayDecorationWithAlpha(this Mat image, Mat decoration, Point center, Size? targetSize = null)
        {
            if (decoration.Empty() || decoration.Channels() != 4)
                return;

            Mat resized = targetSize.HasValue ? decoration.Resize(targetSize.Value) : decoration;

            int x = center.X - resized.Width / 2;
            int y = center.Y - resized.Height / 2;

            if (x < 0 || y < 0 || x + resized.Width > image.Width || y + resized.Height > image.Height)
            {
                if (targetSize.HasValue) resized.Dispose();
                return;
            }

            Rect roi = new Rect(x, y, resized.Width, resized.Height);
            Mat roiMat = new Mat(image, roi);

            Mat[] channels = Cv2.Split(resized);
            Mat alpha3Channel = new Mat();
            Mat foreGround = new Mat();
            Mat backGround = new Mat();

            Cv2.CvtColor(channels[3], alpha3Channel, ColorConversionCodes.GRAY2BGR);
            alpha3Channel.ConvertTo(alpha3Channel, MatType.CV_32FC3, 1.0 / 255.0);

            Cv2.Merge([channels[0], channels[1], channels[2]], foreGround);
            foreGround.ConvertTo(foreGround, MatType.CV_32FC3);
            roiMat.ConvertTo(backGround, MatType.CV_32FC3);

            Cv2.Multiply(alpha3Channel, foreGround, foreGround);
            Cv2.Multiply(Scalar.All(1.0) - alpha3Channel, backGround, backGround);
            Cv2.Add(foreGround, backGround, backGround);
            backGround.ConvertTo(roiMat, MatType.CV_8UC3);

            foreach (var ch in channels)
                ch.Dispose();

            alpha3Channel.Dispose();
            foreGround.Dispose();
            backGround.Dispose();

            if (targetSize.HasValue)
                resized.Dispose();
        }
    }
}
