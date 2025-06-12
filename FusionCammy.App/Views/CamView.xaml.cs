using FusionCammy.App.ViewModels;
using System.Windows.Controls;

namespace FusionCammy.App.Views
{
    /// <summary>
    /// CamView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CamView : UserControl
    {
        public CamView(CamViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
