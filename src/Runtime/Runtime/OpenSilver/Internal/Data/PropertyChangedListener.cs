
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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal.Data
{
    internal class PropertyChangedListener : IPropertyChangedListener
    {
        public DependencyProperty Property { get; set; }
        private INTERNAL_PropertyStorage _storage;
        private Action<object, IDependencyPropertyChangedEventArgs> _sourceCallBack;

        public PropertyChangedListener(INTERNAL_PropertyStorage storage, Action<object, IDependencyPropertyChangedEventArgs> sourceCallBack)
        {
            _storage = storage;
            _sourceCallBack = sourceCallBack;
        }

        public void Detach()
        {
            _storage.PropertyListeners.Remove(this);
            _sourceCallBack = null;
        }


        public void OnPropertyChanged(DependencyObject sender, IDependencyPropertyChangedEventArgs args)
        {
            _sourceCallBack?.Invoke(sender, args);
        }
    }
}
