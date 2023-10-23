
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

namespace System.Windows.Media.Animation;

public interface IKeyFrame
{
    /// <summary>
    /// The key time associated with the key frame.
    /// </summary>
    KeyTime KeyTime { get; set; }

    /// <summary>
    /// The value associated with the key frame.
    /// </summary>
    object Value { get; set; }
}
