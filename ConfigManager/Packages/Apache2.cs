using System.Threading.Tasks;

namespace ConfigManager.Packages
{
    public sealed class Apache2 : Base
    {
        protected override string Name => "apache2";

        public Apache2(Terminal t) : base(t)
        {
        }

        public override async Task Install(bool noDep = false)
        {
            if (!noDep)
            {
                await base.Install();
            }
            Terminal.Bash("sudo apt-get install apache2 apache2-doc apache2-utils", "Installing Apache...");
            Terminal.Bash("sudo a2dismod mpm_event", "Disabling default Ubuntu event module...");
            Terminal.Bash("sudo a2enmod mpm_prefork", "Enabling prefork module...");
            Terminal.Bash("sudo service apache2 restart", "Restarting the Apache...");
        }

        public override async Task<bool> Delete()
        {
            if (!await base.Delete()) return false;
            Terminal.Bash("sudo service apache2 stop", "Stopping Apache...");
            Terminal.Bash("sudo apt-get purge apache2 apache2-utils apache2.2-bin apache2-common", "Deleting Apache...");
            Terminal.Bash("sudo apt-get autoremove", "Deleting all dependencies installed with Apache...");
            Terminal.Bash("sudo rm -rf /etc/apache2", "Cleaning the directory...");

            return true;

        }
    }
}