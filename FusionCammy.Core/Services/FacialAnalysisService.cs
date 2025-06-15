using DlibDotNet;
using FusionCammy.Core.Configurations;
using FusionCammy.Core.Models;
using FusionCammy.Core.Utils;
using Microsoft.Extensions.Options;
using OpenCvSharp;
using System.Runtime.InteropServices;

namespace FusionCammy.Core.Services
{
    public class FacialAnalysisService(IOptions<MlConfiguration> options)
    {
        private readonly ShapePredictor _shapePredictor = ShapePredictor.Deserialize(options.Value.FacialDetectionModelPath);

        private readonly FrontalFaceDetector _faceDetector = Dlib.GetFrontalFaceDetector();

        /// <summary>Performs facial analysis on the provided image and returns face information if detected.</summary>
        public async Task<List<FaceInfo>> AnalyzeAsync(Mat image)
        {
            ArgumentNullException.ThrowIfNull(image);

            List<FaceInfo> faceInfos = [];

            byte[] dataArray = new byte[image.Width * image.Height * image.ElemSize()];
            Marshal.Copy(image.Data, dataArray, 0, dataArray.Length);

            using var dlibImage = Dlib.LoadImageData<BgrPixel>(dataArray, (uint)image.Height, (uint)image.Width, (uint)image.Step());

            var faceRects = _faceDetector.Operator(dlibImage);

            foreach (var faceRect in faceRects)
            {
                var shape = _shapePredictor.Detect(dlibImage, faceRect);
                var faceInfo = new FaceInfo
                {
                    Bounds = faceRect.ConvertToCvRect(),
                    Anchors = shape.ExtractLandmarksFrom68PointPredictor(),
                };
                faceInfos.Add(faceInfo);
            }

            return faceInfos;
        }
    }
}
