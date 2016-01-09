using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientDLL;
using System.Threading;
using Helpers;

namespace RoboticArm
{
    public class RobotHandler
    {
        private AsyncClient asyncClient;
        private System.Timers.Timer asyncTimer = new System.Timers.Timer();
        string _ip;
        int _port;
        public double MaxVelocity { get; set; }

        public RobotHandler(string ip, int port)
        {
            _ip = ip;
            _port = port;
            InitSocket(ip,port);
        }

        #region Socket
        private void InitSocket(string ip,int port)
        {
            try
            {
                InitAsyncTimer();
                if (this.asyncClient != null)
                {
                    this.asyncClient.Dispose();
                    this.asyncClient.onConnected -= new AsyncClient.Connected(client_onConnected);
                    this.asyncClient.onDisConnect -= new AsyncClient.DisConnect(client_onDisConnect);
                    this.asyncClient.onDataByteIn -= new AsyncClient.DataByteIn(client_onDataByteIn);
                }
                asyncClient = new AsyncClient();
                asyncClient.onConnected += new AsyncClient.Connected(client_onConnected);
                asyncClient.Connect(ip, port);
                asyncClient.onDataByteIn += new AsyncClient.DataByteIn(client_onDataByteIn);
                asyncClient.onDisConnect += new AsyncClient.DisConnect(client_onDisConnect);
            }
            catch (Exception ex)
            {
                
            }
        }

        void client_onDataByteIn(byte[] SocketData)
        {
        }

        void client_onConnected()
        {
            try
            {
                Thread.Sleep(100);
                asyncTimer.Stop();
                //string message = string.Format("{0}连线!", ip);
                //this.Invoke(new DelSetText(SetText), new object[] { message });
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        void client_onDisConnect()
        {
            string message = string.Format("{0}断线!",_ip);
            //this.Invoke(new DelSetText(SetText), new object[] { message });
            asyncTimer.Start();
        }

        private void asyncTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ReConnect();
        }

        private void ReConnect()
        {
            asyncClient.Dispose();
            asyncClient.Connect(_ip, _port);
        }

        private void InitAsyncTimer()
        {
            asyncTimer = new System.Timers.Timer();
            asyncTimer.Interval = 1500;
            asyncTimer.Elapsed += new System.Timers.ElapsedEventHandler(asyncTimer_Elapsed);
        }
        #endregion       

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
                asyncClient.Send(dataString);
                //LogHelper.GetInstance().ShowMsg("发送到机械臂:=============" + dataString + "\n");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void MoveArm(double endPointX, int reachTime)
        {
            var dataString = string.Format("{0}|{1}\r\n", endPointX.ToString(), 10);
            asyncClient.Send(dataString);
            //LogHelper.GetInstance().ShowMsg("发送到机械臂:=============" + dataString + "\n");
        }
        #endregion
    }
}
