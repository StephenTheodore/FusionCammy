using CommunityToolkit.Mvvm.ComponentModel;
using FusionCammy.Core.Managers;
using FusionCammy.Core.Models;

namespace FusionCammy.App.ViewModels
{
    public partial class FunctionViewModel(DecorationManager decorationManager) : ObservableObject
    {
        #region Property
        public IReadOnlyCollection<DecorationInfo> Decorations => decorationManager.Decorations;
        #endregion
    }
}
