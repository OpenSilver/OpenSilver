
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