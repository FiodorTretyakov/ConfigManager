using System;
using System.Threading.Tasks;

namespace ConfigManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please, enter the command in next format: ConfigManager {PackageName}. For help, please, run ConfigManager help.");
            }
            else
            {
                await new Terminal().Run(args[0]);
            }
        }
    }
}
