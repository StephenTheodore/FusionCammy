using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FusionCammy.App.Managers;
using FusionCammy.App.Views;
using FusionCammy.Core.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FusionCammy.App.ViewModels
{
    public partial class CamViewModel : ObservableObject
    {
        #region Field
        private readonly ImageProcessingManager _imageProcessingManager;

        private readonly ImageTransferManager _imageTransferManager;

        private readonly DispatcherTimer _pollTimer;

        private readonly string _directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"FusionCammy");
        #endregion

        #region Property
        [ObservableProperty]
        private ImageSource? imageSource;

        [ObservableProperty]
        private Visibility countdownVisibility = Visibility.Hidden;

        [ObservableProperty]
        private int countdownSeconds;
        #endregion

        #region Constructor
        public CamViewModel(ImageProcessingManager imageProcessingManager, ImageTransferManager imageStoreManager, FunctionViewModel functionViewModel)
        {
            _imageProcessingManager = imageProcessingManager;
            _imageTransferManager = imageStoreManager;

            foreach (var decoration in functionViewModel.Decorations)
            {
                decoration.PropertyChanged -= OnDecorationPropertyChanged;
                decoration.PropertyChanged += OnDecorationPropertyChanged;
            }

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

            if (_imageTransferManager.LoadBitmapFromDialog(true) is WriteableBitmap bitmap)
            {
                bitmap.Freeze();
                ImageSource = bitmap;
            }
        }

        [RelayCommand]
        private async Task TimedCapture()
        {
            if (!_imageProcessingManager.IsLive)
            {
                _imageProcessingManager.StartLive();

                var timeout = TimeSpan.FromSeconds(30);
                var startTime = DateTime.Now;
                while (!_imageProcessingManager.IsLive && DateTime.Now - startTime < timeout)
                {
                    await Task.Delay(100);
                }

                if (!_imageProcessingManager.IsLive)
                {
                    MessageBox.Show("카메라를 시작할 수 없습니다. 다시 시도하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            CountdownVisibility = Visibility.Visible;

            CountdownSeconds = 10;
            while (CountdownSeconds-- > 0)
            {
                CountdownVisibility = CountdownSeconds == 0 ? Visibility.Hidden : Visibility.Visible;
                await Task.Delay(1000);
            }

            var processedFrame = await _imageProcessingManager.TryGetFrameDataAsync();
            if (processedFrame is not null)
            {
                SaveCapturedImage(processedFrame);
                SetImageSource(processedFrame);
                new MessageWindow($"사진을 찍었어요!\r\n저장 위치 열기를 눌러서 확인해보세요!").ShowDialog();
            }
        }

        private void SaveCapturedImage(ProcessedFrame processedFrame)
        {
            if (processedFrame.Image.Empty())
                return;

            string imageFilePath = Path.Combine(_directoryPath, $"FusionCammy_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            Cv2.ImWrite(imageFilePath, processedFrame.Image);
        }

        [RelayCommand]
        private void OpenDirectory()
        {
            try
            {
                if (!Directory.Exists(_directoryPath))
                    Directory.CreateDirectory(_directoryPath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _directoryPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"폴더를 열 수 없습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void SaveImage()
        {
            if (_imageTransferManager.SaveLastProcessedImage(_directoryPath))
                new MessageWindow($"마지막 미리보기를 저장했어요!\r\n저장 위치 열기를 눌러서 확인해보세요!").ShowDialog();
            else
                new MessageWindow($"결과 미리보기가 없어요...\r\n이미지를 먼저 불러와 주세요!").ShowDialog();
        }

        private void OnDecorationPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DecorationInfo.IsSelected))
            {
                if (_imageProcessingManager.IsLive)
                    return;
                if (_imageTransferManager.LoadLastImage(true) is WriteableBitmap bitmap)
                {
                    bitmap.Freeze();
                    ImageSource = bitmap;
                }
            }
        }

        private async void OnTick(object? sender, EventArgs e)
        {
            var processedFrame = await _imageProcessingManager.TryGetFrameDataAsync();
            if (processedFrame is not null)
                SetImageSource(processedFrame);
        }

        private void SetImageSource(ProcessedFrame processedFrame)
        {
            if (ImageSource is null ||
                ImageSource.Width - processedFrame.Image.Width > double.Epsilon ||
                ImageSource.Height - processedFrame.Image.Height > double.Epsilon)
                ImageSource = new WriteableBitmap(processedFrame.Image.Width, processedFrame.Image.Height, 96, 96, PixelFormats.Bgr24, null);

            WriteableBitmapConverter.ToWriteableBitmap(processedFrame.Image, (WriteableBitmap)ImageSource);    // TODO : 병목 시 CameraManager쪽으로 이동
        }
        #endregion
    }
}
