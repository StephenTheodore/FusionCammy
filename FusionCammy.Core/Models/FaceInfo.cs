using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace FusionCammy.Core.Models
{
    public partial class FaceInfo : ObservableObject
    {
        [ObservableProperty]
        private Rect bounds;

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
