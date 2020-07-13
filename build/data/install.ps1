#First some common params, delivered by the nuget package installer
param($installPath, $toolsPath, $package, $project)

Write-Host ("CSHTML5 is configuring " + $project.ProjectName)

# Remove all references of object
ForEach ($item in $project.Object.References)
{
    if ($item.Identity.ToLower().StartsWith("system") -or $item.Identity.ToLower().StartsWith("microsoft") -or $item.Identity.ToLower().StartsWith("csharpxamlforhtml5") -or $item.Identity.ToLower().StartsWith("slmigration.csharpxamlforhtml5") -or $item.Identity.ToLower().StartsWith("jsil.meta"))
    {
        $name = $item.Identity
        Try
        {
            $item.Remove()
            Write-Host ("Removed Reference to " + $name)
        }
        Catch
        {
            Write-Host ("Failed to remove Reference to " + $name)
        }
    }
}

# Sets the NoStdLib setting to True for every project configuration.
$project.ConfigurationManager | ForEach-Object {

  # Check for <NoStdLib>
  $nostdlib_setting = $_.Properties.Item("NoStdLib")

  if (-not $nostdlib_setting.Value) 
  {
    $nostdlib_setting.Value = $true
  }


  # Check for AddAdditionalExplicitAssemblyReferences?
  # Check for AdditionalExplicitAssemblyReferences?
}

#------------------------------------------------------------------------
# Remove the import to the target of the legacy CSHTML5 1.x (JSIL-based):
#------------------------------------------------------------------------

$targetsFileName = 'CSharpXamlForHtml5.Build.targets';

# Need to load MSBuild assembly if it's not loaded yet.
Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

# Grab the loaded MSBuild project for the project
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

# Remove imports to .targets
$msbuild.Xml.Imports | Where-Object {$_.Project.ToLowerInvariant().EndsWith($targetsFileName.ToLowerInvariant()) } | Foreach { 
	$_.Parent.RemoveChild( $_ ) 
	[string]::Format( "Removed import of '{0}'" , $_.Project )
}

#------------------------------------------------------------------------
# Set the "StartProgram" to point to the path of the Simulator:
#------------------------------------------------------------------------

$configurationManager = $project.ConfigurationManager
foreach ($name in $configurationManager.ConfigurationRowNames)
{
    $projectConfigurations = $configurationManager.ConfigurationRow($name)
    foreach ($projectConfiguration in $projectConfigurations)
    {
		$newStartAction = 1
        [String]$newStartProgram = $toolsPath + "\simulator\CSharpXamlForHtml5.Simulator.exe"
		Write-Host "Changing project start action to " $newStartAction
        Write-Host "Changing project start program to " $newStartProgram              
		$projectConfiguration.Properties.Item("StartAction").Value = $newStartAction
        $projectConfiguration.Properties.Item("StartProgram").Value = $newStartProgram
    }
}
#$project.Save
Write-Host ("Configured start program")

#------------------------------------------------------------------------
# Final message:
#------------------------------------------------------------------------

Write-Host ("CSHTML5 package installation was successful")