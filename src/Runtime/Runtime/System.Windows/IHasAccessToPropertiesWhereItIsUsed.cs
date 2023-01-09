

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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenSilver.Internal;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    [Obsolete(Helper.ObsoleteMemberMessage)]
    public interface IHasAccessToPropertiesWhereItIsUsed
    {
        HashSet<KeyValuePair<DependencyObject, DependencyProperty>> PropertiesWhereUsed
        {
            get;
        }
    }

    internal interface IHasAccessToPropertiesWhereItIsUsed2
    {
        Dictionary<WeakDependencyObjectWrapper, HashSet<DependencyProperty>> PropertiesWhereUsed
        {
            get;
        }
    }

    internal readonly struct WeakDependencyObjectWrapper
    {
        private readonly WeakReference<DependencyObject> _weakRef;
        private readonly int _hashCode;

        public WeakDependencyObjectWrapper(DependencyObject d)
        {
            Debug.Assert(d != null);
            _weakRef = new WeakReference<DependencyObject>(d);
            _hashCode = d.GetHashCode();
        }

        public bool TryGetDependencyObject(out DependencyObject dependencyObject)
            => _weakRef.TryGetTarget(out dependencyObject);

        public override bool Equals(object obj)
        {
            if (obj is WeakDependencyObjectWrapper wrapper)
            {
                if (_weakRef != wrapper._weakRef)
                {
                    return TryGetDependencyObject(out DependencyObject target1) &&
                        wrapper.TryGetDependencyObject(out DependencyObject target2) &&
                        target1 == target2;
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode() => _hashCode;
    }
}
