
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

using System.Collections.Generic;
using System.Globalization;
using OpenSilver.Internal;

namespace System.Windows.Media
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
            : base(capacity)
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
            : base(points)
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
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;

            var th = new TokenizerHelper(source, formatProvider);

            var collection = new PointCollection();

            while (th.NextToken())
            {
                var value = new Point(
                    Convert.ToDouble(th.GetCurrentToken(), formatProvider),
                    Convert.ToDouble(th.NextTokenRequired(), formatProvider));

                collection.Add(value);
            }

            return collection;
        }

        internal event EventHandler Changed;

        private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);

        internal override void AddOverride(Point point)
        {
            AddInternal(point);
            OnChanged();
        }

        internal override void ClearOverride()
        {
            ClearInternal();
            OnChanged();
        }

        internal override void RemoveAtOverride(int index)
        {
            RemoveAtInternal(index);
            OnChanged();
        }

        internal override void InsertOverride(int index, Point point)
        {
            InsertInternal(index, point);
            OnChanged();
        }

        internal override Point GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, Point point)
        {
            SetItemInternal(index, point);
            OnChanged();
        }
    }
}