using System.Linq;
using System.Threading.Tasks;
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
            var v = _terminal.GetCurrentVersion().ToString().Split('.', '-');

            Assert.IsTrue(_terminal.Packages.Count > 1);
            Assert.AreEqual(v.Length, 4);
            v.AsParallel().ForAll(e => Assert.IsInstanceOfType(int.Parse(e), typeof(int)));
        }

        [TestMethod]
        public async Task VersionIsLatest()
        {
            Assert.IsTrue(_terminal.GetCurrentVersion() >= await _terminal.GetLatestVersion());
        }
    }
}
