
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

namespace System.Windows.Media
{
    /// <summary>
    /// Represents metadata associated with a specific point in a media file.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class TimelineMarker : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineMarker"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimelineMarker() { }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(TimelineMarker),
                new PropertyMetadata((object)null));

        /// <summary>
        /// The text value of the <see cref="TimelineMarker"/>. 
        /// The default value is null.
        /// </summary>
        [OpenSilver.NotImplemented]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValueInternal(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Time"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(
                nameof(Time),
                typeof(TimeSpan),
                typeof(TimelineMarker),
                new PropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// The time at which the <see cref="TimelineMarker"/> is reached. 
        /// The default value is <see cref="TimeSpan.Zero"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValueInternal(TimeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Type"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(
                nameof(Type),
                typeof(string),
                typeof(TimelineMarker),
                new PropertyMetadata((object)null));

        /// <summary>
        /// A string that describes the type of this <see cref="TimelineMarker"/>. 
        /// The default value is null.
        /// </summary>
        [OpenSilver.NotImplemented]
        public string Type
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValueInternal(TextProperty, value); }
        }
    }
}