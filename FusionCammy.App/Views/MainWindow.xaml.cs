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

            var camView = App.Services.GetRequiredService<CamView>();
            MainContentArea.Content = camView;

            var decoSelectionView = App.Services.GetRequiredService<FunctionView>();
            SideContentArea.Content = decoSelectionView;
        }
    }
}