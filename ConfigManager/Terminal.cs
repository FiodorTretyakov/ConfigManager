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
                        break;
                    }
                case "update":
                    {
                        var v = NextVersion();

                        Bash($"sudo wget https://github.com/FiodorTretyakov/ConfigManager/raw/master/builds/ConfigManager.{v}.ubuntu.14.04-x64.deb", "Downloading the new version...");
                        Bash("sudo apt-get install -f", "Get missed packages if any...");
                        Bash($"sudo dpkg -i ConfigManager.{v}.ubuntu.14.04-x64.deb", "Installing...");
                        break;
                    }
                case "apache2":
                    {
                        Bash("sudo apt-get install apache2 apache2-doc apache2-utils", "Installing Apache...");
                        Bash("sudo a2dismod mpm_event", "Disabling default Ubuntu event module...");
                        Bash("sudo a2enmod mpm_prefork", "Enabling prefork module...");
                        Bash("sudo service apache2 restart", "Restarting the Apache...");
                        break;
                    }
                case "php":
                {
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


        public void Bash(string cmd, string desc)
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

            Console.WriteLine(desc);

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
