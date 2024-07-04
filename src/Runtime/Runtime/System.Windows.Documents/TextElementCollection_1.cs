
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
using System.Diagnostics;
using OpenSilver.Internal.Documents;

namespace System.Windows.Documents;

/// <summary>
/// Provides standard facilities for creating and managing a type-safe, ordered collection
/// of <see cref="TextElement"/> objects.
/// </summary>
/// <typeparam name="T">
/// Type constraint for type safety of the constrained collection implementation.
/// </typeparam>
public abstract class TextElementCollection<T> : PresentationFrameworkCollection<T>, IList
    where T : TextElement
{
    private readonly UIElement _owner;
    private bool _isModel;

    internal TextElementCollection(UIElement owner)
        : base(false)
    {
        Debug.Assert(owner is not null);
        _owner = owner;
        TextContainer = TextContainersHelper.Create(owner);
    }

    internal ITextContainer TextContainer { get; }

    internal bool IsModel
    {
        get => _isModel;
        set
        {
            if (_isModel != value)
            {
                _isModel = value;
                foreach (T element in InternalItems)
                {
                    element.IsModel = value;
                }
            }
        }
    }

    internal sealed override void AddOverride(T value)
    {
        AddDependencyObjectInternal(value);
        SetVisualParent(value);
        value.IsModel = IsModel;
        OnAdd(value, Count);
    }

    internal sealed override void ClearOverride()
    {
        if (Count > 0)
        {
            T[] oldItems = InternalItems.ToArray();
            ClearDependencyObjectInternal();

            foreach (T item in oldItems)
            {
                ClearVisualParent(item);
                item.IsModel = false;
            }

            if (TextContainer is ITextContainer textContainer)
            {
                try
                {
                    foreach (T item in oldItems)
                    {
                        textContainer.OnTextRemoved(item);
                    }
                }
                finally
                {
                    textContainer.OnTextContentChanged();
                }
            }
        }
    }

    internal sealed override T GetItemOverride(int index) => GetItemInternal(index);

    internal sealed override void InsertOverride(int index, T value)
    {
        InsertDependencyObjectInternal(index, value);
        SetVisualParent(value);
        value.IsModel = IsModel;
        OnAdd(value, index);
    }

    internal sealed override void RemoveAtOverride(int index)
    {
        T item = GetItemInternal(index);
        RemoveAtDependencyObjectInternal(index);
        ClearVisualParent(item);
        item.IsModel = false;
        OnRemove(item);
    }

    internal sealed override void SetItemOverride(int index, T value)
    {
        T oldItem = GetItemInternal(index);
        SetItemDependencyObjectInternal(index, value);
        ClearVisualParent(oldItem);
        oldItem.IsModel = false;
        OnRemove(oldItem);
        SetVisualParent(value);
        value.IsModel = IsModel;
        OnAdd(value, index);
    }

    private void OnAdd(T item, int index)
    {
        if (TextContainer is ITextContainer textContainer)
        {
            try
            {
                textContainer.OnTextAdded(item, index);
            }
            finally
            {
                textContainer.OnTextContentChanged();
            }
        }
    }

    private void OnRemove(T item)
    {
        if (TextContainer is ITextContainer textContainer)
        {
            try
            {
                textContainer.OnTextRemoved(item);
            }
            finally
            {
                textContainer.OnTextContentChanged();
            }
        }
    }

    private void SetVisualParent(T item) => _owner.AddVisualChild(item);

    private void ClearVisualParent(T item) => _owner.RemoveVisualChild(item);
}
