

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


using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Used to describe how a <see cref="Page"/> should be cached when
    /// used by a <see cref="Frame"/>
    /// </summary>
    public enum NavigationCacheMode
    {
        /// <summary>
        /// The <see cref="Page"/> should never be cached, and a new
        /// instance should be created on each navigation.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// The <see cref="Page"/> should always be cached, and kept
        /// around forever, reused in all subsequent navigations
        /// to the same Uri.
        /// </summary>
        Required = 1,

        /// <summary>
        /// The <see cref="Page"/> should be cached only within
        /// the size of the cache on the <see cref="Frame"/>,
        /// and thrown away if it would exceed that.
        /// </summary>
        Enabled = 2
    }
}
