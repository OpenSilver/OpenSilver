
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
using System.Globalization;
using OpenSilver.Internal;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a collection of <see cref="Point"/> values that can be individually
    /// accessed by index.
    /// </summary>
    public sealed class PointCollection : PresentationFrameworkCollection<Point>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointCollection"/> class.
        /// </summary>
        public PointCollection()
            : base(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointCollection"/> class with 
        /// the specified capacity.
        /// </summary>
        /// <param name="capacity">
        /// The number of <see cref="Point"/> values that the collection is initially 
        /// capable of storing.
        /// </param>
        public PointCollection(int capacity)
            : base(capacity, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointCollection"/> class that contains 
        /// items copied from the specified collection of <see cref="Point"/> values and has the 
        /// same initial capacity as the number of items copied.
        /// </summary>
        /// <param name="points">
        /// The collection whose items are copied to the new <see cref="PointCollection"/>.
        /// </param>
        public PointCollection(IEnumerable<Point> points)
            : base(points, true)
        {
        }

        /// <summary>
        /// Converts a String representation of a collection of points into an 
        /// equivalent <see cref="PointCollection"/>.
        /// </summary>
        /// <param name="source">
        /// The <see cref="string"/> representation of the collection of points.
        /// </param>
        /// <returns>
        /// The equivalent <see cref="PointCollection"/>.
        /// </returns>
        public static PointCollection Parse(string source)
        {
            var result = new PointCollection();

            if (source != null)
            {
                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(formatProvider), ' ' };
                string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                // Points count needs to be an even number
                if (split.Length % 2 == 1)
                {
                    throw new FormatException($"'{source}' is not an eligible value for a {typeof(PointCollection)}.");
                }

                for (int i = 0; i < split.Length; i += 2)
                {
                    result.Add(
                        new Point(
                            Convert.ToDouble(split[i], formatProvider),
                            Convert.ToDouble(split[i + 1], formatProvider)
                        )
                    );
                }
            }

            return result;
        }

        internal override void AddOverride(Point point) => AddInternal(point);

        internal override void ClearOverride() => ClearInternal();

        internal override void RemoveAtOverride(int index) => RemoveAtInternal(index);

        internal override void InsertOverride(int index, Point point) => InsertInternal(index, point);

        internal override Point GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, Point point) => SetItemInternal(index, point);
    }
}