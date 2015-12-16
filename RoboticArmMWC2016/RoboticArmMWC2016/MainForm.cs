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

namespace RoboticArmMWC2016
{
    public partial class MainForm : Form
    {
        private int _cameraXIndex;
        private int _cameraYIndex;
        private int _cameraX2Index;

        private Camera _cameraX;
        private Camera _cameraY;
        private Camera _cameraX2;

        CameraMotionManager _cmmX;
        private List<string> _cameraList;

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
            MonitorForm frm = new MonitorForm(camera,_cmmX);
            frm.Show();
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            //InitCameras();
            InitCamerDeviceList();
            InitXCamera();
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
            InitYCamera();
            InitXCamera();
            InitX2Camera();
        }

        private void InitXCamera()//捕捉X轴摄像头
        {
            InitCamera(_cameraXIndex, ref _cameraX);
            _cmmX = new CameraMotionManager(_cameraX, 2000,DetectDirection.X);
            _cmmX.DetectOnce += new CameraMotionManager.DetectHandle(CameraX_DetectOnce);
        }

        void CameraX_DetectOnce(Bitmap detectedFrame)
        {
            //throw new NotImplementedException();
        }

        private void InitYCamera()//捕捉Y轴摄像头
        {
            throw new NotImplementedException();
        }

        private void InitX2Camera()//捕捉Y轴补充摄像头
        {
            throw new NotImplementedException();
        }

        private void InitCamera(int cameraIndex,ref Camera cameratoInit)
        {
            // create video source
            CaptureDevice videoSource = new CaptureDevice();
            videoSource.VideoSource = _cameraList[cameraIndex];
            cameratoInit = new Camera(videoSource);
            cameratoInit.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
