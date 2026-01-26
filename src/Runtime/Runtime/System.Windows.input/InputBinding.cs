
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

using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Represents a binding between an <see cref="InputGesture"/> and a command. The command is potentially 
/// a <see cref="RoutedCommand"/>.
/// </summary>
public class InputBinding : DependencyObject, ICommandSource
{
    private InputGesture _gesture;

    /// <summary>
    /// Provides base initialization for classes derived from <see cref="InputBinding"/>.
    /// </summary>
    protected InputBinding() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputBinding"/> class with the specified command and 
    /// input gesture.
    /// </summary>
    /// <param name="command">
    /// The command to associate with gesture.
    /// </param>
    /// <param name="gesture">
    /// The input gesture to associate with command.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="command"/> or <paramref name="gesture"/> is null.
    /// </exception>
    public InputBinding(ICommand command, InputGesture gesture)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(gesture);

        Command = command;
        _gesture = gesture;
    }

    /// <summary>
    /// Identifies the <see cref="Command"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(InputBinding),
            null);

    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> associated with this input binding.
    /// </summary>
    /// <returns>
    /// The associated command.
    /// </returns>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValueInternal(CommandProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CommandParameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(
            nameof(CommandParameter),
            typeof(object),
            typeof(InputBinding),
            null);

    /// <summary>
    /// Gets or sets the command-specific data for a particular command.
    /// </summary>
    /// <returns>
    /// The command-specific data. The default is null.
    /// </returns>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValueInternal(CommandParameterProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CommandTarget"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandTargetProperty =
        DependencyProperty.Register(
            nameof(CommandTarget),
            typeof(IInputElement),
            typeof(InputBinding),
            null);

    /// <summary>
    /// Gets or sets the target element of the command.
    /// </summary>
    /// <returns>
    /// The target of the command. The default is null.
    /// </returns>
    public IInputElement CommandTarget
    {
        get => (IInputElement)GetValue(CommandTargetProperty);
        set => SetValueInternal(CommandTargetProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="InputGesture"/> associated with this input binding.
    /// </summary>
    /// <returns>
    /// The associated gesture. The default is null.
    /// </returns>
    public virtual InputGesture Gesture
    {
        get => _gesture;
        set
        {
            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(InputBinding)));
            }

            ArgumentNullException.ThrowIfNull(value);

            _gesture = value;
        }
    }
}
