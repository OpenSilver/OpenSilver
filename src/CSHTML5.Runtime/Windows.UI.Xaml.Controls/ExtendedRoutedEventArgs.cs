
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

#if MIGRATION
namespace Microsoft.Windows
#else
namespace System.Windows
#endif
{
    public abstract class ExtendedRoutedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the present state of the 
        /// event handling for a routed event as it travels the route.
        /// </summary>
        public bool Handled { get; set; }
        //public object OriginalSource { get; }
    }
}