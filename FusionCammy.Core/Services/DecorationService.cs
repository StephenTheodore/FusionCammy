using FusionCammy.Core.Managers;
using FusionCammy.Core.Models;
using FusionCammy.Core.Utils;
using OpenCvSharp;

namespace FusionCammy.Core.Services
{
    public class DecorationService (DecorationManager decorationManager, AssetManager assetManager)
    {
        public void Decorate(Mat frameData, FaceInfo? faceInfo)
        {
            foreach(DecorationInfo decoration in decorationManager.SelectedDecorations)
            {
                var decorationImage = assetManager.GetImage(decoration.Id);

                var anchorPoints = faceInfo?.Anchors[decoration.FacePartType] ?? [];
                Point? anchorPoint = decoration.FacePartType switch
                {
                    FacePartType.LeftEye or
                    FacePartType.RightEye or
                    FacePartType.LeftBrow or
                    FacePartType.RightBrow or
                    FacePartType.OuterMouth or
                    FacePartType.InnerMouth => OpenCvHelper.GetCenterPoint(anchorPoints),

                    FacePartType.Nose => anchorPoints?.LastOrDefault(),

                    FacePartType.Jawline => OpenCvHelper.GetCenterPoint(anchorPoints.GetRange(8, 3) ?? []),

                    _ => throw new NotImplementedException(),
                };

                var firstPoint = anchorPoints?.FirstOrDefault();
                var lastPoint = anchorPoints?.LastOrDefault();
                var partSize = firstPoint.HasValue && lastPoint.HasValue ? firstPoint.Value.DistanceTo(lastPoint.Value) : 1d;
                var drawingScale = partSize / Math.Max(decorationImage.Width, decorationImage.Height);

                if (anchorPoint.HasValue && drawingScale > double.Epsilon)
                {
                    var targetSize = new Size(decorationImage.Width * decoration.scale * drawingScale, decorationImage.Height * decoration.scale * drawingScale);
                    frameData.OverlayWithAlpha(decorationImage, anchorPoint.Value, targetSize);
                }
            }
        }
    }
}
