using System;
using System.Threading.Tasks;
using Mono.Unix;

namespace ConfigManager.Packages
{
    public sealed class Php : Base
    {
        protected override string Name => "php";

        public Php(Terminal t) : base(t)
        {
        }

        public override async Task Install(bool noDep = false)
        {
            if (!noDep)
            {
                await base.Install();
            }
            Terminal.Bash("sudo apt-get install php5 libapache2-mod-php5 -y", "Installing php...");
            Terminal.Bash("sudo rm -rf /var/www/html/*", "Cleaning the directory...");
            Console.WriteLine("Creating the simplest php application...");
            await CreateNewFile("/var/www/html/index.php", @"Content/php.txt",
                FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite
                                               | FileAccessPermissions.GroupRead
                                               | FileAccessPermissions.OtherRead
                                               | FileAccessPermissions.OtherWrite);
            Terminal.Bash("sudo service apache2 restart", "Restarting the apache...");
        }
    }
}
