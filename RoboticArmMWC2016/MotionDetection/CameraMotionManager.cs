using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MotionDetection
{
    public enum DetectDirection { X, Y }
    public class CameraMotionManager
    {
        private Camera _camera;
        private MotionDetector _detector;
        private System.Timers.Timer _detectTimer;
        private Bitmap _lastDetectedFrame;
        public delegate void DetectHandle(Bitmap detectedFrame);
        public event DetectHandle DetectOnce;

        public CameraMotionManager(Camera camera,int interval,DetectDirection detectDirction)
        {
            _detector = new MotionDetector();
            _detector.DetectDirection = detectDirction;
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
                    _detector.ProcessFrame(ref _lastDetectedFrame);
                    if (DetectOnce != null)
                    {
                        DetectOnce((Bitmap)_lastDetectedFrame.Clone());
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
