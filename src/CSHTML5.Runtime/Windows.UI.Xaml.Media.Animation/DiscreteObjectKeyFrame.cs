

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
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Animates from the Object value of the previous key frame to its own Value
    /// using discrete values.
    /// </summary>
    public sealed partial class DiscreteObjectKeyFrame : ObjectKeyFrame
    {
        ///// <summary>
        ///// Initializes a new instance of the DiscreteObjectKeyFrame class.
        ///// </summary>
        //public DiscreteObjectKeyFrame();
    }
}