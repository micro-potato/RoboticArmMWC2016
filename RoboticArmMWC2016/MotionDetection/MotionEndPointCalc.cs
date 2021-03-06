﻿using Helpers;
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
        private int _calcedWidth;
        private int _calcedHeight;

        public MotionEndPointCalc()
        {
            ConfigHelper config = ConfigHelper.GetInstance();
            //桌子尺寸：厘米
            _calcedWidth = config.DetectWidth;
            _calcedHeight = config.CalcedHeigth;
        }

        /// <summary>
        /// 计算小球与桌边的接触
        /// </summary>
        /// <param name="x1">小球公位置1X</param>
        /// <param name="y1">小球公位置1Y</param>
        /// <param name="x2">小球公位置2X</param>
        /// <param name="y2">小球公位置2Y</param>
        /// <param name="eplaseTime">移动间隔时间</param>
        /// <returns></returns>
        public MotionResult CalcEndPoint(double x1,double y1,double x2,double y2,int eplaseTime)
        {
            double endPointTheory;//到达终点的位置（理论值）
            double endPointActual;//到达终点的位置（实际值）
            double middleValue = _calcedWidth / 2;
            MotionResult mResult = new MotionResult() { };
            var dx = x2 - x1;//x方向移动
            var dy = y2 - y1;//y方向移动

            if (dy <= 0)//小球未向机械臂移动
            {
                return null;
            }
            var yVel = dy / eplaseTime;
            var distancetoY = _calcedHeight - y2;//距桌边垂直距离
            int reachTime = (int)(distancetoY / yVel);
            mResult = new MotionResult() { ReachTime=reachTime };
            if (Math.Abs(dx) == 0)//小球向机械臂垂直移动
            {
                mResult.EndPointX = x1;
                return mResult;
            }
            var motionAngle = MotionAngle(dx, dy);//距水平方向移动角度
            var xMoved = distancetoY / Math.Tan(motionAngle);//x方向移动距离
            endPointTheory = xMoved + x2+middleValue;//末端位置，忽略桌宽
            while (endPointTheory > _calcedWidth || endPointTheory < 0)//边界外，有反弹
            {
                if (endPointTheory > _calcedWidth)
                {
                    endPointTheory = _calcedWidth - (endPointTheory - _calcedWidth);
                }
                else if (endPointTheory < 0)
                {
                    endPointTheory = endPointTheory * -1;
                }
            }
            endPointActual = endPointTheory - middleValue;
            mResult.EndPointX = endPointActual;
            return mResult;
            #region old
            //var endPointIngoreBoundry = xMoved + x2;//末端位置，忽略桌宽
            //while (endPointIngoreBoundry > _calcedWidth || endPointIngoreBoundry < 0)//边界外，有反弹
            //{
            //    if (endPointIngoreBoundry > _calcedWidth)
            //    {
            //        endPointIngoreBoundry = _calcedWidth - (endPointIngoreBoundry - _calcedWidth);
            //    }
            //    else if (endPointIngoreBoundry < 0)
            //    {
            //        endPointIngoreBoundry = endPointIngoreBoundry * -1;
            //    }
            //}
            //mResult.EndPointX = RoboticValue(endPointIngoreBoundry); ;
            //return mResult;
            #endregion
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
