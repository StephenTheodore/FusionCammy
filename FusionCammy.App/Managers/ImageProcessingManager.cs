using FusionCammy.Core.Models;
using FusionCammy.Core.Services;
using OpenCvSharp;

namespace FusionCammy.App.Managers
{
    // TODO : CameraManager 역할 추가에 따른 이름 변경 고려
    public class ImageProcessingManager(AcquisitionService acquisitionService, FacialAnalysisService facialAnalysisService, DecorationService decorationService)
    {
        #region Field
        private readonly List<CameraInfo> _cameraInfos = [];
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
            if (!acquisitionService.TryGetFrameData(out Mat? frameData) || frameData is null)
                return null;

            var processedFrame = new ProcessedFrame(frameData);
            var faceInfo = await facialAnalysisService.AnalyzeAsync(frameData);

#if DEBUG
            frameData.Rectangle(faceInfo?.Bounds ?? new Rect(0, 0, 0, 0), Scalar.Red, 2);
            foreach (FacePartType facePart in Enum.GetValues(typeof(FacePartType)))
            {
                if(faceInfo?.Anchors is null || !faceInfo.Anchors.ContainsKey(facePart))
                    continue;

                foreach (var anchorPoint in faceInfo?.Anchors[facePart] ?? [])
                    frameData.Circle(anchorPoint, 3, Scalar.FromDouble(16 * (int)facePart), -1);
            }
#endif

            if (faceInfo is not null)
            {
                decorationService.Decorate(frameData, faceInfo);
                processedFrame.Put(faceInfo);
                acquisitionService.SetCaptureConfigurations(false);
            }
            else
                acquisitionService.SetCaptureConfigurations(true);

            return processedFrame;
        }
        #endregion
    }
}
