using FusionCammy.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FusionCammy.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var mainView = App.Services.GetRequiredService<MainView>();
            MainFrame.Navigate(mainView);
        }
    }
}