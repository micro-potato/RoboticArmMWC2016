using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoSource;
using dshow;
using dshow.Core;
using MotionDetection;
using System.Threading;
using Helpers;
using MotionDetection.Moudle;
using ServerDLL;
using System.Diagnostics;

namespace RoboticArmMWC2016
{
    public partial class MainForm : Form,ILog
    {
        private Camera _camera;
        private int _cameraFpf;
        ConfigHelper _config;
        private List<string> _cameraList;
        private MotionColorInfo _motionColorInfo;
        private MotionPointsManger _motionPointManager;
        private RoboticArm.RobotHandler _robotHandler;
        private ColorSelecter _colorSelector;
        private AsyncServer asyncServer = new AsyncServer();
        private List<int> m_ClientIndexs = new List<int>();

        //模拟数据
        private System.Timers.Timer _simTimer;
        public MainForm()
        {
            InitializeComponent();
        }

        #region 监视画面
        private void buttonX_Click(object sender, EventArgs e)
        {
            MonitorForm frm = new MonitorForm(_camera, _motionColorInfo);
            frm.Show();
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitConfig();
            _motionColorInfo = new MotionColorInfo() { RValue = _config.RValue, GValue = _config.GValue, BValue = _config.BValue, ColorThreshold = _config.ColorThreshold };
            _colorSelector = new ColorSelecter(_motionColorInfo);
            _motionPointManager = new MotionPointsManger();
            InitRoboticArm();
            InitCamerDeviceList();
            InitCamera(_config.CameraID,ref _camera);
            LogHelper.GetInstance().RegLog(this);
            InitServer();
            //模拟数据
            _simTimer = new System.Timers.Timer(1000/_config.CameraFps);
            _simTimer.Elapsed += _simTimer_Elapsed;
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
            string message = string.Format("{0}连线!", ip);
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
            string message = string.Format("{0}断线!", ip);
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
            
        }
        #endregion

        private void InitRoboticArm()
        {
            _robotHandler = new RoboticArm.RobotHandler(_config.RobotIP, _config.RobotPort);
            _robotHandler.MaxVelocity = _config.ReflectVelocity;
        }

        private void InitCamerDeviceList()
        {
            FilterCollection filters = new FilterCollection(FilterCategory.VideoInputDevice);
            if (filters.Count == 0)
                throw new ApplicationException();

            _cameraList=new List<string>();
            for (int i = 0; i < filters.Count; i++)
            {
                _cameraList.Add(filters[i].MonikerString);
            }
        }

        private void InitCamera(string cameraIndex,ref Camera cameratoInit)
        {
            // create video source
            if (cameraIndex == "-1")//没有摄像头
            {
                return;
            }
            CaptureDevice videoSource = new CaptureDevice();
            videoSource.VideoSource = _cameraList[int.Parse(cameraIndex)];
            cameratoInit = new Camera(videoSource);
            _cameraFpf = _config.CameraFps;
            cameratoInit.Start();
            cameratoInit.NewFrame += Camera_NewFrame;
        }

        void Camera_NewFrame(object sender, EventArgs e)
        {
            _camera.Lock();
            var cameraImage =(Bitmap) _camera.LastFrame.Clone();
            var iceballPoints = _colorSelector.DetectColorPoints(cameraImage);
            var result = _motionPointManager.UpdateResult();
            if (result != null)
            {
                _robotHandler.MoveArm(result.EndPointX, result.MoveVelocityY, result.DistancetoY);
            }
            _camera.Unlock();
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
            var xEnd = new Random().Next(92);
            var distance = new Random().Next(100);
            double velocity = ((double)(new Random().Next(20)) / 20) * 100 / 100;
            _robotHandler.MoveArm(xEnd, 0.8, distance);
            var waitforNext = new Random().Next(1000);
            _simTimer.Interval = waitforNext;
            _simTimer.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _robotHandler.MoveArm(double.Parse(this.textBox1.Text), 0.8, 0.8);
        }
        #endregion

        private delegate void DeleString(string arg);
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

        
    }
}
