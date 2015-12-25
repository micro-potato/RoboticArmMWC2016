using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Helpers
{
    public class ConfigHelper
    {
        public int RValue { get; set; }
        public int GValue { get; set; }
        public int BValue { get; set; }
        public int ColorThreshold { get; set; }
        public string CameraID { get; set; }
        public string RobotIP { get; set; }
        public int RobotPort { get; set; }
        public double ReflectVelocity { get; set; }//机械臂能够反应的最大速度（cm/ms）
        public int CameraFps { get; set; }
        public int ServerPort { get; set; }

        private static ConfigHelper _configHelper;
        private ConfigHelper()
        {
            
        }

        public static ConfigHelper GetInstance()
        {
            if (_configHelper == null)
            {
                _configHelper = new ConfigHelper();
            }
            return _configHelper;
        }

        public void ResolveConfig(string configPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(configPath);
            this.RValue = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/RValue").InnerText);
            this.GValue = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/GValue").InnerText);
            this.BValue = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/BValue").InnerText);
            this.ColorThreshold = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/ColorThreshold").InnerText);
            this.CameraID = xmlDocument.SelectSingleNode("Data/CameraID").InnerText;
            this.RobotIP = xmlDocument.SelectSingleNode("Data/RobotIP").InnerText;
            this.RobotPort = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/RobotPort").InnerText);
            this.ReflectVelocity = Convert.ToDouble(xmlDocument.SelectSingleNode("Data/ReflectVelocity").InnerText);
            this.CameraFps = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/CameraFps").InnerText);
            this.ServerPort = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/ServerPort").InnerText);
        }   
    }
}
