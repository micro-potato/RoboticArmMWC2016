using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDetection
{
    public class MotionEndPointCalc
    {
        public static int tableWidth;
        public static int tableHeight;

        public MotionEndPointCalc()
        {

        }

        /// <summary>
        /// 计算小球与桌边的接触
        /// </summary>
        /// <param name="x1">小球公位置1X</param>
        /// <param name="y1">小球公位置1Y</param>
        /// <param name="x2">小球公位置2X</param>
        /// <param name="y2">小球公位置2Y</param>
        /// <returns></returns>
        public static double CalcEndPoint(double x1,double y1,double x2,double y2)
        {
            var dx = x2 - x1;//x方向移动
            var dy = y2 - y1;//y方向移动

            if(dy<=0)//小球未向机械臂移动
            {
                return -1;
            }

            if (dx == 0)//小球向机械臂垂直移动
            {
                return x1;
            }
            var motionAngle = MotionAngle(dx, dy);//距水平方向移动角度
            var ytoTableEdge = tableHeight - y2;//距桌边垂直距离
            var xMoved = ytoTableEdge / Math.Tan(motionAngle);//x方向移动距离
            var endPointIngoreBoundry = xMoved + x2;//末端位置，忽略桌宽
            while (endPointIngoreBoundry > tableWidth || endPointIngoreBoundry < 0)//边界外，有反弹
            {
                if (endPointIngoreBoundry > tableWidth)
                {
                    endPointIngoreBoundry = tableWidth - (endPointIngoreBoundry - tableWidth);
                }
                else if (endPointIngoreBoundry < 0)
                {
                    endPointIngoreBoundry = endPointIngoreBoundry * -1;
                }
            }
            return endPointIngoreBoundry;
        }

        private static double MotionAngle(double dx, double dy)
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
