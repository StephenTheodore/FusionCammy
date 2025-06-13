using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FusionCammy.App.Managers;
using FusionCammy.Core.Managers;

namespace FusionCammy.App.ViewModels
{
    public partial class FunctionViewModel : ObservableObject
    {
        #region Field
        private readonly CameraManager _cameraManager;
        #endregion

        #region Constructor
        public FunctionViewModel(CameraManager cameraManager)
        {
            _cameraManager = cameraManager;
        }
        #endregion

        #region Method
        [RelayCommand]
        private void StartCamera()
        {
            _cameraManager.StartLive();
        }

        [RelayCommand]
        private void StopCamera()
        {
            _cameraManager.StopLive();
        }

        [RelayCommand]
        private void LoadImage()
        {
            // TODO : 이미지 로딩
        }

        [RelayCommand]
        private void SaveImage()
        {
            // TODO : 이미지 저장
            // TODO : 시간차 촬상 기능으로 변경 검토
        }
        #endregion
    }
}
