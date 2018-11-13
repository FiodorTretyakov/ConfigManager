using System.Threading.Tasks;

namespace ConfigManager.Packages
{
    public sealed class Php : Base
    {
        protected override string Name => "php";

        public Php(Terminal t) : base(t)
        {
        }

        public override async Task Run(bool noDep = false)
        {
            if (!noDep)
            {
                await base.Run();
            }
        }
    }
}
