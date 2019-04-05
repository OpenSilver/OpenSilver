
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;

#if BRIDGE
namespace CSHTML5.Internal.Attributes
{
#endif

/// <summary>
/// Specifies the minimum version of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class MinimumRequiredCompilerVersionNumberAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the MinimumRequiredCompilerVersionNumberAttribute class with the minimum
    /// version number of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed.
    /// </summary>
    /// <param name="versionNumber">The minimum version number of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed.</param>
    public MinimumRequiredCompilerVersionNumberAttribute(string versionNumber)
    {
        this.VersionNumber = versionNumber;
    }

    /// <summary>
    /// The minimum version number of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed
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
    /// The minimum version of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed
    /// </summary>
    public Version Version { get; private set; }
}

#if BRIDGE
}
#endif