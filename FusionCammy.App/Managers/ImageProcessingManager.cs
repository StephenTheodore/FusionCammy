using FusionCammy.Core.Managers;
using FusionCammy.Core.Models;
using FusionCammy.Core.Services;
using OpenCvSharp;
using System.Windows.Controls;
using DirectShowLib;
using System.Runtime.InteropServices;

namespace FusionCammy.App.Managers
{
    public class ImageProcessingManager(OpenCvAcquisitionService acquisitionService, FacialAnalysisService facialAnalysisService, DecorationService decorationService)
    {
        #region Field
        private readonly List<CameraInfo> _cameraInfos = [];
        #endregion

        #region Property
        public IReadOnlyList<CameraInfo> Cameras => _cameraInfos;

        public bool IsLive => acquisitionService.IsLive;
        #endregion

        #region Method
        public void Initialize()
        {
            DsDevice[] devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            ICaptureGraphBuilder2 graphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            IFilterGraph2 filterGraph = (IFilterGraph2)new FilterGraph();
            graphBuilder.SetFiltergraph(filterGraph);

            if (devices.Length == 0)
            {
                // 카메라가 없을 때
                throw new InvalidOperationException("No video input devices found.");
            }

            foreach (var device in devices)
            {
                if (!device.DevicePath.Contains("usb", StringComparison.CurrentCultureIgnoreCase) && !device.Name.Contains("usb", StringComparison.CurrentCultureIgnoreCase))
                    continue;

                filterGraph.AddSourceFilterForMoniker(device.Mon, null, device.Name, out IBaseFilter sourceFilter);

                // IAMStreamConfig
                Guid riid = typeof(IAMStreamConfig).GUID;
                int hr = graphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, sourceFilter, riid, out object config);
                DsError.ThrowExceptionForHR(hr);

                if (config is IAMStreamConfig streamConfig)
                {
                    streamConfig.GetNumberOfCapabilities(out _, out int size);
                    IntPtr ptr = Marshal.AllocCoTaskMem(size);

                    AMMediaType mediaType;
                    streamConfig.GetStreamCaps(0, out mediaType, ptr); // 첫 번째 해상도

                    var videoInfo = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader))!;
                    int width = videoInfo.BmiHeader.Width;
                    int height = videoInfo.BmiHeader.Height;

                    Marshal.FreeCoTaskMem(ptr);
                    DsUtils.FreeAMMediaType(mediaType);

                    // USB 카메라 1개만 사용
                    acquisitionService.CameraInfo = new CameraInfo(0, device.Name, width, height);
                    _cameraInfos.Add(acquisitionService.CameraInfo);
                    break;
                }
            }
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
