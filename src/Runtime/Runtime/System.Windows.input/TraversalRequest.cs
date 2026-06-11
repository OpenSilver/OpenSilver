
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
/// Represents a request to move focus to another control.
/// </summary>
public sealed class TraversalRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TraversalRequest"/> class.
    /// </summary>
    /// <param name="focusNavigationDirection">
    /// The intended direction of the focus traversal, as a value of the enumeration.
    /// </param>
    public TraversalRequest(FocusNavigationDirection focusNavigationDirection)
    {
        if (!Enum.IsDefined(typeof(FocusNavigationDirection), focusNavigationDirection))
        {
            throw new InvalidEnumArgumentException(nameof(focusNavigationDirection), (int)focusNavigationDirection, typeof(FocusNavigationDirection));
        }

        FocusNavigationDirection = focusNavigationDirection;
    }

    /// <summary>
    /// Gets or sets a value that indicates whether focus traversal has reached the end of 
    /// child elements that can have focus.
    /// </summary>
    /// <returns>
    /// true if this traversal has reached the end of child elements that can have focus;
    /// otherwise, false. The default is false.
    /// </returns>
    public bool Wrapped { get; set; }

    /// <summary>
    /// Gets the traversal direction.
    /// </summary>
    /// <returns>
    /// One of the traversal direction enumeration values.
    /// </returns>
    public FocusNavigationDirection FocusNavigationDirection { get; }
}

/// <summary>
/// Specifies the direction within a user interface (UI) in which a desired focus change 
/// request is attempted. The direction is either based on tab order or by relative direction 
/// in layout.
/// </summary>
public enum FocusNavigationDirection
{
    /// <summary>
    /// Move focus to the next focusable element in tab order.
    /// </summary>
    Next,

    /// <summary>
    /// Move focus to the previous focusable element in tab order.
    /// </summary>
    Previous,

    /// <summary>
    /// Move focus to the first focusable element in tab order.
    /// </summary>
    First,

    /// <summary>
    /// Move focus to the last focusable element in tab order.
    /// </summary>
    Last,

    /// <summary>
    /// Move the focus to another control to the left of the currently focused element.
    /// </summary>
    Left,

    /// <summary>
    /// Move the focus to another control to the right of the currently focused element.
    /// </summary>
    Right,

    /// <summary>
    /// Move the focus to another control above the currently focused element.
    /// </summary>
    Up,

    /// <summary>
    /// Move the focus to another control below the currently focused element.
    /// </summary>
    Down,
}
