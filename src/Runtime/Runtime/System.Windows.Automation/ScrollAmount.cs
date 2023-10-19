
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

using System.Windows.Automation.Provider;

namespace System.Windows.Automation
{
    /// <summary>
    /// Contains values that are used by the <see cref="IScrollProvider" /> pattern to indicate 
    /// the direction and distance to scroll.
    /// </summary>
    public enum ScrollAmount
    {
        /// <summary>
        /// Specifies that scrolling is performed in large decrements, which is equivalent to pressing 
        /// the PAGE UP key or to clicking a blank part of a scrollbar. If the distance represented by 
        /// the PAGE UP key is not a relevant amount for the control, or if no scrollbar exists, the 
        /// value represents an amount equal to the size of the currently visible window.
        /// </summary>
        LargeDecrement,
        /// <summary>
        /// Specifies that scrolling is performed in small decrements, which is equivalent to pressing 
        /// an arrow key or to clicking the arrow button on a scrollbar.
        /// </summary>
        SmallDecrement,
        /// <summary>
        /// Specifies that scrolling should not be performed.
        /// </summary>
        NoAmount,
        /// <summary>
        /// Specifies that scrolling is performed in large increments, which is equivalent to pressing 
        /// the PAGE DOWN key or to clicking a blank part of a scrollbar. If the distance represented 
        /// by the PAGE DOWN key is not a relevant amount for the control, or if no scrollbar exists, 
        /// the value represents an amount equal to the size of the currently visible region.
        /// </summary>
        LargeIncrement,
        /// <summary>
        /// Specifies that scrolling is performed in small increments, which equivalent to pressing an 
        /// arrow key or to clicking the arrow button on a scrollbar.
        /// </summary>
        SmallIncrement,
    }
}
