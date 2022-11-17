
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
using System.Diagnostics;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal.Data
{
    internal sealed class DependencyPropertyListener
    {
        private readonly DependencyProperty _dp;
        private readonly Action<object, IDependencyPropertyChangedEventArgs> _callback;
        
        private IDependencyPropertyChangedListener _dpListener;
        private object _source;

        internal DependencyPropertyListener(DependencyProperty dp, Action<object, IDependencyPropertyChangedEventArgs> callback) 
        {
            Debug.Assert(dp != null);
            Debug.Assert(callback != null);

            _dp = dp;
            _callback = callback;
        }

        public object Value { get; private set; }

        public object Source
        {
            get => _source;
            set
            {
                IDependencyPropertyChangedListener listener = _dpListener;
                if (listener != null)
                {
                    _dpListener = null;
                    listener.Detach();
                }

                _source = value;

                if (_source is DependencyObject sourceDO)
                {
                    Value = sourceDO.GetValue(_dp);
                    _dpListener = INTERNAL_PropertyStore.ListenToChanged(sourceDO, _dp, OnPropertyChanged);
                }
                else
                {
                    Value = null;
                }
            }
        }

        private void OnPropertyChanged(object sender, IDependencyPropertyChangedEventArgs args)
        {
            Value = args.NewValue;
            _callback(sender, args);
        }
    }
}
