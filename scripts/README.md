This folder contains useful scripts.

## rebuildOpenSilver.ps1

This script helps to update local OpenSilver packages with current source code and pdb files.

### Example of usage:

`.\rebuildOpenSilver.ps1 -OS_VERSION 1.0.0-private-2022-03-25 -OS_SIMULATOR_VERSION 1.0.0-private-2022-03-25 -COPY_PDB $true -UPDATE_LOCAL_CACHE $true -BUILD_SIMULATOR $true`

This command rebuilds the local version and replaces all local nuget files for the packages OpenSilver and OpenSilver.Simulator with version 1.0.0-private-2022-03-25

### Hints and tips

- Close all instaces of Visual Studio(including old) before running this script
- Execute the following command to allow script execution on your machine: `Set-ExecutionPolicy RemoteSigned`
