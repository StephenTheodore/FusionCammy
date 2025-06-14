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

        public void Put(string id, FacePartType facePartType, double scale = 1d, bool isSelected = false)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Decoration ID cannot be null or empty.");

            var decorationInfo = new DecorationInfo
            {
                Id = id,
                FacePartType = facePartType,
                scale = scale,
                IsSelected = isSelected,
            };

            Put(decorationInfo);
        }

        public DecorationInfo Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");

            if (_decorations.TryGetValue(key, out DecorationInfo? info))
                return info;
            else
                throw new KeyNotFoundException($"No decoration found for key: {key}");
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
