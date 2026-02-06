<p align="center">
  <img src="images/OpenSilver-logo.png" alt="OpenSilver Logo" width="400"/>
</p>

<h1 align="center">OpenSilver</h1>

<p align="center">
  <strong>Build Cross-Platform Apps with C#, VB.NET, or F# and XAML</strong><br>
  One codebase. Six platforms. Powered by WebAssembly and .NET.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/OpenSilver"><img src="https://img.shields.io/nuget/v/OpenSilver?color=blue&label=NuGet" alt="NuGet"/></a>
  <a href="LICENSE.txt"><img src="https://img.shields.io/badge/License-MIT-green.svg" alt="MIT License"/></a>
  <a href="https://opensilver.net"><img src="https://img.shields.io/badge/Website-opensilver.net-orange" alt="Website"/></a>
</p>

<p align="center">
  <a href="https://opensilver.net">Website</a> ·
  <a href="https://doc.opensilver.net">Documentation</a> ·
  <a href="https://xaml.io">Try Online</a> ·
  <a href="https://opensilverShowcase.com">200+ Live Samples</a> ·
  <a href="https://opensilver.net/gallery">Gallery</a> ·
  <a href="https://opensilver.net/whats-new/">What's New</a> ·
  <a href="https://opensilver.net/contact/">Contact</a>
</p>

---

## Table of Contents

