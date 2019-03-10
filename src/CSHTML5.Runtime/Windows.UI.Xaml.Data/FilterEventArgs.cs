
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
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Arguments for the Filter event.
    /// </summary>
    /// <remarks>
    /// <p>The event receiver should set Accepted to true if the item
    /// passes the filter, or false if it fails.</p>
    /// </remarks>
    public class FilterEventArgs : EventArgs
    {
        public FilterEventArgs(object item)
        {
            Item = item;
            Accepted = true;
        }

        public bool Accepted { get; set; }

        public object Item { get; private set; }
    }


    /// <summary>
    ///     The delegate to use for handlers that receive FilterEventArgs.
    /// </summary>
    public delegate void FilterEventHandler(object sender, FilterEventArgs e);
}
