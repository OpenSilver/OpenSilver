
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
using System.Linq;
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

    internal TextElementCollection(UIElement owner)
        : base(false)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        TextContainer = TextContainersHelper.Create(owner);
    }

    internal ITextContainer TextContainer { get; }

    internal sealed override void AddOverride(T value)
    {
        AddDependencyObjectInternal(value);
        SetVisualParent(value);
        OnAdd(value, Count);
    }

    internal sealed override void ClearOverride()
    {
        if (Count > 0)
        {
            T[] oldItems = this.ToArray();
            ClearDependencyObjectInternal();

            foreach (T item in oldItems)
            {
                ClearVisualParent(item);
            }

            try
            {
                foreach (T item in oldItems)
                {
                    TextContainer.OnTextRemoved(item);
                }
            }
            finally
            {
                TextContainer.OnTextContentChanged();
            }
        }
    }

    internal sealed override T GetItemOverride(int index) => GetItemInternal(index);

    internal sealed override void InsertOverride(int index, T value)
    {
        InsertDependencyObjectInternal(index, value);
        SetVisualParent(value);
        OnAdd(value, index);
    }

    internal sealed override void RemoveAtOverride(int index)
    {
        T item = GetItemInternal(index);
        RemoveAtDependencyObjectInternal(index);
        ClearVisualParent(item);
        OnRemove(item);
    }

    internal sealed override void SetItemOverride(int index, T value)
    {
        T oldItem = GetItemInternal(index);
        SetItemDependencyObjectInternal(index, value);
        ClearVisualParent(oldItem);
        OnRemove(oldItem);
        SetVisualParent(value);
        OnAdd(value, index);
    }

    private void OnAdd(T item, int index)
    {
        try
        {
            TextContainer.OnTextAdded(item, index);
        }
        finally
        {
            TextContainer.OnTextContentChanged();
        }
    }

    private void OnRemove(T item)
    {
        try
        {
            TextContainer.OnTextRemoved(item);
        }
        finally
        {
            TextContainer.OnTextContentChanged();
        }
    }

    private void SetVisualParent(T item) => _owner.AddVisualChild(item);

    private void ClearVisualParent(T item) => _owner.RemoveVisualChild(item);
}
