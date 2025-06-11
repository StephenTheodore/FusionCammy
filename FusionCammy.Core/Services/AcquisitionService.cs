using FusionCammy.Core.Models;
using FusionCammy.Core.Utils;
using OpenCvSharp;
using System.Runtime.InteropServices;

namespace FusionCammy.Core.Services
{
    public class AcquisitionService
    {
        #region Field
        private readonly VideoCapture _videoCapture = new();

        private readonly ConcurrentDataBuffer<byte> _buffer = new();

        private CancellationTokenSource? _taskCancellation;

        private Task? _acquisitionTask;
        #endregion

        #region Property
        public int TargetFrameRate { get; set; } = 30;
        #endregion

        #region Method
        public void Start(CameraInfo cameraInfo)
        {
            _videoCapture.Open(cameraInfo.Index, VideoCaptureAPIs.ANY);
            _taskCancellation = new CancellationTokenSource();
            _acquisitionTask = CaptureLoopAsync(_taskCancellation.Token);
        }

        public void Stop()
        {
            if (_taskCancellation is not null)
            {
                _taskCancellation.Cancel();
                _taskCancellation.Dispose();
            }

            if (_acquisitionTask is not null)
            {
                try
                {
                    _acquisitionTask.Wait();
                }
                catch (OperationCanceledException)
                {
                    // TODO : 취소 시 동작
                }
            }

            _buffer.Flush();
        }

        private async Task CaptureLoopAsync(CancellationToken token)
        {
            var frame = new Mat();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();

                    if (_videoCapture.Read(frame) && frame.Data != IntPtr.Zero)
                    {
                        var dataSize = (int)(frame.Total() * frame.ElemSize());
                        byte[] data = new byte[dataSize];
                        Marshal.Copy(frame.Data, data, 0, dataSize);
                        _buffer.Put(data);
                    }

                    await Task.Delay(1000 / TargetFrameRate, token).ConfigureAwait(false);
                }
            }
            finally
            {
                frame.Dispose();
                _videoCapture.Release();
            }
        }

        public bool TryGetFrameDatas(out byte[] frame)
        {
            if (_buffer.Get() is IEnumerable<byte> data)
            {
                frame = data.ToArray();
                return true;
            }
            else
            {
                frame = default;
                return false;
            }
        }
        #endregion
    }
}
