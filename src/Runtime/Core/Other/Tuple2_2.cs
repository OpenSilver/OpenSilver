

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


namespace System
{
    //-----------------------------
    // Credits: https://gist.github.com/michaelbartnett/5652076
    // (public domain)
    //-----------------------------

    /// <summary>
    /// Represents a functional tuple that can be used to store
    /// two values of different types inside one object.
    /// </summary>
    /// <typeparam name="T1">The type of the first element</typeparam>
    /// <typeparam name="T2">The type of the second element</typeparam>
    [Obsolete]
    public sealed class Tuple2<T1, T2>
    {
        private readonly T1 item1;
        private readonly T2 item2;

        /// <summary>
        /// Retyurns the first element of the tuple
        /// </summary>
        public T1 Item1
        {
            get { return item1; }
        }

        /// <summary>
        /// Returns the second element of the tuple
        /// </summary>
        public T2 Item2
        {
            get { return item2; }
        }

        /// <summary>
        /// Create a new tuple value
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="item2">Second element of the tuple</param>
        public Tuple2(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public override string ToString()
        {
            return string.Format("Tuple({0}, {1})", Item1, Item2);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (item1 == null ? 0 : item1.GetHashCode());
            hash = hash * 23 + (item2 == null ? 0 : item2.GetHashCode());
            return hash;
        }

        public override bool Equals(object o)
        {
            if (!(o is Tuple2<T1, T2>)) {
                return false;
            }

            var other = (Tuple2<T1, T2>) o;

            return this == other;
        }

        public bool Equals(Tuple2<T1, T2> other)
        {
            return this == other;
        }

        public static bool operator==(Tuple2<T1, T2> a, Tuple2<T1, T2> b)
        {
            if (object.ReferenceEquals(a, null)) {
                return object.ReferenceEquals(b, null);
            }
            if (a.item1 == null && b.item1 != null) return false;
            if (a.item2 == null && b.item2 != null) return false;
            return
                a.item1.Equals(b.item1) &&
                a.item2.Equals(b.item2);
        }

        public static bool operator!=(Tuple2<T1, T2> a, Tuple2<T1, T2> b)
        {
            return !(a == b);
        }
    }
}