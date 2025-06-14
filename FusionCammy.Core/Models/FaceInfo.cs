using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace FusionCammy.Core.Models
{
    public partial class FaceInfo : ObservableObject
    {
        [ObservableProperty]
        private Rect bounds;

        public Dictionary<FacePartType, List<Point>> Anchors { get; set; } = [];
    }
}
