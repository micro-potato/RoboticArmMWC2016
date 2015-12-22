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
        private List<Point> _centerXPoints;
        private List<Point> _centerYPoints;
        private List<Point> _centerX2Points;
        private MotionEndPointCalc _motionEndPointCalc;
        private object _object = new object();

        public MotionPointsManger()
        {
            _motionEndPointCalc = new MotionEndPointCalc();
        }

        public void AddPoint(MotionPointEnum pointType,Point value)
        {
            Monitor.Enter(_object);
            switch (pointType)
            {
                case MotionPointEnum.Xpoint:
                    _centerXPoints.Add(value);
                    break;
                case MotionPointEnum.Ypont:
                    _centerYPoints.Add(value);
                    break;
                case MotionPointEnum.X2point:
                    _centerX2Points.Add(value);
                    break;
                default:
                    throw new ArgumentException();
            }
            Monitor.Exit(_object);
        }

        public void UpdateResult()
        {
            Monitor.Enter(_object);
            if (_centerXPoints.Count >= 2 && _centerYPoints.Count >= 2 && _centerX2Points.Count >= 2)//从最近两个点计算
            {
                double actualX1 = ActualX(_centerYPoints[_centerYPoints.Count - 2]);
                double actualY1 = ActualY(_centerXPoints[_centerXPoints.Count - 2], _centerX2Points[_centerX2Points.Count - 2]);
                double actualX2 = ActualX(_centerYPoints[_centerYPoints.Count - 1]);
                double actualY2 = ActualY(_centerXPoints[_centerXPoints.Count - 2], _centerX2Points[_centerX2Points.Count - 2]);
                _motionEndPointCalc.CalcEndPoint(actualX1, actualY1, actualX2, actualY2);
            }
            Monitor.Exit(_object);
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
