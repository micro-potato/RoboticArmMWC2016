using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboticArm.Sender
{
    public class UDPSender:ISender
    {
        //public 
        private string _ip;
        private int _port;
        private UDPDLL.AsyncUDP udpSender = new UDPDLL.AsyncUDP();

        public UDPSender(string ip, int port)
        {
            InitSender(ip, port);
        }

        public void InitSender(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void SendtoRoboticArm(string data)
        {
            udpSender.Send(_ip, _port, data);
        }
    }
}
