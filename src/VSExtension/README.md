# How to build the VSIX installer?

Building the VSIX is only useful if you wish to test your changes to the Project Templates, the Item Templates, or the XAML editor. For all the other scenarios, building the Runtime or the NuGet package is sufficient to test your changes.

1. If needed, change the version number in "source.extension.vsixmanifest" (located in the project "OpenSilver.VSIX" or "CSHTML5.VSIX", depending on whether you want to build the VSIX of OpenSilver or CSHTML5)
2. Install Visual Studio 2015. During the installation, make sure that the "Visual Studio Extensibility Tools" optional components are installed too
3. With Visual Studio 2015, open the solution "VSExtension.OpenSilver.sln" or "VSExtension.CSHTML5.sln" depending on whether you want to build the VSIX of OpenSilver or CSHTML5
4. Rebuild the solution
5. The VSIX will be generated in a subfolder of the "bin" folder of the .VSIX project

Notes for building the VSIX:
- Note for OpenSilver: while all the projects do build fine with VS 2015, the "OpenSilver.VSIX" project can only be opened and built with VS 2019 (or newer). When building it with VS 2019 (or newer), you can unload the other projects to avoid compilation errors due to the version of VS.
- Note for CSHTML5: when building CSHTML5.VSIX, you may initially get a compilation error saying that some .nupckg files are missing. This is because the .nupckg files of CSHTML5 have not been uploaded to GitHub due to their large size. You need to manually download them and copy them into the folder that has the error. You can download those files from https://www.nuget.org/packages/cshtml5 and https://www.nuget.org/packages/cshtml5.migration

If you still encounter any issues, please contact:
- the OpenSilver team at: https://opensilver.net/contact.aspx
- the CSHTML5 team at: http://cshtml5.com/contact.aspx





