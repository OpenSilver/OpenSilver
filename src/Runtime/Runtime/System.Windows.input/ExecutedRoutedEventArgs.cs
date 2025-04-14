
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

namespace System.Windows.Input;

/// <summary>
/// Represents the method that will handle the <see cref="CommandBinding.Executed"/> and 
/// <see cref="CommandBinding.PreviewExecuted"/> routed events, as well as related attached events.
/// </summary>
/// <param name="sender">
/// The object where the event handler is attached.
/// </param>
/// <param name="e">
/// The event data.
/// </param>
public delegate void ExecutedRoutedEventHandler(object sender, ExecutedRoutedEventArgs e);

/// <summary>
/// Provides data for the CommandManager.Executed and CommandManager.PreviewExecuted routed events.
/// </summary>
public sealed class ExecutedRoutedEventArgs : RoutedEventArgs
{
    internal ExecutedRoutedEventArgs(ICommand command, object parameter)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
        Parameter = parameter;
    }

    /// <summary>
    /// Gets the command that was invoked.
    /// </summary>
    /// <returns>
    /// The command associated with this event.
    /// </returns>
    public ICommand Command { get; }

    /// <summary>
    /// Gets data parameter of the command.
    /// </summary>
    /// <returns>
    /// The command-specific data. The default value is null.
    /// </returns>
    public object Parameter { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object target) =>
        ((ExecutedRoutedEventHandler)genericHandler)(target as DependencyObject, this);
}
