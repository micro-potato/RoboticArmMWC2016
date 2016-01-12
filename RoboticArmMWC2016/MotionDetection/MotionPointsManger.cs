using Helpers;
using MotionDetection.Moudle;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TUIO;

namespace MotionDetection
{
    public class MotionPointsManger : TuioListener
    {
        private MotionEndPointCalc _motionEndPointCalc;
        private object _iceballObject = new object();

        private Dictionary<long, TuioCursor> _cursorList;
        private Dictionary<long, TuioBlob> blobList;
        //private int _cameraFps;

        private int _detectFrequency;
        private int _detectWidth;
        private int _detectHeight;

        public int DetectWidth { set { _detectWidth = value; } }
        public int DetectHeight { set { _detectHeight = value; } }

        private int _ignoreTimes = 0;
        private MotionResult _lastResult;

        public delegate void MotionResultHandler (MotionResult result);
        public event MotionResultHandler ValidResultGot;

        //timer
        private System.Timers.Timer _calibrateTimer;
        //private System.Timers.Timer _detectTimer;

        //async
        bool _isDetecting = false;
        
        public MotionPointsManger(int detectFrefency)
        {
            _detectFrequency = detectFrefency;
            _motionEndPointCalc = new MotionEndPointCalc();
            _calibrateTimer = new System.Timers.Timer(detectFrefency);
            _calibrateTimer.Elapsed += CalibrateTimer_Elapsed;
            InitTUIO();
        }

        #region 校对
        public void StartCalibrate()
        {
            _calibrateTimer.Start();
        }

        public void EndCalibrate()
        {
            _calibrateTimer.Stop();
        }

