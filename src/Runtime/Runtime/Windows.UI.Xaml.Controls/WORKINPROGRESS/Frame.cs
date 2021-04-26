﻿

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


#if WORKINPROGRESS

#if MIGRATION
using System.Windows.Navigation;
#else
using System;
using Windows.UI.Xaml.Navigation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class Frame : ContentControl, INavigate
    {
        public Uri CurrentSource { get; private set; }

        public event NavigatingCancelEventHandler Navigating;

        public event NavigationStoppedEventHandler NavigationStopped;

        /// <summary>
        /// Reloads the current page.
        /// </summary>
        public void Refresh()
        {
        }
    }
}

#endif
