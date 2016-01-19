using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchServer
{
    public class MPoint
    {
        public MPoint()
        {
        }

        public MPoint(double xPoint, double yPoint, bool isEffective = true)
        {
            this.XPoint = xPoint;
            this.YPoint = yPoint;
            this.IsEffective = isEffective;
        }

        public double XPoint
        {
            get;
            set;
        }

        public double YPoint
        {
            get;
            set;
        }

        public bool IsEffective
        {
            get;
            set;
        }
    }
}
