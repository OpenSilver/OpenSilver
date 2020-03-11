This repository contains the source code of both OpenSilver (which compiles to WebAssembly) and CSHTML5 (which compiles to JavaScript). More information about those two products can be found at: www.opensilver.net and www.cshtml5.com

# Instructions for OpenSilver:

TL;DR: Run "restore-packages-opensilver.bat" and then open the solution named "OpenSilver.sln" to compile the OpenSilver runtime.

### How to build the source code?
1. Clone this repository locally or download it as a ZIP archive and extract it on your machine
2. Execute the file "restore-packages-opensilver.bat" (by double-clicking on it in Windows Explorer)
3. Open the solution file "OpenSilver.sln" (located in the "src" folder) with Visual Studio 2019 or newer
4. Build either the "SL" configuration or the "UWP" configuration, depending on whether you want to use the "OpenSilver" NuGet package (recommended) or the "OpenSilver.UWPCompatible" package. This will generate the assembly "OpenSilver.dll" in the "bin/OpenSilver/SL/" folder, or the assembly "OpenSilver.UWPCompatible.dll" in the "bin/OpenSilver/UWP/" folder.

### How to test the changes that I make to the source code?
1. Create a new Visual Studio project of type OpenSilver. To do so, you can install the latest VSIX available at https://opensilver.net/download.aspx , which will add new project templates to the"New Project" dialog of Visual Studio 2019.
2. Build the OpenSilver source code (cf. previous question)
3. Copy the 3 files "OpenSilver.dll", "OpenSilver.pdb", and "OpenSilver.xml" from the "bin/OpenSilver/SL/" folder of the OpenSilver source code (ie. the source code in this repository) into the folder "C:\Users\YOUR_USERNAME\.nuget\packages\opensilver\PACKAGE_VERSION\lib\netstandard2.0" (please replace "YOUR_USERNAME" and "PACKAGE_VERSION" in the path), overwriting the existing files
4. Build and run the project that you created at step 1



# Instructions for CSHTML5:

TL;DR: Run "restore-packages-cshtml5.bat" and then open the solution named "CSHTML5.sln" to compile the CSHTML5 runtime.

### How to build the source code?
1. Clone this repository locally or download it as a ZIP archive and extract it on your machine
2. Execute the file "restore-packages-cshtml5.bat" (by double-clicking on it in Windows Explorer)
3. Open the solution file "CSHTML5.sln" (located in the "src" folder) with Visual Studio 2017 or newer
4. Build either the "Debug" configuration or the "Migration" configuration, depending on whether you want to use the "CSHTML5" NuGet package (recommended for creating new apps) or the "CSHTML5.Migration" package (recommended for migrating existing Silverlight or WPF applications). This will generate the assembly "CSHTML5.dll" in the "bin/Debug/" folder, or the assembly "CSHTML5.Migration.dll" in the "bin/Migration/" folder.

Note: many low-level types are located in the Bridge.NET repository, which is located [here](https://github.com/cshtml5/Bridge).

### How to test the changes that I make to the source code?
1. Create a new Visual Studio project of type CSHTML5 version 2.x. To do so, you can install the VSIX available at http://forums.cshtml5.com/viewforum.php?f=6 (look for the latest version 2.x), which will add new project templates to the Visual Studio "New Project" dialog.
2. Build the CSHTML5 source code (cf. previous question)
3. Copy the 3 files "CSHTML5.dll", "CSHTML5.pdb", and "CSHTML5.xml" from the "bin/Debug/" folder of the CSHTML5 source code (ie. the source code in this repository) into the folder "packages/CSHTML5(...)/lib/net40/" of the project that you created at step 1, overwriting the existing files
4. Build and run the project that you created at step 1


