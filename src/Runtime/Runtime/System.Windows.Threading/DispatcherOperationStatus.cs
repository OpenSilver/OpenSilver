
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

namespace System.Windows.Threading;

/// <summary>
/// Describes the possible values for the status of a <see cref="DispatcherOperation"/>.
/// </summary>
public enum DispatcherOperationStatus
{
    /// <summary>
    /// The operation is pending and is still in the <see cref="Dispatcher"/> queue.
    /// </summary>
    Pending,

    /// <summary>
    /// The operation has aborted.
    /// </summary>
    Aborted,

    /// <summary>
    /// The operation is completed.
    /// </summary>
    Completed,

    /// <summary>
    /// The operation started executing, but has not completed.
    /// </summary>
    Executing,
}
