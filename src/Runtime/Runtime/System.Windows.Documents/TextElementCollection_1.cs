
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

using System.Collections;
using System.Collections.Generic;
using OpenSilver.Internal.Documents;

namespace System.Windows.Documents
{
    public class TextElementCollection<T> : IList, ICollection<T> where T : TextElement
    {
        #region Data
        private readonly DependencyObject _owner;
        private readonly ITextContainer _textContainer;
        private readonly List<T> _collection;
        #endregion

        #region Constructor
        internal TextElementCollection(DependencyObject owner)
        {
            this._owner = owner;
            this._collection = new List<T>();
            this._textContainer = TextContainersHelper.FromOwner(owner);
        }
        #endregion

        #region Public Properties
        public int Count
        {
            get
            {
                return this._collection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object this[int index]
        {
            get
            {
                return this._collection[index];
            }
            set
            {
                T item = value as T;
                if (item == null)
                {
                    throw new ArgumentNullException("Value cannot be null.");
                }
                this._collection[index] = item;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Public Methods
        public void Add(T item)
        {
            this.AddInternal(this.Count, item);
        }

        int IList.Add(object value)
        {
            return AddImpl(value);
        }

        public void Clear()
        {
            this.TextContainer.BeginChange();
            try
            {
                for (int i = this._collection.Count - 1; i > -1; i--)
                {
                    this.TextContainer.OnTextRemoved(this._collection[i]);
                    this._collection.RemoveAt(i);
                }
            }
            finally
            {
                this.TextContainer.EndChange();
            }
        }

        public bool Contains(T item)
        {
            return this._collection.Contains(item);
        }

        bool IList.Contains(object value)
        {
            return this.Contains(value as T);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._collection.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            ((ICollection)this._collection).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._collection.GetEnumerator();
        }

        int IList.IndexOf(object value)
        {
            return this._collection.IndexOf(value as T);
        }

        void IList.Insert(int index, object value)
        {
            T item = value as T;
            this.AddInternal(index, item);
        }

        public bool Remove(T item)
        {
            return this.RemoveInternal(item);
        }

        void IList.Remove(object value)
        {
            this.Remove(value as T);
        }

        public void RemoveAt(int index)
        {
            T item = this._collection[index];
            this._collection.RemoveAt(index);
            this.OnRemove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        #region Internal Properties
        internal ITextContainer TextContainer
        {
            get
            {
                return this._textContainer;
            }
        }
        #endregion

        #region Internal Methods
        private void OnAdd(T item)
        {
            this.TextContainer.BeginChange();
            try
            {
                this.TextContainer.OnTextAdded(item);
            }
            finally
            {
                this.TextContainer.EndChange();
            }
        }

        private void OnRemove(T item)
        {
            this.TextContainer.BeginChange();
            try
            {
                this.TextContainer.OnTextRemoved(item);
            }
            finally
            {
                this.TextContainer.EndChange();
            }
        }

        internal virtual int AddImpl(object value)
        {
            T item = value as T;
            int index = this.Count;
            this.AddInternal(index, item);
            return index;
        }

        private void AddInternal(int index, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            this._collection.Insert(index, item);
            this.SetVisualParent(item);
            this.OnAdd(item);
        }

        private bool RemoveInternal(T item)
        {
            bool success;
            if (success = this._collection.Remove(item))
            {
                this.ClearVisualParent(item);
                this.OnRemove(item);
            }
            return success;
        }

        private void SetVisualParent(T item)
        {
            if (this._owner is UIElement parent)
            {
                parent.AddVisualChild(item);
            }
        }

        private void ClearVisualParent(T item)
        {
            if (this._owner is UIElement parent)
            {
                parent.RemoveVisualChild(item);
            }
        }

        #endregion
    }
}
