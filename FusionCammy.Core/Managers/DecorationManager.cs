using FusionCammy.Core.Models;
using System.ComponentModel;

namespace FusionCammy.Core.Managers
{
    public class DecorationManager
    {
        #region Field
        private readonly Dictionary<string, DecorationInfo> _decorations = [];

        private readonly HashSet<DecorationInfo> _selectedDecorations = [];
        #endregion

        #region Property
        public IReadOnlyCollection<DecorationInfo> Decorations => _decorations.Values;

        public IReadOnlyCollection<DecorationInfo> SelectedDecorations => _selectedDecorations;
        #endregion

        #region Method
        public void Put(DecorationInfo decorationInfo)
        {
            _decorations[decorationInfo.Id] = decorationInfo;

            decorationInfo.PropertyChanged -= OnDecorationPropertyChanged;
            decorationInfo.PropertyChanged += OnDecorationPropertyChanged;

            if (decorationInfo.IsSelected)
                _selectedDecorations.Add(decorationInfo);
            else
                _selectedDecorations.Remove(decorationInfo);
        }

        public void Put(string id, string? name, string? previewPath, FacePartType facePartType, DecorationColor color, double scaleX, double scaleY, bool isSelected = false)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Decoration ID cannot be null or empty.");

            var decorationInfo = new DecorationInfo
            {
                Name = name,
                PreviewImagePath = previewPath,
                Id = id,
                FacePartType = facePartType,
                Color = color,
                ScaleX = scaleX,
                ScaleY = scaleY,
                IsSelected = isSelected,
            };

            Put(decorationInfo);
        }

        public DecorationInfo Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Key cannot be null or empty.");

            if (_decorations.TryGetValue(id, out DecorationInfo? info))
                return info;
            else
                throw new KeyNotFoundException($"No decoration found for {id}");
        }


        private void OnDecorationPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not DecorationInfo decorationInfo || e.PropertyName != nameof(DecorationInfo.IsSelected))
                return;

            if (decorationInfo.IsSelected)
                _selectedDecorations.Add(decorationInfo);
            else
                _selectedDecorations.Remove(decorationInfo);
        }
        #endregion
    }
}
