namespace FusionCammy.Core.Models
{
    public class CameraInfo(string name, int width, int height)
    {
        public string Name { get; set; } = name;

        public bool IsOpened { get; set; }

        public bool IsStreaming { get; set; }

        public int Width { get; set; } = width;

        public int Height { get; set; } = height;
    }
}
