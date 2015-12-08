$openCoverPath = Join-Path $PSScriptRoot "src\packages\OpenCover.4.6.166\tools\OpenCover.Console.exe"
$coverallsPath = Join-Path $PSScriptRoot "src\packages\coveralls.io.1.3.4\tools\coveralls.net.exe"
$testDllPath = Join-Path $PSScriptRoot "src\sendstorm.tests\bin\release\Sendstorm.Tests.dll"
$vsTestPath = "c:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"

$arguments = "-register:user", "`"-filter:+[*]* -[sendstorm.tests]* `"", "-target:$vsTestPath", "`"-targetargs:$testDllPath`"", "-output:coverage.xml"
. $openCoverPath $arguments
. $coverallsPath --opencover coverage.xml