using ClientDLL;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RoboticArm.Sender
{
    public class TcpSender:ISender
    {
        private AsyncClient asyncClient;
        private System.Timers.Timer asyncTimer = new System.Timers.Timer();
        private string _ip; int _port;
        public TcpSender(string ip,int port)
        {
            InitSender(ip, port);
        }

        public void InitSender(string ip, int port)
        {
            try
            {
                _ip = ip;
                _port = port;
                InitAsyncTimer();
                if (this.asyncClient != null)
                {
                    this.asyncClient.Dispose();
                    this.asyncClient.onConnected -= new AsyncClient.Connected(client_onConnected);
                    this.asyncClient.onDisConnect -= new AsyncClient.DisConnect(client_onDisConnect);
                }
                asyncClient = new AsyncClient();
                asyncClient.onConnected += new AsyncClient.Connected(client_onConnected);
                asyncClient.Connect(ip, port);
                asyncClient.onDisConnect += new AsyncClient.DisConnect(client_onDisConnect);
            }
            catch (Exception ex)
            {
                LogHelper.GetInstance().ShowMsg("Client error:"+ex.Message);
            }
        }

        private void InitAsyncTimer()
        {
            asyncTimer = new System.Timers.Timer();
            asyncTimer.Interval = 1500;
            asyncTimer.Elapsed += new System.Timers.ElapsedEventHandler(asyncTimer_Elapsed);
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
            LogHelper.GetInstance().ShowMsg("断线!");
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

        public void SendtoRoboticArm(string data)
        {
            asyncClient.Send(data);
        }
    }
}
