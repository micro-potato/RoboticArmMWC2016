using MotionDetection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Helpers;

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
            //MotionEndPointCalc._tableHeight = 10;
            //MotionEndPointCalc._tableWidth = 5;
            //double x1 = 5; // TODO: 初始化为适当的值
            //double y1 = 0; // TODO: 初始化为适当的值
            //double x2 = 2.5; // TODO: 初始化为适当的值
            //double y2 = 1.25; // TODO: 初始化为适当的值
            //double expected = 5; // TODO: 初始化为适当的值
            //double actual;
            //actual = MotionEndPointCalc.CalcEndPoint(x1, y1, x2, y2);
            //bool isSuccess = Math.Abs(expected - actual) < 1 ? true : false;
            //Assert.IsTrue(isSuccess);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        [TestMethod]
        public void TestConfig()
        {
            ConfigHelper.GetInstance().ResolveConfig(@"E:\code\code2016\RoboticArmMWC2016\sourceCode\RoboticArmMWC2016\RoboticArmMWC2016\bin\Debug\config.xml");
            Assert.AreEqual(255, ConfigHelper.GetInstance().RValue);
            Assert.AreEqual(255, ConfigHelper.GetInstance().BValue);
            Assert.AreEqual(5, ConfigHelper.GetInstance().ColorThreshold);
        }
    }
}
