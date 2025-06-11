using FusionCammy.Core.Models;

namespace FusionCammy.Core.Decorations
{
    internal interface IDecoration
    {
        DecorationInfo DecorationInfo { get; }

        VisualLayout VisualLayout { get; }

        void UpdateLayout(FaceInfo faceInfo);
    }
}
