using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FusionCammy.App.Managers;
using FusionCammy.Core.Managers;
using FusionCammy.Core.Models;

namespace FusionCammy.App.ViewModels
{
    public partial class FunctionViewModel(ImageProcessingManager imageProcessingManager, AssetManager assetManager, DecorationManager decorationManager) : ObservableObject
    {
        #region Property
        public IReadOnlyCollection<DecorationInfo> Decorations => decorationManager.Decorations;
        #endregion

        #region Method
        [RelayCommand]
        private void StartCamera()
        {
            imageProcessingManager.StartLive();
        }

        [RelayCommand]
        private void StopCamera()
        {
            imageProcessingManager.StopLive();
        }

        [RelayCommand]
        private void SaveImage() // -> 아래 TODO 하면서 이름 변경
        {
            // TODO : 시간차 촬상 후 갤러리에 저장
        }
        #endregion
    }
}
