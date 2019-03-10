
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


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
    internal class PropertyChangedListener : IPropertyChangedListener
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
