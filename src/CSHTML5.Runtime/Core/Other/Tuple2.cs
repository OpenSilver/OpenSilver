using System;

namespace OpenSilver.Runtime.Core.Other
{
	/// <summary>
    /// Utility class that simplifies cration of tuples by using
    /// method calls instead of constructor calls
    /// </summary>
    [Obsolete]
    public static class Tuple2
    {
        /// <summary>
        /// Creates a new tuple value with the specified elements. The method
        /// can be used without specifying the generic parameters, because C#
        /// compiler can usually infer the actual types.
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="second">Second element of the tuple</param>
        /// <returns>A newly created tuple</returns>
        [Obsolete]
        public static Tuple2<T1, T2> Create<T1, T2>(T1 item1, T2 second)
        {
            return new Tuple2<T1, T2>(item1, second);
        }

        /// <summary>
        /// Creates a new tuple value with the specified elements. The method
        /// can be used without specifying the generic parameters, because C#
        /// compiler can usually infer the actual types.
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="second">Second element of the tuple</param>
        /// <param name="third">Third element of the tuple</param>
        /// <returns>A newly created tuple</returns>
        [Obsolete]
        public static Tuple2<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 second, T3 third)
        {
            return new Tuple2<T1, T2, T3>(item1, second, third);
        }

        /// <summary>
        /// Creates a new tuple value with the specified elements. The method
        /// can be used without specifying the generic parameters, because C#
        /// compiler can usually infer the actual types.
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="second">Second element of the tuple</param>
        /// <param name="third">Third element of the tuple</param>
        /// <param name="fourth">Fourth element of the tuple</param>
        /// <returns>A newly created tuple</returns>
        [Obsolete]
        public static Tuple2<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 second, T3 third, T4 fourth)
        {
            return new Tuple2<T1, T2, T3, T4>(item1, second, third, fourth);
        }

        /// <summary>
        /// Creates a new tuple value with the specified elements. The method
        /// can be used without specifying the generic parameters, because C#
        /// compiler can usually infer the actual types.
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="second">Second element of the tuple</param>
        /// <param name="third">Third element of the tuple</param>
        /// <param name="fourth">Fourth element of the tuple</param>
        /// <param name="fifth">Fifth element of the tuple</param>
        /// <returns>A newly created tuple</returns>
        [Obsolete]
        public static Tuple2<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 second, T3 third, T4 fourth, T5 fifth)
        {
            return new Tuple2<T1, T2, T3, T4, T5>(item1, second, third, fourth, fifth);
        }

        /// <summary>
        /// Creates a new tuple value with the specified elements. The method
        /// can be used without specifying the generic parameters, because C#
        /// compiler can usually infer the actual types.
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="second">Second element of the tuple</param>
        /// <param name="third">Third element of the tuple</param>
        /// <param name="fourth">Fourth element of the tuple</param>
        /// <param name="fifth">Fifth element of the tuple</param>
        /// <param name="sixth">Sixth element of the tuple</param>
        /// <returns>A newly created tuple</returns>
        [Obsolete]
        public static Tuple2<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth)
        {
            return new Tuple2<T1, T2, T3, T4, T5, T6>(item1, second, third, fourth, fifth, sixth);
        }
    }
}