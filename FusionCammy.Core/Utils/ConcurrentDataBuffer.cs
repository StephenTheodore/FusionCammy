using System.Collections.Concurrent;

namespace FusionCammy.Core.Utils
{
    public class ConcurrentDataBuffer<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        #region Field  
        private readonly ConcurrentQueue<IEnumerable<T>> _queue = new();
        #endregion

        #region Property
        public int Capacity { get; set; } = 3;
        #endregion

        #region Method  
        public bool Put(IEnumerable<T> datas)
        {
            if (_queue.Count >= Capacity)
                _ = Get();

            _queue.Enqueue(datas);
            return true;
        }

        public IEnumerable<T>? Get()
        {
            _queue.TryDequeue(out IEnumerable<T>? datas);
            return datas;
        }

        public void Flush()
        {
            while (Get() is not null) { }
        }
        #endregion
    }
}
