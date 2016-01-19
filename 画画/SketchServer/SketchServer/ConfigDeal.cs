using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SketchServer
{
    public class ConfigDeal
    {
        private readonly string xmlPath = System.Windows.Forms.Application.StartupPath + @"\config.xml";
        public MConfig GetMessage()
        {
            MConfig mConfig = new MConfig();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            mConfig.ServerPort = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/ServerPort").InnerText);
            mConfig.MachineIP = xmlDocument.SelectSingleNode("Data/MachineIP").InnerText;
            mConfig.MachinePort = Convert.ToInt32(xmlDocument.SelectSingleNode("Data/MachinePort").InnerText);

            mConfig.DrawAngle = Convert.ToDouble(xmlDocument.SelectSingleNode("Data/DrawAngle").InnerText);
            mConfig.PaperWidth = Convert.ToDouble(xmlDocument.SelectSingleNode("Data/PaperWidth").InnerText);
            mConfig.PaperHeight = Convert.ToDouble(xmlDocument.SelectSingleNode("Data/PaperHeight").InnerText);

            mConfig.OffsetX = Convert.ToDouble(xmlDocument.SelectSingleNode("Data/OffsetX").InnerText);
            mConfig.OffsetY = Convert.ToDouble(xmlDocument.SelectSingleNode("Data/OffsetY").InnerText);
            mConfig.OffsetZ = Convert.ToDouble(xmlDocument.SelectSingleNode("Data/OffsetZ").InnerText);
            return mConfig;
        }
    }
}
