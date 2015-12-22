using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MotionDetection.Moudle;

namespace MotionDetection
{
    public class CameraMotionDetector
    {
        private Camera _camera;
        private ColorSelecter _colorDetector;
        private System.Timers.Timer _detectTimer;
        private Bitmap _lastDetectedFrame;

        public delegate void CenterPointHandler(List<Point> points);
        public event CenterPointHandler CenterPointGot;

        public CameraMotionDetector(Camera camera,int interval,MotionColorInfo motionColorInfo)
        {
            _colorDetector = new ColorSelecter(motionColorInfo);
            _camera = camera;
            _detectTimer = new System.Timers.Timer(interval);
            _detectTimer.Elapsed += new System.Timers.ElapsedEventHandler(DetectTimer_Elapsed);
            _detectTimer.Start();
        }

        void DetectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                System.Threading.Monitor.Enter(this);
                if (_lastDetectedFrame != null)
                {
                    _lastDetectedFrame.Dispose();
                }
                _camera.Lock();
                if (_camera.LastFrame != null)
                {
                    _lastDetectedFrame = (Bitmap)_camera.LastFrame.Clone();
                    _camera.Unlock();
                   var cmotionPoints= _colorDetector.DetectColorPoints(_lastDetectedFrame);
                   if (CenterPointGot != null)
                   {
                       CenterPointGot(cmotionPoints);
                   }
                }
                else
                {
                    _camera.Unlock();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取运动点失败：" + ex.Message);
            }
            finally
            {
                System.Threading.Monitor.Exit(this);
            }
        }
    }
}
