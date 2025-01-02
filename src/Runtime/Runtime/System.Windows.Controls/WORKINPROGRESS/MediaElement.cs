
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

using System.IO;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents an object that contains audio, video, or both.
    /// </summary>
    public partial class MediaElement
    {
        /// <summary>
        ///  The collection of timeline markers (represented as <see cref="TimelineMarker"/> objects) associated
        ///  with the currently loaded media file.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimelineMarkerCollection Markers { get; } = new TimelineMarkerCollection();

        /// <summary>
        /// Identifies the <see cref="BufferingTime"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty BufferingTimeProperty =
            DependencyProperty.Register(
                nameof(BufferingTime),
                typeof(TimeSpan),
                typeof(MediaElement),
                new PropertyMetadata(TimeSpan.FromSeconds(5)));

        /// <summary>
        /// The amount of time to buffer. The default value is a <see cref="TimeSpan"/> with value of 5 seconds (0:0:05).
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimeSpan BufferingTime
        {
            get { return (TimeSpan)GetValue(BufferingTimeProperty); }
            set { SetValueInternal(BufferingTimeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                nameof(Position),
                typeof(TimeSpan),
                typeof(MediaElement),
                new PropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// Gets or sets the current position of progress through the media's playback time.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
            set { SetValueInternal(PositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="NaturalDuration"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty NaturalDurationProperty =
            DependencyProperty.Register(
                nameof(NaturalDuration),
                typeof(Duration),
                typeof(MediaElement),
                new PropertyMetadata(new Duration(TimeSpan.Zero)));

        /// <summary>
        /// Gets the duration of the media file currently opened.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Duration NaturalDuration
        {
            get { return (Duration)GetValue(NaturalDurationProperty); }
        }

        /// <summary>
        /// Identifies the <see cref="CurrentState"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CurrentStateProperty =
            DependencyProperty.Register(
                nameof(CurrentState),
                typeof(MediaElementState),
                typeof(MediaElement),
                new PropertyMetadata(MediaElementState.Closed));

        /// <summary>
        /// Gets the status of the <see cref="MediaElement"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public MediaElementState CurrentState
        {
            get { return (MediaElementState)GetValue(CurrentStateProperty); }
        }

        /// <summary>
        /// Identifies the <see cref="DownloadProgress"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty DownloadProgressProperty =
            DependencyProperty.Register(
                nameof(DownloadProgress),
                typeof(double),
                typeof(MediaElement),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets a percentage value indicating the amount of download completed for content located on a remote server.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double DownloadProgress
        {
            get { return (double)GetValue(DownloadProgressProperty); }
        }

        /// <summary>
        /// Identifies the <see cref="DownloadProgressOffset"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty DownloadProgressOffsetProperty =
            DependencyProperty.Register(
                nameof(DownloadProgressOffset),
                typeof(double),
                typeof(MediaElement),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets the offset of the download progress.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double DownloadProgressOffset
        {
            get { return (double)GetValue(DownloadProgressOffsetProperty); }
        }

        /// <summary>
        /// Gets or sets a <see cref="Media.Stretch"/> value that describes how a <see cref="MediaElement"/> fills the destination rectangle.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValueInternal(StretchProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Stretch"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                nameof(Stretch),
                typeof(Stretch),
                typeof(MediaElement),
                new PropertyMetadata(Stretch.Uniform));

        /// <summary>
        /// Sets the <see cref="Source"/> property using the supplied stream.
        /// </summary>
        /// <param name="stream">
        /// A stream that contains a natively supported media source.
        /// </param>
        [OpenSilver.NotImplemented]
        public void SetSource(Stream stream) { }

        /// <summary>
        /// Occurs when the value of the <see cref="CurrentState"/> property changes.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event RoutedEventHandler CurrentStateChanged;

        /// <summary>
        /// Occurs when the <see cref="DownloadProgress"/> property has changed.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event RoutedEventHandler DownloadProgressChanged;
    }
}
