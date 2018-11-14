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
    }
}