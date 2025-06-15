using DlibDotNet;
using FusionCammy.Core.Models;

namespace FusionCammy.Core.Utils
{
    public static class DlibDataHelper
    {
        /// <summary>Extracts landmarks from a FullObjectDetection object using the 68-point predictor format.</summary>  
        public static Dictionary<FacePartType, List<OpenCvSharp.Point>> ExtractLandmarksFrom68PointPredictor(this FullObjectDetection detection)
        {
            var result = new Dictionary<FacePartType, List<OpenCvSharp.Point>>();
            var mapIndices = new Dictionary<FacePartType, List<int>>
            {
                { FacePartType.LeftEye,    Enumerable.Range(36, 6).ToList() },
                { FacePartType.RightEye,   Enumerable.Range(42, 6).ToList() },
                { FacePartType.LeftBrow,   Enumerable.Range(17, 5).ToList() },
                { FacePartType.RightBrow,  Enumerable.Range(22, 5).ToList() },
                { FacePartType.Nose,       Enumerable.Range(29, 7).ToList() },
                { FacePartType.OuterMouth, Enumerable.Range(48, 12).ToList() },
                { FacePartType.InnerMouth, Enumerable.Range(60, 8).ToList() },
                { FacePartType.Jawline,    Enumerable.Range(0, 17).ToList() },

                { FacePartType.Eyes,       Enumerable.Range(36, 12).ToList() },//new List<int> { 36, 30, 45 } },
                { FacePartType.Mouth,      Enumerable.Range(48, 20).ToList() }
            };

            foreach (var map in mapIndices)
            {
                var points = new List<OpenCvSharp.Point>();
                foreach (int index in map.Value)
                    points.Add(detection.ConvertToCvPoint((uint)index));

                result[map.Key] = points;
            }

            return result;
        }

        public static OpenCvSharp.Point ConvertToCvPoint(this FullObjectDetection detection, uint index)
        {
            return detection.GetPart(index).ConvertToCvPoint();
        }

        public static OpenCvSharp.Point ConvertToCvPoint(this Point point)
        {
            return new OpenCvSharp.Point(point.X, point.Y);
        }

        public static OpenCvSharp.Rect ConvertToCvRect(this Rectangle rect)
        {
            return new OpenCvSharp.Rect(rect.Left, rect.Top, (int)rect.Width, (int)rect.Height);
        }
    }
}
