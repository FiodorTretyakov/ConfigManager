$runtime = "ubuntu.14.04-x64"

$projectName = "ConfigManager"
$projectFile = "$projectName\$projectName.csproj"
[xml]$xml = Get-Content $projectFile
[int]$suffix = $xml.Project.PropertyGroup.VersionSuffix
$prefix = $xml.Project.PropertyGroup.VersionPrefix
$suffix += 1
$xml.Project.PropertyGroup.VersionSuffix = [string]$suffix
$xml.Save("$projectFile")

Set-Location "Test"
dotnet test "Test.csproj" /p:CollectCoverage=true

Set-Location "..\$projectName"
dotnet deb -c Release -r $runtime -f netcoreapp2.1
Copy-Item ".\bin\Release\$runtime\$projectName.$prefix-$suffix.$runtime.deb" "..\builds\"
Set-Location ..