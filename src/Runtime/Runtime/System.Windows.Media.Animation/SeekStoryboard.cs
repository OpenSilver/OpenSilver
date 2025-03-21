
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

using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows.Media.Animation;

/// <summary>
/// A trigger action that provides functionality for seeking (skipping) to a specified time within the 
/// active period of a <see cref="Storyboard"/>.
/// </summary>
public sealed class SeekStoryboard : ControllableStoryboardAction
{
    private TimeSpan _offset;
    private TimeSeekOrigin _origin;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeekStoryboard"/> class.
    /// </summary>
    public SeekStoryboard()
    {
        _offset = TimeSpan.Zero;
        _origin = TimeSeekOrigin.BeginTime;
    }

    /// <summary>
    /// Gets or sets the amount by which the storyboard should move forward or backward from the seek 
    /// origin <see cref="Origin"/>.
    /// </summary>
    /// <returns>
    /// A positive or negative value that specifies the amount by which the storyboard should move forward 
    /// or backward from the seek origin <see cref="Origin"/>. The default value is 0.
    /// </returns>
    public TimeSpan Offset
    {
        get { return _offset; }
        set
        {
            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(SeekStoryboard)));
            }

            _offset = value;
        }
    }

    /// <summary>
    /// Gets or sets the position from which this seek operation's <see cref="Offset"/> is applied.
    /// </summary>
    /// <returns>
    /// The position from which this seek operation's <see cref="Offset"/> is applied. The default value 
    /// is <see cref="TimeSeekOrigin.BeginTime"/>.
    /// </returns>
    public TimeSeekOrigin Origin
    {
        get { return _origin; }
        set
        {
            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(SeekStoryboard)));
            }

            if (value == TimeSeekOrigin.BeginTime || value == TimeSeekOrigin.Duration)
            {
                _origin = value;
            }
            else
            {
                throw new ArgumentException(Strings.Storyboard_UnrecognizedTimeSeekOrigin);
            }
        }
    }

    internal override void Invoke(IFrameworkElement containingFE, Storyboard storyboard)
    {
        Debug.Assert(containingFE is not null, "Caller of internal function failed to verify that we have a FE.");

        if (containingFE is FrameworkElement fe)
        {
            storyboard.Seek(fe, Offset, Origin);
        }
    }
}
