using Helpers;
using MotionDetection.Moudle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDetection
{
    /// <summary>
    /// 用于预估冰球到达球台另一边的位置和速度，所有坐标已换算成实际尺寸坐标
    /// </summary>
    public class MotionEndPointCalc
    {
        public delegate void MotionResultHandler(MotionResult motionResult);
        private int _tableWidth;
        private int _tableHeight;
        private int _detectFrequency;

        public MotionEndPointCalc()
        {
            ConfigHelper config = ConfigHelper.GetInstance();
            //桌子尺寸：厘米
            _tableWidth = 198;
            _tableHeight = 92;
            _detectFrequency = config.DetectFrequency;
        }

        /// <summary>
        /// 计算小球与桌边的接触
        /// </summary>
        /// <param name="x1">小球公位置1X</param>
        /// <param name="y1">小球公位置1Y</param>
        /// <param name="x2">小球公位置2X</param>
        /// <param name="y2">小球公位置2Y</param>
        /// <returns></returns>
        public MotionResult CalcEndPoint(double x1,double y1,double x2,double y2)
        {
            MotionResult mResult = new MotionResult() { };
            var dx = x2 - x1;//x方向移动
            var dy = y2 - y1;//y方向移动

            if (dy <= 0)//小球未向机械臂移动
            {
                return null;
            }
            var yVel = dy / _detectFrequency;
            var distancetoY = _tableHeight - y2;
            mResult = new MotionResult() { MoveVelocityY = yVel, DistancetoY = distancetoY };
            if (dx == 0)//小球向机械臂垂直移动
            {
                mResult.EndPointX = x1;
                return mResult;
            }
            var motionAngle = MotionAngle(dx, dy);//距水平方向移动角度
            var ytoTableEdge = _tableHeight - y2;//距桌边垂直距离
            var xMoved = ytoTableEdge / Math.Tan(motionAngle);//x方向移动距离
            var endPointIngoreBoundry = xMoved + x2;//末端位置，忽略桌宽
            while (endPointIngoreBoundry > _tableWidth || endPointIngoreBoundry < 0)//边界外，有反弹
            {
                if (endPointIngoreBoundry > _tableWidth)
                {
                    endPointIngoreBoundry = _tableWidth - (endPointIngoreBoundry - _tableWidth);
                }
                else if (endPointIngoreBoundry < 0)
                {
                    endPointIngoreBoundry = endPointIngoreBoundry * -1;
                }
            }
            mResult.EndPointX = endPointIngoreBoundry;
            return mResult;
        }

        private double MotionAngle(double dx, double dy)
        {
            double angle;
            if (dx == 0)
            {
                angle = Math.PI/2;
            }
            else
            {
                angle = Math.Atan(dy / dx);
            }
            return angle;
        }
    }
}
