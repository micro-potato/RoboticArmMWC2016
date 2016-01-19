using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServerDLL;
using ClientDLL;
using Auxiliary;
using System.Diagnostics;
using System.IO;

namespace SketchServer
{
    public partial class MainForm : Form
    {
        private MConfig mConfig;
        private bool isConnect = false;

        //NetWork
        private AsyncServer asyncServer = new AsyncServer();
        private AsyncClient asyncClient;
        private System.Timers.Timer asyncTimer;

        //委托
        private delegate void DelSetText(string data);
        private delegate void DelDataDeal(int index, string data);

        //机械臂是否走完，如果走完可以直接发，如果没走完需要将控制命令保存起来
        private bool isMoveComplete = true;

        //缓存数据
        private List<string> BufferData = new List<string>();

        //
        private int num = 3;

        //
        private int upMark = -1;

        private int downMark = -2;

        private int padWidth = 1696;
        private int padHeight = 1440;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mConfig = (new ConfigDeal()).GetMessage();
            lb_ServerPort.Text = lb_ServerPort.Text + mConfig.ServerPort.ToString();
            lb_MachineIP.Text = lb_MachineIP.Text + mConfig.MachineIP;
            lb_MachinePort.Text = lb_MachinePort.Text + mConfig.MachinePort.ToString();

            InitServer();
            InitClient();
            this.TopMost = true;

            double a = 30;//角度值
            double b = a * Math.PI / 180;//转化为弧度值
            Console.WriteLine(Math.Sin(b));//用弧度值进行计算
            Console.WriteLine(Math.Cos(b));
        }

        #region 服务端通讯
        private void InitServer()
        {
            if (asyncServer.Listen(mConfig.ServerPort, 2000))
            {
                asyncServer.onConnected += new AsyncServer.SocketConnected(asyncServer_onConnected);
                asyncServer.onDisconnected += new AsyncServer.SocketDisconnected(asyncServer_onDisconnected);
                asyncServer.onDataByteIn += new AsyncServer.SocketDataByteIn(asyncServer_onDataByteIn);

                SetServerText(string.Format("监听端口{0}成功!", mConfig.ServerPort.ToString()));
            }
            else
            {
                MessageBox.Show("TCP端口被占用，请重新配置!");
                Process.GetCurrentProcess().Kill();
            }
        }

        //客户端连线
        void asyncServer_onConnected(int index, string ip)
        {
            string message = string.Format("{0}连线!", ip);
            this.Invoke(new DelSetText(SetServerText), new object[] { message });
        }

        //客户端断线
        void asyncServer_onDisconnected(int index, string ip)
        {
            string message = string.Format("{0}断线!", ip);
            this.Invoke(new DelSetText(SetServerText), new object[] { message });
        }

