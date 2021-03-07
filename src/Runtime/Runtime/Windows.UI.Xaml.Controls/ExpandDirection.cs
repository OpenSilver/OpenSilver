

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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Specifies the direction in which an
    /// <see cref="T:System.Windows.Controls.Expander" /> control opens.
    /// </summary>
    public enum ExpandDirection
    {
        /// <summary>
        /// Specifies that the <see cref="T:System.Windows.Controls.Expander" />
        /// control opens in the down direction.
        /// </summary>
        Down = 0,

        /// <summary>
        /// Specifies that the <see cref="T:System.Windows.Controls.Expander" />
        /// control opens in the up direction.
        /// </summary>
        Up = 1,

        /// <summary>
        /// Specifies that the <see cref="T:System.Windows.Controls.Expander" />
        /// control opens in the left direction.
        /// </summary>
        Left = 2,

        /// <summary>
        /// Specifies that the <see cref="T:System.Windows.Controls.Expander" />
        /// control opens in the right direction.
        /// </summary>
        Right = 3
    }
}
