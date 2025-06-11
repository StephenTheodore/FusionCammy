using FusionCammy.Core.Models;

namespace FusionCammy.Core.Decorations
{
    public abstract class DecorationBase(string id, DecorationColor color, VisualLayout visualLayout) : IDecoration
    {
        public string Id { get; protected set; } = id;

        public DecorationColor Color { get; protected set; } = color;

        public VisualLayout VisualLayout { get; protected set; } = visualLayout;

        public abstract void UpdateLayout(FaceInfo faceInfo);
    }
}
