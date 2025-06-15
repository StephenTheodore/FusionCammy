using FusionCammy.Core.Managers;
using FusionCammy.Core.Models;
using FusionCammy.Core.Utils;
using OpenCvSharp;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

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
                    FacePartType.Nose or
                    FacePartType.Mouth or
                    FacePartType.LeftEye or
                    FacePartType.RightEye or
                    FacePartType.LeftBrow or
                    FacePartType.RightBrow or
                    FacePartType.OuterMouth or
                    FacePartType.InnerMouth => OpenCvHelper.GetCenterPoint(anchorPoints),

                    FacePartType.Eyes => new Point(OpenCvHelper.GetCenterPoint(faceInfo?.Anchors[FacePartType.Nose]).X, OpenCvHelper.GetCenterPoint(anchorPoints).Y),
                    //FacePartType.Nose => anchorPoints[0],   // NoseTip Index : 30

                    FacePartType.Jawline => OpenCvHelper.GetCenterPoint(anchorPoints[8..11] ?? []),

                    _ => throw new NotImplementedException(),
                };

                var partRect = Cv2.MinAreaRect(anchorPoints);
                partRect.Size = new Size2f(
                    Math.Max(partRect.Size.Width, partRect.Size.Height) * decoration.ScaleX,
                    Math.Min(partRect.Size.Width, partRect.Size.Height) * decoration.ScaleY);
                partRect.Angle = OpenCvHelper.GetAngleBetween(anchorPoints.MinBy(point => point.X), anchorPoints.MaxBy(point => point.X));

                Debug.WriteLine($"{decoration.FacePartType} Angle {partRect.Angle}");

                if (anchorPoint.HasValue)
                {
                    partRect.Center = new Point2f(anchorPoint.Value.X, anchorPoint.Value.Y);
                    frameData.OverlayWithAlpha(decorationImage, anchorPoint.Value, partRect);
                }
            }
        }
    }
}
