
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

using System.Diagnostics;

namespace System.Windows.Input;

/// <summary>
/// Binds a <see cref="RoutedCommand"/> to the event handlers that implement the command.
/// </summary>
public class CommandBinding
{
    private ICommand _command;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBinding"/> class.
    /// </summary>
    public CommandBinding() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBinding"/> class by using the specified <see cref="ICommand"/>.
    /// </summary>
    /// <param name="command">
    /// The command to base the new <see cref="RoutedCommand"/> on.
    /// </param>
    public CommandBinding(ICommand command)
        : this(command, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBinding"/> class by using the specified <see cref="ICommand"/> 
    /// and the specified <see cref="Executed"/> event handler.
    /// </summary>
    /// <param name="command">
    /// The command to base the new <see cref="RoutedCommand"/> on.
    /// </param>
    /// <param name="executed">
    /// The handler for the <see cref="Executed"/> event on the new <see cref="RoutedCommand"/>.
    /// </param>
    public CommandBinding(ICommand command, ExecutedRoutedEventHandler executed)
        : this(command, executed, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBinding"/> class by using the specified <see cref="ICommand"/>
    /// and the specified <see cref="Executed"/> and <see cref="CanExecute"/> event handlers.
    /// </summary>
    /// <param name="command">
    /// The command to base the new <see cref="RoutedCommand"/> on.
    /// </param>
    /// <param name="executed">
    /// The handler for the <see cref="Executed"/> event on the new <see cref="RoutedCommand"/>.
    /// </param>
    /// <param name="canExecute">
    /// The handler for the <see cref="CanExecute"/> event on the new <see cref="RoutedCommand"/>.
    /// </param>
    public CommandBinding(ICommand command, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
    {
        ArgumentNullException.ThrowIfNull(command);

        _command = command;

        if (executed is not null)
        {
            Executed += executed;
        }

        if (canExecute is not null)
        {
            CanExecute += canExecute;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> associated with this <see cref="CommandBinding"/>.
    /// </summary>
    /// <returns>
    /// The command associated with this binding.
    /// </returns>
    public ICommand Command
    {
        get => _command;
        set => _command = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Occurs when the command associated with this <see cref="CommandBinding"/> executes.
    /// </summary>
    public event ExecutedRoutedEventHandler PreviewExecuted;

    /// <summary>
    /// Occurs when the command associated with this <see cref="CommandBinding"/> executes.
    /// </summary>
    public event ExecutedRoutedEventHandler Executed;

    /// <summary>
    /// Occurs when the command associated with this <see cref="CommandBinding"/> initiates a check to determine 
    /// whether the command can be executed on the current command target.
    /// </summary>
    public event CanExecuteRoutedEventHandler PreviewCanExecute;

    /// <summary>
    /// Occurs when the command associated with this <see cref="CommandBinding"/> initiates a check to determine 
    /// whether the command can be executed on the command target.
    /// </summary>
    public event CanExecuteRoutedEventHandler CanExecute;

    /// <summary>
    ///     Calls the CanExecute or PreviewCanExecute event based on the event argument's RoutedEvent.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    internal void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (e.RoutedEvent == CommandManager.CanExecuteEvent)
        {
            if (CanExecute is CanExecuteRoutedEventHandler canExecute)
            {
                canExecute(sender, e);
                if (e.CanExecute)
                {
                    e.Handled = true;
                }
            }
            else if (!e.CanExecute)
            {
                // If there is an Executed handler, then the command can be executed.
                if (Executed is not null)
                {
                    e.CanExecute = true;
                    e.Handled = true;
                }
            }
        }
        else
        {
            Debug.Assert(e.RoutedEvent == CommandManager.PreviewCanExecuteEvent);

            if (PreviewCanExecute is CanExecuteRoutedEventHandler previewCanExecute)
            {
                previewCanExecute(sender, e);
                if (e.CanExecute)
                {
                    e.Handled = true;
                }
            }
        }
    }

    private bool CheckCanExecute(object sender, ExecutedRoutedEventArgs e)
    {
        var canExecuteArgs = new CanExecuteRoutedEventArgs(e.Command, e.Parameter)
        {
            RoutedEvent = CommandManager.CanExecuteEvent,
            // Since we don't actually raise this event, we have to explicitly set the source.
            Source = e.OriginalSource
        };

        canExecuteArgs.OverrideSource(e.Source);

        OnCanExecute(sender, canExecuteArgs);

        return canExecuteArgs.CanExecute;
    }

    /// <summary>
    ///     Calls Executed or PreviewExecuted based on the event argument's RoutedEvent.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    internal void OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (e.RoutedEvent == CommandManager.ExecutedEvent)
        {
            if (Executed is ExecutedRoutedEventHandler executed)
            {
                if (CheckCanExecute(sender, e))
                {
                    executed(sender, e);
                    e.Handled = true;
                }
            }
        }
        else
        {
            Debug.Assert(e.RoutedEvent == CommandManager.PreviewExecutedEvent);

            if (PreviewExecuted is ExecutedRoutedEventHandler previewExecuted)
            {
                if (CheckCanExecute(sender, e))
                {
                    previewExecuted(sender, e);
                    e.Handled = true;
                }
            }
        }
    }
}
