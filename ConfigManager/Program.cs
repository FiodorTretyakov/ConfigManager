using System.Threading.Tasks;

namespace ConfigManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await new Terminal().Run(args);
        }
    }
}
