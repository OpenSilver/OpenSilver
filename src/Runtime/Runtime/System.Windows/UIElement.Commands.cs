
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

using System.Windows.Input;
using OpenSilver.Internal;

namespace System.Windows;

public partial class UIElement
{
    private static readonly UncommonField<InputBindingCollection> InputBindingCollectionField = new();
    private static readonly UncommonField<CommandBindingCollection> CommandBindingCollectionField = new();

    /// <summary>
    /// Gets the collection of input bindings associated with this element.
    /// </summary>
    /// <returns>
    /// The collection of input bindings.
    /// </returns>
    public InputBindingCollection InputBindings
    {
        get
        {
            if (InputBindingCollectionField.GetValue(this) is not InputBindingCollection bindings)
            {
                bindings = new InputBindingCollection(this);
                InputBindingCollectionField.SetValue(this, bindings);
            }

            return bindings;
        }
    }

    /// <summary>
    /// Gets a collection of <see cref="CommandBinding"/> objects associated with this element.
    /// A <see cref="CommandBinding"/> enables command handling for this element, and declares 
    /// the linkage between a command, its events, and the handlers attached by this element.
    /// </summary>
    /// <returns>
    /// The collection of all <see cref="CommandBinding"/> objects.
    /// </returns>
    public CommandBindingCollection CommandBindings
    {
        get
        {
            if (CommandBindingCollectionField.GetValue(this) is not CommandBindingCollection bindings)
            {
                bindings = [];
                CommandBindingCollectionField.SetValue(this, bindings);
            }

            return bindings;
        }
    }

    internal InputBindingCollection InputBindingsInternal => InputBindingCollectionField.GetValue(this);

    internal CommandBindingCollection CommandBindingsInternal => CommandBindingCollectionField.GetValue(this);
}
