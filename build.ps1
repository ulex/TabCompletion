$resharper_version = "2017.3.0"

$ErrorActionPreference = "Stop"

$version = $resharper_version + "." + [System.DateTime]::Now.ToString("mmss")
$version_content = "[assembly: System.Reflection.AssemblyVersion(""$version"")]
[assembly: System.Reflection.AssemblyFileVersion(""$version"")]"
$version_content | Set-Content "Properties\AssemblyInfo.Generated.cs"

msbuild /t:Build /p:Configuration=Release ReSharper.TabCompletion.sln
nuget.exe pack ReSharper.TabCompletion.nuspec -version $version
