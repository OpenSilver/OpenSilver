
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
using System.Windows;

namespace OpenSilver.Internal.Data
{
    internal sealed class DependencyPropertyChangedListener : IDependencyPropertyChangedListener, IDisposable
    {
        private readonly DependencyProperty _dp;
        private DependencyObject _source;
        private Action<DependencyObject, DependencyPropertyChangedEventArgs> _sourceCallBack;

        public DependencyPropertyChangedListener(
            DependencyObject source,
            DependencyProperty dp,
            Action<DependencyObject, DependencyPropertyChangedEventArgs> sourceCallBack)
        {
            Debug.Assert(source != null);
            Debug.Assert(dp != null);
            Debug.Assert(sourceCallBack != null);

            _source = source;
            _dp = dp;
            _sourceCallBack = sourceCallBack;

            _source.AddDependent(_dp, this);
        }

        public void Dispose()
        {
            if (_source is null)
            {
                return;
            }

            _source.RemoveDependent(_dp, this);
            _source = null;
            _sourceCallBack = null;
        }

        public void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            _sourceCallBack?.Invoke(sender, args);
        }
    }
}
