using FusionCammy.Core.Managers;
using FusionCammy.Core.Models;
using FusionCammy.Core.Services;
using OpenCvSharp;
using System.Windows.Controls;

namespace FusionCammy.App.Managers
{
    // TODO : CameraManager 역할 추가에 따른 이름 변경 고려
    public class ImageProcessingManager(OpenCvAcquisitionService acquisitionService, FacialAnalysisService facialAnalysisService, DecorationService decorationService)
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
            // TODO : WebCam 역할 관리 재정립 필오

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

        [Obsolete("Move to ImageStateManager")]
        public async Task<ProcessedFrame?> TryGetFrameDataAsync()
        {
            if (!acquisitionService.TryGetFrameData(out Mat? frameData) || frameData is null)
                return null;

            return await ProcessImageAsync(frameData);
        }

        public async Task<ProcessedFrame?> ProcessImageAsync(Mat orgImage)
        {
            var processedFrame = new ProcessedFrame(orgImage);
            var faceInfos = await facialAnalysisService.AnalyzeAsync(orgImage);

            foreach (var faceInfo in faceInfos)
            {
                decorationService.Decorate(orgImage, faceInfo);
                processedFrame.Put(faceInfo);
            }

            acquisitionService.SetCaptureConfigurations(faceInfos.Count > 0);

            return processedFrame;
        }
        #endregion
    }
}
