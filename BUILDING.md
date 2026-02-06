# Building OpenSilver from Source

This guide explains how to build the OpenSilver framework from source, whether you want to contribute or create a custom build for your own project.

## Prerequisites

- Visual Studio 2022 or newer (latest version recommended)
- Developer Command Prompt for VS 2022 (or newer)

## Build Steps

1. **Clone the Repository**
   
   ```bash
   git clone https://github.com/OpenSilver/OpenSilver.git
   cd OpenSilver
   ```

2. **Restore Packages**
   
   Run the restoration script at the root of the repository:
   ```bash
   restore-packages-opensilver.bat
   ```

3. **Update Compiler Assemblies**
   
   OpenSilver has a dependency on an older version of itself to convert XAML files to C#. The restored version may sometimes have outdated compiler assemblies. To ensure you have the latest, run:
   ```bash
   cd build
   update-compiler.bat
   ```

4. **Build the NuGet Package**
   
   Open the **Developer Command Prompt for VS 2022** (not the standard Command Prompt) and navigate to the `build` folder:
   ```bash
   cd build
   build-nuget-package-OpenSilver.bat
   ```
   When prompted, enter a version identifier (e.g., `2026-01-30`).

5. **Use Your Custom Build**
   
   The built NuGet packages will be in `build/output/OpenSilver/`. To use them in your projects:
   - Add a [local NuGet source](https://stackoverflow.com/a/55167481/17088417) pointing to that folder
   - In the NuGet Package Manager, check "Include prerelease" to see your custom packages

## Tips for Faster Development

- **Use TestApplication:** The main solution (`OpenSilver.sln`) includes a `TestApplication.OpenSilver.Browser` project that directly references the OpenSilver runtime instead of referencing the NuGet package. This is ideal for rapid development: changes to the OpenSilver runtime immediately affect the TestApplication without rebuilding packages or copying DLLs.

- **Build only the Runtime DLL:** Instead of rebuilding the entire package for every change, you can build just the OpenSilver Runtime project.

- **Auto-copy with Post Build:** Add a Post Build action to the OpenSilver Runtime project to automatically copy the DLL to your NuGet cache:
  ```
  C:\Users\YOUR_USER_NAME\.nuget\packages\opensilver\VERSION\lib\netstandard2.0\
  ```

- **Other packages:** There are also separate batch files for building the Simulator, WebAssembly, MAUI Hybrid, and other packages as needed.

## Repository Structure

| Folder | Description |
|--------|-------------|
| `src/Runtime` | Core OpenSilver runtime (the C#/XAML framework) |
| `src/Compiler` | XAML to C# compiler |
| `src/Simulator` | Windows Simulator for debugging |
| `src/WebAssembly` | WebAssembly integration |
| `src/Maui` | .NET MAUI Hybrid integration |
| `src/Photino` | Photino integration for Linux |
| `src/Tests` | Unit tests and TestApplication |
| `build` | Build scripts, NuGet specifications, and output folder |

## Branches

| Branch | Description |
|--------|-------------|
| `develop` | Active development. Submit pull requests here. |
| `master` | Stable releases. Corresponds to packages on NuGet.org. |

## Troubleshooting

- **Compilation errors:** A Visual Studio workload may need to be installed. Open `OpenSilver.sln` in Visual Studio to check for missing components.
- **Command Prompt issues:** Make sure you use the "Developer Command Prompt for VS 2022" (not the standard Command Prompt). Some paths in the batch files are relative to the current directory.
- **Still having issues?** [Open an Issue on GitHub](https://github.com/OpenSilver/OpenSilver/issues) or [contact the OpenSilver team](https://opensilver.net/contact.aspx).
