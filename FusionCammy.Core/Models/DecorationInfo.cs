using CommunityToolkit.Mvvm.ComponentModel;

namespace FusionCammy.Core.Models
{
    public partial class DecorationInfo : ObservableObject
    {
        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private bool isVisible;
    }
}
