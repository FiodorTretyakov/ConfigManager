using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using ConfigManager;
using ConfigManager.Entity;
using ConfigManager.Packages;
using Microsoft.Extensions.Configuration;
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
            Assert.IsTrue(packages.Any(p =>
                p.Dependencies?.Count > 0 && p.Name.Length > 0 && p.Description.Length > 0));
        }

        [TestMethod]
        public void VersionIsNumeric()
        {
            var v = _terminal.GetCurrentVersion().ToString().Split('.', '-');

            Assert.AreEqual(v.Length, 4);
            v.AsParallel().ForAll(e => Assert.IsInstanceOfType(int.Parse(e), typeof(int)));
        }

        [TestMethod]
        public async Task VersionIsLatest() => Assert.IsTrue(_terminal.GetCurrentVersion() >= await _terminal.GetLatestVersion());

        [TestMethod]
        public async Task TerminalRuns() => Assert.IsTrue(await _terminal.Run(new[] { string.Empty }));

        [TestMethod]
        public async Task TerminalRunsBash()
        {
            const string packageName = "fake";

            Assert.IsTrue(await _terminal.Run(new[] { "help" }));
            Assert.IsTrue(await _terminal.Run(new[] { "update" }));
            Assert.IsTrue(await _terminal.Run(new[] { "install" }));
            Assert.IsTrue(await _terminal.Run(new[] { "install", packageName }));
            Assert.IsTrue(await _terminal.Run(new[] { "delete" }));
            Assert.IsTrue(await _terminal.Run(new[] { "delete", packageName }));
            Assert.IsTrue(await _terminal.Run(new[] { "exists" }));
            Assert.IsTrue(await _terminal.Run(new[] { "version" }));
        }

        [TestMethod]
        public void NoPackageExists() => Assert.IsNull(_terminal.ResolvePackage("fake"));

        [TestMethod]
        public void ApacheExists() => Assert.IsInstanceOfType(_terminal.ResolvePackage("apache2"), typeof(Apache2));

        [TestMethod]
        public async Task ResolveDependencies()
        {
            const string packageName = "php";

            var dependencies = await _terminal.ResolvePackage(packageName).ResolveDependencies(packageName, new List<Package>());
            Assert.IsTrue(dependencies.Count > 0);
            Assert.AreEqual(dependencies[0].Name, "apache2");
        }

        [TestMethod]
        public async Task GetDependentForApache() => Assert.AreEqual((await _terminal.ResolvePackage("apache2").GetDependent()).Count, 1);

        [TestMethod]
        public async Task GetDependentForPhp() => Assert.AreEqual((await _terminal.ResolvePackage("php").GetDependent()).Count, 0);

        [TestMethod]
        public async Task IsFileCreated()
        {
            const string testFile = "test.txt";
            const string phpFile = @"Content/php.txt";

            await _terminal.ResolvePackage("php").CreateNewFile(testFile, phpFile);
            Assert.IsTrue(File.Exists(testFile));
            Assert.AreEqual(await File.ReadAllTextAsync(testFile), await File.ReadAllTextAsync(phpFile));
        }

        [TestMethod]
        public async Task TestResultServers()
        {
            var doc = new XmlDocument();
            doc.Load("Test.csproj");

            var config = new ConfigurationBuilder().AddUserSecrets(doc.SelectSingleNode("Project")
                .SelectSingleNode("PropertyGroup").SelectSingleNode("UserSecretsId").InnerText).Build();

            await TestServerFor200(config.GetSection("server1").Value);
            await TestServerFor200(config.GetSection("server2").Value);
        }

        public async Task TestServerFor200(string server)
        {
            using (var response = await _terminal.Client.GetAsync(server))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("Hello, world!\n", await response.Content.ReadAsStringAsync());
            }

        }
    }
}
