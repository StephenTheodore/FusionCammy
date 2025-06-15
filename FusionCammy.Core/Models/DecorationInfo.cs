using CommunityToolkit.Mvvm.ComponentModel;

namespace FusionCammy.Core.Models
{
    public partial class DecorationInfo : ObservableObject
    {
        #region Property - Bindable
        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private string? name;

        [ObservableProperty]
        private string? previewImagePath;
        #endregion

        #region Property
        public string Id { get; set; } = null!;

        public FacePartType FacePartType { get; set; }

        public DecorationColor Color { get; set; }

        public double ScaleX { get; set; } = 1d;

        public double ScaleY { get; set; } = 1d;
        #endregion
    }
}
