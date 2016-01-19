using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Auxiliary
{
    /// <summary>
    /// 日志文件输出类
    /// </summary>
    public class Log
    {
        private static Log instance;

        /// <summary>
        /// 获取当前类的实例
        /// </summary>
        public static Log Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Log();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }


        private string logPath = System.Windows.Forms.Application.StartupPath + "\\Log\\Log-" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        /// <summary>
        /// 初始化日志
        /// </summary>
        public void Initialize()
        {
            try
            {
                //StreamWriter sw = File.CreateText(logPath);
                //sw.Close();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="logText">写入内容</param>
        public void WriteToLog(string logText)
        {
            try
            {
                //StreamWriter sw = new StreamWriter(Application.StartupPath.ToString() + @"\PRA" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, Encoding.UTF8);
                StreamWriter sw = new StreamWriter(logPath, true, Encoding.UTF8);
                sw.WriteLine(logText);
                sw.WriteLine("Date:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.WriteLine("---------------------------------------------------------");
                sw.Close();
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show(ee.ToString());
            }
        }
    }
}
