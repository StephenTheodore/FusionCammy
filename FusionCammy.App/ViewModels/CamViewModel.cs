using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FusionCammy.App.Managers;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FusionCammy.App.ViewModels
{
    public partial class CamViewModel : ObservableObject
    {
        #region Field
        private readonly ImageProcessingManager _imageProcessingManager;

        private readonly ImageTransferManager _imageStateManager;

        private readonly DispatcherTimer _pollTimer;
        #endregion

        #region Property
        [ObservableProperty]
        private ImageSource? imageSource;
        #endregion

        #region Constructor
        public CamViewModel(ImageProcessingManager imageProcessingManager, ImageTransferManager imageStoreManager)
        {
            _imageProcessingManager = imageProcessingManager;

            _imageStateManager = imageStoreManager;

            _pollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(8)
            };
            _pollTimer.Tick += OnTick;
            _pollTimer.Start();
        }
        #endregion

        #region Method
        [RelayCommand]
        private void StartCamera()
        {
            _imageProcessingManager.StartLive();
        }

        [RelayCommand]
        private void StopCamera()
        {
            _imageProcessingManager.StopLive();
        }

        [RelayCommand]
        private void LoadImage()
        {
            _imageProcessingManager.StopLive();

            if (_imageStateManager.LoadBitmapFromDialog(true) is WriteableBitmap bitmap)
            {
                bitmap.Freeze();
                ImageSource = bitmap;
            }
        }

        [RelayCommand]
        private void SaveImage()
        {
            // Logic to save an image
            // This could involve opening a save file dialog and saving the current ImageSource to a file
        }

        private async void OnTick(object? sender, EventArgs e)
        {
            var processedFrame = await _imageProcessingManager.TryGetFrameDataAsync();
            if (processedFrame is not null)
            {
                if (ImageSource is null ||
                    ImageSource.Width - processedFrame.Image.Width > double.Epsilon ||
                    ImageSource.Height - processedFrame.Image.Height > double.Epsilon)
                    ImageSource = new WriteableBitmap(processedFrame.Image.Width, processedFrame.Image.Height, 96, 96, PixelFormats.Bgr24, null);

                WriteableBitmapConverter.ToWriteableBitmap(processedFrame.Image, (WriteableBitmap)ImageSource);    // TODO : 여기서 하지않고 CameraManager쪽으로 이동
            }
        }
        #endregion
    }
}
