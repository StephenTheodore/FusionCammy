using OpenCvSharp;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;
using Size = OpenCvSharp.Size;

namespace FusionCammy.Core.Utils
{
    public static class OpenCvHelper
    {
        /// <summary>Overlays a decoration image onto the main image without considering alpha transparency.</summary>
        public static void OverlayWithoutAlpha(this Mat orgImagw, Mat overlay, Point center, Size targetSize, float angle)
        {
            Mat resizedOverlay = overlay.Resize(targetSize);

            int x = center.X - resizedOverlay.Width / 2;
            int y = center.Y - resizedOverlay.Height / 2;

            if (x < 0 || y < 0 || x + resizedOverlay.Width > orgImagw.Width || y + resizedOverlay.Height > orgImagw.Height)
            {
                resizedOverlay.Dispose();
                return;
            }

            if (Math.Abs(angle) > double.Epsilon)
            {
                var resizedCenter = new Point2f(resizedOverlay.Width / 2f, resizedOverlay.Height / 2f);
                var rotationMat = Cv2.GetRotationMatrix2D(resizedCenter, -angle, 1.0);

                var rotated = new Mat();
                Cv2.WarpAffine(resizedOverlay, rotated, rotationMat, resizedOverlay.Size(),
                    InterpolationFlags.Linear, BorderTypes.Transparent);

                resizedOverlay.Dispose();
                resizedOverlay = rotated;
            }

            RotatedRect roi = new RotatedRect(new Point2f(x, y), new Size2f(resizedOverlay.Width, resizedOverlay.Height), angle);
            Mat roiMat = new Mat(orgImagw, roi.BoundingRect());

            resizedOverlay.CopyTo(roiMat);
            resizedOverlay.Dispose();
        }

        /// <summary>Overlays a decoration image onto the main image considering alpha transparency.</summary>
        public static void OverlayWithAlpha(this Mat orgImage, Mat overlay, Point center, RotatedRect rotatedRect)
        {
            if (overlay.Empty() || overlay.Channels() != 4)
                return;

            Rect roiBounds = rotatedRect.BoundingRect();

            if (roiBounds.Left < 0 || roiBounds.Top < 0 || roiBounds.Right > orgImage.Width || roiBounds.Bottom > orgImage.Height)
                return;

            // [수정 1] 회전 후의 크기가 아닌, 회전 전의 의도된 크기(rotatedRect.Size)로 리사이즈한다.
            using Mat resizedOverlay = overlay.Resize(new Size(rotatedRect.Size.Width, rotatedRect.Size.Height));

            // [수정 2] 회전 중심은 최종 목적지가 아닌, 'resizedOverlay' 이미지 자체의 중심이어야 한다.
            var rotationCenter = new Point2f(resizedOverlay.Width / 2f, resizedOverlay.Height / 2f);
            using var rotationMatrix = Cv2.GetRotationMatrix2D(rotationCenter, -rotatedRect.Angle, 1.0);

            // [수정 3] 회전된 이미지가 roiBounds의 중앙에 위치하도록 이동(Translation) 값을 보정한다.
            rotationMatrix.Set(0, 2, rotationMatrix.Get<double>(0, 2) + (roiBounds.Width / 2.0) - rotationCenter.X);
            rotationMatrix.Set(1, 2, rotationMatrix.Get<double>(1, 2) + (roiBounds.Height / 2.0) - rotationCenter.Y);


            using Mat rotatedOverlay = new Mat(roiBounds.Size, resizedOverlay.Type(), Scalar.All(0));
            Cv2.WarpAffine(resizedOverlay, rotatedOverlay, rotationMatrix, roiBounds.Size,
                InterpolationFlags.Linear, BorderTypes.Transparent);

            using Mat roiMat = new Mat(orgImage, roiBounds);

            Mat[] channels = Cv2.Split(rotatedOverlay);
            using Mat overlayFg = new Mat();
            using Mat overlayAlpha3ch = new Mat();
            using Mat background = new Mat();

            Cv2.Merge([channels[0], channels[1], channels[2]], overlayFg);
            Cv2.CvtColor(channels[3], overlayAlpha3ch, ColorConversionCodes.GRAY2BGR);

            overlayFg.ConvertTo(overlayFg, MatType.CV_32FC3);
            overlayAlpha3ch.ConvertTo(overlayAlpha3ch, MatType.CV_32FC3, 1.0 / 255);
            roiMat.ConvertTo(background, MatType.CV_32FC3);

            Cv2.Multiply(overlayAlpha3ch, overlayFg, overlayFg);
            Cv2.Multiply(Scalar.All(1.0) - overlayAlpha3ch, background, background);
            Cv2.Add(overlayFg, background, background);

            foreach (var ch in channels)
                ch.Dispose();

            background.ConvertTo(roiMat, MatType.CV_8UC3);
        }

        public static Point GetCenterPoint(IReadOnlyList<Point> points)
        {
            if (points == null || points.Count == 0)
                return new Point(0, 0);
            else if (points.Count == 1)
                return points[0];
            else if (points.Count == 2)
                return new Point(
                    (points[0].X + points[1].X) / 2,
                    (points[0].Y + points[1].Y) / 2
                );
            else
            {
                var moments = Cv2.Moments(points);

                return new Point(
                    (int)(moments.M10 / moments.M00),
                    (int)(moments.M01 / moments.M00)
                );
            }
        }

        public static float GetAngleBetween(Point2f point1, Point2f point2)
        {
            float dx = point2.X - point1.X;
            float dy = point2.Y - point1.Y;
            float angleRad = (float)Math.Atan2(dy, dx);
            float angleDeg = angleRad * 180f / (float)Math.PI;

            return angleDeg;
        }
    }
}
