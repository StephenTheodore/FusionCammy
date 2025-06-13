using FusionCammy.Core.Models;
using FusionCammy.Core.Utils;
using OpenCvSharp;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FusionCammy.Core.Services
{
    public class AcquisitionService
    {
        #region Field
        private readonly VideoCapture _videoCapture = new();

        private readonly ConcurrentDataBuffer<Mat> _liveImageBuffer = new();

        private CancellationTokenSource? _taskCancellation;

        private Task? _acquisitionTask;
        #endregion

        #region Property
        public int TargetFrameRate { get; set; } = 30;
        #endregion

        #region Event
        public event EventHandler<Mat>? OnFrameCaptured;
        #endregion

        #region Method
        public void StartLive(CameraInfo cameraInfo)
        {
            _videoCapture.Open(cameraInfo.Index, VideoCaptureAPIs.MSMF);
            _taskCancellation = new CancellationTokenSource();
            _acquisitionTask = LiveLoopAsync(_taskCancellation.Token);
        }

        public void StopLive()
        {
            _taskCancellation?.Cancel();
            _taskCancellation?.Dispose();
            _acquisitionTask?.Wait();
            _liveImageBuffer?.Flush();
        }

        public async Task TakeSingleFrame(CameraInfo cameraInfo)
        {
            if (!_videoCapture.IsOpened())
                throw new InvalidOperationException("Camera is not opened.");

            using var frame = new Mat();

            if (_videoCapture.Read(frame) && frame.Data != nint.Zero)
                OnFrameCaptured?.Invoke(this, frame.Clone());

            await Task.CompletedTask;
        }

        private async Task LiveLoopAsync(CancellationToken token)
        {
            try
            {
                if (!_videoCapture.IsOpened())
                    throw new InvalidOperationException("Camera is not opened.");

                using var frame = new Mat();
                var frameRate = TimeSpan.FromMilliseconds(1000d / TargetFrameRate);
                var stopwatch = Stopwatch.StartNew();

                while (!token.IsCancellationRequested)
                {
                    var processStart = stopwatch.Elapsed;

                    token.ThrowIfCancellationRequested();

                    if (_videoCapture.Read(frame) && frame.Data != IntPtr.Zero)
                        _liveImageBuffer.Put(frame.Clone());

                    var processEnd = stopwatch.Elapsed;
                    var elapsed = processEnd - processStart;
                    var delay = frameRate - elapsed;
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // 취소 요청은 스킵, 로그만?  
            }
            finally
            {
                _videoCapture.Release();
            }
        }

        public bool TryGetFrameData(out Mat frame)
        {
            if (_liveImageBuffer.Get() is Mat data)
            {
                frame = data;
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
