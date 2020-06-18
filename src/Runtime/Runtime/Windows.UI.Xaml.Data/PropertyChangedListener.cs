

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal partial class PropertyChangedListener : IPropertyChangedListener
    {
        
        public DependencyProperty Property {get; set;}
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
        }


        public void OnPropertyChanged(DependencyObject sender, IDependencyPropertyChangedEventArgs args)
        {
            _sourceCallBack.Invoke(sender, args);
        }
    }
}
