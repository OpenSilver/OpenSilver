

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
    public partial interface IKeyFrame
    {
        /// <summary>
        /// The key time associated with the key frame.
        /// </summary>
        /// <value></value>
        KeyTime KeyTime { get; set; }

        /// <summary>
        /// The value associated with the key frame.
        /// </summary>
        /// <value></value>
        object Value { get; set; }
    }
}
