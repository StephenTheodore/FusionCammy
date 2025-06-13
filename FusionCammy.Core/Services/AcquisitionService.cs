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

        private readonly ConcurrentDataBuffer<Mat> _liveImageBuffer = new();

        private CancellationTokenSource? _taskCancellation;

        private Task? _acquisitionTask;
        #endregion

        #region Property
        public int TargetFrameRate { get; set; } = 30;
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

        private async Task LiveLoopAsync(CancellationToken token)
        {
            try
            {
                if (!_videoCapture.IsOpened())
                    throw new InvalidOperationException("Camera is not opened.");

                using var frame = new Mat();
                while (!token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();

                    var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // 측정 시작  

                    if (_videoCapture.Read(frame) && frame.Data != IntPtr.Zero)
                        _liveImageBuffer.Put(frame.Clone());

                    stopwatch.Stop(); // 측정 종료  


                    var acquireDelay = (int)Math.Max(5, (1000 / TargetFrameRate) - stopwatch.ElapsedMilliseconds);

                    System.Diagnostics.Debug.WriteLine($"Frame acquisition time: {stopwatch.ElapsedMilliseconds} ms, Delay {acquireDelay}");

                    await Task.Delay(acquireDelay, token).ConfigureAwait(false);
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
