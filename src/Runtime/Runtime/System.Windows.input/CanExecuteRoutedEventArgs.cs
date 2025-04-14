
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
/// Represents the method that will handle the <see cref="CommandBinding.CanExecute"/> event.
/// </summary>
/// <param name="sender">
/// The command target that is invoking the handler.
/// </param>
/// <param name="e">
/// The event data.
/// </param>
public delegate void CanExecuteRoutedEventHandler(object sender, CanExecuteRoutedEventArgs e);

/// <summary>
/// Provides data for the CommandManager.CanExecute and CommandManager.PreviewCanExecute routed events.
/// </summary>
public sealed class CanExecuteRoutedEventArgs : RoutedEventArgs
{
    internal CanExecuteRoutedEventArgs(ICommand command, object parameter)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
        Parameter = parameter;
    }

    /// <summary>
    /// Gets the command associated with this event.
    /// </summary>
    /// <returns>
    /// The command. Unless the command is a custom command, this is generally a <see cref="RoutedCommand"/>. 
    /// There is no default value.
    /// </returns>
    public ICommand Command { get; }

    /// <summary>
    /// Gets the command specific data.
    /// </summary>
    /// <returns>
    /// The command data. The default value is null.
    /// </returns>
    public object Parameter { get; }

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="RoutedCommand"/> associated with this event can be 
    /// executed on the command target.
    /// </summary>
    /// <returns>
    /// true if the event can be executed on the command target; otherwise, false. The default value is false.
    /// </returns>
    public bool CanExecute { get; set; }

    /// <summary>
    /// Determines whether the input routed event that invoked the command should continue to route through the element tree.
    /// </summary>
    /// <returns>
    /// true if the routed event should continue to route through element tree; otherwise, false. The default value is false.
    /// </returns>
    public bool ContinueRouting { get; set; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object target) =>
        ((CanExecuteRoutedEventHandler)genericHandler)(target as DependencyObject, this);
}