        void CalibrateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_cursorList.Count == 0)
            {
                return;
            }
            lock (_iceballObject)
            {
                var lastPoint = _cursorList.Values.ToList()[_cursorList.Count - 1];//最新加入的点
                var xLocation = lastPoint.X;
                var yLocation=lastPoint.Y;
                var tableX = ActualX(xLocation);
                var tableY = ActualY(yLocation);
                MotionResult result = new MotionResult() { EndPointX = tableX, ReachTime = 50 };
                if (ValidResultGot != null)//机械臂移动到对应X位置
                {
                    ValidResultGot(result);
                    LogHelper.GetInstance().ShowMsg(string.Format("当前计算出的位置：{0},{1}cm\n", tableX.ToString("f1"),tableY.ToString("f1")));
                    Thread.Sleep(1000);
                }
            }
        }
        #endregion

        #region 运动捕捉和位置计算
        public  void StartDetect()
        {
            _isDetecting = true;
            DetectIceballMovingAsync();
        }

        public void StopDetect()
        {
            _isDetecting = false;
        }

       System.Diagnostics.Stopwatch _sw = new System.Diagnostics.Stopwatch();

       void DetectIceballMovingAsync()
        {
            Task.Run(() =>
            {
                while (_isDetecting)
                {
                    DetectOnce();
                    Thread.Sleep(10);
                }
            });
        }

       void DetectOnce()
       {
           try
           {
               lock (_iceballObject)
               {
                   if (_cursorList.Count == 0)//未检测到非背景点
                   {
                       _lastResult = null;
                       _sw.Reset();
                       return;
                   }
                   _sw.Start();

                   TuioCursor iceball = SearchMovingIceball();
                   if (iceball == null || iceball.Path.Count == 0)//没有找到冰球
                   {
                       //LogHelper.GetInstance().ShowMsg(string.Format("没有检测到冰球，用时{0}\n", _sw.ElapsedMilliseconds));
                       return;
                   }
                   var startPoint = iceball.Path[0];
                   var endPoint = iceball.Path[iceball.Path.Count - 1];
                   //LogHelper.GetInstance().ShowMsg(string.Format("初始位置：{0},{1}，移动到：{2},{3}",startPoint.X,startPoint.Y,endPoint.X,endPoint.Y));

                   //获取换算后的实际位置
                   double x1 = ActualX(startPoint.X);
                   double y1 = ActualY(startPoint.Y);
                   double x2 = ActualX(endPoint.X);
                   double y2 = ActualY(endPoint.Y);

                   var result = _motionEndPointCalc.CalcEndPoint(x1, y1, x2, y2, (_ignoreTimes + 1) * _detectFrequency);
                   if (_lastResult == null)//本次运动的第1个检测结果
                   {
                       if (ValidResultGot != null)
                       {
                           LogHelper.GetInstance().ShowMsg(string.Format("已检测到第一个运动点{0}\n", result.EndPointX));
                          // LogHelper.GetInstance().ShowMsg(_sw.ElapsedMilliseconds.ToString() + "\n");
                           _lastResult = result;
                           ValidResultGot(result);
                       }
                   }
                   else
                   {
                       if (Math.Abs(result.EndPointX - _lastResult.EndPointX) <= 15 /*&& Math.Abs(result.ReachTime - _lastResult.ReachTime) <= 500*/)//忽略相邻近似结果
                       {
                           _ignoreTimes++;
                           //LogHelper.GetInstance().ShowMsg("忽略结果，已忽略：" + _ignoreTimes.ToString() + "\n");
                       }
                       else//更新检测结果
                       {
                           if (ValidResultGot != null)
                           {
                               _lastResult = result;
                               //ValidResultGot(result);
                               _ignoreTimes = 0;
                               //LogHelper.GetInstance().ShowMsg(string.Format("初始位置：{0},{1}，移动到：{2},{3},用时：{4}==========", x1, y1, x2, y2,_sw.ElapsedMilliseconds));
                               //LogHelper.GetInstance().ShowMsg(string.Format("移动到 {0} 用时{1}\n", result.EndPointX, _sw.ElapsedMilliseconds));
                           }
                       }
                   }
               }
           }
           catch (Exception e)
           {
               LogHelper.GetInstance().ShowMsg(string.Format("捕捉冰球运动错误：" + e.Message));
           }
       }

       /// <summary>
       /// 寻找移动的冰球
       /// </summary>
       private TuioCursor SearchMovingIceball()
       {
           try
           {
               List<TuioCursor> validDirPoints = new List<TuioCursor>();
               TuioCursor iceBall;
               var cursors = _cursorList.Values.ToList();
               for (int i = 0; i < cursors.Count; i++)//筛选出向机械臂移动的点
               {
                   var point = cursors[i];
                   var pathCount = point.Path.Count;
                   var currentY = point.Y;
                   var dy = point.Path[pathCount - 1].Y - point.Path[0].Y;
                   if (point.YSpeed < 1 && dy < -0.03 && currentY < 0.6)//速度&移动距离&当前位置
                   {
                       validDirPoints.Add(point);
                   }
               }
               if (validDirPoints.Count != 0)//有向机械臂移动的点
               {
                   var sortedPoints = (validDirPoints.OrderByDescending(p => p.Y)).ToList();//按水平方向距机械距离排序
                   iceBall = sortedPoints[0];
               }
               else
                   iceBall = null;
               return iceBall;
           }
           catch (Exception ex)
           {
               LogHelper.GetInstance().ShowMsg("获取冰球失败：" + ex.Message);
               return null;
           }
       }

        ///// <summary>
        ///// 寻找移动的冰球
        ///// </summary>
        //private TuioCursor SearchMovingIceball(TuioCursor[] cursors)
        //{
        //    try
        //    {
        //        List<TuioCursor> validDirPoints = new List<TuioCursor>();
        //        TuioCursor iceBall;

        //        for (int i = 0; i < cursors.Length; i++)//筛选出向机械臂移动的点
        //        {
        //            var point = cursors[i];
        //            var pathCount = point.Path.Count;
        //            var currentY = point.Y;
        //            var dy = point.Path[pathCount - 1].Y - point.Path[0].Y;
        //            if (point.YSpeed < 1 && dy < -0.05&&currentY<0.75)//速度&移动距离&当前位置
        //            {
        //                validDirPoints.Add(point);
        //            }
        //        }
        //        if (validDirPoints.Count != 0)//有向机械臂移动的点
        //        {
        //            var sortedPoints = (validDirPoints.OrderByDescending(p => p.Y)).ToList();//按水平方向距机械距离排序
        //            iceBall = sortedPoints[0];
        //        }
        //        else
        //            iceBall = null;
        //        return iceBall;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.GetInstance().ShowMsg("获取冰球失败：" + ex.Message);
        //        return null;
        //    }
        //}

        /// <summary>
        /// 根据摄像头位置计算实际Y坐标
        /// </summary>
        /// <param name="cameraX"></param>
        /// <param name="cameraX2"></param>
        /// <returns></returns>
        private double ActualY(double screenY)
        {
            return (1 - screenY) * _detectHeight;
        }

        /// <summary>
        /// 根据摄像头位置计算实际X坐标
        /// </summary>
        /// <param name="cameraY"></param>
        /// <returns></returns>
        private double ActualX(double screenX)
        {
            //return screenX * _detectWidth;
            return screenX * _detectWidth - _detectWidth / 2;
        }
        #endregion

        #region TUIO
        private void InitTUIO()
        {
            _cursorList = new Dictionary<long, TuioCursor>(128);
            TuioClient client = new TuioClient();
            client = new TuioClient(3333);
            client.addTuioListener(this);
            client.connect();
        }

        public void addTuioObject(TuioObject tobj)
        {
            Console.WriteLine("add obj " + tobj.SymbolID + " " + tobj.SessionID + " " + tobj.X + " " + tobj.Y + " " + tobj.Angle);
        }

        public void updateTuioObject(TuioObject tobj)
        {
            //Console.WriteLine("set obj " + tobj.SymbolID + " " + tobj.SessionID + " " + tobj.X + " " + tobj.Y + " " + tobj.Angle + " " + tobj.MotionSpeed + " " + tobj.RotationSpeed + " " + tobj.MotionAccel + " " + tobj.RotationAccel);
        }

        public void removeTuioObject(TuioObject tobj)
        {
            //Console.WriteLine("del obj " + tobj.SymbolID + " " + tobj.SessionID);
        }

        public void addTuioCursor(TuioCursor tcur)
        {
            lock (_iceballObject)
            {
                _cursorList.Add(tcur.SessionID, tcur);
            }
            Console.WriteLine("add cur " + tcur.CursorID + " (" + tcur.SessionID + ") " + tcur.X + " " + tcur.Y);
        }

        public void updateTuioCursor(TuioCursor tcur)
        {
            Console.WriteLine("set cur " + tcur.CursorID + " (" + tcur.SessionID + ") " + tcur.X + " " + tcur.Y + " " + tcur.MotionSpeed + " " + tcur.MotionAccel);
        }

        public void removeTuioCursor(TuioCursor tcur)
        {
            lock (_iceballObject)
            {
                _cursorList.Remove(tcur.SessionID);
            }
            Console.WriteLine("del cur " + tcur.CursorID + " (" + tcur.SessionID + ")");
        }

        public void addTuioBlob(TuioBlob b)
        {
            lock (blobList)
            {
                blobList.Add(b.SessionID, b);
            }
            Console.WriteLine("add blb " + b.BlobID + " (" + b.SessionID + ") " + b.X + " " + b.Y + " " + b.Angle + " " + b.Width + " " + b.Height + " " + b.Area);
        }

        public void updateTuioBlob(TuioBlob tblb)
        {
            Console.WriteLine("set blb " + tblb.BlobID + " (" + tblb.SessionID + ") " + tblb.X + " " + tblb.Y + " " + tblb.Angle + " " + tblb.Width + " " + tblb.Height + " " + tblb.Area + " " + tblb.MotionSpeed + " " + tblb.RotationSpeed + " " + tblb.MotionAccel + " " + tblb.RotationAccel);
        }

        public void removeTuioBlob(TuioBlob b)
        {
            lock (blobList)
            {
                blobList.Remove(b.SessionID);
            }
            Console.WriteLine("del blb " + b.BlobID + " (" + b.SessionID + ")");
        }

        public void refresh(TuioTime ftime)
        {
            //
        }
        #endregion      
    }
}
