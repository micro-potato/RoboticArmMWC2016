using Helpers;
using MotionDetection.Moudle;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using TUIO;

namespace MotionDetection
{
    public class MotionPointsManger : TuioListener
    {
        private MotionEndPointCalc _motionEndPointCalc;
        private object _object = new object();

        private Dictionary<long, TuioCursor> _cursorList;
        private Dictionary<long, TuioBlob> blobList;
        private int _cameraFps;
        private System.Timers.Timer _detectTimer;
        private System.Timers.Timer _calibrateTimer;

        private int _detectFrequency;
        private int _detectWidth;
        private int _detectHeight;

        public int DetectWidth { set { _detectWidth = value; } }
        public int DetectHeight { set { _detectHeight = value; } }

        private int _ignoreTimes = 0;
        private MotionResult _lastResult;

        public delegate void MotionResultHandler (MotionResult result);
        public event MotionResultHandler ValidResultGot;

        public MotionPointsManger(int detectFrefency)
        {
            _detectFrequency = detectFrefency;
            _motionEndPointCalc = new MotionEndPointCalc();
            _detectTimer = new System.Timers.Timer(detectFrefency);
            _detectTimer.Elapsed += DetectTimer_Elapsed;
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
            lock (_object)
            {
                var lastPoint = _cursorList.Values.ToList()[_cursorList.Count - 1];//最新加入的点
                var xLocation = lastPoint.X;
                var tableX = ActualX(xLocation);
                MotionResult result = new MotionResult() { EndPointX = tableX, ReachTime = 50 };
                if (ValidResultGot != null)
                {
                    ValidResultGot(result);
                    LogHelper.GetInstance().ShowMsg(string.Format("当前计算出的位置：{0}cm", result.EndPointX));
                }
            }
        }
        #endregion

        #region 运动捕捉和位置计算
        public void StartDetect()
        {
            _detectTimer.Start();
        }

        public void StopDetect()
        {
            _detectTimer.Stop();
        }

        void DetectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_object)
            {
                if (_cursorList.Count == 0)//未检测到非背景点
                {
                    return;
                }
                TuioCursor[] detectedPoints=new TuioCursor[_cursorList.Count];
                _cursorList.Values.ToList<TuioCursor>().CopyTo(detectedPoints);
                TuioCursor iceball= SearchMovingIceball(detectedPoints);
                if (iceball == null)//没有找到冰球
                {
                    return;
                }
                var  startPoint = iceball.Path[0];
                var endPoint=iceball.Path[iceball.Path.Count-1];
                //LogHelper.GetInstance().ShowMsg(string.Format("初始位置：{0},{1}，移动到：{2},{3}",startPoint.X,startPoint.Y,endPoint.X,endPoint.Y));
                double x1 = ActualX(startPoint.X);
                double y1 = ActualY(startPoint.Y);
                double x2 = ActualX(endPoint.X);
                double y2 = ActualY(endPoint.Y);
                var result = _motionEndPointCalc.CalcEndPoint(x1, y1, x2, y2,(_ignoreTimes+1)*_detectFrequency);
                if (_lastResult == null)//第1个检测结果
                {
                    if (ValidResultGot != null)
                    {
                        _lastResult = result;
                        //ValidResultGot(result);//忽略第一个测量结果
                    }
                }
                else
                {
                    if (Math.Abs(result.EndPointX - _lastResult.EndPointX) <= 5 /*&& Math.Abs(result.ReachTime - _lastResult.ReachTime) <= 500*/)//忽略相邻近似结果
                    {
                        _ignoreTimes++;
                        //LogHelper.GetInstance().ShowMsg("忽略结果，已忽略：" + _ignoreTimes.ToString() + "\n");
                    }
                    else//更新检测结果
                    {
                        if (ValidResultGot != null)
                        {
                            _lastResult = result;
                            LogHelper.GetInstance().ShowMsg(string.Format("初始位置：{0},{1}，移动到：{2},{3}==========", x1, y1, x2, y2));
                            ValidResultGot(result);
                            _ignoreTimes = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 寻找移动的冰球
        /// </summary>
        private TuioCursor SearchMovingIceball(TuioCursor[] cursors)
        {
            try
            {
                List<TuioCursor> validDirPoints = new List<TuioCursor>();
                TuioCursor iceBall;

                for (int i = 0; i < cursors.Length; i++)//筛选出向机械臂移动的点
                {
                    var point = cursors[i];
                    var pathCount = point.Path.Count;
                    var dy = point.Path[pathCount - 1].Y - point.Path[0].Y;
                    if (point.YSpeed < 1 && dy < -0.02)
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
            return screenX * _detectWidth;
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
            lock (_cursorList)
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
            lock (_cursorList)
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
