
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

using System.ComponentModel;

namespace System.Windows.Input;
/// <summary>
/// Represents a request to an element to move focus to another control.
/// </summary>
internal sealed class TraversalRequest
{
    /// <summary>
    /// Constructor that requests passing FocusNavigationDirection
    /// </summary>
    /// <param name="focusNavigationDirection">Type of focus traversal to perform</param>
    public TraversalRequest(FocusNavigationDirection focusNavigationDirection)
    {
        if (focusNavigationDirection != FocusNavigationDirection.Next &&
            focusNavigationDirection != FocusNavigationDirection.Previous &&
            focusNavigationDirection != FocusNavigationDirection.First &&
            focusNavigationDirection != FocusNavigationDirection.Last)
        {
            throw new InvalidEnumArgumentException(nameof(focusNavigationDirection), (int)focusNavigationDirection, typeof(FocusNavigationDirection));
        }

        FocusNavigationDirection = focusNavigationDirection;
    }

    /// <summary>
    /// true if reached the end of child elements that should have focus
    /// </summary>
    public bool Wrapped { get; set; }

    /// <summary>
    /// Determine how to move the focus
    /// </summary>
    public FocusNavigationDirection FocusNavigationDirection { get; }
}

/// <summary>
/// Determine how to move the focus
/// </summary>
internal enum FocusNavigationDirection
{
    /// <summary>
    /// Move the focus to the next Control in Tab order.
    /// </summary>
    Next,

    /// <summary>
    /// Move the focus to the previous Control in Tab order. Shift+Tab
    /// </summary>
    Previous,

    /// <summary>
    /// Move the focus to the first Control in Tab order inside the subtree.
    /// </summary>
    First,

    /// <summary>
    /// Move the focus to the last Control in Tab order inside the subtree.
    /// </summary>
    Last,

    // If you add a new value you should also add a validation check to TraversalRequest constructor
}
