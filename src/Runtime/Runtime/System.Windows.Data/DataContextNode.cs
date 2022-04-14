
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
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal sealed class DataContextNode : PropertyPathNode
    {
        private IPropertyChangedListener _dpListener;

        internal DataContextNode(PropertyPathWalker listener)
            : base(listener)
        {
        }

        public override Type Type => typeof(object);
        
        internal override void SetValue(object value)
        {
            DependencyObject sourceDO = SourceDO;
            if (sourceDO != null)
            {
                sourceDO.SetValue(FrameworkElement.DataContextProperty, value);
            }
        }

        internal override void UpdateValue()
        {
            DependencyObject sourceDO = SourceDO;
            if (sourceDO != null)
            {
                object value = sourceDO.GetValue(FrameworkElement.DataContextProperty);
                UpdateValueAndIsBroken(value, value == null);
            }
        }

        internal override void OnSourceChanged(object oldvalue, object newValue)
        {
            IPropertyChangedListener listener = _dpListener;
            if (listener != null)
            {
                _dpListener = null;
                listener.Detach();
            }

            DependencyObject sourceDO = SourceDO;
            if (sourceDO != null)
            {
                _dpListener = INTERNAL_PropertyStore.ListenToChanged(
                    sourceDO, FrameworkElement.DataContextProperty, OnPropertyChanged
                );
            }
        }

        private DependencyObject SourceDO => Source as DependencyObject;

        private void OnPropertyChanged(object sender, IDependencyPropertyChangedEventArgs args)
        {
            try
            {
                UpdateValue();
                if (Next != null)
                {
                    Next.SetSource(Value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Binding exception: {ex}");
            }
        }
    }
}
