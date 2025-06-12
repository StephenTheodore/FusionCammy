using FusionCammy.Core.Decorations;
using System.Collections.ObjectModel;

namespace FusionCammy.Core.Managers
{
    public class DecorationManager
    {
        ObservableCollection<IDecoration> Decorations { get; } = [];

        public void Initialize()
        {
        }
    }
}
