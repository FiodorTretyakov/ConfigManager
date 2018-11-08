dotnet deb -c Release -r debian-x64 -f netcoreapp2.1
Copy-Item ".\bin\Release\debian-x64\ConfigManager.1.0.0.debian-x64.deb" "..\builds\"