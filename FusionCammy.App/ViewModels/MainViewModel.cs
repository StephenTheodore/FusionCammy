using CommunityToolkit.Mvvm.ComponentModel;
using FusionCammy.App.Utils;
using FusionCammy.Core.Managers;
using System.Windows.Media;
using System.Windows.Threading;

namespace FusionCammy.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly CameraManager cameraManager;

        private readonly DispatcherTimer _pollTimer;

        [ObservableProperty]
        private ImageSource imageSource;

        public MainViewModel(CameraManager cameraManager)
        {
            this.cameraManager = cameraManager;

            _pollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _pollTimer.Tick += OnTick;
            _pollTimer.Start();

            // 테스트 코드
            cameraManager.Initialize();
            cameraManager.ChangeSelectedCamera(0);
            cameraManager.Start();
            //
        }

        private void OnTick(object? sender, EventArgs e)
        {
            if (cameraManager.TryGetFrameData(out byte[] frameData))
            {
                ImageSource = ImageHelper.ConvertBgrRawDataToBitmap(frameData, 640, 480);
            }
        }
    }
}
