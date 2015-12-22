using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDetection.Moudle
{
    /// <summary>
    /// 摄像头摆放位置信息
    /// </summary>
    public class CameraPositionInfo
    {
        public double CameraFarendWidth { get; set; }//cm
        public double CameraFarendHeight { get; set; }//pixel
        public double CameraXNearendWidth { get; set; }//cm
        public double CameraXnearendLengthtoTableBoundry { get; set; }//cm
    }
}
