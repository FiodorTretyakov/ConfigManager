using System;
using ConfigManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var v = new Version(new Terminal().GetVersion());
            Assert.AreEqual(v.Major, 0);
            Assert.AreEqual(v.Minor, 0);
            Assert.AreEqual(v.Build, 1);
            Assert.AreEqual(v.Revision, 3);
        }
    }
}
