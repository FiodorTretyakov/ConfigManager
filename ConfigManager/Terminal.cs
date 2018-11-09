using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;

namespace ConfigManager
{
    public class Terminal
    {
        public void Run(string package)
        {
            switch (package)
            {
                case "help":
                {
                    Bash("sudo");
                    break;
                }
                case "update":
                {
                    var v = NextVersion();

                    Bash($"sudo wget https://github.com/FiodorTretyakov/ConfigManager/raw/master/builds/ConfigManager.{v}.ubuntu.14.04-x64.deb"); //Download
                    Bash("sudo apt-get install -f"); //Get missed packages if any
                    Bash($"sudo dpkg -i ConfigManager.{v}.ubuntu.14.04-x64.deb");
                    break;
                }
                case "apache2":
                {
                    Bash("sudo apt-get install apache2 apache2-doc apache2-utils"); //Install apache
                    Bash("sudo a2dismod mpm_event"); //On Ubuntu 14.04, the event module is enabled by default. Disable it, and enable the prefork module
                    Bash("sudo a2enmod mpm_prefork");
                    Bash("sudo service apache2 restart");
                    break;
                }
            }
        }

        public Version GetVersion()
        {
            var doc = new XmlDocument();
            doc.Load("ConfigManager.csproj");
            var node = doc.SelectSingleNode("Project").SelectSingleNode("PropertyGroup");

            return new Version($"{node.SelectSingleNode("VersionPrefix").InnerText}.{node.SelectSingleNode("VersionSuffix").InnerText}");
        }

        public string NextVersion()
        {
            var v = GetVersion();

            return $"{v.Major}.{v.Minor}.{v.Build}-{v.Revision + 1}";
        }


        public void Bash(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            Console.WriteLine($"Running the {cmd}");
            process.Start();

            Task.WaitAll(Task.Run(() =>
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }
            }), Task.Run(() =>
            {
                while (!process.StandardError.EndOfStream)
                {
                    var line = process.StandardError.ReadLine();
                    Console.WriteLine(line);
                }
            }));
        }
    }
}
