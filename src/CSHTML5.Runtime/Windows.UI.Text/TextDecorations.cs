
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


#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Text
#endif
{
    /// <summary>
    /// Provides a set of predefined text decorations.
    /// </summary>
    public enum TextDecorations
    {
        /// <summary>
        /// Defines a line above the text
        /// </summary>
        None = 0,

        ///// <summary>
        ///// Defines a line below the text
        ///// </summary>
        //Baseline = 1,

        /// <summary>
        /// Defines a line above the text
        /// </summary>
        OverLine = 2,

        /// <summary>
        /// Defines a line through the text
        /// </summary>
        Strikethrough = 3,

        /// <summary>
        /// Defines a line below the text
        /// </summary>
        Underline = 4
    }
}