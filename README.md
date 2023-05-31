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

1. **Update Visual Studio:** Make sure you are using the very latest version of Visual Studio. To check for updates, please launch the Visual Studio Installer from your Start Menu.

2. **Clone the repo:** Clone this repository locally or download it as a ZIP archive and extract it on your machine.

3. **Run the restoration .BAT:** Execute the .BAT file "**restore-packages-opensilver.bat**" located at the root of this repository.

4. **Update the OpenSilver compiler assemblies:** OpenSilver has a dependency on an older version of OpenSilver to convert xaml files to C#, which is retrieved by the restore batch file (from Step No. **3.**). 
Due to continous updates, this restored version can sometimes have outdated OpenSilver compiler assemblies which might cause unexpected compilation or runtime errors. To fix this, run the "**update-compiler.bat**" script located in the "build" folder.

5. **Delete bin/obj:** Make sure to remove the "bin" and "obj" folders, if any. They can cause issues when building using the ".bat" files.

6. **Launch the Developer Command Prompt:** Open the "Developer Command Prompt for VS 2022" (or whatever is the latest version of Visual Studio) from your Start Menu and navigate to the "build" folder of this repository.

7. **Run the compilation .BAT:** Launch the file "**build-nuget-package-OpenSilver-private.bat**" located inside the "build" folder of this repository, and **enter today's date** or any other unique identifier to use for the version number (eg. 2023-10-18).

8. **Use the newly built packages:** The previous command will create new NuGet packages inside the "build/output/OpenSilver" folder. You can now reference those packages from any OpenSilver application project ([here is how](https://stackoverflow.com/a/55167481/17088417)) (Note: you may need to check the options "Include prerelease" in the "Manage NuGet Packages" window in order to see the newly created NuGet packages). For example, to use them on a new "Hello World" OpenSilver application, make sure you have the VSIX installed (see the top of this document), then launch Visual Studio, click "Create a new project" -> "OpenSilver application", and replace the default NuGet package references with the new packages that you have built.
  
For convenience, instead of re-building the whole packages every time that you make a change to the OpenSilver code, you can build only the OpenSilver Runtime DLL, To do so, open the solution file "OpenSilver.sln" and choose the appropriate "Solution Configuration" (see list of configurations below).
  
To reduce the development inner loop time, you can also add a "Post Build" action to the OpenSilver Runtime project that will automatically copy the Runtime DLL from the "bin" folder into the OpenSilver NuGet package folder at "C:\Users\YOUR_USER_NAME\.nuget\packages\opensilver\ENTER_LATEST_VERSION_HERE\lib\netstandard2.0\"

Note: while rarely needed, there is also a .BAT file for building the Simulator package.

# What are those solution configurations?

### &nbsp;&nbsp;&nbsp;&nbsp;OpenSilver configurations:

- **SL**: Uses the Silverlight-like dialect of XAML.
- **UWP**: Uses the UWP-like dialect of XAML.

### &nbsp;&nbsp;&nbsp;&nbsp;CSHTML5 configurations:

- **Debug**: Uses the UWP-like dialect of XAML.
- **Migration**: Uses the Silverlight-like dialect of XAML.

Note: the legacy **"WorkInProgress"** configurations have been removed as the unimplemented members have been merged into the current configurations.

# What if I get a compilation error with the code in this repository?

If you get a compilation error, it may be that a Visual Studio workload needs to be installed. To find out, please open the solution "OpenSilver.sln" and attempt to compile it with the latest version of the Visual Studio IDE.

If you are compiling using the Command Prompt, please double-check that you are using the "Developer Command Prompt" instead of the standard Command Prompt, and that the current directory is set to the "build" directory of this repository, because some paths in the .BAT files may be relative to the current directory.

If you still encounter any issues, please contact the OpenSilver team at: https://opensilver.net/contact.aspx





