using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboticArm.Sender
{
    interface ISender
    {
        void InitSender(string ip,int port);
        void SendtoRoboticArm(string data);
    }
}
