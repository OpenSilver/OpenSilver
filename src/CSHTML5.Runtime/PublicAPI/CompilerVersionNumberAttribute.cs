
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
/// Specifies the version number of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class CompilerVersionNumberAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the CompilerVersionNumberAttribute class with the
    /// version number of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.
    /// </summary>
    /// <param name="versionNumber">The version number of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.</param>
    public CompilerVersionNumberAttribute(string versionNumber)
    {
        this.VersionNumber = versionNumber;
    }

    /// <summary>
    /// The version number of the C#/XAML for HTML5 compiler used to compile the assembly being attributed
    /// </summary>
    public string VersionNumber
    {
        get
        {
            return (this.Version != null ? this.Version.ToString() : null);
        }
        set
        {
            this.Version = new Version(value);
        }
    }

    /// <summary>
    /// The version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed
    /// </summary>
    public Version Version { get; private set; }
}

#if BRIDGE
}
#endif