This repository contains the source code of the 2 following products, which share most of their code:
- **OpenSilver** (www.opensilver.net) &rarr; It compiles C#/XAML/.NET to WebAssembly/HTML/CSS (Free, Open Source, MIT Licensed)
- **CSHTML5** (www.cshtml5.com) &rarr; It compiles C#/XAML/.NET to JavaScript/HTML/CSS (Dual Licensed)

The main branches are:
- **develop**: this branch is where day to day development occurs
- **master**: this branch corresponds to the version of the packages that are on Nuget.org


# How to download the software and get started?

Read the "[Getting Started](http://doc.opensilver.net/documentation/general/getting-started-tour.html)" page of the OpenSilver [documentation](http://doc.opensilver.net/) for a step-by-step tutorial.

Basically, you should download the .VSIX file (the extension for Microsoft Visual Studio) which installs the Project Templates:
- The .VSIX for OpenSilver can be downloaded from: https://opensilver.net/download.aspx (Free, Open Source, MIT Licensed)
- The .VSIX for CSHTML5 can be downloaded from: http://cshtml5.com/download.aspx (Dual Licensed)

Then, launch Visual Studio, click "Create a new project", and choose one of the installed templates.

After creating the project, you may then want to update the NuGet package to reference the very latest version (note: in the NuGet Package Manager, be sure to check the option "include pre-releases", otherwise you may not see the latest package version).



# How to build the source code in this repository?

1. **Update Visual Studio:** Make sure you are using the very latest version of Visual Studio 2019. To check for updates, please launch the Visual Studio Installer from your Start Menu.

2. **Clone the repo:** Clone this repository locally or download it as a ZIP archive and extract it on your machine

3. **Run the restoration .BAT:** Execute the .BAT file "**restore-packages-opensilver.bat**" *(or "restore-packages-cshtml5.bat" depending on whether you are using OpenSilver or CSHTML5)*

4. **Delete bin/obj:** Make sure to remove the "bin" and "obj" folders, if any. They can cause issues when building using the ".bat" files.

5. **Launch Developer Command Prompt:** Open the "Developer Command Prompt for VS 2019" (or newer) from your Start Menu

6. **Run the compilation .BAT:** Launch the file "**build-nuget-package-OpenSilver-private.bat**" *(or "build-nuget-package-CSHTML5-private.bat" depending on whether you are using OpenSilver or CSHTML5)* and **enter today's date** or any other unique identifier to use for the version number (eg. 2021-10-12)

For convenience, instead of building the whole packages as instructed above, you can alternatively build only the Runtime DLL, To do so, open the solution file "OpenSilver.sln" or "CSHTML5.sln" (depending on whether you are using OpenSilver or CSHTML5), choose the appropriate "Solution Configuration" (see below) 

Note: while rarely needed, there is also a .BAT file for building the Simulator package

# What are those solution configurations?

### &nbsp;&nbsp;&nbsp;&nbsp;OpenSilver configurations:

- **SL**: Uses the Silverlight-like dialect of XAML.
- **UWP**: Uses the UWP-like dialect of XAML.

### &nbsp;&nbsp;&nbsp;&nbsp;CSHTML5 configurations:

- **Debug**: Uses the UWP-like dialect of XAML.
- **Migration**: Uses the Silverlight-like dialect of XAML.

Note: the legacy **"WorkInProgress"** configurations have been removed as the unimplemented members have been merged into the current configurations.

# What if I get a compilation error with the code in this repository?

If you get a compilation error, it may be that a Visual Studio workload needs to be installed. To find out, please open the solution "OpenSilver.sln" or "CSHTML5.sln" and attempt to compile it with the Visual Studio IDE (2019 or newer).

If you are compiling using the Command Prompt, please double-check that you are using the "Developer Command Prompt", not the standard Command Prompt, and that the current directory is set to the "build" directory of this repository, because some paths in the .BAT files may be relative to the current directory.

If you still encounter any issues, please contact:
- the OpenSilver team at: https://opensilver.net/contact.aspx
- the CSHTML5 team at: http://cshtml5.com/contact.aspx





