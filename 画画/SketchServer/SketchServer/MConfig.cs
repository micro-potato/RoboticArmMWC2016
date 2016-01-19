using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchServer
{
    public class MConfig
    {
        /// <summary>
        /// TCP监听端口
        /// </summary>
        public int ServerPort
        {
            get;
            set;
        }

        /// <summary>
        /// 机械臂IP
        /// </summary>
        public string MachineIP
        {
            get;
            set;
        }

        /// <summary>
        /// 机械臂端口
        /// </summary>
        public int MachinePort
        {
            get;
            set;
        }

        /// <summary>
        /// 画板角度
        /// </summary>
        public double DrawAngle
        {
            get;
            set;
        }

        /// <summary>
        /// 画板宽度(毫米)
        /// </summary>
        public double PaperWidth
        {
            get;
            set;
        }

        /// <summary>
        /// 画板高度(毫米)
        /// </summary>
        public double PaperHeight
        {
            get;
            set;
        }

        /// <summary>
        /// X轴偏移(毫米)
        /// </summary>
        public double OffsetX
        {
            get;
            set;
        }

        /// <summary>
        /// Y轴偏移(毫米)
        /// </summary>
        public double OffsetY
        {
            get;
            set;
        }

        /// <summary>
        /// Z轴偏移
        /// </summary>
        public double OffsetZ
        {
            get;
            set;
        }
    }
}
