$runtime = "ubuntu.14.04-x64"

dotnet deb -c Release -r $runtime -f netcoreapp2.1
Copy-Item ".\bin\Release\$runtime\ConfigManager.1.0.0.$runtime.deb" "..\builds\"