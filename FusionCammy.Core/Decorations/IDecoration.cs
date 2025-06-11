using FusionCammy.Core.Models;

namespace FusionCammy.Core.Decorations
{
    public interface IDecoration
    {
        string Id { get; }

        DecorationColor Color { get; }

        VisualLayout VisualLayout { get; }

        void UpdateLayout(FaceInfo faceInfo);
    }
}
