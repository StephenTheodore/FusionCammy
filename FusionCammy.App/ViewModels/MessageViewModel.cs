using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FusionCammy.App.ViewModels
{
    public partial class MessageViewModel : ObservableObject
    {
        #region Field
        private readonly Action? _closingAction;
        #endregion

        #region Property
        [ObservableProperty]
        private string? message;
        #endregion

        #region Constructor
        public MessageViewModel(string message, Action? closingAction)
        {
            Message = message;
            _closingAction = closingAction;
        }
        #endregion

        #region Method
        [RelayCommand]
        private void CloseWindow()
        {
            _closingAction?.Invoke();
        }
        #endregion
    }
}
