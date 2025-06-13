using DlibDotNet;
using FusionCammy.Core.Configurations;
using FusionCammy.Core.Models;
using FusionCammy.Core.Utils;
using Microsoft.Extensions.Options;
using OpenCvSharp;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FusionCammy.Core.Services
{
    public class FacialAnalysisService(IOptions<MlConfiguration> options)
    {
        private readonly ShapePredictor _shapePredictor = ShapePredictor.Deserialize(options.Value.FacialDetectionModelPath);

        private readonly FrontalFaceDetector _faceDetector = Dlib.GetFrontalFaceDetector();

        public async Task<FaceInfo?> AnalyzeAsync(Mat image)
        {
            ArgumentNullException.ThrowIfNull(image);

            byte[] dataArray = new byte[image.Width * image.Height * image.ElemSize()];
            Marshal.Copy(image.Data, dataArray, 0, dataArray.Length);

            using var dlibImage = Dlib.LoadImageData<BgrPixel>(dataArray, (uint)image.Height, (uint)image.Width, (uint)image.Step());

            var faceRects = _faceDetector.Operator(dlibImage);
            if (faceRects.Length == 0)
                return null;

            var faceRect = faceRects[0];
            var shape = _shapePredictor.Detect(dlibImage, faceRect);
            var faceInfo = new FaceInfo
            {
                Bounds = faceRect.ConvertToCvRect(),
                NosePosition = shape.GetPart(30).ConvertToCvPoint(),
                MouthPosition = shape.GetPart(62).ConvertToCvPoint(),
                LeftEyePosition = shape.GetPart(38).ConvertToCvPoint(),
                RightEyePosition = shape.GetPart(43).ConvertToCvPoint(),
                LeftEarPosition = shape.GetPart(0).ConvertToCvPoint(),
                RightEarPosition = shape.GetPart(16).ConvertToCvPoint(),
                LeftCheekPosition = shape.GetPart(1).ConvertToCvPoint(),
                RightCheekPosition = shape.GetPart(15).ConvertToCvPoint()
            };

            return faceInfo;
        }
    }
}
