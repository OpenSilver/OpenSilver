#First some common params, delivered by the nuget package installer
param($installPath, $toolsPath, $package)

Write-Host ("OpenSilver is configuring " + $dte.Solution.name)

#------------------------------------------------------------------------
# Set the "StartProgram" to point to the path of the Simulator for the Simulator project:
#------------------------------------------------------------------------

$solutionName =[System.IO.Path]::GetFileNameWithoutExtension($dte.Solution.FullName)
$simulatorProjectName = $solutionName  + '.Simulator'
Write-Host ("OpenSilver is trying to configure " + $simulatorProjectName + " project")

foreach ($project in $dte.Solution.Projects)
{
	$configurationManager = $project.ConfigurationManager

	if ($project.name -like $simulatorProjectName)
	{
		Write-Host ("found Simulator project " + $project.name)

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

			$project.Save
		}
	}
}
Write-Host ("Configured start program")

#------------------------------------------------------------------------
# Final message:
#------------------------------------------------------------------------

Write-Host ("OpenSilver package installation was successful")