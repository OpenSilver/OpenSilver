
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
/// Defines a command that implements <see cref="ICommand"/> and is routed through the element tree.
/// </summary>
[TypeConverter(typeof(CommandConverter))]
public class RoutedCommand : ICommand
{
    private readonly string _name;
    private readonly Type _ownerType;
    private readonly byte _commandId;
    private InputGestureCollection _inputGestureCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedCommand"/> class.
    /// </summary>
    public RoutedCommand()
    {
        _name = string.Empty;
        _ownerType = null;
        _inputGestureCollection = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedCommand"/> class with the specified name and owner type.
    /// </summary>
    /// <param name="name">
    /// Declared name for serialization.
    /// </param>
    /// <param name="ownerType">
    /// The type which is registering the command.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="name"/> or <paramref name="ownerType"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The length of <paramref name="name"/> is zero.
    /// </exception>
    public RoutedCommand(string name, Type ownerType)
        : this(name, ownerType, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedCommand"/> class with the specified name, owner type, and collection of gestures.
    /// </summary>
    /// <param name="name">
    /// Declared name for serialization.
    /// </param>
    /// <param name="ownerType">
    /// The type that is registering the command.
    /// </param>
    /// <param name="inputGestures">
    /// Default input gestures associated with this command.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="name"/> or <paramref name="ownerType"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The length of <paramref name="name"/> is zero.
    /// </exception>
    public RoutedCommand(string name, Type ownerType, InputGestureCollection inputGestures)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(ownerType);

        _name = name;
        _ownerType = ownerType;
        _inputGestureCollection = inputGestures;
    }

    /// <summary>
    /// RoutedCommand Constructor with Name and OwnerType and command identifier.
    /// </summary>
    /// <param name="name">Declared Name of the RoutedCommand for Serialization</param>
    /// <param name="ownerType">Type that is registering the property</param>
    /// <param name="commandId">Byte identifier for the command assigned by the owning type</param>
    internal RoutedCommand(string name, Type ownerType, byte commandId)
        : this(name, ownerType, null)
    {
        _commandId = commandId;
    }

    internal byte CommandId => _commandId;

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    /// <returns>
    /// The name of the command.
    /// </returns>
    public string Name => _name;

    /// <summary>
    /// Gets the type that is registered with the command.
    /// </summary>
    /// <returns>
    /// The type of the command owner.
    /// </returns>
    public Type OwnerType => _ownerType;

    /// <summary>
    /// Gets the collection of <see cref="InputGesture"/> objects that are associated with this command.
    /// </summary>
    /// <returns>
    /// The input gestures.
    /// </returns>
    public InputGestureCollection InputGestures
    {
        get
        {
            if (InputGesturesInternal is null)
            {
                _inputGestureCollection = [];
            }
            return _inputGestureCollection;
        }
    }

    internal InputGestureCollection InputGesturesInternal
    {
        get
        {
            if (_inputGestureCollection is null && AreInputGesturesDelayLoaded)
            {
                _inputGestureCollection = GetInputGestures();
                AreInputGesturesDelayLoaded = false;
            }
            return _inputGestureCollection;
        }
    }

    /// <summary>
    ///    Fetches the default input gestures for the command by invoking the LoadDefaultGestureFromResource function on the owning type.
    /// </summary>
    /// <returns>collection of input gestures for the command</returns>
    private InputGestureCollection GetInputGestures()
    {
        if (OwnerType == typeof(ApplicationCommands))
        {
            return ApplicationCommands.LoadDefaultGestureFromResource(_commandId);
        }
        else if (OwnerType == typeof(NavigationCommands))
        {
            return NavigationCommands.LoadDefaultGestureFromResource(_commandId);
        }
        else if (OwnerType == typeof(MediaCommands))
        {
            return MediaCommands.LoadDefaultGestureFromResource(_commandId);
        }
        else if (OwnerType == typeof(ComponentCommands))
        {
            return ComponentCommands.LoadDefaultGestureFromResource(_commandId);
        }
        return [];
    }

    internal bool AreInputGesturesDelayLoaded { get; set; }

    /// <summary>
    /// For a description of this members, see <see cref="ICommand.Execute"/>.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed, this object can be set to null.
    /// </param>
    void ICommand.Execute(object parameter) => Execute(parameter, FilterInputElement(Keyboard.FocusedElement));

    /// <summary>
    /// For a description of this members, see <see cref="ICommand.CanExecute"/>.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed, this object can be set to null.
    /// </param>
    /// <returns>
    /// true if this command can be executed; otherwise, false.
    /// </returns>
    bool ICommand.CanExecute(object parameter) => CanExecuteImpl(parameter, FilterInputElement(Keyboard.FocusedElement), false, out _);

    /// <summary>
    /// Occurs when changes to the command source are detected by the command manager. These changes often 
    /// affect whether the command should execute on the current command target.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Executes the <see cref="RoutedCommand"/> on the current command target.
    /// </summary>
    /// <param name="parameter">
    /// User defined parameter to be passed to the handler.
    /// </param>
    /// <param name="target">
    /// Element at which to begin looking for command handlers.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="target"/> is not a <see cref="UIElement"/>.
    /// </exception>
    public void Execute(object parameter, IInputElement target)
    {
        // We only support UIElement
        if (target is not null && !IsValidInputElement(target))
        {
            throw new InvalidOperationException(string.Format(Strings.Invalid_IInputElement, target.GetType()));
        }

        target ??= FilterInputElement(Keyboard.FocusedElement);

        ExecuteImpl(parameter, target, false);
    }

    /// <summary>
    /// Determines whether this <see cref="RoutedCommand"/> can execute in its current state.
    /// </summary>
    /// <param name="parameter">
    /// A user defined data type.
    /// </param>
    /// <param name="target">
    /// The command target.
    /// </param>
    /// <returns>
    /// true if the command can execute on the current command target; otherwise, false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="target"/> is not a <see cref="UIElement"/>.
    /// </exception>
    public bool CanExecute(object parameter, IInputElement target) => CriticalCanExecute(parameter, target, false, out _);

    /// <summary>
    ///     Whether the command can be executed with the given parameter on the given target.
    /// </summary>
    /// <param name="parameter">Parameter to be passed to any command handlers.</param>
    /// <param name="target">The target element on which to begin looking for command handlers.</param>
    /// <param name="trusted">Determines whether this call will elevate for userinitiated input or not.</param>
    /// <param name="continueRouting">Determines whether the input event (if any) that caused this command should continue its route.</param>
    /// <returns>true if the command can be executed, false otherwise.</returns>
    internal bool CriticalCanExecute(object parameter, IInputElement target, bool trusted, out bool continueRouting)
    {
        // We only support UIElement
        if (target is not null && !IsValidInputElement(target))
        {
            throw new InvalidOperationException(string.Format(Strings.Invalid_IInputElement, target.GetType()));
        }

        target ??= FilterInputElement(Keyboard.FocusedElement);

        return CanExecuteImpl(parameter, target, trusted, out continueRouting);
    }

    private static IInputElement FilterInputElement(IInputElement elem)
    {
        // We only support UIElement
        if (elem is not null && IsValidInputElement(elem))
        {
            return elem;
        }

        return null;
    }

    private bool CanExecuteImpl(object parameter, IInputElement target, bool trusted, out bool continueRouting)
    {
        if (target is null)
        {
            continueRouting = false;
            return false;
        }

        // Raise the Preview Event, check the Handled value, and raise the regular event.
        var args = new CanExecuteRoutedEventArgs(this, parameter)
        {
            RoutedEvent = CommandManager.PreviewCanExecuteEvent
        };

        CriticalCanExecuteWrapper(parameter, target, trusted, args);

        if (!args.Handled)
        {
            args.RoutedEvent = CommandManager.CanExecuteEvent;
            CriticalCanExecuteWrapper(parameter, target, trusted, args);
        }

        continueRouting = args.ContinueRouting;
        return args.CanExecute;
    }

    private void CriticalCanExecuteWrapper(object parameter, IInputElement target, bool trusted, CanExecuteRoutedEventArgs args)
    {
        if (target is UIElement uie)
        {
            uie.RaiseEvent(args, trusted);
        }
    }

    internal bool ExecuteCore(object parameter, IInputElement target, bool userInitiated)
    {
        target ??= FilterInputElement(Keyboard.FocusedElement);

        return ExecuteImpl(parameter, target, userInitiated);
    }

    private bool ExecuteImpl(object parameter, IInputElement target, bool userInitiated)
    {
        if (target is not UIElement targetUIElement)
        {
            return false;
        }

        // Raise the Preview Event and check for Handled value, and
        // Raise the regular ExecuteEvent.
        var args = new ExecutedRoutedEventArgs(this, parameter)
        {
            RoutedEvent = CommandManager.PreviewExecutedEvent
        };

        targetUIElement.RaiseEvent(args, userInitiated);

        if (!args.Handled)
        {
            args.RoutedEvent = CommandManager.ExecutedEvent;
            targetUIElement.RaiseEvent(args, userInitiated);
        }

        return args.Handled;
    }

    private static bool IsValidInputElement(IInputElement element) => element is UIElement;
}
