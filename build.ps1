$runtime = "ubuntu.14.04-x64"

$projectName = "ConfigManager"
$projectFile = "$projectName\$projectName.csproj"
[xml]$xml = Get-Content $projectFile
$version = [version]$xml.Project.PropertyGroup[0].VersionPrefix
$xml.Project.PropertyGroup[0].VersionPrefix = [string](New-Object -TypeName System.Version -ArgumentList $version.Major,$version.Minor,$version.Build, ($version.Revision + 1))
$xml.Save("$projectFile")

Set-Location "Test"
dotnet test "Test.csproj" /p:CollectCoverage=true

Set-Location "..\$projectName"
dotnet deb -c Release -r $runtime -f netcoreapp2.1
Copy-Item ".\bin\Release\$runtime\$projectName.$prefix-$suffix.$runtime.deb" "..\builds\$projectName.$prefix.$suffix.$runtime.deb"

Set-Location ..