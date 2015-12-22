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

namespace RoboticArmMWC2016
{
    public partial class MainForm : Form
    {
        private Camera _cameraX;
        private Camera _cameraY;
        private Camera _cameraX2;
        private int _detectFrequency = 17;//监测频率，毫秒
        ConfigHelper _config;
        private List<string> _cameraList;
        private MotionColorInfo _motionColorInfo;
        private MotionPointsManger _motionPointManager;
        private RoboticArm.RobotHandler _robotHandler;

        //模拟数据
        private System.Timers.Timer _simTimer;
        public MainForm()
        {
            InitializeComponent();
        }

        #region 监视画面
        private void buttonX_Click(object sender, EventArgs e)
        {
            ShowMotionDection(_cameraX);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ShowMotionDection(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ShowMotionDection(2);
        }


        private void ShowMotionDection(Camera camera)
        {
            MonitorForm frm = new MonitorForm(camera,_motionColorInfo);
            frm.Show();
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitConfig();
            _detectFrequency = _config.DetectFrequency;
            _motionColorInfo = new MotionColorInfo() { RValue = _config.RValue, GValue = _config.GValue, BValue = _config.BValue, ColorThreshold = _config.ColorThreshold };
            _motionPointManager = new MotionPointsManger();
            InitRoboticArm();
            InitCameras();

            _simTimer = new System.Timers.Timer(_config.DetectFrequency);
            _simTimer.Elapsed += _simTimer_Elapsed;
        }

        private void InitRoboticArm()
        {
            _robotHandler = new RoboticArm.RobotHandler(_config.IP, _config.Port);
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

        private void InitCameras()
        {
            InitCamerDeviceList();
            InitXCamera();
            InitYCamera();
            InitX2Camera();
        }

        private void InitXCamera()//捕捉X轴摄像头
        {
            InitCamera(_config.CameraXID, ref _cameraX);
            CameraMotionDetector _cmdtorX = new CameraMotionDetector(_cameraX,_detectFrequency ,_motionColorInfo);
            _cmdtorX.CenterPointGot += CameraX_CenterPointGot;
        }

        private void InitYCamera()//捕捉Y轴摄像头
        {
            InitCamera(_config.CameraYID, ref _cameraY);
            CameraMotionDetector _cmdtorY = new CameraMotionDetector(_cameraY, _detectFrequency, _motionColorInfo);
            _cmdtorY.CenterPointGot += CmdtorY_CenterPointGot;
        }

        private void InitX2Camera()//捕捉Y轴补充摄像头
        {
            InitCamera(_config.CameraX2ID, ref _cameraX2);
            CameraMotionDetector _cmdorX2 = new CameraMotionDetector(_cameraX2, _detectFrequency, _motionColorInfo);
            _cmdorX2.CenterPointGot += CmdorX2_CenterPointGot;
        }

        private void InitCamera(string cameraIndex,ref Camera cameratoInit)
        {
            // create video source
            if (cameraIndex == "-1")//没有该位置摄像头
            {
                return;
            }
            CaptureDevice videoSource = new CaptureDevice();
            videoSource.VideoSource = _cameraList[int.Parse(cameraIndex)];
            cameratoInit = new Camera(videoSource);
            cameratoInit.Start();
        }

        void CameraX_CenterPointGot(List<Point> pointValue)
        {
            //_motionPointManager.AddPoint(MotionPointEnum.Xpoint, pointValue);
        }

        void CmdtorY_CenterPointGot(List<Point> pointValue)
        {
            //_motionPointManager.AddPoint(MotionPointEnum.Ypont, pointValue);
            //_motionPointManager.UpdateResult();
        }

        void CmdorX2_CenterPointGot(List<Point> pointValue)
        {
            //_motionPointManager.AddPoint(MotionPointEnum.X2point, pointValue);
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 指定坐标移动
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            var x = textBox1.Text;
            var y = textBox2.Text;
            _robotHandler.MoveArm(double.Parse(x), double.Parse(y));
        }

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
            var x = new Random().Next(92);
            _robotHandler.MoveArm(x, 0);
        }
    }
}
