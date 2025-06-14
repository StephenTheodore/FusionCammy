using FusionCammy.App.Utils;
using FusionCammy.Core.Managers;
using FusionCammy.Core.Models;
using FusionCammy.Core.Services;
using OpenCvSharp;
using System.Windows.Media;

namespace FusionCammy.App.Managers
{
    // TODO : CameraManager 역할 추가에 따른 이름 변경 고려
    public class CameraManager(AcquisitionService acquisitionService, FacialAnalysisService facialAnalysisService, AssetManager assetManager)
    {
        #region Field
        private readonly List<CameraInfo> _cameraInfos = [];

        private readonly string testAssetName = $"{Colors.Red}_{DecorationType.Nose}_Ball";
        #endregion

        #region Property
        public IReadOnlyList<CameraInfo> Cameras => _cameraInfos;

        #endregion

        #region Method
        public void Initialize()
        {
            // TODO : OpenCV 모든 카메라 정보 가져오기
            // TODO : 카메라가 하나도 없는 경우 예외처리

            var cam = new CameraInfo(0, "TempCam", 640, 480);
            _cameraInfos.Add(cam);
        }

        // TODO : ComboBox 커맨드랑 Binding, 바꾸는 경우 라이브 관리 어떻게?
        public void ChangeSelectedCamera(int index)
        {
            if (Cameras.First(camera => camera.Index == index) is CameraInfo cameraInfo)
                acquisitionService.CameraInfo = cameraInfo;
        }

        public void StartLive()
        {
            if (acquisitionService.CameraInfo is not null)
                Task.Run(acquisitionService.StartLive);
        }

        public void StopLive()
        {
            foreach (var cam in _cameraInfos)
                cam.IsStreaming = false;

            Task.Run(acquisitionService.StopLive);
        }

        public async Task<ProcessedFrame?> TryGetFrameDataAsync()
        {
            if (!acquisitionService.TryGetFrameData(out Mat frameData))
                return null;

            var processedFrame = new ProcessedFrame(frameData);
            var faceInfo = await facialAnalysisService.AnalyzeAsync(frameData);

            frameData.Rectangle(faceInfo?.Bounds ?? new Rect(0, 0, 0, 0), Scalar.Red, 2);
            frameData.Circle(faceInfo?.NosePosition ?? new Point(0, 0), 2, Scalar.Blue, -1);
            frameData.Circle(faceInfo?.MouthPosition ?? new Point(0, 0), 2, Scalar.Green, -1);
            frameData.Circle(faceInfo?.LeftEyePosition ?? new Point(0, 0), 2, Scalar.Yellow, -1);
            frameData.Circle(faceInfo?.RightEyePosition ?? new Point(0, 0), 2, Scalar.Yellow, -1);
            frameData.Circle(faceInfo?.LeftEarPosition ?? new Point(0, 0), 2, Scalar.Purple, -1);
            frameData.Circle(faceInfo?.RightEarPosition ?? new Point(0, 0), 2, Scalar.Purple, -1);
            frameData.Circle(faceInfo?.LeftCheekPosition ?? new Point(0, 0), 2, Scalar.Cyan, -1);
            frameData.Circle(faceInfo?.RightCheekPosition ?? new Point(0, 0), 2, Scalar.Cyan, -1);
            
            var noseTestImage = assetManager.GetImage(testAssetName);
            frameData.OverlayDecorationWithAlpha(noseTestImage, faceInfo?.NosePosition ?? new Point(0, 0), new Size(60, 60));

            if (faceInfo is not null)
                processedFrame.Put(faceInfo);

            return processedFrame;
        }
        #endregion
    }
}
