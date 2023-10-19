
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

namespace System.Windows.Data
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
