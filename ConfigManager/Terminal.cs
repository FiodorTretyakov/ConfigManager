﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using ConfigManager.Entity;
using ConfigManager.Packages;
using Newtonsoft.Json;

namespace ConfigManager
{
    public class Terminal
    {
        private const string BaseUrl = "https://github.com/FiodorTretyakov/ConfigManager/raw/master/";
        private const string UtilName = "ConfigManager";
        private const string Platform = "ubuntu.14.04-x64";

        private readonly string _versionFileName = $"{UtilName}.csproj";
        private readonly string _commandFailed =
            $"Please, run the command in format: ./{UtilName} command-name arguments.";

        private List<Command> _commands;
        private List<Package> _packages;

        public async Task<List<Command>> GetCommands() => _commands ?? (_commands = JsonConvert
                       .DeserializeObject<CollectionRoot<Command>>(await File.ReadAllTextAsync("command.json"))
                       .Elements);

        public async Task<List<Package>> GetPackages() => _packages ?? (_packages = JsonConvert
                                                              .DeserializeObject<CollectionRoot<Package>>(await File.ReadAllTextAsync("package.json"))
                                                              .Elements);

        public readonly HttpClient Client = new HttpClient();

        public async Task<bool> Run(string[] args)
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
                        (await GetCommands()).AsParallel().ForAll(c => Console.WriteLine($"{c.Name}: {c.Description}"));
                        Console.WriteLine("List of packages:");
                        (await GetPackages()).AsParallel().ForAll(c => Console.WriteLine($"{c.Name}: {c.Description}, Dependencies: {string.Join(',', c.Dependencies)}"));
                        Console.WriteLine(_commandFailed);
                        break;
                    }
                case "update":
                    {
                        var v = await GetLatestVersion();

                        Console.WriteLine("Checking if the new version available...");
                        if (v > GetCurrentVersion())
                        {
                            Bash($"sudo wget https://github.com/FiodorTretyakov/ConfigManager/raw/master/builds/ConfigManager.{v}.{Platform}.deb", "Downloading the new version...");
                            Bash("sudo apt-get install -f", "Getting missed packages if any...");
                            Bash($"sudo dpkg -i ConfigManager.{v}.{Platform}.deb", "Installing...");
                        }
                        else
                        {
                            Console.WriteLine("You are using the latest version already.");
                        }

                        break;
                    }

                case "install":
                    {
                        var package = ResolvePackage(packageName);

                        if (package != null)
                        {
                            await package.Install();
                        }

                        break;
                    }
                case "delete":
                    {
                        var package = ResolvePackage(packageName);

                        if (package != null)
                        {
                            await package.Delete();
                        }

                        break;
                    }
                case "exists":
                    {
                        if (string.IsNullOrWhiteSpace(packageName))
                        {
                            Console.WriteLine("You missed the argument - which package check the status. To see the list, please, run help command.");
                        }
                        else
                        {
                            Bash($"apt-cache search --names-only '^{packageName}.*'", "Searching the package...");
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
                        Console.WriteLine($"Command not found. {_commandFailed} To see all possible commands, please, run ./ConfigManager help.");
                        break;
                    }
            }

            return true;
        }

        public Base ResolvePackage(string packageName)
        {
            switch (packageName)
            {
                case "apache2":
                    {
                        return new Apache2(this);
                    }
                case "php":
                    {
                        return new Php(this);
                    }
                default:
                    {
                        Console.WriteLine("You missed the argument - the package name. To see the list, please, run help command.");
                        return null;
                    }
            }
        }

        public Version GetCurrentVersion()
        {
            var doc = new XmlDocument();
            doc.Load(_versionFileName);

            return GetVersion(doc);
        }

        private static Version GetVersion(XmlNode doc)
        {
            return new Version($"{doc.SelectSingleNode("Project").SelectSingleNode("PropertyGroup").SelectSingleNode("Version").InnerText}");
        }

        public async Task<Version> GetLatestVersion()
        {
            using (var response = await Client.GetAsync($"{BaseUrl}/ConfigManager/ConfigManager.csproj"))
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
