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
        public string CameraXID { get; set; }
        public string CameraYID { get; set; }
        public string CameraX2ID { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int DetectFrequency { get; set; }

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
            this.CameraXID = xmlDocument.SelectSingleNode("Data/CameraXID").InnerText;
            this.CameraX2ID = xmlDocument.SelectSingleNode("Data/CameraX2ID").InnerText;
            this.CameraYID = xmlDocument.SelectSingleNode("Data/CameraYID").InnerText;
            this.IP = xmlDocument.SelectSingleNode("Data/IP").InnerText;
            this.Port = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/Port").InnerText);
            this.DetectFrequency = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/DetectFrequency").InnerText);
        }   
    }
}
