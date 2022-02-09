// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace System.Windows.Interactivity
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Globalization;
    using System.Windows.Input;
    using OpenSilver.Internal.Interactivity;

#if MIGRATION
    using System.Windows;
#else
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
    /// Executes a specified ICommand when invoked.
    /// </summary>
    public sealed class InvokeCommandAction : TriggerAction<DependencyObject>
    {
        private string commandName;

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction), null);
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction), null);

        /// <summary>
        /// Gets or sets the name of the command this action should invoke.
        /// </summary>
        /// <value>The name of the command this action should invoke.</value>
        /// <remarks>This property will be superseded by the Command property if both are set.</remarks>
        public string CommandName
        {
            get
            {
#if __WPF__
                this.ReadPreamble();
#endif
                return this.commandName;
            }
            set
            {
                if (this.CommandName != value)
                {
#if __WPF__
                    this.WritePreamble();
#endif
                    this.commandName = value;
#if __WPF__
                    this.WritePostscript();
#endif
                }
            }
        }

        /// <summary>
        /// Gets or sets the command this action should invoke. This is a dependency property.
        /// </summary>
        /// <value>The command to execute.</value>
        /// <remarks>This property will take precedence over the CommandName property if both are set.</remarks>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command parameter. This is a dependency property.
        /// </summary>
        /// <value>The command parameter.</value>
        /// <remarks>This is the value passed to ICommand.CanExecute and ICommand.Execute.</remarks>
        public object CommandParameter
        {
            get { return this.GetValue(InvokeCommandAction.CommandParameterProperty); }
            set { this.SetValue(InvokeCommandAction.CommandParameterProperty, value); }
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject != null)
            {
                ICommand command = this.ResolveCommand();

                if (command != null)
                {
                    if (command.CanExecute(this.CommandParameter))
                    {
                        command.Execute(this.CommandParameter);
                    }
                }
                else
                {
                    Debug.WriteLine(ExceptionStringTable.CommandDoesNotExistOnBehaviorWarningMessage, this.CommandName, this.AssociatedObject.GetType().Name);
                }
            }
        }

        private ICommand ResolveCommand()
        {
            ICommand command = null;

            if (this.Command != null)
            {
                command = this.Command;
            }
            else if (this.AssociatedObject != null)
            {
                // todo jekelly 06/09/08: we could potentially cache some or all of this information if needed, updating when AssociatedObject changes
                Type associatedObjectType = this.AssociatedObject.GetType();
                PropertyInfo[] typeProperties = associatedObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo propertyInfo in typeProperties)
                {
                    if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (string.Equals(propertyInfo.Name, this.CommandName, StringComparison.Ordinal))
                        {
                            command = (ICommand)propertyInfo.GetValue(this.AssociatedObject, null);
                        }
                    }
                }
            }

            return command;
        }
    }
}
