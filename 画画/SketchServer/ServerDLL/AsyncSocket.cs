using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ServerDLL
{
    internal class AsyncSocket
    {
        public Socket socket;
        private int Index; //索引

        //事件
        public delegate void SocketDisconnected(int index,string ip);
        public event SocketDisconnected socketDisconnected; //断线

        public delegate void SocketDataByteIn(int index,string ip, byte[] tmp);
        public event SocketDataByteIn socketDataByteIn; //消息进入

        /// <summary>
        /// Socket类构造函数
        /// </summary>
        /// <param name="socket">当前socket</param>
        /// <param name="index">索引</param>
        public AsyncSocket(Socket socket, int index)
        {
            try
            {
                this.socket = socket;
                this.Index = index;
               // this.SetXinTiao(this.socket);
                StateObject socketState = new StateObject();
                socketState.workSocket = socket;
                socket.BeginReceive(socketState.buffer, 0, StateObject.Buffersize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socketState);
            }
            catch
            {
                
            }

        }

        //回调
        private void ReceiveCallBack(IAsyncResult ar)
        {
            string ip = "";
            try
            {
                StateObject socketState = (StateObject)ar.AsyncState;
                Socket objSocket = socketState.workSocket;
                ip = objSocket.RemoteEndPoint.ToString().Split(':')[0];
                int bytesRead = objSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    byte[] tmp = new byte[bytesRead];
                    Array.ConstrainedCopy(socketState.buffer, 0, tmp, 0, bytesRead);
                    socketDataByteIn(this.Index,ip, tmp);
                }
                else //断线
                {
                    socketDisconnected(this.Index, ip);
                    return;
                }
                objSocket.BeginReceive(socketState.buffer, 0, StateObject.Buffersize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socketState);
            }
            catch
            {
                if (socketDisconnected != null)
                {
                    socketDisconnected(this.Index, ip);
                }
            }
        }

        //设置心跳
        private void SetXinTiao(Socket tmpsock)
        {
            try
            {
                //int keepAlive = -1744830460;
                byte[] inValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 };// 首次探测时间20 秒, 间隔侦测时间2 秒
                tmpsock.IOControl(IOControlCode.KeepAliveValues, inValue, null);
            }
            catch
            {

            }
        }

        public void Send(string data)
        {
            try
            {
                byte[] tmp = Encoding.GetEncoding("UTF-8").GetBytes(data);
                socket.BeginSend(tmp, 0, tmp.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);
            }
            catch
            {

            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket objSocket = (Socket)ar.AsyncState;
                int bytesSent = objSocket.EndSend(ar);
            }
            catch
            {

            }

        }

        public void Dispose()
        {
            socket.Close();
        }
    }
}
