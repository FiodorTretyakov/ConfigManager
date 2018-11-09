using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ConfigManager
{
    public class Terminal
    {
        private readonly IConfigurationRoot _config;

        public Terminal()
        {
            _config = new ConfigurationBuilder().AddXmlFile("ConfigurationManager/Configuration.csproj").Build();
        }

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
                    var props = _config.GetSection("Project").GetSection("PropertyGroup").GetChildren().ToList();

                    Bash("sudo wget https://github.com/FiodorTretyakov/ConfigManager/raw/master/builds/ConfigManager.1.0.0.ubuntu.14.04-x64.deb"); //Download
                    Bash("sudo apt-get install -f"); //Get missed packages if any
                    Bash($"sudo dpkg -i ConfigManager.{props.First(n => n.Key == "VersionPrefix")}-{props.First(n => n.Key == "VersionSuffix")}.ubuntu.14.04-x64.deb");
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
