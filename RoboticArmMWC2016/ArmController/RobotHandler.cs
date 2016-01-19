using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientDLL;
using System.Threading;
using Helpers;
using RoboticArm.Sender;

namespace RoboticArm
{
    public class RobotHandler
    {
        private ISender _sender;
        private ISender _patternSender;
        string _ip;
        int _port;
        public double MaxVelocity { get; set; }

        public RobotHandler(string ip, int port, SenderTypes type,int patternPort)
        {
            _ip = ip;
            _port = port;
            if (type == SenderTypes.TCP)
            {
                _sender = new TcpSender(ip, port);
            }
            else
            {
                _sender = new UDPSender(ip, port);
            }
            _patternSender = new TcpSender(ip, patternPort);
        }

        #region Move

        /// <summary>
        /// 移动机械臂至x,y，单位为厘米
        /// </summary>
        [Obsolete]
        public void MoveArm(double endPointX,double yVelocity,double yDisance)
        {
            try
            {
                string dataString = string.Empty;
                if (yVelocity >= this.MaxVelocity)
                {
                    dataString = "defence\r\n";
                }
                else
                {
                    var delayTime = yDisance / yVelocity;
                    dataString = string.Format("{0}|{1}\r\n", endPointX.ToString(), delayTime.ToString());
                }
                _sender.SendtoRoboticArm(dataString);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void MoveArm(double endPointX, int reachTime)
        {
            var dataString = string.Format("{0}|{1}\r\n", endPointX.ToString(), 10);
            _sender.SendtoRoboticArm(dataString);
        }
        #endregion

        public void SetPattern(string patternName)
        {
            _patternSender.SendtoRoboticArm(patternName + "\r\n");
        }
    }
}
