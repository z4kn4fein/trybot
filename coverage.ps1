if($env:APPVEYOR_PULL_REQUEST_NUMBER) {
	exit 0
} 

choco install opencover.portable
choco install codecov

$testPath = Join-Path $PSScriptRoot "test\trybot.tests.csproj"
$coverageReportDir = Join-Path $PSScriptRoot "coverageresults"

$arguments = "-returntargetcode", "-register:user", "`"-filter:+[*]Trybot.* -[Trybot.Tests]* -[Trybot]*.Utils*`"", "-target:dotnet.exe", "`"-targetargs:test $testPath -f net45 -c Release`"", "-output:coverage.xml", "-skipautoprops", "-hideskipped:All"
. OpenCover.Console.exe $arguments
. codecov -f coverage.xml