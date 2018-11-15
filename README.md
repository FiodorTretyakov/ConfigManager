# ConfigManager
Analogues: Chef, Puppet.

### The ConfigManager allows to add/remove and configure packages for linux.

## Now it supports commands:
* help: shows all functionality for the ConfigManger;
* version: shows the current version;
* update: updates the ConfigManager to the latest version is any;
* system-update: updates all the packages in the system to the latest versions;
* exists: shows all installed packages by the pattern;
* install: adds and tunes the package. Supports apache2 and php;
* delete: uninstall package.

## Configuration
Solution wrote on .NET Core 2.1 with target platform ubuntu 14.04 wrapped to debian package. Every new platform can be added to configuration. To run, you need to:
* install .NET Core SDK >=2.1; https://www.microsoft.com/net/download/dotnet-core/2.1
* install .NET Core Runtime >=2.1; https://www.microsoft.com/net/download/dotnet-core/2.1
* add a secret file as in `secretTemplate.json` (only need for unit test that checks is the simplest php application already installed for the servers); https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows

## Installation
The solution can be installed to the machine without any dependencies of .NET Core. What you need to do:
1. Download the debian package: `sudo wget https://github.com/FiodorTretyakov/ConfigManager/raw/master/builds/ConfigManager.0.0.1.19.ubuntu.14.04-x64.deb` (the latest version from github-builds).
2. Install unresolved dependencies: `apt-get -f install`.
3. Install the package: `sudo dpkg -i ConfigManager.0.0.1.19.ubuntu.14.04-x64.deb`.
4. Go to the package folder: `cd /usr/share/ConfigManager`.
5. Run `./ConfigManager ` command. 


## Extension

### Commands
All commands store in ConfigManager/command.json file.
It has a root array "elements", which consists of commands represented that way:

    `{
      "name": "name",
      "desc": "description"
    }`  

You can add a new command here. Then you should add it and its implementation to `Terminal.Run` method-switch.
Command implementation is the list of actions, they can include:
* Console Output;
* Bash command;
* Console Input: Y/n;
* Some internal actions;

### Packages
All packages store in ConfigManager/package.json file.
It has a root array "elements", which consists of packages represented that way:

    `{
      "name": "name",
      "desc": "description",
      "dep": ["dependency1", "dependency2"]
    }`

Dep is optional element, if your packages has no dependencies, you can omit it.

You can add a new package here. Then you should add it and its to method `Terminal.ResolvePackage` that connects class and package name.
And finally, you should add it to the class to ConfigManager/Packages and inherit it from Base.

## What to improve
Go to Project in the github