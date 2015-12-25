using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace ServerDLL
{
    public class AsyncServer
    {

        //socket
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int SocketIndex = 0; //socket索引
        private SortedList<int, AsyncSocket> SocketList = new SortedList<int, AsyncSocket>(); //socket集合

        //事件
        public delegate void SocketConnected(int index, string ip);
        public event SocketConnected onConnected; //连线
        public delegate void SocketDisconnected(int index, string ip);
        public event SocketDisconnected onDisconnected; //断线
        public delegate void SocketDataByteIn(int index, string ip, byte[] SocketData);
        public event SocketDataByteIn onDataByteIn; //消息进入

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="maxount">最大连接数</param>
        public bool Listen(int port, int maxCount)
        {
            try
            {
                IPAddress ip = IPAddress.Any;
                IPEndPoint point = new IPEndPoint(ip, port);
                socket.Bind(point);
                socket.Listen(maxCount);
                socket.BeginAccept(new AsyncCallback(AcceptCallBack), socket);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public void Dispose()
        {
            try
            {
                for (int i = 0; i < SocketList.Count; i++)
                {
                    try
                    {
                        SocketList[i].Dispose();
                    }
                    catch
                    {

                    }
                }
                socket.Close(100);
            }
            catch
            {

            }
        }


        private void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                Socket objSocket = (Socket)ar.AsyncState;
                try
                {
                    Socket objConnected = objSocket.EndAccept(ar);
                    IPEndPoint point = (IPEndPoint)objConnected.RemoteEndPoint;
                    AsyncSocket workSocket = new AsyncSocket(objConnected, SocketIndex);
                    SocketList.Add(SocketIndex, workSocket);
                    if (onConnected != null)
                    {
                        onConnected(SocketIndex, point.ToString().Split(':')[0]); //连线
                    }
                    workSocket.socketDataByteIn += new AsyncSocket.SocketDataByteIn(workSocket_socketDataByteIn);
                    workSocket.socketDisconnected += new AsyncSocket.SocketDisconnected(workSocket_socketDisconnected);
                    SocketIndex++;
                }
                catch
                {

                }
                objSocket.BeginAccept(new AsyncCallback(AcceptCallBack), objSocket);
            }
            catch
            {

            }
        }

        //断线
        void workSocket_socketDisconnected(int index, string ip)
        {
            if (onDisconnected != null)
            {
                onDisconnected(index, ip);
            }
        }

        //消息进入
        void workSocket_socketDataByteIn(int index, string ip, byte[] tmp)
        {
            if (onDataByteIn != null)
            {
                onDataByteIn(index, ip, tmp);
            }
        }

        //发送
        public void Send(int index, string data)
        {
            try
            {
                SocketList[index].Send(data);
            }
            catch
            {

            }
        }

        //发送给所有客户端
        public void SendToAll(string data)
        {
            for (int i = 0; i < SocketList.Count; i++)
            {
                SocketList[i].Send(data);
            }
        }
    }
}
