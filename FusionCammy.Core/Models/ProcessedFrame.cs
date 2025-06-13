using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace FusionCammy.Core.Models
{
    public class ProcessedFrame(Mat image) : IDisposable
    {
        private bool disposedValue;
        #region Property
        public Mat Image { get; private set; } = image;

        public Dictionary<Type, ObservableObject> Datas { get; private set; } = [];
        #endregion

        #region Method
        public void Put<T>(T data) where T : ObservableObject
        {
            Datas[typeof(T)] = data;
        }

        public T Get<T>() where T : ObservableObject
        {
            if (Datas.TryGetValue(typeof(T), out var value) && value is T data)
                return data;
            else
                throw new KeyNotFoundException($"Data of type {typeof(T).Name} not found in the processed frame.");
        }

        public bool TryGet<T>(out T? result) where T : ObservableObject
        {
            if (Datas.TryGetValue(typeof(T), out var value) && value is T data)
            {
                result = data;
                return true;
            }

            result = null;
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Image.Dispose();
                    Image = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
