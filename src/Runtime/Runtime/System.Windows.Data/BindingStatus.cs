
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

namespace System.Windows.Data;

/// <summary>
/// Describes the status of a binding.
/// </summary>
public enum BindingStatus
{
    /// <summary>
    /// The binding has not yet been attached to its target property.
    /// </summary>
    Unattached = 0,

    /// <summary>
    /// The binding has not been activated.
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// The binding has been successfully activated. This means that the binding has
    /// been attached to its binding target (target) property and has located the binding
    /// source (source), resolved the Path and/or XPath, and begun transferring values.
    /// </summary>
    Active = 2,

    /// <summary>
    /// The binding has been detached from its target property.
    /// </summary>
    Detached = 3,

    /// <summary>
    /// The binding is waiting for an asynchronous operation to complete.
    /// </summary>
    AsyncRequestPending = 4,

    /// <summary>
    /// The binding was unable to resolve the source path.
    /// </summary>
    PathError = 5,

    /// <summary>
    /// The binding could not successfully return a source value to update the target value.
    /// For more information, see <see cref="BindingBase.FallbackValue"/>.
    /// </summary>
    UpdateTargetError = 6,

    /// <summary>
    /// The binding was unable to send the value to the source property.
    /// </summary>
    UpdateSourceError = 7,
}
