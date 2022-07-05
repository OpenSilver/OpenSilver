﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Windows.Controls.DataVisualization.Collections
{
    /// <summary>
    /// Implements a dictionary that can store multiple values for the same key.
    /// </summary>
    /// <typeparam name="TKey">Type for keys.</typeparam>
    /// <typeparam name="TValue">Type for values.</typeparam>
    internal class MultipleDictionary<TKey, TValue>
    {
        /// <summary>
        /// Gets or sets the BinaryTree instance used to store the dictionary values.
        /// </summary>
        protected LeftLeaningRedBlackTree<TKey, TValue> BinaryTree { get; set; }

        /// <summary>
        /// Initializes a new instance of the MultipleDictionary class.
        /// </summary>
        protected MultipleDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultipleDictionary class.
        /// </summary>
        /// <param name="allowDuplicateValues">The parameter is not used.</param>
        /// <param name="keyEqualityComparer">The parameter is not used.</param>
        /// <param name="valueEqualityComparer">The parameter is not used.</param>
        public MultipleDictionary(bool allowDuplicateValues, IEqualityComparer<TKey> keyEqualityComparer, IEqualityComparer<TValue> valueEqualityComparer)
        {
            Debug.Assert(null != keyEqualityComparer, "keyEqualityComparer must not be null.");
            Debug.Assert(null != valueEqualityComparer, "valueEqualityComparer must not be null.");
            this.BinaryTree = new LeftLeaningRedBlackTree<TKey, TValue>((Comparison<TKey>)((left, right) => keyEqualityComparer.GetHashCode(left).CompareTo(keyEqualityComparer.GetHashCode(right))), (Comparison<TValue>)((left, right) => valueEqualityComparer.GetHashCode(left).CompareTo(valueEqualityComparer.GetHashCode(right))));
        }

        /// <summary>Adds a key/value pair to the dictionary.</summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        public void Add(TKey key, TValue value)
        {
            this.BinaryTree.Add(key, value);
        }

        /// <summary>Removes a key/value pair from the dictionary.</summary>
        /// <param name="key">Key to remove.</param>
        /// <param name="value">Value to remove.</param>
        /// <returns>True if the value was present and removed.</returns>
        public bool Remove(TKey key, TValue value)
        {
            return this.BinaryTree.Remove(key, value);
        }

        /// <summary>Gets the count of values in the dictionary.</summary>
        public int Count
        {
            get
            {
                return this.BinaryTree.Count;
            }
        }

        /// <summary>
        /// Returns the collection of values corresponding to a key.
        /// </summary>
        /// <param name="key">Specified key.</param>
        /// <returns>Collection of values.</returns>
        public ICollection<TValue> this[TKey key]
        {
            get
            {
                return (ICollection<TValue>)this.BinaryTree.GetValuesForKey(key).ToList<TValue>();
            }
        }

        /// <summary>Clears the items in the dictionary.</summary>
        public void Clear()
        {
            this.BinaryTree.Clear();
        }
    }
}

