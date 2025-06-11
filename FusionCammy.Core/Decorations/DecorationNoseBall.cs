using FusionCammy.Core.Models;

namespace FusionCammy.Core.Decorations
{
    public class DecorationNoseBall(string id, DecorationColor color, VisualLayout visualLayout) : DecorationBase(id, color, visualLayout)
    {
        public override void UpdateLayout(FaceInfo faceInfo)
        {
            if (faceInfo is not null)
            {
                VisualLayout.MainAnchorX = faceInfo.NosePosition.X - (VisualLayout.Width / 2);
                VisualLayout.MainAnchorY = faceInfo.NosePosition.Y - (VisualLayout.Height / 2);
            }
        }
    }
}
