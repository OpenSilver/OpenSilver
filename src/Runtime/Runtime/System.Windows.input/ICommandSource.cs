
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
/// Defines an object that knows how to invoke a command.
/// </summary>
public interface ICommandSource
{
    /// <summary>
    /// Gets the command that will be executed when the command source is invoked.
    /// </summary>
    /// <returns>
    /// The command that will be executed when the command source is invoked.
    /// </returns>
    ICommand Command { get; }

    /// <summary>
    /// Represents a user defined data value that can be passed to the command when it is executed.
    /// </summary>
    /// <returns>
    /// The command specific data.
    /// </returns>
    object CommandParameter { get; }

    /// <summary>
    /// The object that the command is being executed on.
    /// </summary>
    /// <returns>
    /// The object that the command is being executed on.
    /// </returns>
    IInputElement CommandTarget { get; }
}
