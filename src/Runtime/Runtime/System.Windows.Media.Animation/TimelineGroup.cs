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

using System.Windows.Markup;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media.Animation;

/// <summary>
/// Provides a base class for timelines that group child timelines.
/// </summary>
[ContentProperty(nameof(Children))]
public class TimelineGroup : Timeline
{
    private TimelineCollection _children;

    /// <summary>
    /// Gets the collection of child <see cref="Timeline"/> objects.
    /// </summary>
    public TimelineCollection Children => _children ??= new TimelineCollection(this);

    internal override TimelineClock CreateClock() => new TimelineGroupClock(this);
}
