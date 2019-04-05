
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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
