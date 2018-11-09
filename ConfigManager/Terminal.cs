using System;
using System.Diagnostics;

namespace ConfigManager
{
    public class Terminal
    {
        public void Run(string package)
        {
            switch (package)
            {
                case "apache2":
                {
                    Bash("sudo apt-get install apache2 apache2-doc apache2-utils");
                    Bash("sudo a2dismod mpm_event");
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

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(string.IsNullOrEmpty(error) ? output : error);
        }
    }
}
