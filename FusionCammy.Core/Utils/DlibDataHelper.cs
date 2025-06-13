using DlibDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionCammy.Core.Utils
{
    public static class DlibDataHelper
    {
        public static OpenCvSharp.Point ConvertToCvPoint(this FullObjectDetection detection, uint index)
        {
            return detection.GetPart(index).ConvertToCvPoint();
        }

        public static OpenCvSharp.Point ConvertToCvPoint(this Point point)
        {
            return new OpenCvSharp.Point(point.X, point.Y);
        }

        public static OpenCvSharp.Rect ConvertToCvRect(this Rectangle rect)
        {
            return new OpenCvSharp.Rect(rect.Left, rect.Top, (int)rect.Width, (int)rect.Height);
        }
    }
}
