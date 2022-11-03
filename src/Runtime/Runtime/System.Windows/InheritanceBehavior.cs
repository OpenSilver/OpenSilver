
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Modes used for resource lookup
    /// </summary>
    public enum InheritanceBehavior
    {
        /// <summary>
        /// Resource lookup will query through the current element and further.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Resource lookup will not query the current element but will skip over to the app dictionary.
        /// </summary>
        SkipToAppNow = 1,

        /// <summary>
        /// Resource lookup will not query the current element or any further.
        /// </summary>
        SkipAllNow = 2,
    }
}
