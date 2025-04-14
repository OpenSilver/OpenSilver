
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
/// Abstract class that describes input device gestures.
/// </summary>
public abstract class InputGesture
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputGesture"/> class.
    /// </summary>
    protected InputGesture() { }

    /// <summary>
    /// When overridden in a derived class, determines whether the specified <see cref="InputGesture"/>
    /// matches the input associated with the specified <see cref="InputEventArgs"/> object.
    /// </summary>
    /// <param name="targetElement">
    /// The target of the command.
    /// </param>
    /// <param name="inputEventArgs">
    /// The input event data to compare this gesture to.
    /// </param>
    /// <returns>
    /// true if the gesture matches the input; otherwise, false.
    /// </returns>
    public abstract bool Matches(object targetElement, InputEventArgs inputEventArgs);
}
