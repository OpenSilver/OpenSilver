
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;

#if BRIDGE
namespace CSHTML5.Internal.Attributes
{
#endif

/// <summary>
/// Specifies the friendly name of the version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class CompilerVersionFriendlyNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the CompilerVersionFriendlyNameAttribute class with the
    /// friendly name of the version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.
    /// </summary>
    /// <param name="versionFriendlyName">The friendly name of the version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.</param>
    public CompilerVersionFriendlyNameAttribute(string versionFriendlyName)
    {
        this.VersionFriendlyName = versionFriendlyName;
    }

    /// <summary>
    /// The friendly name of the version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed
    /// </summary>
    public string VersionFriendlyName { get; private set; }
}

#if BRIDGE
}
#endif