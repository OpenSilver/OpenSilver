

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHTML5.Internal
{
    static internal class INTERNAL_WorkaroundObservableCollectionBugWithJSIL
    {
#if !BRIDGE
        [JSReplacement("true")]
#else
        [Template("true")]
#endif
        static bool IsRunningInJavaScript()
        {
            return false;
        }

        public static bool Contains(IList collection, object value)
        {
            if (IsRunningInJavaScript())
            {
                return ContainsForJavascript(collection, value);
            }
            return collection.Contains(value);
        }

        private static bool ContainsForJavascript(IList collection, object value)
        {
            return ((ObservableCollection<object>)collection).Contains(value);
        }


        public static void Remove(IList collection, object value)
        {
            if (IsRunningInJavaScript())
            {
                RemoveForJavascript(collection, value);
            }
            else
            {
                collection.Remove(value);
            }
        }

        private static void RemoveForJavascript(IList collection, object value)
        {
            ((ObservableCollection<object>)collection).Remove(value);
        }

        public static void Clear(IList collection)
        {
            if (IsRunningInJavaScript())
            {
                ClearForJavascript(collection);
            }
            else
            {
                collection.Clear();
            }
        }

        private static void ClearForJavascript(IList collection)
        {
            ((ObservableCollection<object>)collection).Clear();
        }


        public static void Add(IList collection, object value)
        {
            if (IsRunningInJavaScript())
            {
                AddForJavascript(collection, value);
            }
            else
            {
                collection.Add(value);
            }
        }

        private static void AddForJavascript(IList collection, object value)
        {
            ((ObservableCollection<object>)collection).Add(value);
        }

    }
}
