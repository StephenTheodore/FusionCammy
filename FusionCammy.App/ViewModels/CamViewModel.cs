using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FusionCammy.App.Utils;
using FusionCammy.Core.Managers;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FusionCammy.App.ViewModels
{
    public partial class CamViewModel : ObservableObject
    {
        #region Field
        private readonly CameraManager _cameraManager;

        private readonly DispatcherTimer _pollTimer;

        [ObservableProperty]
        private ImageSource? imageSource;
        #endregion

        #region Constructor
        public CamViewModel(CameraManager cameraManager)
        {
            _cameraManager = cameraManager;

            _pollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(8)
            };
            _pollTimer.Tick += OnTick;
            _pollTimer.Start();

            // 테스트 코드
            _cameraManager.Initialize();
            _cameraManager.ChangeSelectedCamera(0);
        }
        #endregion

        #region Method
        [RelayCommand]
        private void StartCamera()
        {
            _cameraManager.StartLive();
        }

        [RelayCommand]
        private void StopCamera()
        {
            _cameraManager.StopLive();
        }

        [RelayCommand]
        private void LoadImage()
        {
            // Logic to load an image
            // This could involve opening a file dialog and loading an image into the ImageSource
        }

        [RelayCommand]
        private void SaveImage()
        {
            // Logic to save an image
            // This could involve opening a save file dialog and saving the current ImageSource to a file
        }

        private void OnTick(object? sender, EventArgs e)
        {
            if (_cameraManager.TryGetFrameData(out Mat frameData))
            {
                if (ImageSource is null ||
                    ImageSource.Width - frameData.Width > double.Epsilon ||
                    ImageSource.Height - frameData.Height > double.Epsilon)
                    ImageSource = new WriteableBitmap(frameData.Width, frameData.Height, 96, 96, PixelFormats.Bgr24, null);

                WriteableBitmapConverter.ToWriteableBitmap(frameData, (WriteableBitmap)ImageSource);
            }
        }
        #endregion
    }
}
