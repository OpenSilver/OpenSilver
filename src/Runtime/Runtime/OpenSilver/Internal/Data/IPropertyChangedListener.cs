
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
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal.Data
{
    internal interface IDependencyPropertyChangedListener
    {
        DependencyProperty Property { get; set; }

        void OnPropertyChanged(DependencyObject sender, IDependencyPropertyChangedEventArgs args);
        
        void Detach();
    }
}
