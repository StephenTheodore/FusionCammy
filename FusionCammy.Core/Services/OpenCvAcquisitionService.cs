﻿using FusionCammy.Core.Models;
using FusionCammy.Core.Utils;
using OpenCvSharp;
using System.Diagnostics;

namespace FusionCammy.Core.Services
{
    public class OpenCvAcquisitionService
    {
        #region Field
        private readonly VideoCapture _videoCapture = new();

        private readonly ConcurrentDataBuffer<Mat> _liveImageBuffer = new();

        private CancellationTokenSource? _taskCancellation;

        private Task? _acquisitionTask;

        private bool isAutoOptionEnabled = true;
        #endregion

        #region Property
        public CameraInfo CameraInfo { get; set; } = null!;

        public int TargetFrameRate { get; set; } = 30;

        public bool IsLive => CameraInfo.IsStreaming;
        #endregion

        #region Event
        public event EventHandler<Mat>? OnFrameCaptured;
        #endregion

        #region Method
        public async Task StartLive()
        {
            if (_videoCapture.IsOpened())
                return;

            bool openSuccess = _videoCapture.Open(CameraInfo.Index, VideoCaptureAPIs.MSMF)
                || _videoCapture.Open(CameraInfo.Index, VideoCaptureAPIs.DSHOW)
                || _videoCapture.Open(CameraInfo.Index, VideoCaptureAPIs.ANY);

            if (!openSuccess)
                throw new InvalidOperationException($"Failed to open camera at index {CameraInfo.Index}.");

            _videoCapture.Set(VideoCaptureProperties.FrameWidth, CameraInfo.Width);
            _videoCapture.Set(VideoCaptureProperties.FrameHeight, CameraInfo.Height);
            _taskCancellation = new CancellationTokenSource();
            _acquisitionTask = LiveLoopAsync(_taskCancellation.Token);

            CameraInfo.IsStreaming = true;
        }

        public async Task StopLive()
        {
            if (CameraInfo is null)
                throw new InvalidOperationException("연결된 카메라가 없어요");

            if (_taskCancellation is not null)
                await _taskCancellation.CancelAsync();

            if (_acquisitionTask is not null)
                await _acquisitionTask.ConfigureAwait(false);

            _taskCancellation?.Dispose();
            _acquisitionTask?.Dispose();
            _liveImageBuffer?.Flush();

            CameraInfo.IsStreaming = false;
        }

        public async Task TakeSingleFrame()
        {
            if (!_videoCapture.IsOpened())
                throw new InvalidOperationException("Camera is not opened.");
            else
                _videoCapture.Open(CameraInfo.Index, VideoCaptureAPIs.MSMF);

            using var frame = new Mat();

            if (_videoCapture.Read(frame) && frame.Data != nint.Zero)
                OnFrameCaptured?.Invoke(this, frame.Clone());

            _videoCapture.Release();
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
                        _liveImageBuffer.Put(frame.Flip(FlipMode.Y));

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

        public bool TryGetFrameData(out Mat? frame)
        {
            if (_liveImageBuffer.Get() is Mat data)
            {
                frame = data;
                return true;
            }
            else
            {
                frame = null;
                return false;
            }
        }

        public void SetCaptureConfigurations(bool isEnable)
        {
            if (isAutoOptionEnabled == isEnable)
                return;
            else
                isAutoOptionEnabled = isEnable;

            if (_videoCapture.IsOpened())
            {
                _videoCapture.Set(VideoCaptureProperties.AutoFocus, isAutoOptionEnabled ? 1 : 0);
                _videoCapture.Set(VideoCaptureProperties.AutoWB, isAutoOptionEnabled ? 1 : 0);
            }
        }
        #endregion
    }
}
