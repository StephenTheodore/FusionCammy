using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FusionCammy.App.Managers;
using FusionCammy.Core.Managers;

namespace FusionCammy.App.ViewModels
{
    public partial class FunctionViewModel : ObservableObject
    {
        #region Field
        private readonly ImageProcessingManager _cameraManager;
        #endregion

        #region Constructor
        public FunctionViewModel(ImageProcessingManager cameraManager)
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
        private void SaveImage() // -> 아래 TODO 하면서 이름 변경
        {
            // TODO : 시간차 촬상 후 갤러리에 저장
        }
        #endregion
    }
}
