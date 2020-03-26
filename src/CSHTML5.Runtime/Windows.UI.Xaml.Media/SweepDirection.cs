

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Specifies the direction in which an elliptical arc is drawn.
    /// </summary>
    public enum SweepDirection
    {
        /// <summary>
        /// Arcs are drawn in a counterclockwise (negative-angle) direction.
        /// </summary>
        Counterclockwise = 0,

        /// <summary>
        /// Arcs are drawn in a clockwise (positive-angle) direction.
        /// </summary>
        Clockwise = 1,
    }
}
