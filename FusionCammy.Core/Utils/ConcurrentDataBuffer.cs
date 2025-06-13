using System.Collections.Concurrent;

namespace FusionCammy.Core.Utils
{
    public class ConcurrentDataBuffer<T> where T : class, IDisposable
    {
        #region Field  
        private readonly ConcurrentQueue<T> _queue = new();
        #endregion

        #region Property
        public int Capacity { get; set; } = 3;
        #endregion

        #region Method  
        public bool Put(T data)
        {
            if (_queue.Count >= Capacity)
                _ = Get();

            _queue.Enqueue(data);
            return true;
        }

        public bool Put(IEnumerable<T> datas)
        {
            foreach (T data in datas)
                Put(data);

            return true;
        }

        public T? Get()
        {
            _queue.TryDequeue(out T? datas);
            return datas;
        }

        public void Flush()
        {
            while (Get() is T data)
                data.Dispose();
        }
        #endregion
    }
}
