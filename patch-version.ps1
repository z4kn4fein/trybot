param($projectPath, $version)

$csprojPath = Join-Path $PSScriptRoot $projectPath
[xml]$project = Get-Content -Path $csprojPath
$project.Project.PropertyGroup[0].Version = $version
$project.Save($csprojPath)