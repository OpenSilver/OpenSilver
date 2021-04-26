This repository contains the source code of the 2 following products, which share most of their code:
- OpenSilver (www.opensilver.net) &rarr; It compiles C#/XAML/.NET to WebAssembly/HTML/CSS (Free, Open Source, MIT Licensed)
- CSHTML5 (www.cshtml5.com) &rarr; It compiles C#/XAML/.NET to JavaScript/HTML/CSS (Dual Licensed)



# How to download the software and get started?

Read the "[Getting Started](http://doc.opensilver.net/documentation/general/getting-started-tour.html)" page of the OpenSilver [documentation](http://doc.opensilver.net/) for a step-by-step tutorial.

Basically, you should download the .VSIX file (the extension for Microsoft Visual Studio) which installs the Project Templates:
- The .VSIX for OpenSilver can be downloaded from: https://opensilver.net/download.aspx (Free, Open Source, MIT Licensed)
- The .VSIX for CSHTML5 can be downloaded from: http://cshtml5.com/download.aspx (Dual Licensed)

Then, launch Visual Studio, click "Create a new project", and choose one of the installed templates.

In the newly-created project, you may then want to update the NuGet package to reference the very latest version, in order to be sure that you have the same version as the one in the "master" branch of this repository (note: in the NuGet Package Manager, be sure to check the option "include pre-releases", otherwise you may not see the latest package version).



# How to build the source code in this repository?


### 1. If you want to build only the Runtime (recommended because simpler and faster to compile):

The "Runtime" corresponds to the assembly that is referenced by your application project. It is the assembly that contains most of the interesting code of this repository. In particular, it contains the code for the UI engine, the XAML controls, the framework classes, and more.

To compile the Runtime source code, follow these steps:

1. Clone this repository locally or download it as a ZIP archive and extract it on your machine

2. Execute the .BAT file "restore-packages-opensilver.bat" or "restore-packages-cshtml5.bat" (depending on whether you are using OpenSilver or CSHTML5)

3. Launch Visual Studio 2019 or newer

4. Open the solution file "OpenSilver.sln" or "CSHTML5.sln" (depending on whether you are using OpenSilver or CSHTML5)

5. Choose the appropriate "Solution Configuration" depending on your needs:
- For OpenSilver, you may choose the "SL" configuration for Silverlight-like dialect, or the "UWP" configuration for UWP-like dialect. Note: there is also a "SL.WorkInProgress" configuration that contains work-in-progress features. It is useful during migrations from Silverlight to quickly arrive to a point where the migrated project compiles.
- For CSHTML5, you may choose the "Debug" configuration for UWP-like dialect, and the "Migration" configuration for Silverlight-like dialect.
6. Build the project named "Runtime"

7. If you wish to test the Runtime, start by creating a test project using the Project Templates (as described in the section "How to download the software and get started?" above). Then, update the NuGet package to reference the latest version. Lastly, copy/paste the assemblies that you just compiled from the "bin" folder of the "Runtime" project into the "lib" folder of the installed NuGet package (the one that is used by your test project). To know precisely where this folder is located, follow these tips:
- If you are compiling the Runtime of OpenSilver, simply copy/paste "OpenSilver.dll" from the "bin/SL" folder into the folder:
C:\Users\%USERNAME%\.nuget\packages\opensilver\1.X.Y\lib\netstandard2.0
- If you are compiling the Runtime of CSHTML5, simply copy/paste "CSHTML5.dll" from the "bin/Debug" folder into the the folder "packages\CSHTML5.2.X.Y\lib\net40" located inside the solution of your test project.
Then, rebuild your test project and launch it.

### 2. If you want to build the whole NuGet package (including the Runtime, the Compiler, the Simulator, etc.):

1. Clone this repository locally or download it as a ZIP archive and extract it on your machine
2. Execute the .BAT file "restore-packages-opensilver.bat" or "restore-packages-cshtml5.bat" (depending on whether you are using OpenSilver or CSHTML5)
3. Launch the "Developer Command Prompt for VS 2019" (or newer) from your Start Menu
4. Navigate with the command prompt to the folder "build" that is located inside the cloned repository
5. Launch the .BAT file "build-nuget-package-OpenSilver-alpha.bat" or "build-nuget-package-CSHTML5-alpha.bat" (depending on whether you are using OpenSilver or CSHTML5)
6. When asked, enter a version number (if you are unsure about which version number to enter, look at the latest version number in the "Version History" [here](https://www.nuget.org/packages/OpenSilver) and [here](https://www.nuget.org/packages/cshtml5), and increment by 1, or just enter any number)

Wait for the compilation to finish. The generated NuGet packages are created inside the "build\output\XXXXX" directory.

To test the NuGet package, simply reference it from your test project. If you are unfamiliar with custom package sources in Visual Studio, please read  [this page](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio#package-sources).

### 3. If you want to build the VSIX installer (note: this is rarely needed - the steps above are usually enough)

Building the VSIX is only useful if you wish to test your changes to the Project Templates, the Item Templates, or the XAML editor. For all the other scenarios, building the Runtime or the NuGet package is sufficient to test your changes.

1. If needed, change the version number in "source.extension.vsixmanifest" (located in the project "OpenSilver.VSIX" or "CSHTML5.VSIX", depending on whether you want to build the VSIX of OpenSilver or CSHTML5)
2. Install Visual Studio 2015. During the installation, make sure that the "Visual Studio Extensibility Tools" optional components are installed too
3. With Visual Studio 2015, open the solution "VSExtension.OpenSilver.sln" or "VSExtension.CSHTML5.sln" depending on whether you want to build the VSIX of OpenSilver or CSHTML5
4. Rebuild the solution
5. The VSIX will be generated in a subfolder of the "bin" folder of the .VSIX project

Notes for building the VSIX:
- Note for OpenSilver: while all the projects do build fine with VS 2015, the "OpenSilver.VSIX" project can only be opened and built with VS 2019 (or newer). When building it with VS 2019 (or newer), you can unload the other projects to avoid compilation errors due to the version of VS.
- Note for CSHTML5: when building CSHTML5.VSIX, you may initially get a compilation error saying that some .nupckg files are missing. This is because the .nupckg files of CSHTML5 have not been uploaded to GitHub due to their large size. You need to manually download them and copy them into the folder that has the error. You can download those files from https://www.nuget.org/packages/cshtml5 and https://www.nuget.org/packages/cshtml5.migration

# What if I get a compilation error with the code in this repository?

If you get a compilation error, it may be that a Visual Studio workload needs to be installed. To find out, please open the solution "OpenSilver.sln" or "CSHTML5.sln" and attempt to compile it with the Visual Studio IDE (2019 or newer).

If the error is about the generation of the .VSIX file, just ignore the error, because the .VSIX file is not needed for testing the NuGet package.

If you are compiling using the Command Prompt, please double-check that you are using the "Developer Command Prompt", not the standard Command Prompt, and that the current directory is set to the "build" directory of this repository, because the paths in the .BAT files are relative to the current directory.

If you still encounter any issues, please contact:
- the OpenSilver team at: https://opensilver.net/contact.aspx
- the CSHTML5 team at: http://cshtml5.com/contact.aspx





