

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
using System.Collections;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    [OpenSilver.NotImplemented]
    public partial class Int32KeyFrameCollection : Freezable, IList
    {
        [OpenSilver.NotImplemented]
        object IList.this[int index] { get => default(Int32KeyFrame); set { } }

        [OpenSilver.NotImplemented]
        public Int32KeyFrame this[int index] { get => default(Int32KeyFrame); set { } }

        [OpenSilver.NotImplemented]
        public bool IsFixedSize { get; }

        [OpenSilver.NotImplemented]
        public bool IsReadOnly { get; }

        [OpenSilver.NotImplemented]
        public int Count { get; }

        [OpenSilver.NotImplemented]
        public bool IsSynchronized { get; }

        [OpenSilver.NotImplemented]
        public object SyncRoot { get; }

        [OpenSilver.NotImplemented]
        public int Add(object value) => default(int);

        [OpenSilver.NotImplemented]
        public void Clear()
        {
        }

        [OpenSilver.NotImplemented]
        public bool Contains(object value) => default(bool);

        [OpenSilver.NotImplemented]
        public void CopyTo(Array array, int index)
        {
        }

        [OpenSilver.NotImplemented]
        public IEnumerator GetEnumerator() => default(IEnumerator);

        [OpenSilver.NotImplemented]
        public int IndexOf(object value) => default(int);

        [OpenSilver.NotImplemented]
        public void Insert(int index, object value)
        {
        }

        [OpenSilver.NotImplemented]
        public void Remove(object value)
        {
        }

        [OpenSilver.NotImplemented]
        public void RemoveAt(int index)
        {
        }

        [OpenSilver.NotImplemented]
        protected override Freezable CreateInstanceCore() => default(Freezable);
    }
}
