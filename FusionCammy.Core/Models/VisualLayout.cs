using OpenCvSharp;

namespace FusionCammy.Core.Models
{
    public record VisualLayout
    {
        public Size Size { get; init; }

        public Point MainAnchor { get; init; }

        public Point? SubAnchor { get; init; }

        public double Rotation { get; init; }
    }
}
