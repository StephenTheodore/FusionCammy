using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace FusionCammy.Core.Models
{
    public partial class FaceInfo : ObservableObject
    {
        [ObservableProperty]
        private Rect bounds;

        // TODO : 부위별 Single Anchor -> 다수 Point 로 변경 필요
        // TODO : 다수 Point 변경 후 회전 계산, 스케일 보정 추가
        [ObservableProperty]
        private Point nosePosition;

        [ObservableProperty]
        private Point mouthPosition;

        [ObservableProperty]
        private Point leftEyePosition;

        [ObservableProperty]
        private Point rightEyePosition;

        [ObservableProperty]
        private Point leftEarPosition;

        [ObservableProperty]
        private Point rightEarPosition;

        [ObservableProperty]
        private Point leftCheekPosition;

        [ObservableProperty]
        private Point rightCheekPosition;
    }
}
