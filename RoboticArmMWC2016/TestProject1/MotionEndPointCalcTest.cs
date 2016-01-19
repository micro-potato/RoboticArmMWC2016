using MotionDetection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Helpers;
using MotionDetection.Moudle;

namespace TestProject1
{
    
    
    /// <summary>
    ///这是 MotionEndPointCalcTest 的测试类，旨在
    ///包含所有 MotionEndPointCalcTest 单元测试
    ///</summary>
    [TestClass()]
    public class MotionEndPointCalcTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///MotionAngle 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("MotionDetection.dll")]
        public void MotionAngleTest()
        {
            
        }

        /// <summary>
        ///CalcEndPoint 的测试
        ///</summary>
        [TestMethod()]
        public void CalcEndPointTest()
        {
            MotionEndPointCalc mepc = new MotionEndPointCalc();
            double x1 = -48; // TODO: 初始化为适当的值
            double y1 = 0; // TODO: 初始化为适当的值
            double x2 = -32; // TODO: 初始化为适当的值
            double y2 = 37; // TODO: 初始化为适当的值
            MotionResult actual;
            actual = mepc.CalcEndPoint(x1, y1, x2, y2,80);
            Assert.AreEqual(actual.EndPointX, -32);
        }

        [TestMethod]
        public void TestConfig()
        {
            ConfigHelper.GetInstance().ResolveConfig(@"E:\code\code2016\RoboticArmMWC2016\sourceCode\RoboticArmMWC2016\RoboticArmMWC2016\bin\Debug\config.xml");
        }
    }
}
