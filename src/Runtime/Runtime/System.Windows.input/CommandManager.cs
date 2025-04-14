
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

using System.Collections.Generic;
using System.Windows.Threading;

namespace System.Windows.Input;

/// <summary>
/// Provides command related utility methods that register <see cref="CommandBinding"/> and <see cref="InputBinding"/> 
/// objects for class owners and commands, add and remove command event handlers, and provides services for querying 
/// the status of a command.
/// </summary>
public sealed class CommandManager
{
    [ThreadStatic]
    private static CommandManager _commandManager; // The CommandManager associated with the current thread
    private static readonly Dictionary<Type, CommandBindingCollection> _classCommandBindings = [];
    private static readonly Dictionary<Type, InputBindingCollection> _classInputBindings = [];

    private DispatcherOperation _requerySuggestedOperation;
    private event EventHandler PrivateRequerySuggested;

    private CommandManager() { }

    /// <summary>
    /// Occurs when the <see cref="CommandManager"/> detects conditions that might change the ability of a command to execute.
    /// </summary>
    public static event EventHandler RequerySuggested
    {
        add => RequerySuggestedEventManager.AddHandler(null, value);
        remove => RequerySuggestedEventManager.RemoveHandler(null, value);
    }

    /// <summary>
    /// Identifies the CommandManager.PreviewExecuted attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewExecutedEvent =
        EventManager.RegisterRoutedEvent(
            "PreviewExecuted",
            RoutingStrategy.Tunnel,
            typeof(ExecutedRoutedEventHandler),
            typeof(CommandManager));

