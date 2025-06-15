using FusionCammy.Core.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.IO;
using System.Windows.Media.Imaging;

namespace FusionCammy.App.Managers
{
    public class ImageTransferManager(ImageProcessingManager imageProcessingManager)
    {
        // TODO : Gallery 기능 추가 시 역할 확장

        private string _lastLoadedImagePath = string.Empty;

        public WriteableBitmap LoadBitmap(string path, bool withProcessing)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                throw new FileNotFoundException($"Image file not found: {path}");

            using var mat = Cv2.ImRead(path, ImreadModes.Unchanged);

            if (mat.Empty())
                throw new InvalidDataException("Failed to load image or image is empty.");

            if (withProcessing && imageProcessingManager.ProcessImageAsync(mat).Result is ProcessedFrame processedFrame)
                return processedFrame.Image.ToWriteableBitmap();
            else if (mat.Channels() == 3 || mat.Channels() == 4)
                return mat.ToWriteableBitmap();
            else
                throw new NotSupportedException($"Unsupported channel count: {mat.Channels()}");
        }

        public WriteableBitmap? LoadBitmapFromDialog(bool withProcessing)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Select an Image"
            };

            if (dialog.ShowDialog() == true)
            {
                _lastLoadedImagePath = dialog.FileName;
                return LoadBitmap(dialog.FileName, withProcessing);
            }
            else
                return null;
        }

        public WriteableBitmap? LoadLastImage(bool withProcessing)
        {
            if (string.IsNullOrEmpty(_lastLoadedImagePath) || !File.Exists(_lastLoadedImagePath))
                return null;

            return LoadBitmap(_lastLoadedImagePath, withProcessing);
        }
    }
}
