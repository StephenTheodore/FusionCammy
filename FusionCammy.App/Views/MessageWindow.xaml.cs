using FusionCammy.App.ViewModels;
using System.Windows;

namespace FusionCammy.App.Views
{
    /// <summary>
    /// MessageWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageWindow : Window
    {
        #region Constructor
        public MessageWindow(string message)
        {
            InitializeComponent();
            if (Application.Current.MainWindow != this)
                Owner = Application.Current.MainWindow;
            DataContext = new MessageViewModel(message, Close);
        }
        #endregion
    }
}