- [What is OpenSilver?](#what-is-opensilver)
- [Features](#features)
- [Try OpenSilver Without Installing Anything](#-try-opensilver-without-installing-anything)
- [Migrating from WPF, Silverlight, or LightSwitch?](#-migrating-from-wpf-silverlight-or-lightswitch)
- [Create Your First OpenSilver App](#-create-your-first-opensilver-app)
- [Code Sample](#code-sample)
- [Performance Tips](#-performance-tips)
- [Documentation and Resources](#-documentation-and-resources)
- [Building from Source](#-building-from-source)
- [Contributing](#-contributing)
- [Related Repositories](#-related-repositories)
- [License](#-license)
- [Get in Touch](#-get-in-touch)

---

## What is OpenSilver?

**OpenSilver** is a modern, open-source framework for building cross-platform applications using **C#**, **VB.NET**, or **F#** combined with **XAML**. Write your code once and deploy to:

- **Web** (via WebAssembly)
- **Android**, **iOS**, **Windows**, **macOS** (via .NET MAUI Hybrid)
- **Linux** (via Photino)

OpenSilver is like **WPF, but cross-platform and evolved**. It brings the productivity of XAML and the power of .NET to every major platform.

<p align="center">
  <img src="images/OpenSilver-like-WPF.png" alt="OpenSilver is like WPF but cross-platform and evolved" width="600"/>
</p>

### How It Works

OpenSilver is **not** an emulator or a wrapper. It is a complete reimplementation of the WPF/Silverlight API from scratch, using modern .NET, WebAssembly, and the browser's DOM.

Unlike canvas-based rendering approaches, OpenSilver renders XAML using **real HTML elements**: `TextBox` becomes `<textarea>`, `MediaElement` becomes `<video>`, `PasswordBox` becomes `<input type="password">`, `Image` becomes `<img>`, and so on.

This DOM-based approach unlocks native browser behaviors: Ctrl+F search, text selection, screen readers, right-click context menus, SEO indexing, browser translation, copy/paste, mobile long-press, browser extensions, and regulatory accessibility compliance.

---

## Features

- **WPF API Compatibility (Subset):** OpenSilver implements a large and growing subset of the WPF API. Same namespaces, same XAML, same patterns (MVVM, data binding, commands, styles, templates). If you know WPF, you already know OpenSilver.
- **True Cross-Platform:** Single codebase compiles to Web, Android, iOS, Windows, macOS, and Linux.
- **DOM-Based Rendering:** XAML renders to real HTML elements, unlocking native browser behaviors: accessibility, SEO, Ctrl+F, text selection, screen readers, browser translation, and more.
- **Multi-Language Support:** Write in C#, VB.NET, or F#. OpenSilver is one of the very few solutions that lets you build web apps with VB.NET + XAML or F# + XAML.
- **Full .NET Ecosystem:** Reference any .NET NuGet package. OpenSilver and Blazor WASM share the same .NET for WebAssembly stack, so any non-UI package that works in Blazor will work in OpenSilver too.
- **Blazor Integration:** Mix XAML and Razor files in the same project. Embed Blazor components (DevExpress, Syncfusion, Radzen, Blazorise, MudBlazor, etc.) directly inside XAML. [Learn more](https://doc.opensilver.net/documentation/general/opensilver-blazor.html)
- **JavaScript Interop:** Call any JavaScript library from C#. Full access to the JS ecosystem from all target platforms. [Learn more](https://doc.opensilver.net/documentation/general/javascript-interop-and-libraries.html) · [Interop Samples](https://opensilvershowcase.com/#/Interop_Samples) · [JS Libraries Samples](https://opensilvershowcase.com/#/JS_Libs)
- **Visual XAML Designer:** Drag-and-drop UI designer in Visual Studio, VS Code (Windows, macOS, Linux), and online at [XAML.io](https://xaml.io). The first XAML designer for VS Code!

<p align="center">
  <img src="images/OpenSilver-VS-Code-XAML-designer-macOS.jpg" alt="Drag-and-drop XAML designer in VS Code on macOS" width="900"/>
</p>

---

## 🌐 Try OpenSilver Without Installing Anything

**[XAML.io](https://xaml.io):** Write and run C#/XAML code directly in your browser. No installation required. Perfect for quick experiments, learning, and sharing code snippets.

<p align="center">
  <img src="images/xaml-io-screenshot1.png" alt="XAML.io Screenshot" width="900"/>
</p>

**[OpenSilverShowcase.com](https://opensilverShowcase.com):** Over 200 interactive C#/XAML samples running live. View source code for each sample with a toggle to switch between C#, VB.NET, and F#. Includes examples of Blazor component integration. Also available on [Android](https://play.google.com/store/apps/details?id=net.opensilver.showcase) and [iOS](https://apps.apple.com/us/app/opensilver-showcase/id6746472943). [Source code](https://github.com/OpenSilver/OpenSilver.Samples.Showcase).

<p align="center">
  <img src="images/OpenSilverShowcase-screenshot1.png" alt="OpenSilverShowcase.com Screenshot" width="500"/>
  &nbsp;&nbsp;
  <img src="images/OpenSilverShowcase-screenshot-mobile1.png" alt="OpenSilver Showcase on mobile" width="150"/>
</p>

Visit the [Gallery](https://opensilver.net/gallery) to see real-world applications built with OpenSilver.

---

## 🔄 Migrating from WPF, Silverlight, or LightSwitch?

Because OpenSilver implements a subset of the WPF API, it provides a realistic path to bring existing applications to the web and mobile without a full rewrite:

- **Silverlight** apps: 99% API compatibility, minimal code changes needed
- **WPF** apps: 70%+ API compatibility and growing with every release
- **LightSwitch** apps: supported via a dedicated Compatibility Pack ([Learn more](https://opensilver.net/announcements/2-2/) · [Request Compatibility Pack](https://opensilver.net/request-lightswitch-compatibility-pack/))
- **WinForms** apps: possible, though more refactoring is required

The team that built OpenSilver has been migrating enterprise applications since 2014. Whether you need a full end-to-end migration, help porting a specific module, or want to accelerate the implementation of a WPF feature your app depends on, we work with teams of all sizes.

We offer free migration assessments. [Let's talk about your project](https://opensilver.net/contact.aspx)

---

## 📦 Create Your First OpenSilver App

### Visual Studio (Windows)

1. Install Visual Studio 2022 or newer (VS 2026 is supported) with the `ASP.NET and web development` and `.NET desktop development` workloads, plus .NET 8.0 SDK or later
2. Download the OpenSilver extension (VSIX) from [opensilver.net/download](https://opensilver.net/download/)
3. Click "Create a new project", search for "OpenSilver", and choose your target platforms
<p align="center">
  <img src="images/OpenSilver-project-wizard.png" alt="OpenSilver Project Wizard" width="600"/>
</p>
4. Press `F5` to build and run!

--
> **Preview packages:** If you want the very latest iteration of OpenSilver, check "Include prerelease" in the NuGet Package Manager. Preview packages are built for each commit on the `develop` branch and published to a [MyGet feed](https://www.myget.org/F/opensilver/api/v3/index.json) (already configured in your `nuget.config`). [Learn more](https://doc.opensilver.net/documentation/how-to-topics/get-latest-preview-version.html)



### Visual Studio Code (Windows, macOS, Linux)

Install the [OpenSilver extension for VS Code](https://marketplace.visualstudio.com/items?itemName=userware.vscode-opensilver) (includes project templates and the visual XAML designer), then use the Command Palette (`Ctrl+Shift+P`) and run `OpenSilver: Create New Project`. [Learn more](https://doc.opensilver.net/documentation/how-to-topics/visual-studio-code-support.html)

### Command Line (CLI)

```bash
dotnet new install OpenSilver.Templates
dotnet new opensilverapp -n MyApp
cd MyApp && dotnet run --project MyApp.Browser
```

### XAML.io (No Installation)

Visit [XAML.io](https://xaml.io), create your project in the browser, then click **File > Download Visual Studio Solution** to continue working locally in Visual Studio or VS Code.




### Solution Structure

<p align="center">
  <img src="images/OpenSilver-solution-structure.png" alt="OpenSilver Solution Structure" width="400"/>
</p>

| Project | Description |
|---------|-------------|
| **MyApp** | Main project containing all your C#/XAML files. Cannot be run directly. |
| **MyApp.Browser** | Entry point for Web deployment (WebAssembly) |
| **MyApp.MauiHybrid** | Entry point for Android, iOS, Windows, macOS (via .NET MAUI) |
| **MyApp.Photino** | Entry point for Linux desktop (via Photino) |
| **MyApp.Simulator** | Entry point for the Simulator, which provides the best debugging experience (faster startup, better exception reporting, full .NET debugging features such as move execution point, etc.) |

<p align="center">
  <img src="images/OpenSilver-compilation-targets.png" alt="OpenSilver compilation targets" width="800"/>
</p>

---

## Code Sample

```xml
<!-- MainPage.xaml -->
<Page x:Class="MyApp.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
        <TextBlock Text="Hello, OpenSilver!" FontSize="32" Margin="0,0,0,20"/>
        <Button Content="Click Me!" Click="Button_Click"/>
        <TextBlock x:Name="ResultText" FontSize="18" Margin="0,20,0,0"/>
    </StackPanel>
</Page>
```

```csharp
// MainPage.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;

namespace MyApp;

public partial class MainPage : Page
{
    private int clickCount = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        clickCount++;
        ResultText.Text = $"Button clicked {clickCount} time(s)!";
    }
}
```

This example uses C#, but **VB.NET** and **F#** work equally well with XAML. See all three languages side by side on [OpenSilverShowcase.com](https://opensilverShowcase.com).

---

## ⚡ Performance Tips

Debug mode is not representative of production performance. Publishing is ~3x faster than Debug, and enabling AOT compilation doubles that again (~6x faster than Debug). For best results in production, also enable IIS compression, virtualization for large lists/comboboxes/treeviews, lazy-loading of large assemblies, and configure trimming to reduce app size. [Learn more](https://doc.opensilver.net/documentation/in-depth-topics/performance-improvement.html)

<p align="center">
  <img src="images/OpenSilver-3-2-performance-comparison.jpg" alt="Performance comparison across different modes" width="350"/>
</p>

---

## 📖 Documentation and Resources

| Resource | Description |
|----------|-------------|
| [Official Documentation](https://doc.opensilver.net) | Comprehensive guides, tutorials, and API reference |
| [Getting Started Tutorial](https://doc.opensilver.net/documentation/general/getting-started-tour.html) | Step-by-step walkthrough for beginners |
| [Common Issues and Solutions](https://doc.opensilver.net/documentation/troubleshooting/common-issues-and-solutions.html) | Troubleshooting tips |
| [OpenSilverShowcase.com](https://OpenSilverShowcase.com) | 200+ live samples with source code |

---

## 🛠️ Building from Source

Want to contribute or customize OpenSilver? See **[BUILDING.md](BUILDING.md)** for full instructions on cloning, building, and using custom NuGet packages.

The extensions for VS and VS Code can also built from source: [github.com/OpenSilver/OpenSilver.VSIX](https://github.com/OpenSilver/OpenSilver.VSIX)

---

## 🤝 Contributing

We welcome contributions from the community! Whether it's fixing bugs, improving documentation, or adding new features, every contribution helps make OpenSilver better.

- Read our [Contributing Guide](CONTRIBUTING.md)
- Check out the [open issues](https://github.com/OpenSilver/OpenSilver/issues)
- Submit pull requests to the `develop` branch

Missing a WPF feature? [Request it on GitHub](https://github.com/OpenSilver/OpenSilver/issues) or [sponsor its development](https://opensilver.net/contact.aspx) to accelerate its implementation.

---

## 🔗 Related Repositories

| Repository | Description |
|------------|-------------|
| [OpenSilver.VSIX](https://github.com/OpenSilver/OpenSilver.VSIX) | Source code for Visual Studio, VS Code, and CLI extensions |
| [OpenSilver.Documentation](https://github.com/OpenSilver/OpenSilver.Documentation) | Documentation source |
| [OpenSilver.Samples.Showcase](https://github.com/OpenSilver/OpenSilver.Samples.Showcase) | Source code for the Showcase app (200+ samples) |

---

## 📜 License

OpenSilver is **free and open source**, released under the **[MIT License](LICENSE.txt)**.

You can use OpenSilver in commercial projects without any licensing fees. While not required, attribution and links to [opensilver.net](https://opensilver.net) are greatly appreciated!

---

## 💬 Get in Touch

| | |
|--|--|
| **Website** | [opensilver.net](https://opensilver.net) |
| **Documentation** | [doc.opensilver.net](https://doc.opensilver.net) |
| **Contact** | [opensilver.net/contact](https://opensilver.net/contact) |
| **Email** | contact@opensilver.net |

### Work with the Core Team

OpenSilver is built and maintained by [Userware](https://opensilver.net/contact/). It is free and MIT licensed. Working with Userware directly funds the roadmap toward full WPF API coverage and helps make OpenSilver better for everyone.

**How we can help:**
- **Migrate** your WPF, Silverlight, LightSwitch, or WinForms application
- **Accelerate** the implementation of a specific feature your project needs
- **Support** your team with priority assistance, training, or consulting

[Get in touch](https://opensilver.net/contact.aspx)

---

<p align="center">
  <strong>⭐ Star this repository if you find OpenSilver useful!</strong><br>
  Your support helps us grow the community and improve the framework.
</p>
