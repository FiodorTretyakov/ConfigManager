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
        public void IsCommandsLoaded()
        {
            Assert.IsTrue(_terminal.Commands.Count > 5);
            Assert.IsTrue(_terminal.Commands.Any(c => c.Name.Length > 0 && c.Description.Length > 0));
        }

        [TestMethod]
        public void IsPackagesLoaded()
        {
            Assert.IsTrue(_terminal.Packages.Count > 1);
            Assert.IsTrue(_terminal.Packages.Any(p => p.Dependencies?.Count > 0 && p.Name.Length > 0 && p.Description.Length > 0));
        }

        [TestMethod]
        public void VersionIsNumeric()
        {
            var v = _terminal.GetCurrentVersion().ToString().Split('.', '-');

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
