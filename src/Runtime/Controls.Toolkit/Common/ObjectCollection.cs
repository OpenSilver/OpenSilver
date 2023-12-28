// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.ObjectModel;

namespace System.Windows.Controls
{
    /// <summary>
    /// Implements a collection of objects.
    /// </summary>
    /// <remarks>
    /// ObjectCollection is intended to simplify the task of populating an
    /// ItemsSource property in XAML.
    /// </remarks>
    /// <example>
    /// <code language="XAML">
    /// <![CDATA[
    /// <ItemsControl.ItemsSource>
    ///     <controls:ObjectCollection>
    ///         <TextBlock Text="Object 1" />
    ///         <TextBlock Text="Object 2" />
    ///     </controls:ObjectCollection>
    /// </ItemsControl.ItemsSource>
    /// ]]>
    /// </code>
    /// </example>
    /// <QualityBand>Stable</QualityBand>
    public partial class ObjectCollection : Collection<object>
    {
        /// <summary>
        /// Initializes a new instance of the ObjectCollection class.
        /// </summary>
        public ObjectCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObjectCollection class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new ObjectCollection.</param>
        public ObjectCollection(IEnumerable collection)
        {
            foreach (object obj in collection)
            {
                Add(obj);
            }
        }
    }
}