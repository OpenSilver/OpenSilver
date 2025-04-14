
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

using System.Windows;
using System.Windows.Input;

namespace OpenSilver.Internal.Commands;

internal static class CommandHelpers
{
    internal static bool CanExecuteCommandSource(ICommandSource commandSource)
    {
        if (commandSource.Command is ICommand command)
        {
            object parameter = commandSource.CommandParameter;
            IInputElement target = commandSource.CommandTarget;

            if (command is RoutedCommand routed)
            {
                target ??= (commandSource as IInputElement);
                return routed.CanExecute(parameter, target);
            }
            else
            {
                return command.CanExecute(parameter);
            }
        }

        return false;
    }

    internal static void ExecuteCommandSource(ICommandSource commandSource) => CriticalExecuteCommandSource(commandSource, false);

    internal static void CriticalExecuteCommandSource(ICommandSource commandSource, bool userInitiated)
    {
        if (commandSource.Command is ICommand command)
        {
            object parameter = commandSource.CommandParameter;
            IInputElement target = commandSource.CommandTarget;

            if (command is RoutedCommand routed)
            {
                target ??= (commandSource as IInputElement);
                if (routed.CanExecute(parameter, target))
                {
                    routed.ExecuteCore(parameter, target, userInitiated);
                }
            }
            else if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}
