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

    /// <summary>
    /// 监测摄像头内的运动
    /// </summary>
    public class MotionDetector
    {
        private IFilter grayscaleFilter = new GrayscaleBT709();
        private Difference differenceFilter = new Difference();
        private Threshold thresholdFilter = new Threshold(40);
        private IFilter erosionFilter = new Erosion();
        private Merge mergeFilter = new Merge();

        private IFilter extrachChannel = new ExtractChannel(RGB.R);
        private ReplaceChannel replaceChannel = new ReplaceChannel(RGB.R, null);

        private Bitmap backgroundFrame;
        private BitmapData bitmapData;

        private bool calculateMotionLevel = false;
        private int width;	// image width
        private int height;	// image height

        private DetectDirection _detectDirection=DetectDirection.X;
        public DetectDirection DetectDirection
        {
            set { _detectDirection = value; }
        }

        // Constructor
        public MotionDetector()
        {
        }

        // Reset detector to initial state
        public void Reset()
        {
            if (backgroundFrame != null)
            {
                backgroundFrame.Dispose();
                backgroundFrame = null;
            }
        }

        // Process new frame
        public void ProcessFrame(ref Bitmap image)
        {
            if (backgroundFrame == null)
            {
                // create initial backgroung image
                backgroundFrame = grayscaleFilter.Apply(image);

                // get image dimension
                width = image.Width;
                height = image.Height;

                // just return for the first time
                return;
            }

            Bitmap tmpImage;

            // apply the grayscale file
            tmpImage = grayscaleFilter.Apply(image);

            // set backgroud frame as an overlay for difference filter
            differenceFilter.OverlayImage = backgroundFrame;

            // apply difference filter
            Bitmap tmpImage2 = differenceFilter.Apply(tmpImage);

            // lock the temporary image and apply some filters on the locked data
            bitmapData = tmpImage2.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            // threshold filter
            thresholdFilter.ApplyInPlace(bitmapData);
            //thresholdFilter.ExtractChangedPoints(bitmapData);

            #region 变化点
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Color clr = Color.Empty;
            var depth = Bitmap.GetPixelFormatSize(tmpImage2.PixelFormat);
            int cCount = depth / 8;
            var pixels = new byte[this.width * this.height * cCount];
            var Iptr = bitmapData.Scan0;

            // Copy data from pointer to array
            Marshal.Copy(Iptr, pixels, 0, pixels.Length);
            Dictionary<int, int> motionPoints = new Dictionary<int, int>();
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
                    if (clr.R == 255 && clr.G == 255 && clr.B == 255)//白色
                    {
                        if (_detectDirection == DetectDirection.X)
                        {
                            if (!motionPoints.ContainsKey(x))
                            {
                                motionPoints.Add(x, 0);
                            }
                        }
                        else
                        {
                            if (!motionPoints.ContainsKey(y))
                            {
                                motionPoints.Add(y, 0);
                            }
                        }
                    }
                    x++;
                }
                y++;
            }
            Console.WriteLine(string.Format("Found {0} motion points in {1} miliseconds", motionPoints.Count, sw.Elapsed.TotalMilliseconds));//all 255 is white
            sw.Stop();
            #endregion

            tmpImage2.UnlockBits(bitmapData);

            // dispose old background
            backgroundFrame.Dispose();
            // set backgound to current
            backgroundFrame = tmpImage;
            image.Dispose();
            image = tmpImage2;
        }

        // Calculate white pixels
        private int CalculateWhitePixels(Bitmap image)
        {
            int count = 0;

            // lock difference image
            BitmapData data = image.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

            int offset = data.Stride - width;

            unsafe
            {
                byte* ptr = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, ptr++)
                    {
                        count += ((*ptr) >> 7);
                    }
                    ptr += offset;
                }
            }
            // unlock image
            image.UnlockBits(data);

            return count;
        }
    }

    static class DetectMotionExtendMethods
    {
        public unsafe static int[,] ExtractChangedPoints(this Threshold threshold, BitmapData imageData)
        {
            //threshold.ApplyInPlace(imageData);
            //return null;
            if (imageData.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new ArgumentException("The filter can be applied to graysclae (8bpp indexed) image only");
            }
            object obj;
            int width = imageData.Width;
            int height = imageData.Height;
            int stride = imageData.Stride - width;
            byte* pointer = (byte*)imageData.Scan0.ToPointer();
            for (int i = 0; i < height; i++)
            {
                int num = 0;
                while (num < width)
                {
                    byte* numPointer = pointer;
                    if (*pointer >= threshold.ThresholdValue)
                    {
                        obj = 255;
                    }
                    else
                    {
                        obj = null;
                    }
                    *numPointer = (byte)obj;
                    num++;
                    pointer++;
                }
                pointer = pointer + stride;
            }
            return null;
        }
    }
}
