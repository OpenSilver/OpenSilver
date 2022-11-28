

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public class WrapperOfWeakReferenceOfDependencyObject
    {
        public WrapperOfWeakReferenceOfDependencyObject(DependencyObject d)
        {
            DependencyObject = new WeakReference<DependencyObject>(d);
        }

        public WeakReference<DependencyObject> DependencyObject { get; }

        public static implicit operator WrapperOfWeakReferenceOfDependencyObject(DependencyObject d)
        {
            return new WrapperOfWeakReferenceOfDependencyObject(d);
        }
    }

    internal interface IHasAccessToPropertiesWhereItIsUsed2
    {
        Dictionary<WrapperOfWeakReferenceOfDependencyObject, HashSet<DependencyProperty>> PropertiesWhereUsed
        {
            get;
        }
    }

    [Obsolete("Use IHasAccessToPropertiesWhereItIsUsed2 instead.")]
    public partial interface IHasAccessToPropertiesWhereItIsUsed
    {

        HashSet<KeyValuePair<WeakReference<DependencyObject>, WeakReference<DependencyProperty>>> PropertiesWhereUsed
        {
            get;
        }
    }
}
