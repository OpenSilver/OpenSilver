

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

#if WORKINPROGRESS

using System;
using System.Collections;
using System.Collections.Generic;
#if MIGRATION
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Media.Animation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    public sealed partial class GeometryCollection : Animatable, IList, IList<Geometry>
    {
        public int IndexOf(Geometry item)
        {
            return default(int);
        }

        public void Insert(int index, Geometry item)
        {
            
        }

        public void RemoveAt(int index)
        {
            
        }

        public Geometry this[int index]
        {
            get { return default(Geometry); }
            set
            {
                
            }
        }

        public void Add(Geometry item)
        {
            
        }

        public void Clear()
        {
            
        }

        public bool Contains(Geometry item)
        {
            return default(bool);
        }

        public void CopyTo(Geometry[] array, int arrayIndex)
        {
            
        }

        public int Count
        {
            get;
        }

        public bool IsReadOnly
        {
            get;
        }

        public bool Remove(Geometry item)
        {
            return default(bool);
        }

        public IEnumerator<Geometry> GetEnumerator()
        {
            return default(IEnumerator<Geometry>);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }

        public int Add(object value)
        {
            return default(int);
        }

        public bool Contains(object value)
        {
            return default(bool);
        }

        public int IndexOf(object value)
        {
            return default(int);
        }

        public void Insert(int index, object value)
        {
            
        }

        public bool IsFixedSize
        {
            get;
        }

        public void Remove(object value)
        {
            
        }

        object IList.this[int index]
        {
            get { return default(object); }
            set
            {
                
            }
        }

        public void CopyTo(Array array, int index)
        {
            
        }

        public bool IsSynchronized
        {
            get;
        }

        public object SyncRoot
        {
            get;
        }
    }
}

#endif