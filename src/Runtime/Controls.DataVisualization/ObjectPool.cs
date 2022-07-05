using System.Collections.Generic;
using System.Diagnostics;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>A pool of objects that can be reused.</summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    internal class ObjectPool<T>
    {
        /// <summary>
        /// A value indicating whether the pool is being traversed.
        /// </summary>
        private bool _traversing = false;
        /// <summary>The index of the current item in the list.</summary>
        private int currentIndex = 0;
        /// <summary>
        /// The default minimum number objects to keep in the pool.
        /// </summary>
        private const int DefaultMinimumObjectsInThePool = 10;
        /// <summary>A function which creates objects.</summary>
        private Func<T> _createObject;
        /// <summary>The list of objects.</summary>
        private List<T> _objects;
        /// <summary>The minimum number of objects to keep in the pool.</summary>
        private int minimumObjectsInThePool;

        /// <summary>Initializes a new instance of the ObjectPool class.</summary>
        /// <param name="minimumObjectsInThePool">The minimum number of objects
        /// to keep in the pool.</param>
        /// <param name="createObject">The function that creates the objects.</param>
        public ObjectPool(int minimumObjectsInThePool, Func<T> createObject)
        {
            this._objects = new List<T>(minimumObjectsInThePool);
            this.minimumObjectsInThePool = minimumObjectsInThePool;
            this._createObject = createObject;
            this.Reset();
        }

        /// <summary>Initializes a new instance of the ObjectPool class.</summary>
        /// <param name="createObject">The function that creates the objects.</param>
        public ObjectPool(Func<T> createObject)
          : this(10, createObject)
        {
        }

        /// <summary>
        /// Performs an operation on the subsequent, already-created objects
        /// in the pool.
        /// </summary>
        /// <param name="action">The action to perform on the remaining objects.</param>
        public void ForEachRemaining(Action<T> action)
        {
            for (int currentIndex = this.currentIndex; currentIndex < this._objects.Count; ++currentIndex)
                action(this._objects[currentIndex]);
        }

        /// <summary>
        /// Creates a new object or reuses an existing object in the pool.
        /// </summary>
        /// <returns>A new or existing object in the pool.</returns>
        public T Next()
        {
            if (this.currentIndex == this._objects.Count)
                this._objects.Add(this._createObject());
            T obj = this._objects[this.currentIndex];
            ++this.currentIndex;
            return obj;
        }

        /// <summary>Resets the pool of objects.</summary>
        public void Reset()
        {
            this._traversing = true;
            this.currentIndex = 0;
        }

        /// <summary>Finishes the object creation process.</summary>
        /// <remarks>
        /// If there are substantially more remaining objects in the pool those
        /// objects may be removed.
        /// </remarks>
        public void Done()
        {
            this._traversing = false;
            if (this.currentIndex == 0 || this._objects.Count <= 0 || this.currentIndex < this.minimumObjectsInThePool || this.currentIndex >= this._objects.Count / 2)
                return;
            this._objects.RemoveRange(this.currentIndex, this._objects.Count - this.currentIndex);
        }

        /// <summary>Removes the objects from the pool.</summary>
        public void Clear()
        {
            Debug.Assert(!this._traversing, "Cannot clear an object pool while it is being traversed.");
            this._objects.Clear();
        }
    }
}
