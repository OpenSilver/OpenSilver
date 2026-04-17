
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

namespace System.Windows.Media.Animation;

/// <summary>
/// Defines a segment of time that may contain child <see cref="Timeline"/> objects. These 
/// child timelines become active according to their respective <see cref="Timeline.BeginTime"/>
/// properties. Also, child timelines are able to overlap (run in parallel) with each other.
/// </summary>
public class ParallelTimeline : TimelineGroup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParallelTimeline"/> class.
    /// </summary>
    public ParallelTimeline()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParallelTimeline"/> class with the specified 
    /// <see cref="Timeline.BeginTime"/>.
    /// </summary>
    /// <param name="beginTime">
    /// The <see cref="Timeline.BeginTime"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    public ParallelTimeline(TimeSpan? beginTime)
        : base(beginTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParallelTimeline"/> class with the specified 
    /// <see cref="Timeline.BeginTime"/> and <see cref="Timeline.Duration"/>.
    /// </summary>
    /// <param name="beginTime">
    /// The <see cref="Timeline.BeginTime"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    /// <param name="duration">
    /// The <see cref="Timeline.Duration"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    public ParallelTimeline(TimeSpan? beginTime, Duration duration)
        : base(beginTime, duration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParallelTimeline"/> class with the specified 
    /// <see cref="Timeline.BeginTime"/>, <see cref="Timeline.Duration"/>, and <see cref="Timeline.RepeatBehavior"/>.
    /// </summary>
    /// <param name="beginTime">
    /// The <see cref="Timeline.BeginTime"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    /// <param name="duration">
    /// The <see cref="Timeline.Duration"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    /// <param name="repeatBehavior">
    /// The <see cref="Timeline.RepeatBehavior"/> for this <see cref="TimelineGroup"/>.
    /// </param>
    public ParallelTimeline(TimeSpan? beginTime, Duration duration, RepeatBehavior repeatBehavior)
        : base(beginTime, duration, repeatBehavior)
    {
    }
}
