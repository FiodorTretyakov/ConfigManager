using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigManager.Entity;

namespace ConfigManager.Packages
{
    public abstract class Base
    {
        protected readonly Terminal Terminal;

        protected abstract string Name { get;  }

        protected Base(Terminal t)
        {
            Terminal = t;
        }

        public virtual async Task Run(bool noDep = false)
        {
            Console.WriteLine("It is strictly recommended to update your system before package installation. Do you wanna do it now? Y/n");
            if (Console.ReadLine() == "Y")
            {
                await Terminal.Run(new[] { "system-update", string.Empty });
            }

            (await ResolveDependencies(Name, new List<Package>())).ForEach(async p => await Terminal.ResolvePackage(p.Name).Run(true));
        }

        public async Task<List<Package>> ResolveDependencies(string packageName, List<Package> dependencies)
        {
            var package = (await Terminal.GetPackages()).First(p => p.Name == packageName);

            foreach (var dep in package.Dependencies)
            {
                if (dependencies.Any(d => d.Name == dep))
                {
                    continue;
                }
                dependencies.AddRange(await ResolveDependencies(dep, dependencies));
            }

            dependencies.Add(package);

            return dependencies;
        }
    }
}
