properties {
   $Version = $null
   $BasePath = Resolve-Path ..
   $SrcPath = "$BasePath\src"
   $ArtifactsPath = "$BasePath\artifacts"
   $ProjectJsonPath = "$SrcPath\Banshee\project.json"
   $TestProjectJsonPath = "$SrcPath\Banshee.Tests\project.json"
   $Configuration = if ($Configuration) {$Configuration} else { "Debug" }
}

task default -depends Clean, Build, Test, Package

task Clean {
   if (Test-Path -path $ArtifactsPath)
   {
      Remove-Item -path $ArtifactsPath -Recurse -Force
   }

   New-Item -Path $ArtifactsPath -ItemType Directory
}

task Build {
   Update-Project $ProjectJsonPath $Version

   exec { dotnet --version }
   exec { dotnet restore $ProjectJsonPath }
   exec { dotnet build $ProjectJsonPath -c $Configuration --no-incremental -f netstandard1.6 }
   exec { dotnet build $ProjectJsonPath -c $Configuration --no-incremental -f net451 }
}

task Test -depends Build {
   exec { dotnet restore $TestProjectJsonPath }
   exec { dotnet test $TestProjectJsonPath -c $Configuration -f netcoreapp1.0 }
   exec { dotnet test $TestProjectJsonPath -c $Configuration -f net451 }
}

task Package -depends Build {
   exec { dotnet pack $ProjectJsonPath -c $Configuration -o $ArtifactsPath }
}

function Update-Project ([string]$projectPath, [string]$version)
{
   if ($version -eq $null -or $version -eq "")
   {
      return
   }
	
   $json = (Get-Content $projectPath) -join "`n" | ConvertFrom-Json
   
   $json.version = $version
   
   ConvertTo-Json $json -Depth 10 -Compress | Set-Content $projectPath
}