
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

using System.Runtime.CompilerServices;
using System.Windows.Markup;

[assembly: InternalsVisibleTo("OpenSilver.Simulator")]
[assembly: InternalsVisibleTo("Runtime.OpenSilver.Tests")]
[assembly: InternalsVisibleTo("OpenSilver.ControlsKit.FastControls")]
[assembly: InternalsVisibleTo("XRSharp")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "System.Windows.Markup")] // This is used for example in the {x:Static ...} markup extension.

[assembly: InternalsVisibleTo("Telerik.Windows.Controls.GridView")]

[assembly: OpenSilver.XamlDesigner(1)]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Data")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Documents")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Input")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Markup")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media.Animation")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media.Effects")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Media.Imaging")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Shapes")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Automation")]

// http://schemas.microsoft.com/client/2007 is the default XAML namespace of Silverlight 1.0
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Data")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Documents")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Input")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Markup")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Media")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Media.Animation")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Media.Effects")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Media.Imaging")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Shapes")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Automation")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls")] // This is used for example when migrating Silverlight apps, for example with the DockPanel control that was in the Toolkit.
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls.Primitives")]
