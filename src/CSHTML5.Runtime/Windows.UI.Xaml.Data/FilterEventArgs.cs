
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
    public partial class FilterEventArgs : EventArgs
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
