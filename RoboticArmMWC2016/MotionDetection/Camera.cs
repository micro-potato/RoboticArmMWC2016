// Motion Detector
//
// Copyright ?Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//
namespace MotionDetection
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Threading;
    using VideoSource;

    /// <summary>
    /// Camera class
    /// </summary>
    public class Camera
    {
        private IVideoSource videoSource = null;
        private ColorSelecter motionDetecotor = null;
        private Bitmap lastFrame = null;

        // image width and height
        private int width = -1, height = -1;

        // alarm level
        //private double	alarmLevel = 0.005;

        //
        public event EventHandler NewFrame;
        //public event EventHandler	Alarm;

        // LastFrame property
        public Bitmap LastFrame
        {
            get { return lastFrame; }
        }
        // Width property
        public int Width
        {
            get { return width; }
        }
        // Height property
        public int Height
        {
            get { return height; }
        }
        // FramesReceived property
        public int FramesReceived
        {
            get { return (videoSource == null) ? 0 : videoSource.FramesReceived; }
        }
        // BytesReceived property
        public int BytesReceived
        {
            get { return (videoSource == null) ? 0 : videoSource.BytesReceived; }
        }
        // Running property
        public bool Running
        {
            get { return (videoSource == null) ? false : videoSource.Running; }
        }
        // MotionDetector property
        public ColorSelecter MotionDetector
        {
            get { return motionDetecotor; }
            set { motionDetecotor = value; }
        }

        // Constructor
        public Camera(IVideoSource source)
        {
            this.videoSource = source;
            videoSource.NewFrame += new CameraEventHandler(video_NewFrame);
        }

        // Start video source
        public void Start()
        {
            if (videoSource != null)
            {
                videoSource.Start();
            }
        }

        // Siganl video source to stop
        public void SignalToStop()
        {
            if (videoSource != null)
            {
                videoSource.SignalToStop();
            }
        }

        // Wait video source for stop
        public void WaitForStop()
        {
            // lock
            Monitor.Enter(this);

            if (videoSource != null)
            {
                videoSource.WaitForStop();
            }
            // unlock
            Monitor.Exit(this);
        }

        // Abort camera
        public void Stop()
        {
            // lock
            Monitor.Enter(this);

            if (videoSource != null)
            {
                videoSource.Stop();
            }
            // unlock
            Monitor.Exit(this);
        }

        // Lock it
        public void Lock()
        {
            Monitor.Enter(this);
        }

        // Unlock it
        public void Unlock()
        {
            Monitor.Exit(this);
        }

        // On new frame
        private void video_NewFrame(object sender, CameraEventArgs e)
        {
            try
            {
                // lock
                Monitor.Enter(this);

                // dispose old frame
                if (lastFrame != null)
                {
                    lastFrame.Dispose();
                }

                lastFrame = (Bitmap)e.Bitmap.Clone();

                // image dimension
                width = lastFrame.Width;
                height = lastFrame.Height;
            }
            catch (Exception)
            {
            }
            finally
            {
                // unlock
                Monitor.Exit(this);
            }
            // notify client
            if (NewFrame != null)
                NewFrame(this, new EventArgs());
        }

        public Color GetColorofPoint(int x, int y)
        {
            try
            {
                Monitor.Enter(this);
                Bitmap bitmap = (Bitmap)this.lastFrame.Clone();
                var width = bitmap.Width;
                var height = bitmap.Height;
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                #region 颜色点
                Color clr = Color.Empty;
                var depth = Bitmap.GetPixelFormatSize(bitmap.PixelFormat);
                int cCount = depth / 8;
                var pixels = new byte[this.width * this.height * cCount];
                var Iptr = bitmapData.Scan0;
                var t = ((y * this.width) + x) * cCount;
                // Copy data from pointer to array
                Marshal.Copy(Iptr, pixels, 0, pixels.Length);
                if (depth == 24) // For 24 bpp get Red, Green and Blue
                {
                    byte b = pixels[t];
                    byte g = pixels[t + 1];
                    byte r = pixels[t + 2];
                    clr = Color.FromArgb(r, g, b);
                }
                #endregion
                Monitor.Exit(this);
                bitmap.Dispose();
                return clr;
            }
            catch (Exception e)
            {
                throw new Exception("未能获取颜色：" + e.Message);
            }
        }
    }
}
