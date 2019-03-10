
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
#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Core
#endif
{
    /// <summary>
    /// Contains the argument returned by a window size change event.
    /// </summary>
    public sealed class WindowSizeChangedEventArgs
    {
        /// <summary>
        /// Gets or sets whether the window size event was handled.
        /// </summary>
        public bool Handled { get; set; }
        /// <summary>
        /// Gets the new size of the window.
        /// </summary>
        public Size Size { get; internal set; }
    }
}