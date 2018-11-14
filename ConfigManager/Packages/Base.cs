using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConfigManager.Entity;
using Mono.Unix;

namespace ConfigManager.Packages
{
    public abstract class Base
    {
        protected readonly Terminal Terminal;

        protected abstract string Name { get; }

        protected Base(Terminal t)
        {
            Terminal = t;
        }

        public virtual async Task Install(bool noDep = false)
        {
            Console.WriteLine(
                "It is strictly recommended to update your system before package installation. Do you wanna do it now? Y/n");
            if (Console.ReadLine() == "Y")
            {
                await Terminal.Run(new[] { "system-update", string.Empty });
            }

            (await ResolveDependencies(Name, new List<Package>())).ForEach(async p =>
                await Terminal.ResolvePackage(p.Name).Install(true));
        }

        public virtual async Task<bool> Delete()
        {
            Console.WriteLine("Be careful. This package has dependencies:");
            (await GetDependent()).ForEach(Console.WriteLine);
            Console.WriteLine("Do you still want to remove the package? Y/n");

            return Console.ReadLine() == "Y";
        }

        public async Task<List<string>> GetDependent()
        {
            var result = new List<string>();

            (await Terminal.GetPackages()).AsParallel().ForAll(p =>
            {
                var dependencies = GetDependentByPackage(p, result);
                lock (result)
                {
                    result.AddRange(dependencies);
                }
            });

            return result.Distinct().ToList();
        }

        private IEnumerable<string> GetDependentByPackage(Package package, List<string> dep)
        {
            var result = new List<string>();

            Parallel.ForEach(package.Dependencies, async (d, state) =>
            {
                if (dep.Contains(d) || Name == d)
                {
                    lock (result)
                    {
                        result.Add(package.Name);
                    }
                    state.Break();
                    return;
                }

                var depPack = (await Terminal.GetPackages()).First(p => p.Name == d);
                lock (result)
                {
                    result.AddRange(GetDependentByPackage(depPack, dep));
                }
            });

            return result;
        }

        public async Task<List<Package>> ResolveDependencies(string packageName, List<Package> dependencies)
        {
            var package = (await Terminal.GetPackages()).First(p => p.Name == packageName);

            package.Dependencies.AsParallel().ForAll(async dep =>
            {
                if (dependencies.Any(d => d.Name == dep))
                {
                    return;
                }

                var resolvedDependencies = await ResolveDependencies(dep, dependencies);

                lock (dependencies)
                {
                    dependencies.AddRange(resolvedDependencies);
                }
            });

            dependencies.Add(package);

            return dependencies;
        }

        public async Task CreateNewFile(string path, string localFileName)
        {
            using (var content = File.OpenRead(localFileName))
            {
                var file = new FileInfo(path);

                using (var w = file.OpenWrite())
                {
                    content.Position = 0;
                    await content.CopyToAsync(w);
                    w.Position = 0;
                }
            }
        }

        public async Task CreateNewFile(string path, string localFileName, FileAccessPermissions permissions)
        {
            await CreateNewFile(path, localFileName);
            new UnixFileInfo(path) { FileAccessPermissions = permissions };
        }
    }
}