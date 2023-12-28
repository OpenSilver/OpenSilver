// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Resource = OpenSilver.Controls.Input.Toolkit.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// Implements a collection of objects.
    /// </summary>
    /// <remarks>
    /// ObservableObjectCollection is intended to simplify the task of populating an
    /// ItemsSource property in XAML and allows for readonly collections.
    /// </remarks>
    /// <example>
    /// <code language="XAML">
    /// <![CDATA[
    /// <ItemsControl.ItemsSource>
    ///     <controls:ObservableObjectCollection>
    ///         <TextBlock Text="Object 1" />
    ///         <TextBlock Text="Object 2" />
    ///     </controls:ObservableObjectCollection>
    /// </ItemsControl.ItemsSource>
    /// ]]>
    /// </code>
    /// </example>
    /// <QualityBand>Preview</QualityBand>
    public class ObservableObjectCollection : ObservableCollection<object>, ICollection<object>
    {
        /// <summary>
        /// Gets a value indicating whether the collection is read only.
        /// </summary>
        /// <value><c>True</c> if read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool ICollection<object>.IsReadOnly
        {
            get { return IsReadOnly; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableObjectCollection"/> class.
        /// </summary>
        public ObservableObjectCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableObjectCollection"/> class.
        /// </summary>
        /// <param name="collection">The collection whose items will be copied.</param>
        public ObservableObjectCollection(IEnumerable collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (object obj in collection)
            {
                Add(obj);
            }
        }
        
        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, object item)
        {
            if (IsReadOnly)
            {
                // throw exception
                throw new InvalidOperationException(Resource.ObservableObjectCollection_ReadOnly);
            }
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            if (IsReadOnly)
            {
                // throw exception
                throw new InvalidOperationException(Resource.ObservableObjectCollection_ReadOnly);
            }
            base.RemoveItem(index);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, object item)
        {
            if (IsReadOnly)
            {
                // throw exception
                throw new InvalidOperationException(Resource.ObservableObjectCollection_ReadOnly);
            }
            base.SetItem(index, item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            if (IsReadOnly)
            {
                // throw exception
                throw new InvalidOperationException(Resource.ObservableObjectCollection_ReadOnly);
            }
            base.ClearItems();
        }
    }
}