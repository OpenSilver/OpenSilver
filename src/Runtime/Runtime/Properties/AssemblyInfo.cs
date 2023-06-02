

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !CSHTML5BLAZOR
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("CSHTML5.Runtime")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("CSHTML5.Runtime")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
#endif

#if OPENSILVER
[assembly: InternalsVisibleTo("OpenSilver.Simulator")]
#else
[assembly: InternalsVisibleTo("CSharpXamlForHtml5.Simulator")]
#endif
[assembly: InternalsVisibleTo("Runtime.OpenSilver.Tests")]
[assembly: InternalsVisibleTo("OpenSilver.ControlsKit.FastControls")]
[assembly: InternalsVisibleTo("XRSharp")]

[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "System.Windows.Markup")] // This is used for example in the {x:Static ...} markup extension.

[assembly: InternalsVisibleTo("Telerik.Windows.Controls.GridView")]


#if MIGRATION
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls.Primitives")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Data")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Documents")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Input")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Markup")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media.Animation")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media.Effects")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media.Imaging")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Shapes")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Automation")]

// http://schemas.microsoft.com/client/2007 is the default XAML namespace of Silverlight 1.0
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls.Primitives")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Data")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Documents")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Input")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Markup")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Media")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Media.Animation")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Media.Effects")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Media.Imaging")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Shapes")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Automation")]

[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Controls.Primitives")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows")]

[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls")] // This is used for example when migrating Silverlight apps, for example with the DockPanel control that was in the Toolkit.
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls.Primitives")]

#else
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls.Primitives")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Markup")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media.Effects")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.Devices.Input")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.Foundation")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Controls.Primitives")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Data")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Documents")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Media")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Media.Animation")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Media.Imaging")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Shapes")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Automation")]

// http://schemas.microsoft.com/client/2007 is the default XAML namespace of Silverlight 1.0
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls.Primitives")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Markup")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Media.Effects")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.Devices.Input")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.Foundation")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml.Controls.Primitives")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml.Data")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml.Documents")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml.Media")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml.Media.Animation")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml.Media.Imaging")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml.Shapes")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/client/2007", "Windows.UI.Xaml.Automation")]

[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "Windows.UI.Xaml.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "Windows.UI.Xaml.Controls.Primitives")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "Windows.UI.Xaml")]

[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "Windows.UI.Xaml.Controls")] // This is used for example when migrating Silverlight apps, for example with the DockPanel control that was in the Toolkit.

#endif

