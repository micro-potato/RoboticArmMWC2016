namespace MotionDetection
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Diagnostics;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using MotionDetection.Moudle;

    /// <summary>
    /// 监测摄像头内的运动
    /// </summary>
    public class ColorSelecter
    {
        private int width;	// image width
        private int height;	// image height
        private MotionColorInfo _motionColorInfo;

        // Constructor
        public ColorSelecter(MotionColorInfo motionColorinfo)
        {
            _motionColorInfo = motionColorinfo;
        }

        public List<Point> DetectColorPoints(Bitmap image)
        {
            this.width = image.Width;
            this.height = image.Height;
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            #region 颜色点
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Color clr = Color.Empty;
            var depth = Bitmap.GetPixelFormatSize(image.PixelFormat);
            int cCount = depth / 8;
            var pixels = new byte[this.width * this.height * cCount];
            var Iptr = bitmapData.Scan0;

            // Copy data from pointer to array
            Marshal.Copy(Iptr, pixels, 0, pixels.Length);
            System.Collections.Generic.List<Point> motionPoints = new List<Point>();
            for (int y = 0; y < height; y++)
            {
                int x = 0;
                while (x < width)
                {
                    int t = ((y * this.width) + x) * cCount;

                    if (t > pixels.Length - cCount)
                        throw new IndexOutOfRangeException();

                    if (depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
                    {
                        byte b = pixels[t];
                        byte g = pixels[t + 1];
                        byte r = pixels[t + 2];
                        byte a = pixels[t + 3]; // a
                        clr = Color.FromArgb(a, r, g, b);
                    }
                    if (depth == 24) // For 24 bpp get Red, Green and Blue
                    {
                        byte b = pixels[t];
                        byte g = pixels[t + 1];
                        byte r = pixels[t + 2];
                        clr = Color.FromArgb(r, g, b);
                    }
                    if (depth == 8)
                    // For 8 bpp get color value (Red, Green and Blue values are the same)
                    {
                        byte c = pixels[t];
                        clr = Color.FromArgb(c, c, c);
                    }
                    if (Math.Abs(clr.R - this._motionColorInfo.RValue) <= this._motionColorInfo.ColorThreshold && Math.Abs(clr.G - this._motionColorInfo.GValue) <= this._motionColorInfo.ColorThreshold && Math.Abs(clr.B - this._motionColorInfo.BValue) <= this._motionColorInfo.ColorThreshold)//选定颜色
                    {
                        Point point = new Point(x, y);
                        motionPoints.Add(point);
                    }
                    x++;
                }
                y++;
            }
            Console.WriteLine(string.Format("Found {0} motion points in {1} miliseconds", motionPoints.Count, sw.Elapsed.TotalMilliseconds));
            sw.Stop();
            return motionPoints;
            #endregion
        }

        private int CenterPointValue(Dictionary<int, int> motionPoints)
        {
            //motionPoints.
            return 0;//do it later
        }
    }
}
