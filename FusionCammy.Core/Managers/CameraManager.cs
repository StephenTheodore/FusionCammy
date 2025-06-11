using FusionCammy.Core.Models;
using FusionCammy.Core.Services;
using OpenCvSharp;

namespace FusionCammy.Core.Managers
{
    public class CameraManager
    {
        #region Field
        private readonly List<CameraInfo> _cameraInfos = new();

        private readonly AcquisitionService _acquisitionService;

        private CameraInfo _selectedCameraInfo = null;
        #endregion

        #region Property
        public IReadOnlyList<CameraInfo> Cameras => _cameraInfos;
        #endregion

        #region Constructor
        public CameraManager(AcquisitionService acquisitionService)
        {
            _acquisitionService = acquisitionService;
        }
        #endregion

        #region Method
        public void Initialize()
        {
            // TODO : OpenCV 모든 카메라 정보 가져오기
            // TODO : 카메라가 하나도 없는 경우 예외처리

            var cam = new CameraInfo(0, "TempCam", 640, 480);
            _cameraInfos.Add(cam);
        }

        // TODO : ComboBox 커맨드랑 Binding
        public void ChangeSelectedCamera(int index)
        {
            if (Cameras.First(camera => camera.Index == index) is CameraInfo cameraInfo)
                _selectedCameraInfo = cameraInfo;
        }

        public void Start()
        {
            _acquisitionService.Start(_selectedCameraInfo);
        }

        public void Stop()
        {
            foreach (var cam in _cameraInfos)
                cam.IsStreaming = false;

            _acquisitionService.Stop();
        }

        public bool TryGetFrameData(out byte[] frameData)
        {
            return _acquisitionService.TryGetFrameDatas(out frameData);
        }
        #endregion
    }
}
