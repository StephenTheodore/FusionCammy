using FusionCammy.Core.Managers;
using FusionCammy.Core.Models;
using FusionCammy.Core.Utils;
using OpenCvSharp;

namespace FusionCammy.Core.Services
{
    public class DecorationService(DecorationManager decorationManager, AssetManager assetManager)
    {
        public void Decorate(Mat frameData, FaceInfo? faceInfo)
        {
            Dictionary<FacePartType, List<Point>> partPoints = [];
            Dictionary<FacePartType, Point> partCenterPoints = [];

            foreach (var facePartType in Enum.GetValues(typeof(FacePartType)))
            {
                if (faceInfo?.Anchors.TryGetValue((FacePartType)facePartType, out var anchors) ?? false)
                {
                    partPoints[(FacePartType)facePartType] = anchors;
                    partCenterPoints[(FacePartType)facePartType] = OpenCvHelper.GetCenterPoint(anchors);
                }
            }

            foreach (DecorationInfo decoration in decorationManager.SelectedDecorations)
            {
                var decorationImage = assetManager.GetImage(decoration.Id);

                if (!partPoints.TryGetValue(decoration.FacePartType, out var anchorPoints))
                    continue;
                if (!partCenterPoints.TryGetValue(decoration.FacePartType, out var partCenterPoint))
                    continue;

                Point? anchorPoint;
                if (decoration.FacePartType == FacePartType.Eyes && partCenterPoints.TryGetValue(FacePartType.Nose, out var noseCenterPoint))
                    anchorPoint = new Point(noseCenterPoint.X, partCenterPoint.Y);
                else
                    anchorPoint = partCenterPoint;

                var partRect = Cv2.MinAreaRect(anchorPoints);
                partRect.Size = new Size2f(
                    Math.Max(partRect.Size.Width, partRect.Size.Height) * decoration.ScaleX,
                    Math.Min(partRect.Size.Width, partRect.Size.Height) * decoration.ScaleY);
                partRect.Angle = OpenCvHelper.GetAngleBetween(anchorPoints.MinBy(point => point.X), anchorPoints.MaxBy(point => point.X));

                if (anchorPoint.HasValue)
                {
                    partRect.Center = new Point2f(anchorPoint.Value.X, anchorPoint.Value.Y);
                    frameData.OverlayWithAlpha(decorationImage, anchorPoint.Value, partRect);
                }
            }
        }
    }
}
