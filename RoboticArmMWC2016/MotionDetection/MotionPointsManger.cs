using MotionDetection.Moudle;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace MotionDetection
{
    public class MotionPointsManger
    {
        private List<Point> _centerPoints=new List<Point>();
        private MotionEndPointCalc _motionEndPointCalc;
        private object _object = new object();

        public MotionPointsManger()
        {
            _motionEndPointCalc = new MotionEndPointCalc();
        }

        public MotionResult UpdateResult()
        {
            Monitor.Enter(_object);
            MotionResult result;
            if (_centerPoints.Count >= 2)//从最近两个点计算
            {
                double actualX1 = ActualX(_centerPoints[_centerPoints.Count - 2]);
                double actualY1 = ActualY(_centerPoints[_centerPoints.Count - 2], _centerPoints[_centerPoints.Count - 2]);
                double actualX2 = ActualX(_centerPoints[_centerPoints.Count - 1]);
                double actualY2 = ActualY(_centerPoints[_centerPoints.Count - 2], _centerPoints[_centerPoints.Count - 2]);
                result = _motionEndPointCalc.CalcEndPoint(actualX1, actualY1, actualX2, actualY2);
                Monitor.Exit(_object);
                return result;
            }
            else
            {
                Monitor.Exit(_object);
                return null;
            }
        }

        /// <summary>
        /// 根据摄像头位置计算实际Y坐标
        /// </summary>
        /// <param name="cameraX"></param>
        /// <param name="cameraX2"></param>
        /// <returns></returns>
        private double ActualY(Point cameraX, Point cameraX2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据摄像头位置计算实际X坐标
        /// </summary>
        /// <param name="cameraY"></param>
        /// <returns></returns>
        private double ActualX(Point cameraY)
        {
            throw new NotImplementedException();
        }
    }
}
