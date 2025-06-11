using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace FusionCammy.Core.Models
{
    public partial class VisualLayout : ObservableObject
    {
        #region 속성 - ObservableProperty
        [ObservableProperty]
        private int width;

        [ObservableProperty]
        private int height;

        [ObservableProperty]
        private int mainAnchorX;

        [ObservableProperty]
        private int mainAnchorY;

        [ObservableProperty]
        private int? subAnchorX;

        [ObservableProperty]
        private int? subAnchorY;

        [ObservableProperty]
        private double rotation;
        #endregion

        #region 속성 - Helper
        public Size Size
        {
            get => new(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Point MainAnchor
        {
            get => new(MainAnchorX, MainAnchorY);
            set
            {
                MainAnchorX = value.X;
                MainAnchorY = value.Y;
            }
        }

        public Point? SubAnchor
        {
            get => SubAnchorX.HasValue && SubAnchorY.HasValue ? new Point(SubAnchorX.Value, SubAnchorY.Value) : null;
            set
            {
                if (value.HasValue)
                {
                    SubAnchorX = value.Value.X;
                    SubAnchorY = value.Value.Y;
                }
                else
                {
                    SubAnchorX = null;
                    SubAnchorY = null;
                }
            }
        }
        #endregion
    }
}
