using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MotionDetection;
using System.Threading;
using Helpers;
using MotionDetection.Moudle;
using ServerDLL;
using System.Diagnostics;
using RoboticArm.Sender;

namespace RoboticArmMWC2016
{
    public partial class MainForm : Form,ILog
    {
        ConfigHelper _config;
        private MotionPointsManger _motionPointManager;
        private RoboticArm.RobotHandler _robotHandler;
        private AsyncServer asyncServer = new AsyncServer();
        private List<int> m_ClientIndexs = new List<int>();
        private delegate void DeleString(string arg);
        private delegate void DeleVoid();

        //模拟数据
        private System.Timers.Timer _simTimer;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitConfig();
            _motionPointManager = new MotionPointsManger(_config.DetectFrequence) { DetectWidth = _config.DetectWidth, DetectHeight = _config.DetectHeight ,CalibrateX=_config.CalibrateX};
            _motionPointManager.ValidResultGot += OnValidResultGot;
            InitRoboticArm();
            LogHelper.GetInstance().RegLog(this);
            InitServer();
           // _motionPointManager.StartDetect();
            //模拟数据
            _simTimer = new System.Timers.Timer(_config.DetectFrequence);
            _simTimer.Elapsed += _simTimer_Elapsed;
        }

        /// <summary>
        /// 获得最终到达位置的结果
        /// </summary>
        /// <param name="result"></param>
        void OnValidResultGot(MotionResult result)
        {
            var endpointX = result.EndPointX;
            var reachTime = result.ReachTime;
            _robotHandler.MoveArm(endpointX, reachTime);
            //LogHelper.GetInstance().ShowMsg(string.Format("到达位置：{0}，到达时间：{1}ms\n",endpointX,reachTime));
        }

        #region Socket
        private void InitServer()
        {
            if (asyncServer.Listen(_config.ServerPort, 2000))
            {
                asyncServer.onConnected += new AsyncServer.SocketConnected(asyncServer_onConnected);
                asyncServer.onDisconnected += new AsyncServer.SocketDisconnected(asyncServer_onDisconnected);
                asyncServer.onDataByteIn += new AsyncServer.SocketDataByteIn(asyncServer_onDataByteIn);
            }
            else
            {
                MessageBox.Show("该端口被占用，请重新配置!");
                Process.GetCurrentProcess().Kill();
            }
        }

        //客户端连线
        void asyncServer_onConnected(int index, string ip)
        {
            string message = string.Format("{0}连线!\n", ip);
            this.Invoke(new DeleString(SetText), new object[] { message });
            System.Threading.Thread.Sleep(100);
            if (!m_ClientIndexs.Contains(index))
            {
                m_ClientIndexs.Add(index);
            }
        }

        //客户端断线
        void asyncServer_onDisconnected(int index, string ip)
        {
            string message = string.Format("{0}断线!\n", ip);
            LogHelper.GetInstance().ShowMsg(message);
            if (m_ClientIndexs.Contains(index))
            {
                m_ClientIndexs.Remove(index);
            }
        }

        //消息进入
        void asyncServer_onDataByteIn(int index, string ip, byte[] SocketData)
        {
            try
            {
                string message = Encoding.UTF8.GetString(SocketData).Replace("\n", "");
                LogHelper.GetInstance().ShowMsg(message);
                string[] dataList = message.Split('|');//协议以|符号分割
                foreach (string data in dataList)
                {
                    if (!String.IsNullOrEmpty(data))
                    {
                        this.Invoke(new DeleString(DealMsg), new object[] { data });
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private void DealMsg(string msg)
        {
            string msgType = msg.Split(':')[0];
            string msgArg = msg.Split(':')[1];
            switch (msgType)
            {
                case "pattern":
                    this.Invoke(new DeleString(SetRobotPattern), msgArg);
                    break;
                case "control":
                    if (msgArg == "calibrate")
                    {
                        this.Invoke(new DeleVoid(StartCalibrate));
                    }
                    else if (msgArg == "stopCalibrate")
                    {
                        this.Invoke(new DeleVoid(StopCalibrate));
                    }
                    break;
                case "forward":
                    string toSend = msgArg;
                    this.Invoke(new DeleString(SendtoRobot), toSend);
                    break;
            }
        }
        #endregion

        private void InitRoboticArm()
        {
            string robotInterType = _config.RobotInterType;
            SenderTypes interType = (SenderTypes)Enum.Parse(typeof(SenderTypes), robotInterType);
            //_robotHandler = new RoboticArm.RobotHandler(_config.RobotIP, _config.RobotPort);

            _robotHandler = new RoboticArm.RobotHandler(_config.RobotIP, _config.RobotPort,interType,_config.RobotPatternPort);
            _robotHandler.MaxVelocity = _config.ReflectVelocity;
        }

        private void SetRobotPattern(string patternName)
        {
            _robotHandler.SetPattern(patternName);
            if (patternName == "playhockey")
            {
                _motionPointManager.StartDetect();
            }
            else
            {
                _motionPointManager.StopDetect();
            }
        }

        private void StartCalibrate()
        {
            _motionPointManager.StartCalibrate();
        }

        private void StopCalibrate()
        {
            _motionPointManager.EndCalibrate();
        }

        private void SendtoRobot(string msg)
        {
            _robotHandler.Send(msg);
        }

        private void InitConfig()
        {
            _config = ConfigHelper.GetInstance();
            _config.ResolveConfig(System.Windows.Forms.Application.StartupPath + @"\config.xml");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        #region 模拟数据
        private void button5_Click(object sender, EventArgs e)
        {
            _simTimer.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _simTimer.Stop();
        }

        void _simTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SimOnce();
        }

        private void SimOnce()
        {
            _simTimer.Stop();
            var xEnd = new Random().Next(85);
            var distance = new Random().Next(100);
            double velocity = ((double)(new Random().Next(20)) / 20) * 100 / 100;
            //_robotHandler.MoveArm(xEnd, 0.8, distance);
            _robotHandler.MoveArm(xEnd, 1);
            var waitforNext = new Random().Next(1,1000);
            _simTimer.Interval = waitforNext;
            _simTimer.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _robotHandler.MoveArm(double.Parse(this.textBox1.Text), 0.8, 0.8);
        }
        #endregion

        

        public void ShowLog(string msg)
        {
            this.Invoke(new DeleString(SetText), msg);
        }

        private void SetText(string text)
        {
            this.InfoText.AppendText(text);
            if (InfoText.Lines.Length > 5000)
            {
                InfoText.Clear();
            }
            this.InfoText.ScrollToCaret();
        }

        #region 检测开关
        private void button1_Click(object sender, EventArgs e)
        {
            _motionPointManager.StartDetect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _motionPointManager.StopDetect();
        }
        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            this.InfoText.Clear();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            _motionPointManager.StartCalibrate();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            _motionPointManager.EndCalibrate();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SetRobotPattern("playhockey");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SetRobotPattern("drawing");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SetRobotPattern("dancing");
        }
    }
}
