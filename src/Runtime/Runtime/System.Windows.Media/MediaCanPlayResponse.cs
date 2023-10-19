
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

namespace System.Windows.Media
{
    /// <summary>
    /// Describes the likelihood that the media engine can play a media source based
    /// on its file type and characteristics.
    /// </summary>
    public enum MediaCanPlayResponse
    {
        /// <summary>
        /// Media engine cannot support the media source.
        /// </summary>
        NotSupported = 0,
        /// <summary>
        /// Media engine might support the media source.
        /// </summary>
        Maybe = 1,
        /// <summary>
        /// Media engine can probably support the media source.
        /// </summary>
        Probably = 2,
    }
}