    /// <summary>
    /// Attaches the specified <see cref="ExecutedRoutedEventHandler"/> to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to attach handler to.
    /// </param>
    /// <param name="handler">
    /// The can execute handler.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> or <paramref name="handler"/> is null.
    /// </exception>
    public static void AddPreviewExecutedHandler(UIElement element, ExecutedRoutedEventHandler handler)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        element.AddHandler(PreviewExecutedEvent, handler);
    }

    /// <summary>
    /// Detaches the specified <see cref="ExecutedRoutedEventHandler"/> from the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to remove handler from.
    /// </param>
    /// <param name="handler">
    /// The executed handler.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> or <paramref name="handler"/> is null.
    /// </exception>
    public static void RemovePreviewExecutedHandler(UIElement element, ExecutedRoutedEventHandler handler)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        element.RemoveHandler(PreviewExecutedEvent, handler);
    }

    /// <summary>
    /// Identifies the CommandManager.Executed attached event.
    /// </summary>
    public static readonly RoutedEvent ExecutedEvent =
        EventManager.RegisterRoutedEvent(
            "Executed",
            RoutingStrategy.Bubble,
            typeof(ExecutedRoutedEventHandler),
            typeof(CommandManager));

    /// <summary>
    /// Attaches the specified <see cref="ExecutedRoutedEventHandler"/> to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to attach handler to.
    /// </param>
    /// <param name="handler">
    /// The executed handler.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> or <paramref name="handler"/> is null.
    /// </exception>
    public static void AddExecutedHandler(UIElement element, ExecutedRoutedEventHandler handler)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        element.AddHandler(ExecutedEvent, handler);
    }

    /// <summary>
    /// Detaches the specified <see cref="ExecutedRoutedEventHandler"/> from the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to remove handler from.
    /// </param>
    /// <param name="handler">
    /// The executed handler.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> or <paramref name="handler"/> is null.
    /// </exception>
    public static void RemoveExecutedHandler(UIElement element, ExecutedRoutedEventHandler handler)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        element.RemoveHandler(ExecutedEvent, handler);
    }

    /// <summary>
    /// Identifies the CommandManager.PreviewCanExecute attached event.
    /// </summary>
    public static readonly RoutedEvent PreviewCanExecuteEvent =
        EventManager.RegisterRoutedEvent(
            "PreviewCanExecute",
            RoutingStrategy.Tunnel,
            typeof(CanExecuteRoutedEventHandler),
            typeof(CommandManager));

    /// <summary>
    /// Attaches the specified <see cref="CanExecuteRoutedEventHandler"/> to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to attach handler to.
    /// </param>
    /// <param name="handler">
    /// The can execute handler.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> or <paramref name="handler"/> is null.
    /// </exception>
    public static void AddPreviewCanExecuteHandler(UIElement element, CanExecuteRoutedEventHandler handler)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        element.AddHandler(PreviewCanExecuteEvent, handler);
    }

    /// <summary>
    /// Detaches the specified <see cref="CanExecuteRoutedEventHandler"/> from the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to remove handler from.
    /// </param>
    /// <param name="handler">
    /// The can execute handler.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> or <paramref name="handler"/> is null.
    /// </exception>
    public static void RemovePreviewCanExecuteHandler(UIElement element, CanExecuteRoutedEventHandler handler)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        element.RemoveHandler(PreviewCanExecuteEvent, handler);
    }

    /// <summary>
    /// Identifies the CommandManager.CanExecute attached event.
    /// </summary>
    public static readonly RoutedEvent CanExecuteEvent =
        EventManager.RegisterRoutedEvent(
            "CanExecute",
            RoutingStrategy.Bubble,
            typeof(CanExecuteRoutedEventHandler),
            typeof(CommandManager));

    /// <summary>
    /// Attaches the specified <see cref="CanExecuteRoutedEventHandler"/> to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to attach handler to.
    /// </param>
    /// <param name="handler">
    /// The can execute handler.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> or <paramref name="handler"/> is null.
    /// </exception>
    public static void AddCanExecuteHandler(UIElement element, CanExecuteRoutedEventHandler handler)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        element.AddHandler(CanExecuteEvent, handler);
    }

    /// <summary>
    /// Detaches the specified <see cref="CanExecuteRoutedEventHandler"/> from the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to remove handler from.
    /// </param>
    /// <param name="handler">
    /// The can execute handler.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> or <paramref name="handler"/> is null.
    /// </exception>
    public static void RemoveCanExecuteHandler(UIElement element, CanExecuteRoutedEventHandler handler)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        element.RemoveHandler(CanExecuteEvent, handler);
    }

    /// <summary>
    /// Registers the specified <see cref="InputBinding"/> with the specified type.
    /// </summary>
    /// <param name="type">
    /// The type to register inputBinding with.
    /// </param>
    /// <param name="inputBinding">
    /// The input binding to register.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="type"/> or <paramref name="inputBinding"/> is null.
    /// </exception>
    public static void RegisterClassInputBinding(Type type, InputBinding inputBinding)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (inputBinding is null)
        {
            throw new ArgumentNullException(nameof(inputBinding));
        }

        lock (_classInputBindings)
        {
            if (!_classInputBindings.TryGetValue(type, out InputBindingCollection inputBindings))
            {
                inputBindings = [];
                _classInputBindings[type] = inputBindings;
            }

            inputBindings.Add(inputBinding);

            if (!inputBinding.IsSealed)
            {
                inputBinding.Seal();
            }
        }
    }

    /// <summary>
    /// Registers a <see cref="CommandBinding"/> with the specified type.
    /// </summary>
    /// <param name="type">
    /// The class with which to register commandBinding.
    /// </param>
    /// <param name="commandBinding">
    /// The command binding to register.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="type"/> or <paramref name="commandBinding"/> is null.
    /// </exception>
    public static void RegisterClassCommandBinding(Type type, CommandBinding commandBinding)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (commandBinding is null)
        {
            throw new ArgumentNullException(nameof(commandBinding));
        }

        lock (_classCommandBindings)
        {
            if (!_classCommandBindings.TryGetValue(type, out CommandBindingCollection bindings))
            {
                bindings = [];
                _classCommandBindings[type] = bindings;
            }

            bindings.Add(commandBinding);
        }
    }

    /// <summary>
    /// Forces the <see cref="CommandManager"/> to raise the <see cref="RequerySuggested"/> event.
    /// </summary>
    public static void InvalidateRequerySuggested() => Current.RaiseRequerySuggested();

    /// <summary>
    ///     Return the CommandManager associated with the current thread.
    /// </summary>
    private static CommandManager Current => _commandManager ??= new();

    /// <summary>
    ///     Adds an idle priority dispatcher operation to raise RequerySuggested.
    /// </summary>
    private void RaiseRequerySuggested()
    {
        if (_requerySuggestedOperation is null)
        {
            if (Dispatcher.CurrentDispatcher is Dispatcher dispatcher)
            {
                _requerySuggestedOperation = dispatcher.InvokeAsync(RaiseRequerySuggestedCallback, DispatcherPriority.Background);
            }
        }
    }

    private void RaiseRequerySuggestedCallback()
    {
        // Call the RequerySuggested handlers
        _requerySuggestedOperation = null;

        PrivateRequerySuggested?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>
    ///     Scans input and command bindings for matching gestures and executes the appropriate command
    /// </summary>
    /// <remarks>
    ///     Scans for command to execute in the following order:
    ///     - input bindings associated with the targetElement instance
    ///     - input bindings associated with the targetElement class
    ///     - command bindings associated with the targetElement instance
    ///     - command bindings associated with the targetElement class
    /// </remarks>
    /// <param name="targetElement">UIElement/ContentElement to be scanned for input and command bindings</param>
    /// <param name="inputEventArgs">InputEventArgs to be matched against for gestures</param>
    internal static void TranslateInput(IInputElement targetElement, InputEventArgs inputEventArgs)
    {
        if (targetElement is null || inputEventArgs is null)
        {
            return;
        }

        ICommand command = null;
        IInputElement target = null;
        object parameter = null;

        UIElement targetElementAsUIElement = targetElement as UIElement;

        // Step 1: Check local input bindings
        InputBindingCollection localInputBindings = targetElementAsUIElement?.InputBindingsInternal;

        if (localInputBindings is not null)
        {
            if (localInputBindings.FindMatch(targetElement, inputEventArgs) is InputBinding inputBinding)
            {
                command = inputBinding.Command;
                target = inputBinding.CommandTarget;
                parameter = inputBinding.CommandParameter;
            }
        }

        // Step 2: If no command, check class input bindings
        if (command is null)
        {
            lock (_classInputBindings)
            {
                Type classType = targetElement.GetType();
                while (classType is not null)
                {
                    if (_classInputBindings.TryGetValue(classType, out InputBindingCollection classInputBindings))
                    {
                        if (classInputBindings.FindMatch(targetElement, inputEventArgs) is InputBinding inputBinding)
                        {
                            command = inputBinding.Command;
                            target = inputBinding.CommandTarget;
                            parameter = inputBinding.CommandParameter;
                            break;
                        }
                    }
                    classType = classType.BaseType;
                }
            }
        }

        // Step 3: If no command, check local command bindings
        if (command is null)
        {
            // Check for the instance level ones Next
            CommandBindingCollection localCommandBindings = targetElementAsUIElement?.CommandBindingsInternal;

            if (localCommandBindings is not null)
            {
                command = localCommandBindings.FindMatch(targetElement, inputEventArgs);
            }
        }

        // Step 4: If no command, look at class command bindings
        if (command is null)
        {
            lock (_classCommandBindings)
            {
                Type classType = targetElement.GetType();
                while (classType is not null)
                {
                    if (_classCommandBindings.TryGetValue(classType, out CommandBindingCollection classCommandBindings))
                    {
                        command = classCommandBindings.FindMatch(targetElement, inputEventArgs);
                        if (command is not null)
                        {
                            break;
                        }
                    }
                    classType = classType.BaseType;
                }
            }
        }

        // Step 5: If found a command, then execute it (unless it is
        // the special "NotACommand" command, which we simply ignore without
        // setting Handled=true, so that the input bubbles up to the parent)
        if (command is not null && command != ApplicationCommands.NotACommand)
        {
            // We currently do not support declaring the element with focus as the target
            // element by setting target == null.  Instead, we interpret a null target to indicate
            // the element that we are routing the event through, e.g. the targetElement parameter.
            target ??= targetElement;

            bool continueRouting = false;

            if (command is RoutedCommand routedCommand)
            {
                if (routedCommand.CriticalCanExecute(parameter, target, trusted: inputEventArgs.UserInitiated, out continueRouting))
                {
                    // If the command can be executed, we never continue to route the
                    // input event.
                    continueRouting = false;

                    ExecuteCommand(routedCommand, parameter, target, inputEventArgs);
                }
            }
            else
            {
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }

            // If we mapped an input event to a command, we should always
            // handle the input event - regardless of whether the command
            // was executed or not.  Unless the CanExecute handler told us
            // to continue the route.
            inputEventArgs.Handled = !continueRouting;
        }
    }

    private static bool ExecuteCommand(RoutedCommand routedCommand, object parameter, IInputElement target, InputEventArgs inputEventArgs)
    {
        return routedCommand.ExecuteCore(parameter, target, inputEventArgs.UserInitiated);
    }

    /// <summary>
    ///     Forwards CanExecute events to CommandBindings.
    /// </summary>
    internal static void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (sender is not null && e is not null && e.Command is not null)
        {
            FindCommandBinding(sender, e, e.Command, false);
        }
    }

    private static bool CanExecuteCommandBinding(object sender, CanExecuteRoutedEventArgs e, CommandBinding commandBinding)
    {
        commandBinding.OnCanExecute(sender, e);
        return e.CanExecute || e.Handled;
    }

    /// <summary>
    ///     Forwards Executed events to CommandBindings.
    /// </summary>
    internal static void OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is not null && e is not null && e.Command is not null)
        {
            FindCommandBinding(sender, e, e.Command, true);
        }
    }

    private static bool ExecuteCommandBinding(object sender, ExecutedRoutedEventArgs e, CommandBinding commandBinding)
    {
        commandBinding.OnExecuted(sender, e);
        return e.Handled;
    }

    private static void FindCommandBinding(object sender, RoutedEventArgs e, ICommand command, bool execute)
    {
        // Check local command bindings
        CommandBindingCollection commandBindings = sender switch
        {
            UIElement uiElement => uiElement.CommandBindingsInternal,
            _ => default,
        };

        if (commandBindings is not null)
        {
            FindCommandBinding(commandBindings, sender, e, command, execute);
        }

        Type senderType = sender.GetType();

        // If no command binding is found, check class command bindings
        // First find the relevant command bindings, under the lock.
        // Most of the time there are no such bindings;  most of the rest of
        // the time there is only one.   Lazy-allocate with this in mind.
        (Type OwnerType, CommandBinding CommandBinding)? tuple = default;       // zero or one binding
        List<(Type OwnerType, CommandBinding CommandBinding)> list = default;   // more than one

        lock (_classCommandBindings)
        {
            // Check from the current type to all the base types
            Type classType = senderType;
            while (classType is not null)
            {
                if (_classCommandBindings.TryGetValue(classType, out CommandBindingCollection classCommandBindings))
                {
                    int index = 0;
                    while (true)
                    {
                        if (classCommandBindings.FindMatch(command, ref index) is not CommandBinding commandBinding)
                        {
                            break;
                        }

                        if (tuple is null)
                        {
                            tuple = (classType, commandBinding);
                        }
                        else
                        {
                            list ??= new(8)
                            {
                                // We know that tuple cannot be null here
                                tuple.Value
                            };
                            list.Add((classType, commandBinding));
                        }
                    }
                }
                classType = classType.BaseType;
            }
        }

        // execute the bindings.  This can call into user code, so it must
        // be done outside the lock to avoid deadlock.
        if (list is not null)
        {
            // more than one binding
            ExecutedRoutedEventArgs exArgs = execute ? (ExecutedRoutedEventArgs)e : default;
            CanExecuteRoutedEventArgs canExArgs = execute ? default : (CanExecuteRoutedEventArgs)e;
            for (int i = 0; i < list.Count; ++i)
            {
                // invoke the binding
                if ((!execute || !ExecuteCommandBinding(sender, exArgs, list[i].CommandBinding)) &&
                    (execute || !CanExecuteCommandBinding(sender, canExArgs, list[i].CommandBinding)))
                {
                    continue;
                }

                // if it succeeds, advance past the remaining bindings for this type
                Type classType = list[i].OwnerType;
                while (++i < list.Count && list[i].OwnerType == classType)
                {
                    // no body needed
                }
                --i;    // back up, so that the outer for-loop advances to the right place
            }
        }
        else if (tuple is (_, CommandBinding commandBinding))
        {
            // only one binding
            if (execute)
            {
                ExecuteCommandBinding(sender, (ExecutedRoutedEventArgs)e, commandBinding);
            }
            else
            {
                CanExecuteCommandBinding(sender, (CanExecuteRoutedEventArgs)e, commandBinding);
            }
        }
    }

    private static void FindCommandBinding(CommandBindingCollection commandBindings, object sender, RoutedEventArgs e, ICommand command, bool execute)
    {
        int index = 0;
        while (true)
        {
            CommandBinding commandBinding = commandBindings.FindMatch(command, ref index);
            if (HandleCommandBinding(sender, e, commandBinding, execute))
            {
                break;
            }
        }
    }

    private static bool HandleCommandBinding(object sender, RoutedEventArgs e, CommandBinding commandBinding, bool execute)
    {
        return commandBinding is null ||
               execute && ExecuteCommandBinding(sender, (ExecutedRoutedEventArgs)e, commandBinding) ||
               !execute && CanExecuteCommandBinding(sender, (CanExecuteRoutedEventArgs)e, commandBinding);
    }

    private sealed class RequerySuggestedEventManager : WeakEventManager
    {
        private RequerySuggestedEventManager() { }

        /// <summary>
        /// Add a handler for the given source's event.
        /// </summary>
        public static void AddHandler(CommandManager source, EventHandler handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            CurrentManager.ProtectedAddHandler(source, handler);
        }

        /// <summary>
        /// Remove a handler for the given source's event.
        /// </summary>
        public static void RemoveHandler(CommandManager source, EventHandler handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            CurrentManager.ProtectedRemoveHandler(source, handler);
        }

        /// <summary>
        /// Return a new list to hold listeners to the event.
        /// </summary>
        protected override ListenerList NewListenerList() => new();

        /// <summary>
        /// Listen to the given source for the event.
        /// </summary>
        protected override void StartListening(object source) => Current.PrivateRequerySuggested += new EventHandler(OnRequerySuggested);

        /// <summary>
        /// Stop listening to the given source for the event.
        /// </summary>
        protected override void StopListening(object source) => Current.PrivateRequerySuggested -= new EventHandler(OnRequerySuggested);

        // get the event manager for the current thread
        private static RequerySuggestedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(RequerySuggestedEventManager);

                // at first use, create and register a new manager
                if (GetCurrentManager(managerType) is not RequerySuggestedEventManager manager)
                {
                    manager = new RequerySuggestedEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        // event handler for CurrentChanged event
        private void OnRequerySuggested(object sender, EventArgs args) => DeliverEvent(sender, args);
    }
}
