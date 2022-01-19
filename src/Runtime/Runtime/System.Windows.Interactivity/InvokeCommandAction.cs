

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

#if MIGRATION

namespace System.Windows.Interactivity //Windows.UI.Interactivity
{
    /// <summary>
    /// Executes a specified ICommand when invoked.
    /// 
    /// </summary>
    public sealed partial class InvokeCommandAction : TriggerAction<FrameworkElement> //DependencyObject, IAttachedObject //TriggerAction<FrameworkElement>
    {
        //Example of a currently working code:
        //<Button x:Name="TestButton" Content="a" Width="100" Height="30">
        //    <i:Interaction.Triggers>
        //        <i:EventTrigger EventName="Click" >
        //            < i:InvokeCommandAction Command="{Binding Path=SomeCommand, Mode=OneWay}"/>
        //        </i:EventTrigger>
        //    </i:Interaction.Triggers>
        //</Button>

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction), null);
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction), null);
        private string commandName;

        /// <summary>
        /// Gets or sets the name of the command this action should invoke.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The name of the command this action should invoke.
        /// </value>
        /// 
        /// <remarks>
        /// This property will be superseded by the Command property if both are set.
        /// </remarks>
        public string CommandName
        {
            get
            {
                return this.commandName;
            }
            set
            {
                if (!(this.CommandName != value))
                {
                    return;
                }
                this.commandName = value;
            }
        }

        /// <summary>
        /// Gets or sets the command this action should invoke. This is a dependency property.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The command to execute.
        /// </value>
        /// 
        /// <remarks>
        /// This property will take precedence over the CommandName property if both are set.
        /// </remarks>
        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(InvokeCommandAction.CommandProperty);
            }
            set
            {
                this.SetValue(InvokeCommandAction.CommandProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the command parameter. This is a dependency property.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The command parameter.
        /// </value>
        /// 
        /// <remarks>
        /// This is the value passed to ICommand.CanExecute and ICommand.Execute.
        /// </remarks>
        public object CommandParameter
        {
            get
            {
                return this.GetValue(InvokeCommandAction.CommandParameterProperty);
            }
            set
            {
                this.SetValue(InvokeCommandAction.CommandParameterProperty, value);
            }
        }

        static InvokeCommandAction()
        {
        }

        /// <summary>
        /// Invokes the action.
        /// 
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null)
            {
                return;
            }
            ICommand command = this.ResolveCommand();
            object param = this.CommandParameter ?? parameter;
            if (command != null && command.CanExecute(param))
            {
                command.Execute(param);
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
                foreach (PropertyInfo propertyInfo in this.AssociatedObject.GetType().GetProperties())// .GetTypeInfo().DeclaredProperties)
                {
                    if (typeof(ICommand).GetType().IsAssignableFrom(propertyInfo.PropertyType.GetType()) && string.Equals(propertyInfo.Name, this.CommandName, StringComparison.Ordinal))
                    {
                        command = (ICommand)propertyInfo.GetValue((object)this.AssociatedObject, (object[])null);
                    }
                }
            }
            return command;
        }
    }
}

#endif