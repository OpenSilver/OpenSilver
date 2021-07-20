

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
using System.ComponentModel;
using System.Globalization;
#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    [TypeConverter(typeof(PointCollectionConverter))]
    public sealed partial class PointCollection : PresentationFrameworkCollection<Point>
    {
        #region Data

        private Path _parentPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance that is empty.
        /// </summary>
        public PointCollection()
        {

        }

        /// <summary>
        /// Initializes a new instance that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">int - The number of elements that the new list is initially capable of storing.</param>
        public PointCollection(int capacity) : base(capacity)
        {

        }

        /// <summary>
        /// Creates a PointCollection with all of the same elements as collection
        /// </summary>
        public PointCollection(IEnumerable<Point> points) : base(points)
        {

        }

        #endregion

        #region Overriden Methods

        internal override void AddOverride(Point point)
        {
            this.AddInternal(point);
            this.NotifyCollectionChanged();
        }

        internal override void ClearOverride()
        {
            this.ClearInternal();
            this.NotifyCollectionChanged();
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtInternal(index);
            this.NotifyCollectionChanged();
        }

        internal override bool RemoveOverride(Point point)
        {
            if (this.RemoveInternal(point))
            {
                this.NotifyCollectionChanged();
                return true;
            }
            return false;
        }

        internal override void InsertOverride(int index, Point point)
        {
            this.InsertInternal(index, point);
            this.NotifyCollectionChanged();
        }

        internal object ToString(object p, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is PointCollection collection;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        internal override Point GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, Point point)
        {
            this.SetItemInternal(index, point);
            this.NotifyCollectionChanged();
        }

        #endregion

        #region Internal Methods

        internal void SetParentPath(Path path)
        {
            this._parentPath = path;
        }

        private void NotifyCollectionChanged()
        {
            if (this._parentPath != null)
            {
                this._parentPath.ScheduleRedraw();
            }
        }

        #endregion
    }

#if no
    /// <summary>
    /// Represents a collection of Point values that can be individually accessed
    /// by index.
    /// </summary>
    public sealed partial class PointCollection : List<Point>//: IList<Point>, IEnumerable<Point>
    {
        ///// <summary>
        ///// Initializes a new instance of the PointCollection class.
        ///// </summary>
        //public PointCollection() { }



        //public int Count { get; }
        //public bool IsReadOnly { get; }

        //public Point this[int index] { get; set; }

        //public void Add(Point item);
        ///// <summary>
        ///// Removes all items from the collection.
        ///// </summary>
        //public void Clear();
        //public bool Contains(Point item);
        //public void CopyTo(Point[] array, int arrayIndex);
        //public int IndexOf(Point item);
        //public void Insert(int index, Point item);
        //public bool Remove(Point item);
        //public void RemoveAt(int index);
    }
#endif
}