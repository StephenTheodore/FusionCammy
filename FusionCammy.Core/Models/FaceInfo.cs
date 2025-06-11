using OpenCvSharp;

namespace FusionCammy.Core.Models
{
    public class FaceInfo
    {
        public Rect Bounds { get; set; }

        public Point NosePosition { get; set; }

        public Point MouthPosition { get; set; }

        public Point LeftEyePosition { get; set; }

        public Point RightEyePosition { get; set; }

        public Point LeftEarPosition { get; set; }

        public Point RightEarPosition { get; set; }

        public Point LeftCheekPosition { get; set; }

        public Point RightCheekPosition { get; set; }
    }
}
