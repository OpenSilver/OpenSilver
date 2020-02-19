
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
    /// <typeparam name="T3">The type of the third element</typeparam>
    /// <typeparam name="T4">The type of the fourth element</typeparam>
    /// <typeparam name="T5">The type of the fifth element</typeparam>
    [Obsolete]
    public sealed class Tuple2<T1, T2, T3, T4, T5>
    {
        private readonly T1 item1;
        private readonly T2 item2;
        private readonly T3 item3;
        private readonly T4 item4;
        private readonly T5 item5;

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
        /// Returns the third element of the tuple
        /// </summary>
        public T3 Item3
        {
            get { return item3; }
        }

        /// <summary>
        /// Returns the fourth element of the tuple
        /// </summary>
        public T4 Item4
        {
            get { return item4; }
        }

        /// <summary>
        /// Returns the fifth element of the tuple
        /// </summary>
        public T5 Item5
        {
            get { return item5; }
        }

        /// <summary>
        /// Create a new tuple value
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="item2">Second element of the tuple</param>
        /// <param name="item3">Third element of the tuple</param>
        /// <param name="item4">Fourth element of the tuple</param>
        /// <param name="item5">Fifth element of the tuple</param>
        public Tuple2(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (item1 == null ? 0 : item1.GetHashCode());
            hash = hash * 23 + (item2 == null ? 0 : item2.GetHashCode());
            hash = hash * 23 + (item3 == null ? 0 : item3.GetHashCode());
            hash = hash * 23 + (item4 == null ? 0 : item4.GetHashCode());
            hash = hash * 23 + (item5 == null ? 0 : item5.GetHashCode());
            return hash;
        }

        public override bool Equals(object o)
        {
            if (o.GetType() != typeof(Tuple2<T1, T2, T3, T4, T5>))
            {
                return false;
            }

            var other = (Tuple2<T1, T2, T3, T4, T5>)o;

            return this == other;
        }

        public static bool operator ==(Tuple2<T1, T2, T3, T4, T5> a, Tuple2<T1, T2, T3, T4, T5> b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }
            if (a.item1 == null && b.item1 != null) return false;
            if (a.item2 == null && b.item2 != null) return false;
            if (a.item3 == null && b.item3 != null) return false;
            if (a.item4 == null && b.item4 != null) return false;
            if (a.item5 == null && b.item5 != null) return false;
            return
                a.item1.Equals(b.item1) &&
                a.item2.Equals(b.item2) &&
                a.item3.Equals(b.item3) &&
                a.item4.Equals(b.item4) &&
                a.item5.Equals(b.item5);
        }

        public static bool operator !=(Tuple2<T1, T2, T3, T4, T5> a, Tuple2<T1, T2, T3, T4, T5> b)
        {
            return !(a == b);
        }
    }
}