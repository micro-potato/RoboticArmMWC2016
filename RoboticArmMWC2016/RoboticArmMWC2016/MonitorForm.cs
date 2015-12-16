using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MotionDetection;

namespace RoboticArmMWC2016
{
    public partial class MonitorForm : Form
    {
        private Camera _camera;
        private CameraMotionManager _cmm;
        public Camera CamerainWindow
        {
            get { return _camera; }
        }
        public MonitorForm(Camera camera,CameraMotionManager cmm)
        {
            InitializeComponent();
            _camera = camera;
            _cmm = cmm;
            _cmm.DetectOnce += new CameraMotionManager.DetectHandle(_cmm_DetectOnce);
        }

        void _cmm_DetectOnce(Bitmap detectedFrame)
        {
            this.pictureBox1.Image = detectedFrame;
        }

        private void MonitorForm_Load(object sender, EventArgs e)
        {
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            cameraWindow1.Camera = _camera;
        }
    }
}
