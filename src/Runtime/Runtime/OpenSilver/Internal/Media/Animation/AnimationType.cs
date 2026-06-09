
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

namespace OpenSilver.Internal.Media.Animation;

/// <summary>
/// Describes the behavior of an animation.
/// </summary>
internal enum AnimationType : byte
{
    /// <summary>
    /// The animation animates from the defaultOriginValue value to the defaultDestinationValue.
    /// </summary>
    Automatic,

    /// <summary>
    /// The animation animates from the From property value to the defaultDestinationValue.
    /// </summary>
    From,

    /// <summary>
    /// The animation animates from the defaultOriginValue to the To property value.
    /// </summary>
    To,

    /// <summary>
    /// The animation animates from the defaultOriginValue value to the defaultOriginValue value plus
    /// the By property value.
    /// </summary>
    By,

    /// <summary>
    /// The animation animates from the From property value to the To property value.
    /// </summary>
    FromTo,

    /// <summary>
    /// The animation animates from the From property value to the From property value plus
    /// the By property value.
    /// </summary>
    FromBy,
}
