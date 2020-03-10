This repository contains the source of both CSHTML5 and OpenSilver. More information is available at: www.cshtml5.com and www.opensilver.net

# For OpenSilver:

Open the solution named "OpenSilver.sln" to compile the OpenSilver runtime.

We are working to provide more detailed instructions on how to build the OpenSilver runtime and how to contribute. We are also updating the headers to reflect the MIT license.

Please check back soon for updates.



# For CSHTML5:

Open the solution named "CSHTML5.sln" to compile the CSHTML5 runtime.


```diff
- NOTE: The source code above corresponds to CSHTML5 version 2.x,
- which is currently in the "Technology Preview" phase. You can
- download the VSIX of the version 2.x Preview at:
- http://forums.cshtml5.com/viewforum.php?f=6
```

### What is CSHTML5?
CSHTML5 – also called 'C#/XAML for HTML5' – is the first production-ready solution to make web apps in C# and XAML. It is also the only tool that enables to port existing Silverlight and WPF applications to the web, by compiling C# and XAML files to HTML and JavaScript.

Further information can be found at: http://cshtml5.com/

CSHTML5 version 2.x is powered by [Bridge.NET](https://bridge.net/)

### How to build the source code?
1. Clone this repository locally or download it as a ZIP archive and extract it on your machine
2. Execute the file "restore-packages.bat" (by double-clicking on it in Windows Explorer)
3. Open the solution file "CSHTML5.sln" (located in the "src" folder) with Visual Studio 2017 or newer
4. Build either the "Debug" configuration or the "Migration" configuration, depending on whether you want to use the "CSHTML5" NuGet package (recommended for creating new apps) or the "CSHTML5.Migration" package (recommended for migrating existing Silverlight or WPF applications). This will generate the assembly "CSHTML5.dll" in the "bin/Debug/" folder, or the assembly "CSHTML5.Migration.dll" in the "bin/Migration/" folder.

### How to test the changes that I make to the source code?
1. Create a new Visual Studio project of type CSHTML5 version 2.x. To do so, you can install the VSIX available at http://forums.cshtml5.com/viewforum.php?f=6 (look for the latest version 2.x), which will add new project templates to the Visual Studio "New Project" dialog.
2. Build the CSHTML5 source code (cf. previous question)
3. Copy the 3 files "CSHTML5.dll", "CSHTML5.pdb", and "CSHTML5.xml" from the "bin/Debug/" folder of the CSHTML5 source code (ie. the source code in this repository) into the folder "packages/CSHTML5(...)/lib/net40/" of the project that you created at step 1, overwriting the existing files
4. Build and run the project that you created at step 1

### Where is the rest of the source code?
The full source code of the CSHTML5 runtime has now been published to GitHub. You should be able to view and modify the source code of all the .NET types, the C# types, and the XAML types. Many low-level types are located in the Bridge.NET repository, which is located [here](https://github.com/cshtml5/Bridge). Any remaining parts will be published to GitHub as soon as possible.

