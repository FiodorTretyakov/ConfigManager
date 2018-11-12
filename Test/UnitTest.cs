using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ConfigManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class UnitTest
    {
        private readonly Terminal _terminal = new Terminal();

        [TestMethod]
        public void VersionIsNumeric()
        {
            var doc = new XmlDocument();
            doc.Load(Terminal.VersionFileName);

            var v = _terminal.GetVersion(doc).ToString().Split('.', '-');

            Assert.AreEqual(v.Length, 4);
            v.AsParallel().ForAll(e => Assert.IsInstanceOfType(int.Parse(e), typeof(int)));
        }

        [TestMethod]
        public async Task VersionIsLatest()
        {
            var doc = new XmlDocument();
            doc.Load(Terminal.VersionFileName);

            Assert.AreEqual(_terminal.GetVersion(doc), await _terminal.GetLatestVersion());
        }
    }
}
