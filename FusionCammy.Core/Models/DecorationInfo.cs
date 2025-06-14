using CommunityToolkit.Mvvm.ComponentModel;

namespace FusionCammy.Core.Models
{
    public partial class DecorationInfo : ObservableObject
    {
        public string Id { get; set; } = null!;

        public FacePartType FacePartType { get; set; }

        public double scale { get; set; } = 1d;

        [ObservableProperty]
        private bool isSelected;
    }
}
