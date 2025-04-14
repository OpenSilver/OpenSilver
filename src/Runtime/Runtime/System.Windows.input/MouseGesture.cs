
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

using System.ComponentModel;

namespace System.Windows.Input;

/// <summary>
/// Defines a mouse input gesture that can be used to invoke a command.
/// </summary>
[TypeConverter(typeof(MouseGestureConverter))]
public class MouseGesture : InputGesture
{
    private MouseAction _mouseAction;
    private ModifierKeys _modifiers;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseGesture"/> class.
    /// </summary>
    public MouseGesture()
    {
        _mouseAction = MouseAction.None;
        _modifiers = ModifierKeys.None;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseGesture"/> class using the specified <see cref="Input.MouseAction"/>.
    /// </summary>
    /// <param name="mouseAction">
    /// The action associated with this gesture.
    /// </param>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="mouseAction"/> is not a valid <see cref="MouseAction"/> value.
    /// </exception>
    public MouseGesture(MouseAction mouseAction)
        : this(mouseAction, ModifierKeys.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseGesture"/> class using the specified <see cref="Input.MouseAction"/>
    /// and <see cref="ModifierKeys"/>.
    /// </summary>
    /// <param name="mouseAction">
    /// The action associated with this gesture.
    /// </param>
    /// <param name="modifiers">
    /// The modifiers associated with this gesture.
    /// </param>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="mouseAction"/> is not a valid <see cref="Input.MouseAction"/> value -or-
    /// <paramref name="modifiers"/> is not a valid <see cref="ModifierKeys"/> value.
    /// </exception>
    public MouseGesture(MouseAction mouseAction, ModifierKeys modifiers)
    {
        if (!MouseActionConverter.IsDefinedMouseAction(mouseAction))
        {
            throw new InvalidEnumArgumentException(nameof(mouseAction), (int)mouseAction, typeof(MouseAction));
        }

        if (!ModifierKeysConverter.IsDefinedModifierKeys(modifiers))
        {
            throw new InvalidEnumArgumentException(nameof(modifiers), (int)modifiers, typeof(ModifierKeys));
        }

        _modifiers = modifiers;
        _mouseAction = mouseAction;
    }

    /// <summary>
    /// Gets or sets the <see cref="Input.MouseAction"/> associated with this gesture. 
    /// </summary>
    /// <returns>
    /// The mouse action associated with this gesture. The default value is <see cref="MouseAction.None"/>.
    /// </returns>
    public MouseAction MouseAction
    {
        get => _mouseAction;
        set
        {
            if (!MouseActionConverter.IsDefinedMouseAction(value))
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(MouseAction));
            }

            if (_mouseAction != value)
            {
                _mouseAction = value;
                OnPropertyChanged(nameof(MouseAction));
            }
        }
    }

    /// <summary>
    /// Gets or sets the modifier keys associated with this <see cref="MouseGesture"/>. 
    /// </summary>
    /// <returns>
    /// The modifier keys associated with this gesture. The default value is <see cref="ModifierKeys.None"/>.
    /// </returns>
    public ModifierKeys Modifiers
    {
        get => _modifiers;
        set
        {
            if (!ModifierKeysConverter.IsDefinedModifierKeys(value))
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ModifierKeys));
            }

            if (_modifiers != value)
            {
                _modifiers = value;
                OnPropertyChanged(nameof(Modifiers));
            }
        }
    }

    /// <summary>
    /// Determines whether <see cref="MouseGesture"/> matches the input associated with the specified <see cref="InputEventArgs"/> object.
    /// </summary>
    /// <param name="targetElement">
    /// The target.
    /// </param>
    /// <param name="inputEventArgs">
    /// The input event data to compare with this gesture.
    /// </param>
    /// <returns>
    /// true if the event data matches this <see cref="MouseGesture"/>; otherwise, false.
    /// </returns>
    public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
    {
        MouseAction mouseAction = GetMouseAction(inputEventArgs);
        if (mouseAction != MouseAction.None)
        {
            return MouseAction == mouseAction && Modifiers == Keyboard.Modifiers;
        }
        return false;
    }

    internal event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static MouseAction GetMouseAction(InputEventArgs inputArgs)
    {
        return inputArgs switch
        {
            MouseWheelEventArgs => MouseAction.WheelClick,
            MouseButtonEventArgs args => (args.ChangedButton, args.ClickCount) switch
            {
                (MouseButton.Left, 1) => MouseAction.LeftClick,
                (MouseButton.Left, 2) => MouseAction.LeftDoubleClick,
                (MouseButton.Right, 1) => MouseAction.RightClick,
                (MouseButton.Right, 2) => MouseAction.RightDoubleClick,
                (MouseButton.Middle, 1) => MouseAction.MiddleClick,
                (MouseButton.Middle, 2) => MouseAction.MiddleDoubleClick,
                _ => MouseAction.None,
            },
            _ => MouseAction.None,
        };
    }
}
