using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace ConfigManager
{
    public class Terminal
    {
        private const string BaseUrl = "https://github.com/FiodorTretyakov/ConfigManager/raw/master/";
        private const string VersionFileName = "ConfigManager.csproj";
        private const string Platform = "ubuntu.14.04-x64";

        private readonly HttpClient _client = new HttpClient();

        private readonly Dictionary<string, string> _commands = new Dictionary<string, string>
        {
            { "help", "Shows the list of commands, their descriptions and syntax."},
            { "update", "Updates the software to the latest version."},
            {"install", "Adds and configures the package. Usage ConfigManager install {package-name}"},
            { "remove", "Removes the package if it exists. Usage ConfigManager remove {package-name}"},
            { "exists", "Checks if the package exists. Usage ConfigManager exists {package-name}"},
            {"version", "Displays the current version of the package." },
            {"system-update", "Updates the system." }
        };

        private readonly Dictionary<string, string> _packages = new Dictionary<string, string>
        {
            {"apache2", "Installs the Apache server."},
            {"php", "Install the PHP runtime with the simplest Hello-World application."}
        };

        public async Task Run(string[] args)
        {
            var command = string.Empty;
            string packageName = null;

            if (args.Length > 0)
            {
                command = args[0];

                if (args.Length > 1)
                {
                    packageName = args[1];
                }
            }

            switch (command)
            {
                case "help":
                    {
                        Console.WriteLine("There are list of possible commands with descriptions.");
                        _commands.AsParallel().ForAll(c => Console.WriteLine($"{c.Key}: {c.Value}"));
                        Console.WriteLine("List of packages:");
                        _packages.AsParallel().ForAll(c => Console.WriteLine($"{c.Key}: {c.Value}"));
                        break;
                    }
                case "update":
                    {
                        var v = await GetLatestVersion();

                        if (v > GetCurrentVersion())
                        {
                            Bash($"sudo wget https://github.com/FiodorTretyakov/ConfigManager/raw/master/builds/ConfigManager.{v}.{Platform}.deb", "Downloading the new version...");
                            Bash("sudo apt-get install -f", "Get missed packages if any...");
                            Bash($"sudo dpkg -i ConfigManager.{v}.{Platform}.deb", "Installing...");
                        }
                        else
                        {
                            Console.WriteLine("You are using the latest version.");
                        }

                        break;
                    }
                case "install":
                    {
                        if (string.IsNullOrWhiteSpace(packageName))
                        {
                            Console.WriteLine("You missed the argument - which package to install. To see the list, please, run help command.");
                        }
                        else
                        {
                            //Bash("sudo apt-get install apache2 apache2-doc apache2-utils", "Installing Apache...");
                            //Bash("sudo a2dismod mpm_event", "Disabling default Ubuntu event module...");
                            //Bash("sudo a2enmod mpm_prefork", "Enabling prefork module...");
                            //Bash("sudo service apache2 restart", "Restarting the Apache...");
                        }
                        break;
                    }
                case "delete":
                    {
                        if (string.IsNullOrWhiteSpace(packageName))
                        {
                            Console.WriteLine("You missed the argument - which package to delete. To see the list, please, run help command.");
                        }
                        break;
                    }
                case "exists":
                    {
                        if (string.IsNullOrWhiteSpace(packageName))
                        {
                            Console.WriteLine("You missed the argument - which package check the status. To see the list, please, run help command.");
                        }
                        break;
                    }
                case "version":
                    {
                        Console.WriteLine($"Current Version is: {GetCurrentVersion()}");
                        break;
                    }
                case "system-update":
                    {
                        Bash("sudo apt-get update && sudo apt-get upgrade", "Updating the system...");
                        break;
                    }

                default:
                    {
                        Console.WriteLine(
                            "Please, enter the command in next format: ConfigManager {PackageName}. For help, please, run ConfigManager help.");
                        break;
                    }
            }
        }

        public Version GetCurrentVersion()
        {
            var doc = new XmlDocument();
            doc.Load(VersionFileName);

            return GetVersion(doc);
        }

        private static Version GetVersion(XmlNode doc)
        {
            var node = doc.SelectSingleNode("Project").SelectSingleNode("PropertyGroup");

            return new Version($"{node.SelectSingleNode("VersionPrefix").InnerText}.{node.SelectSingleNode("VersionSuffix").InnerText}");
        }

        public async Task<Version> GetLatestVersion()
        {
            using (var response = await _client.GetAsync($"{BaseUrl}/ConfigManager/ConfigManager.csproj"))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var doc = new XmlDocument();
                    doc.Load(stream);
                    return GetVersion(doc);
                }
            }
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
                    CreateNoWindow = true
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
