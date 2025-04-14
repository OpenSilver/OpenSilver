
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
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Binds a <see cref="KeyGesture"/> to a <see cref="RoutedCommand"/> (or another <see cref="ICommand"/> implementation).
/// </summary>
public class KeyBinding : InputBinding
{
    private bool _settingGesture = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyBinding"/> class.
    /// </summary>
    public KeyBinding() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyBinding"/> class using the specified <see cref="ICommand"/> 
    /// and the specified <see cref="Input.Key"/> and <see cref="ModifierKeys"/> which will be converted into a 
    /// <see cref="KeyGesture"/>.
    /// </summary>
    /// <param name="command">
    /// The command to invoke.
    /// </param>
    /// <param name="key">
    /// The key to be associated with command.
    /// </param>
    /// <param name="modifiers">
    /// The modifiers to be associated with command.
    /// </param>
    public KeyBinding(ICommand command, Key key, ModifierKeys modifiers)
        : this(command, new KeyGesture(key, modifiers))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyBinding"/> class using the specified <see cref="ICommand"/> 
    /// and <see cref="KeyGesture"/>.
    /// </summary>
    /// <param name="command">
    /// The command to associate with gesture.
    /// </param>
    /// <param name="gesture">
    /// The key combination to associate with command.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="command"/> or <paramref name="gesture"/> is null.
    /// </exception>
    public KeyBinding(ICommand command, KeyGesture gesture)
        : base(command, gesture)
    {
        SynchronizePropertiesFromGesture(gesture);
    }

    /// <summary>
    /// Gets or sets the gesture associated with this <see cref="KeyBinding"/>.
    /// </summary>
    /// <returns>
    /// The key sequence. The default value is null.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// the value gesture is not being set to a <see cref="KeyGesture"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Attempted to set this property on a sealed <see cref="KeyBinding"/>.
    /// </exception>
    [TypeConverter(typeof(KeyGestureConverter))]
    public override InputGesture Gesture
    {
        get => base.Gesture;
        set
        {
            if (value is not KeyGesture keyGesture)
            {
                throw new ArgumentException(string.Format(Strings.InputBinding_ExpectedInputGesture, typeof(KeyGesture)));
            }

            base.Gesture = value;
            SynchronizePropertiesFromGesture(keyGesture);
        }
    }

    /// <summary>
    /// Identifies the <see cref="Modifiers"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ModifiersProperty =
        DependencyProperty.Register(
            nameof(Modifiers),
            typeof(ModifierKeys),
            typeof(KeyBinding),
            new UIPropertyMetadata(ModifierKeys.None, OnModifiersPropertyChanged));

    /// <summary>
    /// Gets or sets the <see cref="ModifierKeys"/> of the <see cref="KeyGesture"/> associated with this <see cref="KeyBinding"/>.
    /// </summary>
    /// <returns>
    /// The modifier keys of the <see cref="KeyGesture"/>. The default value is <see cref="ModifierKeys.None"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Attempted to set this property on a sealed <see cref="KeyBinding"/>.
    /// </exception>
    public ModifierKeys Modifiers
    {
        get => (ModifierKeys)GetValue(ModifiersProperty);
        set => SetValueInternal(ModifiersProperty, value);
    }

    private static void OnModifiersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        KeyBinding keyBinding = (KeyBinding)d;
        keyBinding.SynchronizeGestureFromProperties(keyBinding.Key, (ModifierKeys)(e.NewValue));
    }

    /// <summary>
    /// Identifies the <see cref="Key"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register(
            nameof(Key),
            typeof(Key),
            typeof(KeyBinding),
            new UIPropertyMetadata(Key.None, OnKeyPropertyChanged));

    /// <summary>
    /// Gets or sets the <see cref="Input.Key"/> of the <see cref="KeyGesture"/> associated with this <see cref="KeyBinding"/>.
    /// </summary>
    /// <returns>
    /// The key part of the <see cref="KeyGesture"/>. The default value is <see cref="Key.None"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Attempted to set this property on a sealed <see cref="KeyBinding"/>.
    /// </exception>
    public Key Key
    {
        get => (Key)GetValue(KeyProperty);
        set => SetValueInternal(KeyProperty, value);
    }

    private static void OnKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        KeyBinding keyBinding = (KeyBinding)d;
        keyBinding.SynchronizeGestureFromProperties((Key)(e.NewValue), keyBinding.Modifiers);
    }

    /// <summary>
    ///     Synchronized Properties from Gesture
    /// </summary>
    private void SynchronizePropertiesFromGesture(KeyGesture keyGesture)
    {
        if (_settingGesture)
        {
            return;
        }

        _settingGesture = true;
        try
        {
            Key = keyGesture.Key;
            Modifiers = keyGesture.Modifiers;
        }
        finally
        {
            _settingGesture = false;
        }
    }

    /// <summary>
    ///     Synchronized Gesture from properties
    /// </summary>
    private void SynchronizeGestureFromProperties(Key key, ModifierKeys modifiers)
    {
        if (_settingGesture)
        {
            return;
        }

        _settingGesture = true;
        try
        {
            Gesture = new KeyGesture(key, modifiers, validateGesture: false);
        }
        finally
        {
            _settingGesture = false;
        }
    }
}
