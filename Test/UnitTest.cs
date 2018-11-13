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
        public async Task IsCommandsLoaded()
        {
            var commands = await _terminal.GetCommands();
            Assert.IsTrue(commands.Count > 5);
            Assert.IsTrue(commands.Any(c => c.Name.Length > 0 && c.Description.Length > 0));
        }

        [TestMethod]
        public async Task IsPackagesLoaded()
        {
            var packages = await _terminal.GetPackages();
            Assert.IsTrue(packages.Count > 1);
            Assert.IsTrue(packages.Any(p => p.Dependencies?.Count > 0 && p.Name.Length > 0 && p.Description.Length > 0));
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
