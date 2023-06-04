
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

#if MIGRATION
namespace System.Windows.Threading;
#else
namespace Windows.UI.Core;
#endif

/// <summary>
///     An enunmeration describing the status of a DispatcherOperation.
/// </summary>
///
public enum DispatcherOperationStatus
{
    /// <summary>
    ///     The operation is still pending.
    /// </summary>
    Pending,

    /// <summary>
    ///     The operation has been aborted.
    /// </summary>
    Aborted,

    /// <summary>
    ///     The operation has been completed.
    /// </summary>
    Completed,

    /// <summary>
    ///     The operation has started executing, but has not completed yet.
    /// </summary>
    Executing
}
