
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
