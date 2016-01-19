using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Helpers
{
    public class ConfigHelper
    {
        public string RobotIP { get; set; }
        public int RobotPort { get; set; }
        public int RobotPatternPort { get; set; }
        public double ReflectVelocity { get; set; }//机械臂能够反应的最大速度（cm/ms）
        public int ServerPort { get; set; }
        public int DetectWidth { get; set; }
        public int DetectHeight { get; set; }
        public int DetectFrequence { get; set; }
        public int CalcedHeigth { get; set; }
        public int CalibrateX { get; set; }
        public string RobotInterType { get; set; }

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
            this.RobotIP = xmlDocument.SelectSingleNode("Data/RobotIP").InnerText;
            this.RobotPort = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/RobotPort").InnerText);
            this.RobotPatternPort = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/RobotPatternPort").InnerText);
            this.ReflectVelocity = Convert.ToDouble(xmlDocument.SelectSingleNode("Data/ReflectVelocity").InnerText);
            this.ServerPort = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/ServerPort").InnerText);
            this.DetectWidth = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/DetectWidth").InnerText);
            this.DetectHeight = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/DetectHeight").InnerText);
            this.DetectFrequence = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/DetectFrequence").InnerText);
            this.CalcedHeigth = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/CalcedHeigth").InnerText);
            this.CalibrateX = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/CalibrateX").InnerText);
            this.RobotInterType = xmlDocument.SelectSingleNode("Data/RobotInterType").InnerText;
        }       
    }
}
