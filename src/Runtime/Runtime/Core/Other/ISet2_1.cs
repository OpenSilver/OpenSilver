

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


using System.ComponentModel;

namespace System.Collections.Generic
{
    [Obsolete("Use System.Collections.Generic.ISet instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ISet2<T> : ICollection<T>
    {
        new bool Add(T item);

        void UnionWith(IEnumerable<T> other);

        void IntersectWith(IEnumerable<T> other);

        void ExceptWith(IEnumerable<T> other);

        void SymmetricExceptWith(IEnumerable<T> other);

        bool IsSubsetOf(IEnumerable<T> other);

        bool IsSupersetOf(IEnumerable<T> other);

        bool IsProperSupersetOf(IEnumerable<T> other);

        bool IsProperSubsetOf(IEnumerable<T> other);

        bool Overlaps(IEnumerable<T> other);

        bool SetEquals(IEnumerable<T> other);
    }
}