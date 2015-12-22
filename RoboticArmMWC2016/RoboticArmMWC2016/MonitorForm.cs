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
using MotionDetection.Moudle;

namespace RoboticArmMWC2016
{
    public partial class MonitorForm : Form
    {
        private Camera _camera;
        private Bitmap _lastFrame;
        private ColorSelecter _colorSelecter;
        public MotionColorInfo _detectedColorInfo;

        public Camera CamerainWindow
        {
            get { return _camera; }
        }
        public MonitorForm(Camera camera,MotionColorInfo colorInfo)
        {
            InitializeComponent();
            _detectedColorInfo = colorInfo;
            _camera = camera;
            _camera.NewFrame += _camera_NewFrame;
            _colorSelecter = new ColorSelecter(_detectedColorInfo);
        }

        void _camera_NewFrame(object sender, EventArgs e)
        {
            Monitor.Enter(this);
            if (_lastFrame!=null)
            {
                _lastFrame.Dispose();
            }
            _camera.Lock();
            _lastFrame = (Bitmap)_camera.LastFrame.Clone();
            var points=_colorSelecter.DetectColorPoints(_lastFrame);
            var pointsCount = points.Count;
            Bitmap bm = new Bitmap(_camera.Width, _camera.Height);
            Graphics g = Graphics.FromImage(bm);
            g.FillRectangle(Brushes.Black, new Rectangle(0, 0, _camera.Width, _camera.Height));
            for (int i = 0; i < pointsCount; i++)
            {
                var x = points[i].X;
                var y = points[i].Y;
                bm.SetPixel(x, y, Color.White);
            }
            this.pictureBox1.Image = bm;
            _camera.Unlock();
            Monitor.Exit(this);
        }

        private void MonitorForm_Load(object sender, EventArgs e)
        {
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            cameraWindow1.Camera = _camera;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int x = int.Parse(this.textBox1.Text);
            int y = int.Parse(this.textBox2.Text);
            var color = _camera.GetColorofPoint(x, y);
            this.labelR.Text = color.R.ToString();
            this.labelG.Text = color.G.ToString();
            this.labelB.Text = color.B.ToString();
        }
    }
}
