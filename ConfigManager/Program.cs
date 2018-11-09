using System;

namespace ConfigManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please, enter the command in next format: ConfigManager {PackageName}. For help, please, dun ConfigManager help");
            }
            else
            {
                new Terminal().Run(args[0]);
            }
        }
    }
}
