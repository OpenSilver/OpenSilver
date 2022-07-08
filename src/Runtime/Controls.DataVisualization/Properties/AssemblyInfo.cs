using System.Runtime.InteropServices;
using System.Windows.Markup;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties


// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.

[assembly: Guid("9cdf077c-e066-45fe-93eb-6d0bfd806569")]

[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "toolkit")]
#if MIGRATION
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls.DataVisualization")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls.DataVisualization.Charting")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls.DataVisualization.Charting.Primitives")]
#else
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "Windows.UI.Xaml.Controls.DataVisualization")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "Windows.UI.Xaml.Controls.DataVisualization.Charting")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "Windows.UI.Xaml.Controls.DataVisualization.Charting.Primitives")]
#endif

