
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

namespace System.Windows;

/// <summary>
/// Represents a collection of objects that inherit from <see cref="SetterBase"/>.
/// </summary>
public sealed class SetterBaseCollection : PresentationFrameworkCollection<SetterBase>
{
    private bool _sealed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetterBaseCollection"/> class.
    /// </summary>
    public SetterBaseCollection()
        : base(false)
    {
    }

    /// <summary>
    /// Returns the sealed state of this object.  If true, any attempt
    /// at modifying the state of this object will trigger an exception.
    /// </summary>
    public bool IsSealed => _sealed;

    // Note: Even if SetterBase derives from DependencyObject, we don't use
    // the methods that are supposed to handle collections of DependencyObject
    // as we don't want the inheritance context to be propagated to Setters.

    internal override void AddOverride(SetterBase value)
    {
        CheckSealed();
        AddInternal(value);
    }

    internal override void ClearOverride()
    {
        CheckSealed();
        ClearInternal();
    }

    internal override void InsertOverride(int index, SetterBase value)
    {
        CheckSealed();
        InsertInternal(index, value);
    }

    internal override void RemoveAtOverride(int index)
    {
        CheckSealed();
        RemoveAtInternal(index);
    }

    internal override SetterBase GetItemOverride(int index) => GetItemInternal(index);

    internal override void SetItemOverride(int index, SetterBase value)
    {
        CheckSealed();
        SetItemInternal(index, value);
    }

    internal void Seal()
    {
        _sealed = true;

        // Seal all the setters
        foreach (SetterBase setter in InternalItems)
        {
            setter.Seal();
        }
    }

    private void CheckSealed()
    {
        if (_sealed)
        {
            throw new InvalidOperationException($"Cannot modify a '{typeof(SetterBaseCollection).Name}' after it is sealed.");
        }
    }
}
