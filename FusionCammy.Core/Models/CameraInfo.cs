using CommunityToolkit.Mvvm.ComponentModel;

namespace FusionCammy.Core.Models
{
    public partial class CameraInfo(string name, int width, int height) : ObservableObject
    {
        [ObservableProperty]
        private string name = name;

        [ObservableProperty]
        private bool isOpened;

        [ObservableProperty]
        private bool isStreaming;

        [ObservableProperty]
        private int width = width;

        [ObservableProperty]
        private int height = height;
    }
}