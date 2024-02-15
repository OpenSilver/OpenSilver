
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

namespace System.Windows;

/// <summary>
/// Represents the base class for value setters.
/// </summary>
public abstract class SetterBase : DependencyObject
{
    internal SetterBase() { }

    /// <summary>
    /// Gets a value that indicates whether this object is in an immutable state.
    /// </summary>
    /// <returns>
    /// true if this object is in an immutable state; otherwise, false.
    /// </returns>
    public bool IsSealed => _sealed;

    internal virtual void Seal() => _sealed = true;

    /// <summary>
    /// Subclasses need to call this method before any changes to their state.
    /// </summary>
    private protected void CheckSealed()
    {
        if (_sealed)
        {
            throw new InvalidOperationException("Cannot modify a 'SetterBase' after it is sealed.");
        }
    }

    // Derived
    private bool _sealed;
}
