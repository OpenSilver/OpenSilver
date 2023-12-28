// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Windows.Controls
{
    /// <summary>
    /// A set of extension methods for manipulating collections.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal static class CollectionHelper
    {
        /// <summary>
        /// Returns a value indicating whether a collection is read-only.
        /// </summary>
        /// <param name="collection">The collection to examine.</param>
        /// <returns>A value indicating whether a collection is read-only.</returns>
        public static bool IsReadOnly(IEnumerable collection)
        {
            return
                collection.GetType().IsArray
                    || System.Windows.Controls.EnumerableExtensions
                        .Iterate(collection.GetType(), type => type.BaseType)
                        .TakeWhile(type => type != null)
                        .Any(type => type.FullName.StartsWith("System.Collections.ObjectModel.ReadOnlyCollection`1", StringComparison.Ordinal));
        }

        /// <summary>
        /// Returns a value Indicating whether an item can be inserted in a 
        /// collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to be inserted.</param>
        /// <returns>A value Indicating whether an item can be inserted in a 
        /// collection.</returns>
        public static bool CanInsert(IEnumerable collection, object item)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                return CanInsert(collectionView.SourceCollection, item);
            }

            if (IsReadOnly(collection))
            {
                return false;
            }

            Type genericListType = collection.GetType().GetInterfaces().Where(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal)).FirstOrDefault();
            if (genericListType != null)
            {
                return genericListType.GetGenericArguments()[0] == item.GetType();
            }

            if (collection is IList)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Inserts an item into the collection at an index.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The index at which to insert the item.</param> 
        /// <param name="item">The item to be inserted.</param>
        public static void Insert(IEnumerable collection, int index, object item)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                Insert(collectionView.SourceCollection, index, item);
            }
            else
            {
                Type genericListType = collection.GetType().GetInterfaces().Where(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal)).FirstOrDefault();
                if (genericListType != null)
                {
                    genericListType.GetMethod("Insert").Invoke(collection, new object[] { index, item });
                }
                else
                {
                    IList list = collection as IList;
                    list.Insert(index, item);
                }
            }
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>The number of items in the collection.</returns>
        public static int Count(IEnumerable collection)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                return Count(collectionView.SourceCollection);
            }
            else
            {
                Type genericListType = collection.GetType().GetInterfaces().Where(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.ICollection`1", StringComparison.Ordinal)).FirstOrDefault();
                if (genericListType != null)
                {
                    return (int)genericListType.GetProperty("Count").GetValue(collection, new object[] { });
                }

                IList list = collection as IList;
                if (list != null)
                {
                    return list.Count;
                }

                return System.Linq.Enumerable.Count(collection.OfType<object>());
            }
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to be added.</param>
        public static void Add(IEnumerable collection, object item)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                Add(collectionView.SourceCollection, item);
            }
            else
            {
                PropertyInfo countProperty = collection.GetType().GetProperty("Count");
                int count = (int)countProperty.GetValue(collection, new object[] { });
                Insert(collection, count, item);
            }
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to be removed.</param>
        public static void Remove(IEnumerable collection, object item)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                Remove(collectionView.SourceCollection, item);
            }
            else
            {
                Type genericListType = collection.GetType().GetInterfaces().Where(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal)).FirstOrDefault();
                if (genericListType != null)
                {
                    int index = (int)genericListType.GetMethod("IndexOf").Invoke(collection, new object[] { item });
                    if (index != -1)
                    {
                        genericListType.GetMethod("RemoveAt").Invoke(collection, new object[] { index });
                    }
                }
                else
                {
                    IList list = collection as IList;
                    list.Remove(item);
                }
            }
        }

        /// <summary>
        /// Removes an item at a given index from the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The index of the item to be removed.</param>
        public static void RemoveAt(IEnumerable collection, int index)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                RemoveAt(collectionView.SourceCollection, index);
            }
            else
            {
                Type genericListType = collection.GetType().GetInterfaces().Where(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal)).FirstOrDefault();
                if (genericListType != null)
                {
                    genericListType.GetMethod("RemoveAt").Invoke(collection, new object[] { index });
                }
                else
                {
                    IList list = collection as IList;
                    list.RemoveAt(index);
                }
            }
        }
    }
}