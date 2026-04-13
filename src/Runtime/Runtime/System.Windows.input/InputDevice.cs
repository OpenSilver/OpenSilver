
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

using System.Windows.Threading;

namespace System.Windows.Input;

/// <summary>
/// Abstract class that describes an input device.
/// </summary>
public abstract class InputDevice : DispatcherObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputDevice"/> class.
    /// </summary>
    protected InputDevice() { }

    /// <summary>
    /// When overridden in a derived class, gets the element that receives input from this device.
    /// </summary>
    /// <returns>
    /// The element that receives input.
    /// </returns>
    public abstract IInputElement Target { get; }
}
