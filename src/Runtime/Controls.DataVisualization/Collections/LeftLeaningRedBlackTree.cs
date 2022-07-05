
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Windows.Controls.DataVisualization.Collections
{
    /// <summary>Implements a left-leaning red-black tree.</summary>
    /// <remarks>
    /// Based on the research paper "Left-leaning Red-Black Trees"
    /// by Robert Sedgewick. More information available at:
    /// http://www.cs.princeton.edu/~rs/talks/LLRB/RedBlack.pdf
    /// http://www.cs.princeton.edu/~rs/talks/LLRB/08Penn.pdf
    /// </remarks>
    /// <typeparam name="TKey">Type of keys.</typeparam>
    /// <typeparam name="TValue">Type of values.</typeparam>
    internal class LeftLeaningRedBlackTree<TKey, TValue>
    {
        /// <summary>Stores the key comparison function.</summary>
        private Comparison<TKey> _keyComparison;
        /// <summary>Stores the value comparison function.</summary>
        private Comparison<TValue> _valueComparison;
        /// <summary>Stores the root node of the tree.</summary>
        private LeftLeaningRedBlackTree<TKey, TValue>.Node _rootNode;

        /// <summary>
        /// Initializes a new instance of the LeftLeaningRedBlackTree class implementing a normal dictionary.
        /// </summary>
        /// <param name="keyComparison">The key comparison function.</param>
        public LeftLeaningRedBlackTree(Comparison<TKey> keyComparison)
        {
            if (null == keyComparison)
                throw new ArgumentNullException(nameof(keyComparison));
            this._keyComparison = keyComparison;
        }

        /// <summary>
        /// Initializes a new instance of the LeftLeaningRedBlackTree class implementing an ordered multi-dictionary.
        /// </summary>
        /// <param name="keyComparison">The key comparison function.</param>
        /// <param name="valueComparison">The value comparison function.</param>
        public LeftLeaningRedBlackTree(Comparison<TKey> keyComparison, Comparison<TValue> valueComparison)
          : this(keyComparison)
        {
            if (null == valueComparison)
                throw new ArgumentNullException(nameof(valueComparison));
            this._valueComparison = valueComparison;
        }

        /// <summary>
        /// Gets a value indicating whether the tree is acting as an ordered multi-dictionary.
        /// </summary>
        private bool IsMultiDictionary
        {
            get
            {
                return null != this._valueComparison;
            }
        }

        /// <summary>Adds a key/value pair to the tree.</summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        public void Add(TKey key, TValue value)
        {
            this._rootNode = this.Add(this._rootNode, key, value);
            this._rootNode.IsBlack = true;
        }

        /// <summary>
        /// Removes a key (and its associated value) from a normal (non-multi) dictionary.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        /// <returns>True if key present and removed.</returns>
        public bool Remove(TKey key)
        {
            if (this.IsMultiDictionary)
                throw new InvalidOperationException("Remove is only supported when acting as a normal (non-multi) dictionary.");
            return this.Remove(key, default(TValue));
        }

        /// <summary>Removes a key/value pair from the tree.</summary>
        /// <param name="key">Key to remove.</param>
        /// <param name="value">Value to remove.</param>
        /// <returns>True if key/value present and removed.</returns>
        public bool Remove(TKey key, TValue value)
        {
            int count = this.Count;
            if (null != this._rootNode)
            {
                this._rootNode = this.Remove(this._rootNode, key, value);
                if (null != this._rootNode)
                    this._rootNode.IsBlack = true;
            }
            return count != this.Count;
        }

        /// <summary>Removes all nodes in the tree.</summary>
        public void Clear()
        {
            this._rootNode = (LeftLeaningRedBlackTree<TKey, TValue>.Node)null;
            this.Count = 0;
        }

        /// <summary>Gets a sorted list of keys in the tree.</summary>
        /// <returns>Sorted list of keys.</returns>
        public IEnumerable<TKey> GetKeys()
        {
            TKey lastKey = default(TKey);
            bool lastKeyValid = false;
            return this.Traverse<TKey>(this._rootNode, (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, bool>)(n => !lastKeyValid || !object.Equals((object)lastKey, (object)n.Key)), (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, TKey>)(n =>
            {
                lastKey = n.Key;
                lastKeyValid = true;
                return lastKey;
            }));
        }

        /// <summary>
        /// Gets the value associated with the specified key in a normal (non-multi) dictionary.
        /// </summary>
        /// <param name="key">Specified key.</param>
        /// <returns>Value associated with the specified key.</returns>
        public TValue GetValueForKey(TKey key)
        {
            if (this.IsMultiDictionary)
                throw new InvalidOperationException("GetValueForKey is only supported when acting as a normal (non-multi) dictionary.");
            LeftLeaningRedBlackTree<TKey, TValue>.Node nodeForKey = this.GetNodeForKey(key);
            if (null != nodeForKey)
                return nodeForKey.Value;
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Gets a sequence of the values associated with the specified key.
        /// </summary>
        /// <param name="key">Specified key.</param>
        /// <returns>Sequence of values.</returns>
        public IEnumerable<TValue> GetValuesForKey(TKey key)
        {
            return this.Traverse<TValue>(this.GetNodeForKey(key), (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, bool>)(n => 0 == this._keyComparison(n.Key, key)), (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, TValue>)(n => n.Value));
        }

        /// <summary>Gets a sequence of all the values in the tree.</summary>
        /// <returns>Sequence of all values.</returns>
        public IEnumerable<TValue> GetValuesForAllKeys()
        {
            return this.Traverse<TValue>(this._rootNode, (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, bool>)(n => true), (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, TValue>)(n => n.Value));
        }

        /// <summary>Gets the count of key/value pairs in the tree.</summary>
        public int Count { get; private set; }

        /// <summary>Gets the minimum key in the tree.</summary>
        public TKey MinimumKey
        {
            get
            {
                return LeftLeaningRedBlackTree<TKey, TValue>.GetExtreme<TKey>(this._rootNode, (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, LeftLeaningRedBlackTree<TKey, TValue>.Node>)(n => n.Left), (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, TKey>)(n => n.Key));
            }
        }

        /// <summary>Gets the maximum key in the tree.</summary>
        public TKey MaximumKey
        {
            get
            {
                return LeftLeaningRedBlackTree<TKey, TValue>.GetExtreme<TKey>(this._rootNode, (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, LeftLeaningRedBlackTree<TKey, TValue>.Node>)(n => n.Right), (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, TKey>)(n => n.Key));
            }
        }

        /// <summary>Gets the minimum key's minimum value.</summary>
        public TValue MinimumValue
        {
            get
            {
                return LeftLeaningRedBlackTree<TKey, TValue>.GetExtreme<TValue>(this._rootNode, (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, LeftLeaningRedBlackTree<TKey, TValue>.Node>)(n => n.Left), (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, TValue>)(n => n.Value));
            }
        }

        /// <summary>Gets the maximum key's maximum value.</summary>
        public TValue MaximumValue
        {
            get
            {
                return LeftLeaningRedBlackTree<TKey, TValue>.GetExtreme<TValue>(this._rootNode, (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, LeftLeaningRedBlackTree<TKey, TValue>.Node>)(n => n.Right), (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, TValue>)(n => n.Value));
            }
        }

        /// <summary>Returns true if the specified node is red.</summary>
        /// <param name="node">Specified node.</param>
        /// <returns>True if specified node is red.</returns>
        private static bool IsRed(LeftLeaningRedBlackTree<TKey, TValue>.Node node)
        {
            if (null == node)
                return false;
            return !node.IsBlack;
        }

        /// <summary>
        /// Adds the specified key/value pair below the specified root node.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        /// <returns>New root node.</returns>
        private LeftLeaningRedBlackTree<TKey, TValue>.Node Add(LeftLeaningRedBlackTree<TKey, TValue>.Node node, TKey key, TValue value)
        {
            if (null == node)
            {
                ++this.Count;
                return new LeftLeaningRedBlackTree<TKey, TValue>.Node()
                {
                    Key = key,
                    Value = value
                };
            }
            if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left) && LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Right))
                LeftLeaningRedBlackTree<TKey, TValue>.FlipColor(node);
            int num = this.KeyAndValueComparison(key, value, node.Key, node.Value);
            if (num < 0)
                node.Left = this.Add(node.Left, key, value);
            else if (0 < num)
                node.Right = this.Add(node.Right, key, value);
            else if (this.IsMultiDictionary)
            {
                ++node.Siblings;
                ++this.Count;
            }
            else
                node.Value = value;
            if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Right))
                node = LeftLeaningRedBlackTree<TKey, TValue>.RotateLeft(node);
            if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left) && LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left.Left))
                node = LeftLeaningRedBlackTree<TKey, TValue>.RotateRight(node);
            return node;
        }

        /// <summary>
        /// Removes the specified key/value pair from below the specified node.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <param name="key">Key to remove.</param>
        /// <param name="value">Value to remove.</param>
        /// <returns>True if key/value present and removed.</returns>
        private LeftLeaningRedBlackTree<TKey, TValue>.Node Remove(LeftLeaningRedBlackTree<TKey, TValue>.Node node, TKey key, TValue value)
        {
            if (this.KeyAndValueComparison(key, value, node.Key, node.Value) < 0)
            {
                if (null != node.Left)
                {
                    if (!LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left) && !LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left.Left))
                        node = LeftLeaningRedBlackTree<TKey, TValue>.MoveRedLeft(node);
                    node.Left = this.Remove(node.Left, key, value);
                }
            }
            else
            {
                if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left))
                    node = LeftLeaningRedBlackTree<TKey, TValue>.RotateRight(node);
                if (this.KeyAndValueComparison(key, value, node.Key, node.Value) == 0 && null == node.Right)
                {
                    Debug.Assert(null == node.Left, "About to remove an extra node.");
                    --this.Count;
                    if (0 >= node.Siblings)
                        return (LeftLeaningRedBlackTree<TKey, TValue>.Node)null;
                    Debug.Assert(this.IsMultiDictionary, "Should not have siblings if tree is not a multi-dictionary.");
                    --node.Siblings;
                    return node;
                }
                if (null != node.Right)
                {
                    if (!LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Right) && !LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Right.Left))
                        node = LeftLeaningRedBlackTree<TKey, TValue>.MoveRedRight(node);
                    if (0 == this.KeyAndValueComparison(key, value, node.Key, node.Value))
                    {
                        --this.Count;
                        if (0 < node.Siblings)
                        {
                            Debug.Assert(this.IsMultiDictionary, "Should not have siblings if tree is not a multi-dictionary.");
                            --node.Siblings;
                        }
                        else
                        {
                            LeftLeaningRedBlackTree<TKey, TValue>.Node extreme = LeftLeaningRedBlackTree<TKey, TValue>.GetExtreme<LeftLeaningRedBlackTree<TKey, TValue>.Node>(node.Right, (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, LeftLeaningRedBlackTree<TKey, TValue>.Node>)(n => n.Left), (Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, LeftLeaningRedBlackTree<TKey, TValue>.Node>)(n => n));
                            node.Key = extreme.Key;
                            node.Value = extreme.Value;
                            node.Siblings = extreme.Siblings;
                            node.Right = this.DeleteMinimum(node.Right);
                        }
                    }
                    else
                        node.Right = this.Remove(node.Right, key, value);
                }
            }
            return LeftLeaningRedBlackTree<TKey, TValue>.FixUp(node);
        }

        /// <summary>
        /// Flip the colors of the specified node and its direct children.
        /// </summary>
        /// <param name="node">Specified node.</param>
        private static void FlipColor(LeftLeaningRedBlackTree<TKey, TValue>.Node node)
        {
            node.IsBlack = !node.IsBlack;
            node.Left.IsBlack = !node.Left.IsBlack;
            node.Right.IsBlack = !node.Right.IsBlack;
        }

        /// <summary>Rotate the specified node "left".</summary>
        /// <param name="node">Specified node.</param>
        /// <returns>New root node.</returns>
        private static LeftLeaningRedBlackTree<TKey, TValue>.Node RotateLeft(LeftLeaningRedBlackTree<TKey, TValue>.Node node)
        {
            LeftLeaningRedBlackTree<TKey, TValue>.Node right = node.Right;
            node.Right = right.Left;
            right.Left = node;
            right.IsBlack = node.IsBlack;
            node.IsBlack = false;
            return right;
        }

        /// <summary>Rotate the specified node "right".</summary>
        /// <param name="node">Specified node.</param>
        /// <returns>New root node.</returns>
        private static LeftLeaningRedBlackTree<TKey, TValue>.Node RotateRight(LeftLeaningRedBlackTree<TKey, TValue>.Node node)
        {
            LeftLeaningRedBlackTree<TKey, TValue>.Node left = node.Left;
            node.Left = left.Right;
            left.Right = node;
            left.IsBlack = node.IsBlack;
            node.IsBlack = false;
            return left;
        }

        /// <summary>
        /// Moves a red node from the right child to the left child.
        /// </summary>
        /// <param name="node">Parent node.</param>
        /// <returns>New root node.</returns>
        private static LeftLeaningRedBlackTree<TKey, TValue>.Node MoveRedLeft(LeftLeaningRedBlackTree<TKey, TValue>.Node node)
        {
            LeftLeaningRedBlackTree<TKey, TValue>.FlipColor(node);
            if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Right.Left))
            {
                node.Right = LeftLeaningRedBlackTree<TKey, TValue>.RotateRight(node.Right);
                node = LeftLeaningRedBlackTree<TKey, TValue>.RotateLeft(node);
                LeftLeaningRedBlackTree<TKey, TValue>.FlipColor(node);
                if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Right.Right))
                    node.Right = LeftLeaningRedBlackTree<TKey, TValue>.RotateLeft(node.Right);
            }
            return node;
        }

        /// <summary>
        /// Moves a red node from the left child to the right child.
        /// </summary>
        /// <param name="node">Parent node.</param>
        /// <returns>New root node.</returns>
        private static LeftLeaningRedBlackTree<TKey, TValue>.Node MoveRedRight(LeftLeaningRedBlackTree<TKey, TValue>.Node node)
        {
            LeftLeaningRedBlackTree<TKey, TValue>.FlipColor(node);
            if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left.Left))
            {
                node = LeftLeaningRedBlackTree<TKey, TValue>.RotateRight(node);
                LeftLeaningRedBlackTree<TKey, TValue>.FlipColor(node);
            }
            return node;
        }

        /// <summary>Deletes the minimum node under the specified node.</summary>
        /// <param name="node">Specified node.</param>
        /// <returns>New root node.</returns>
        private LeftLeaningRedBlackTree<TKey, TValue>.Node DeleteMinimum(LeftLeaningRedBlackTree<TKey, TValue>.Node node)
        {
            if (null == node.Left)
                return (LeftLeaningRedBlackTree<TKey, TValue>.Node)null;
            if (!LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left) && !LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left.Left))
                node = LeftLeaningRedBlackTree<TKey, TValue>.MoveRedLeft(node);
            node.Left = this.DeleteMinimum(node.Left);
            return LeftLeaningRedBlackTree<TKey, TValue>.FixUp(node);
        }

        /// <summary>
        /// Maintains invariants by adjusting the specified nodes children.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <returns>New root node.</returns>
        private static LeftLeaningRedBlackTree<TKey, TValue>.Node FixUp(LeftLeaningRedBlackTree<TKey, TValue>.Node node)
        {
            if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Right))
                node = LeftLeaningRedBlackTree<TKey, TValue>.RotateLeft(node);
            if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left) && LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left.Left))
                node = LeftLeaningRedBlackTree<TKey, TValue>.RotateRight(node);
            if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left) && LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Right))
                LeftLeaningRedBlackTree<TKey, TValue>.FlipColor(node);
            if (node.Left != null && LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left.Right) && !LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left.Left))
            {
                node.Left = LeftLeaningRedBlackTree<TKey, TValue>.RotateLeft(node.Left);
                if (LeftLeaningRedBlackTree<TKey, TValue>.IsRed(node.Left))
                    node = LeftLeaningRedBlackTree<TKey, TValue>.RotateRight(node);
            }
            return node;
        }

        /// <summary>
        /// Gets the (first) node corresponding to the specified key.
        /// </summary>
        /// <param name="key">Key to search for.</param>
        /// <returns>Corresponding node or null if none found.</returns>
        private LeftLeaningRedBlackTree<TKey, TValue>.Node GetNodeForKey(TKey key)
        {
            LeftLeaningRedBlackTree<TKey, TValue>.Node node = this._rootNode;
            while (null != node)
            {
                int num = this._keyComparison(key, node.Key);
                if (num < 0)
                {
                    node = node.Left;
                }
                else
                {
                    if (0 >= num)
                        return node;
                    node = node.Right;
                }
            }
            return (LeftLeaningRedBlackTree<TKey, TValue>.Node)null;
        }

        /// <summary>Gets an extreme (ex: minimum/maximum) value.</summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="node">Node to start from.</param>
        /// <param name="successor">Successor function.</param>
        /// <param name="selector">Selector function.</param>
        /// <returns>Extreme value.</returns>
        private static T GetExtreme<T>(LeftLeaningRedBlackTree<TKey, TValue>.Node node, Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, LeftLeaningRedBlackTree<TKey, TValue>.Node> successor, Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, T> selector)
        {
            T obj = default(T);
            for (LeftLeaningRedBlackTree<TKey, TValue>.Node node1 = node; null != node1; node1 = successor(node1))
                obj = selector(node1);
            return obj;
        }

        /// <summary>
        /// Traverses a subset of the sequence of nodes in order and selects the specified nodes.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="node">Starting node.</param>
        /// <param name="condition">Condition method.</param>
        /// <param name="selector">Selector method.</param>
        /// <returns>Sequence of selected nodes.</returns>
        private IEnumerable<T> Traverse<T>(LeftLeaningRedBlackTree<TKey, TValue>.Node node, Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, bool> condition, Func<LeftLeaningRedBlackTree<TKey, TValue>.Node, T> selector)
        {
            Stack<LeftLeaningRedBlackTree<TKey, TValue>.Node> stack = new Stack<LeftLeaningRedBlackTree<TKey, TValue>.Node>();
            LeftLeaningRedBlackTree<TKey, TValue>.Node current = node;
            while (null != current)
            {
                if (null != current.Left)
                {
                    stack.Push(current);
                    current = current.Left;
                }
                else
                {
                    do
                    {
                        for (int i = 0; i <= current.Siblings; ++i)
                        {
                            if (condition(current))
                                yield return selector(current);
                        }
                        current = current.Right;
                    }
                    while (current == null && 0 < stack.Count && null != (current = stack.Pop()));
                }
            }
        }

        /// <summary>
        /// Compares the specified keys (primary) and values (secondary).
        /// </summary>
        /// <param name="leftKey">The left key.</param>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightKey">The right key.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>CompareTo-style results: -1 if left is less, 0 if equal, and 1 if greater than right.</returns>
        private int KeyAndValueComparison(TKey leftKey, TValue leftValue, TKey rightKey, TValue rightValue)
        {
            int num = this._keyComparison(leftKey, rightKey);
            if (num == 0 && null != this._valueComparison)
                num = this._valueComparison(leftValue, rightValue);
            return num;
        }

        /// <summary>Represents a node of the tree.</summary>
        /// <remarks>
        /// Using fields instead of properties drops execution time by about 40%.
        /// </remarks>
        [DebuggerDisplay("Key={Key}, Value={Value}, Siblings={Siblings}")]
        private class Node
        {
            /// <summary>Gets or sets the node's key.</summary>
            public TKey Key;
            /// <summary>Gets or sets the node's value.</summary>
            public TValue Value;
            /// <summary>Gets or sets the left node.</summary>
            public LeftLeaningRedBlackTree<TKey, TValue>.Node Left;
            /// <summary>Gets or sets the right node.</summary>
            public LeftLeaningRedBlackTree<TKey, TValue>.Node Right;
            /// <summary>Gets or sets the color of the node.</summary>
            public bool IsBlack;
            /// <summary>
            /// Gets or sets the number of "siblings" (nodes with the same key/value).
            /// </summary>
            public int Siblings;
        }
    }
}