        //消息进入
        void asyncServer_onDataByteIn(int index, string ip, byte[] SocketData)
        {
            try
            {
                string message = System.Text.Encoding.GetEncoding("GB2312").GetString(SocketData).Replace("\n", "");
                this.Invoke(new DelSetText(SetServerText), new object[] { message });
                string[] dataList = message.Split('|');//协议以|符号分割
                foreach (string data in dataList)
                {
                    if (!String.IsNullOrEmpty(data))
                    {
                        //Flash交互
                        this.Invoke(new DelDataDeal(DataDeal), new object[] { index, data });
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private void DataDeal(int index, string data)
        {
            string title = data.Split(':')[0];
            string content = data.Split(':')[1];
            switch (title)
            {
                //Pad端传递坐标信息
                case "P":
                    //需要转换成3维立体坐标信息并发送给机械臂
                    if (isConnect)
                    {
                        double mouseX = Convert.ToDouble(content.Split(',')[0]);
                        double mouseY = Convert.ToDouble(content.Split(',')[1]);

                        double xPercent = mouseX / padWidth;
                        double yPercent = mouseY / padHeight;

                        //换算成纸张的横坐标和纵坐标
                        double xPaper = xPercent * mConfig.PaperWidth;
                        double yPaper = yPercent * mConfig.PaperHeight;

                        //换算成三维坐标
                        double pointX = xPaper;
                        double pointY = Math.Cos(mConfig.DrawAngle * Math.PI / 180) * yPaper;
                        double pointZ = Math.Sin(mConfig.DrawAngle * Math.PI / 180) * yPaper;

                        //计算偏移量，并乘以10发送到机械臂
                        int paperX = Convert.ToInt32(Math.Round(pointX + mConfig.OffsetX, 1) * 10);
                        int paperY = Convert.ToInt32(Math.Round(pointY + mConfig.OffsetY, 1) * 10);
                        int paperZ = Convert.ToInt32(Math.Round(pointZ + mConfig.OffsetZ, 1) * 10);

                        string sendData = GetFullNumber(paperX) + "," + GetFullNumber(paperY) + "," + GetFullNumber(paperZ) + "|";
                        SetClientText("Send:" + sendData);
                        asyncClient.Send(sendData);
                    }
                    break;
                case "Track":
                    try
                    {
                        if (isConnect)
                        {
                            if (isMoveComplete == true)
                            {
                                isMoveComplete = false;
                                string[] iniPointsArray = content.Split('^');
                                string[] pointsArray = new string[0];
                                for (int i = 0; i < iniPointsArray.Length; i++)
                                {
                                    if (i % num == 0)
                                    {
                                        Array.Resize(ref pointsArray, pointsArray.Length + 1);
                                        pointsArray[pointsArray.Length - 1] = iniPointsArray[i];
                                    }
                                }

                                if (iniPointsArray.Length > 1)
                                {
                                    if (iniPointsArray.Length < num)
                                    {
                                        Array.Resize(ref pointsArray, pointsArray.Length + 1);
                                        pointsArray[pointsArray.Length - 1] = iniPointsArray[iniPointsArray.Length - 1];
                                    }
                                    else
                                    {
                                        if (iniPointsArray.Length % 3 == 0)
                                        {
                                            Array.Resize(ref pointsArray, pointsArray.Length + 1);
                                            pointsArray[pointsArray.Length - 1] = iniPointsArray[iniPointsArray.Length - 1];
                                        }
                                    }
                                }
                                SendPoints(pointsArray);
                            }
                            else
                            {
                                //需要保存到内存里面
                                BufferData.Add(content);
                            }
                        }
                    }
                    catch
                    {

                    }
                    break;
                case "Down":

                    /*
                    if (isConnect)
                    {
                        if (isMoveComplete == true)
                        {
                            isMoveComplete = false;

                            string[] pointsArray = content.Split('^');
                            byte[] byteAll = new byte[4 + 16 + pointsArray.Length * 8];

                            byte[] ByteNumS = BitConverter.GetBytes(16 + pointsArray.Length * 8);
                            ByteNumS.CopyTo(byteAll, 0);

                            byte[] SketchNumS = BitConverter.GetBytes(1);
                            SketchNumS.CopyTo(byteAll, 4);

                            byte[] AllPointsNums = BitConverter.GetBytes(pointsArray.Length);
                            AllPointsNums.CopyTo(byteAll, 8);

                            byte[] Mark = BitConverter.GetBytes(-1);
                            Mark.CopyTo(byteAll, 12);

                            byte[] CurrentPtsNums = BitConverter.GetBytes(pointsArray.Length);
                            CurrentPtsNums.CopyTo(byteAll, 16);

                            for (int i = 0; i < pointsArray.Length; i++)
                            {
                                double mouseX = Convert.ToDouble(pointsArray[i].Split(',')[0]);
                                double mouseY = Convert.ToDouble(pointsArray[i].Split(',')[1]);

                                double xPercent = mouseX / 1440;
                                double yPercent = mouseY / 2160;

                                //换算成纸张的横坐标和纵坐标
                                double xPaper = xPercent * mConfig.PaperWidth;
                                double yPaper = yPercent * mConfig.PaperHeight;

                                int paperX = Convert.ToInt32(Math.Round(xPaper + mConfig.OffsetX, 1) * 10);
                                int paperY = Convert.ToInt32(Math.Round(yPaper + mConfig.OffsetY, 1) * 10);

                                byte[] byteX = BitConverter.GetBytes(paperX);
                                byte[] byteY = BitConverter.GetBytes(paperY);

                                byteX.CopyTo(byteAll, 20 + i * 8);
                                byteY.CopyTo(byteAll, 24 + i * 8);
                            }
                            asyncClient.Send(byteAll);

                            //打印
                            string printData = "发送到机械臂:";
                            for (int i = 0; i < byteAll.Length; i++)
                            {
                                printData += byteAll[i].ToString() + " ";
                            }

                            printData += GetDataByBytes(byteAll);

                            rtb_client.AppendText(printData + "\n");
                        }
                        else
                        {
                           
                        }
                    }
                     */
                    break;
            }
        }

        private void SendPoints(string[] pointsArray)
        {
            byte[] byteAll = new byte[4 + 16 + pointsArray.Length * 8];

            byte[] ByteNumS = BitConverter.GetBytes(16 + pointsArray.Length * 8);
            ByteNumS.CopyTo(byteAll, 0);

            byte[] SketchNumS = BitConverter.GetBytes(1);
            SketchNumS.CopyTo(byteAll, 4);

            byte[] AllPointsNums = BitConverter.GetBytes(pointsArray.Length);
            AllPointsNums.CopyTo(byteAll, 8);

            byte[] Mark = BitConverter.GetBytes(upMark);
            Mark.CopyTo(byteAll, 12);

            byte[] CurrentPtsNums = BitConverter.GetBytes(pointsArray.Length);
            CurrentPtsNums.CopyTo(byteAll, 16);

            for (int i = 0; i < pointsArray.Length; i++)
            {
                double mouseX = Convert.ToDouble(pointsArray[i].Split(',')[0]);
                double mouseY = Convert.ToDouble(pointsArray[i].Split(',')[1]);

                double xPercent = mouseX / padWidth;
                double yPercent = mouseY / padHeight;

                //换算成纸张的横坐标和纵坐标
                double xPaper = xPercent * mConfig.PaperWidth;
                double yPaper = yPercent * mConfig.PaperHeight;

                int paperX = Convert.ToInt32(Math.Round(xPaper + mConfig.OffsetX, 1) * 10);
                int paperY = Convert.ToInt32(Math.Round(yPaper + mConfig.OffsetY, 1) * 10);

                byte[] byteX = BitConverter.GetBytes(paperX);
                byte[] byteY = BitConverter.GetBytes(paperY);

                byteX.CopyTo(byteAll, 20 + i * 8);
                byteY.CopyTo(byteAll, 24 + i * 8);
            }
            asyncClient.Send(byteAll);

            //打印
            string printData = "发送到机械臂:";
            for (int i = 0; i < byteAll.Length; i++)
            {
                printData += byteAll[i].ToString() + " ";
            }

            printData += GetDataByBytes(byteAll);

            rtb_client.AppendText(printData + "\n");
        }

        private void SetServerText(string data)
        {
            rtb_server.AppendText(data + "\n");
        }

        private string GetFullNumber(int num)
        {
            string fullNumber = "";
            string str = num.ToString();
            for (int i = str.Length; i < 6; i++)
            {
                fullNumber += "0";
            }
            fullNumber += str;
            return fullNumber;
        }
        #endregion

        #region 客户端通讯
        //初始化客户端通讯(连接F1Server,并将Server端指令转发到Flash或Unity)
        private void InitClient()
        {
            asyncClient = new AsyncClient();
            asyncClient.onConnected += new AsyncClient.Connected(asyncClient_onConnected);
            asyncClient.onDisConnect += new AsyncClient.DisConnect(asyncClient_onDisConnect);
            asyncClient.onDataByteIn += new AsyncClient.DataByteIn(asyncClient_onDataByteIn);
            asyncClient.Connect(mConfig.MachineIP, mConfig.MachinePort);

            asyncTimer = new System.Timers.Timer();
            asyncTimer.Interval = 3000;
            asyncTimer.Elapsed += new System.Timers.ElapsedEventHandler(asyncTimer_Elapsed);
            asyncTimer.Start();
        }

        //连线
        private void asyncClient_onConnected()
        {
            string message = string.Format("连接{0}成功!", mConfig.MachineIP);
            this.Invoke(new DelSetText(SetClientText), new object[] { message });
            isConnect = true;
            isMoveComplete = true;
            asyncTimer.Stop();
        }

        //收到消息
        void asyncClient_onDataByteIn(byte[] SocketData)
        {
            string message = "收到机械臂指令:";
            for (int i = 0; i < SocketData.Length; i++)
            {
                message += SocketData[i].ToString() + " ";
            }
            //this.Invoke(new DelSetText(SetClientText), new object[] { message });

            //判断收到的指令
            try
            {
                int mark = BitConverter.ToInt32(SocketData, 0)/10;
                this.Invoke(new DelSetText(SetClientText), new object[] { mark.ToString() });
                if (mark == 11)//画完
                {
                    isMoveComplete = true;
                    BufferDeal();
                }
                else if (mark == 111)//退出
                {
                    isConnect= false;
                    asyncTimer.Start();
                }
            }
            catch
            {
            }

        }

        //缓存数据处理
        private void BufferDeal()
        {
            //如果缓存数据不为空，则需要
            if (BufferData.Count > 0)
            {
                System.Threading.Thread.Sleep(100);

                int ByteNumS = 8 + 8 * BufferData.Count;
                int ByteSketchNumS = BufferData.Count;
                int ByteAllPointsNums = 0;
                for (int i = 0; i < BufferData.Count; i++)
                {
                    string[] iniPointsArray = BufferData[i].Split('^');
                    string[] pointsArray = new string[0];
                    for (int m = 0; m < iniPointsArray.Length; m++)
                    {
                        if (m % num == 0)
                        {
                            Array.Resize(ref pointsArray, pointsArray.Length + 1);
                            pointsArray[pointsArray.Length - 1] = iniPointsArray[m];
                        }
                    }
                    if (iniPointsArray.Length > 1)
                    {
                        if (iniPointsArray.Length < num)
                        {
                            Array.Resize(ref pointsArray, pointsArray.Length + 1);
                            pointsArray[pointsArray.Length - 1] = iniPointsArray[iniPointsArray.Length - 1];
                        }
                        else
                        {
                            if (iniPointsArray.Length % 3 == 0)
                            {
                                Array.Resize(ref pointsArray, pointsArray.Length + 1);
                                pointsArray[pointsArray.Length - 1] = iniPointsArray[iniPointsArray.Length - 1];
                            }
                        }
                    }

                    ByteAllPointsNums += pointsArray.Length;
                    ByteNumS += pointsArray.Length * 8;
                }


                byte[] ByteAllList = new byte[12];

                byte[] ByteList = BitConverter.GetBytes(ByteNumS);
                ByteList.CopyTo(ByteAllList, 0);

                byte[] ByteSketchList = BitConverter.GetBytes(ByteSketchNumS);
                ByteSketchList.CopyTo(ByteAllList, 4);

                byte[] AllPointsNums = BitConverter.GetBytes(ByteAllPointsNums);
                AllPointsNums.CopyTo(ByteAllList, 8);


                for (int i = 0; i < BufferData.Count; i++)
                {
                    string[] iniPointsArray = BufferData[i].Split('^');
                    string[] pointsArray = new string[0];
                    for (int m = 0; m < iniPointsArray.Length; m++)
                    {
                        if (m % num == 0)
                        {
                            Array.Resize(ref pointsArray, pointsArray.Length + 1);
                            pointsArray[pointsArray.Length - 1] = iniPointsArray[m];
                        }
                    }
                    if (iniPointsArray.Length > 1)
                    {
                        if (iniPointsArray.Length < num)
                        {
                            Array.Resize(ref pointsArray, pointsArray.Length + 1);
                            pointsArray[pointsArray.Length - 1] = iniPointsArray[iniPointsArray.Length - 1];
                        }
                        else
                        {
                            if (iniPointsArray.Length % 3 == 0)
                            {
                                Array.Resize(ref pointsArray, pointsArray.Length + 1);
                                pointsArray[pointsArray.Length - 1] = iniPointsArray[iniPointsArray.Length - 1];
                            }
                        }
                    }

                    Array.Resize(ref ByteAllList, ByteAllList.Length + 8);
                    byte[] Mark = BitConverter.GetBytes(upMark);
                    Mark.CopyTo(ByteAllList, ByteAllList.Length - 8);

                    byte[] CurrentPtsNums = BitConverter.GetBytes(pointsArray.Length);
                    CurrentPtsNums.CopyTo(ByteAllList, ByteAllList.Length - 4);

                    for (int j = 0; j < pointsArray.Length; j++)
                    {
                        double mouseX = Convert.ToDouble(pointsArray[j].Split(',')[0]);
                        double mouseY = Convert.ToDouble(pointsArray[j].Split(',')[1]);

                        double xPercent = mouseX / padWidth;
                        double yPercent = mouseY / padHeight;

                        //换算成纸张的横坐标和纵坐标
                        double xPaper = xPercent * mConfig.PaperWidth;
                        double yPaper = yPercent * mConfig.PaperHeight;

                        int paperX = Convert.ToInt32(Math.Round(xPaper + mConfig.OffsetX, 1) * 10);
                        int paperY = Convert.ToInt32(Math.Round(yPaper + mConfig.OffsetY, 1) * 10);

                        byte[] byteX = BitConverter.GetBytes(paperX);
                        byte[] byteY = BitConverter.GetBytes(paperY);

                        Array.Resize(ref ByteAllList, ByteAllList.Length + 8);
                        byteX.CopyTo(ByteAllList, ByteAllList.Length - 8);
                        byteY.CopyTo(ByteAllList, ByteAllList.Length - 4);

                    }
                }

                isMoveComplete = false;
                BufferData = new List<string>();
                asyncClient.Send(ByteAllList);

                //打印
                string printData = "发送到机械臂:";
                for (int i = 0; i < ByteAllList.Length; i++)
                {
                    printData += ByteAllList[i].ToString() + " ";
                }
                printData += GetDataByBytes(ByteAllList);
                //rtb_client.AppendText(printData + "\n");
                this.Invoke(new DelSetText(SetClientText), new object[] { printData + "\n" });
            }
        }

        //断线
        private void asyncClient_onDisConnect()
        {
            if (isConnect)
            {
                string message = string.Format("与{0}断开连接!", mConfig.MachineIP);
                this.Invoke(new DelSetText(SetClientText), new object[] { message });
            }
            isConnect = false;
            asyncTimer.Start();
        }

        private void asyncTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ReConnect();
        }

        /// <summary>
        /// 重连
        /// </summary>
        public void ReConnect()
        {
            asyncClient.Dispose();
            asyncClient.Connect(mConfig.MachineIP, mConfig.MachinePort);
        }

        private void SetClientText(string data)
        {
            rtb_client.AppendText(data + "\n");
        }

        #endregion

        private string GetInfoByTitle(string content, string title)
        {
            try
            {
                string[] msg = content.Split(',');
                for (int i = 0; i < msg.Length; i++)
                {
                    if (msg[i] != "")
                    {
                        string t = msg[i].Split('=')[0];
                        string c = msg[i].Split('=')[1];
                        if (t == title)
                        {
                            return c;
                        }
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        private void btn_serverClear_Click(object sender, EventArgs e)
        {
            rtb_server.Clear();
        }

        private void btn_clientClear_Click(object sender, EventArgs e)
        {
            rtb_client.Clear();
        }

        //***********************************************测试********************************************************
        private void btn_commit_Click(object sender, EventArgs e)
        {
            byte[] byteAll = new byte[28];

            byte[] ByteNumS = BitConverter.GetBytes(24);
            ByteNumS.CopyTo(byteAll, 0);

            byte[] SketchNumS = BitConverter.GetBytes(1);
            SketchNumS.CopyTo(byteAll, 4);

            byte[] AllPointsNums = BitConverter.GetBytes(1);
            AllPointsNums.CopyTo(byteAll, 8);

            byte[] Mark = BitConverter.GetBytes(upMark);
            Mark.CopyTo(byteAll, 12);

            byte[] CurrentPtsNums = BitConverter.GetBytes(1);
            CurrentPtsNums.CopyTo(byteAll, 16);

            int paperX = Convert.ToInt32(Convert.ToInt32(tb_pointX.Text) * 10);
            int paperY = Convert.ToInt32(Convert.ToInt32(tb_pointY.Text) * 10);

            byte[] byteX = BitConverter.GetBytes(paperX);
            byte[] byteY = BitConverter.GetBytes(paperY);

            byteX.CopyTo(byteAll, 20);
            byteY.CopyTo(byteAll, 24);

            asyncClient.Send(byteAll);

            //打印
            string printData = "发送到机械臂:";
            for (int i = 0; i < byteAll.Length; i++)
            {
                printData += byteAll[i].ToString() + " ";
            }
            rtb_client.AppendText(printData + "\n");
        }

        private string GetDataByBytes(byte[] bytesList)
        {
            string data = "(";
            for (int i = 0; i < bytesList.Length / 4; i++)
            {
                byte[] bList = new byte[4];
                Array.Copy(bytesList, i * 4, bList, 0, 4);
                data += BitConverter.ToInt32(bList, 0).ToString() + " ";
            }
            data += ")";
            return data;
        }

        private void btn_HelloKitty_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(Application.StartupPath + "\\Result_hellokitty.txt");
            string data = sr.ReadToEnd().Replace("\r\n", "|");
            data = data.Substring(0, data.Length - 1);
            string[] list = data.Split('|');

            byte[] bytesAll = new byte[list.Length * 4];
            for (int i = 0; i < list.Length; i++)
            {
                byte[] byteData = BitConverter.GetBytes(Convert.ToInt32(list[i]));

                byteData.CopyTo(bytesAll, i * 4);
            }
            asyncClient.Send(bytesAll);

            //打印
            string printData = "发送到机械臂:";
            for (int i = 0; i < bytesAll.Length; i++)
            {
                printData += bytesAll[i].ToString() + " ";

            }
            rtb_client.AppendText(printData + "\n");
        }

        private void btn_up_Click(object sender, EventArgs e)
        {
            byte[] byteAll = BitConverter.GetBytes(upMark);
            asyncClient.Send(byteAll);

            //打印
            string printData = "发送到机械臂:";
            for (int i = 0; i < byteAll.Length; i++)
            {
                printData += byteAll[i].ToString() + " ";

            }
            rtb_client.AppendText(printData + "\n");
        }
        //**********************************************************************************************************
    }
}