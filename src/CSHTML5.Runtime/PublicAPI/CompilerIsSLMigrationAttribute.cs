
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
/// Specifies whether the project was compiled in the "SLMigration" mode.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class CompilerIsSLMigrationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the CompilerIsSLMigrationAttribute class with a
    /// boolean specifying whether the project was compiled in the "SLMigration" mode.
    /// </summary>
    /// <param name="isSLMigration">A boolean that is true if the project compiled in the "SLMigration" mode, and false otherwise.</param>
    public CompilerIsSLMigrationAttribute(bool isSLMigration)
    {
        this.IsSLMigration = isSLMigration;
    }

    /// <summary>
    /// Gets a boolean that is true if the project compiled in the "SLMigration" mode, and false otherwise.
    /// </summary>
    public bool IsSLMigration { get; private set; }
}

#if BRIDGE
}
#endif