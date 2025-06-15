using OpenCvSharp;

namespace FusionCammy.Core.Managers
{
    public class AssetManager
    {
        private readonly Dictionary<string, Mat> images = [];

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void RegisterImage (string key, string imagePath)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(imagePath))
                throw new ArgumentException("Key or ImagePath cannot be null or empty.");

            Mat image = Cv2.ImRead(imagePath, ImreadModes.Unchanged);
            if (image.Empty())
                throw new FileNotFoundException($"Image not found at path: {imagePath}");

            images[key] = image;
        }

        public Mat GetImage(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");

            if (images.TryGetValue(key, out Mat? image))
                return image;
            else
                throw new KeyNotFoundException($"No image found for key: {key}");
        }
    }
}
