
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5;
using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents an object that contains audio, video, or both.
    /// </summary>
    public sealed class MediaElement : FrameworkElement
    {
        const string HTML_SHOWCONTROLS_PROPERTY_NAME = "controls";
        const string HTML_AUTOPLAY_PROPERTY_NAME = "autoplay";
        const string HTML_ISLOOPING_PROPERTY_NAME = "loop";
        const string HTML_ISMUTED_PROPERTY_NAME = "muted";

        static readonly List<string> SupportedVideoTypes = new List<string>() { "mp4", "ogv", "webm", "3gp" }; // IMPORTANT: if you change this list, remember to also change the error messages in this class.
        static readonly List<string> SupportedAudioTypes = new List<string>() { "mp3", "ogg" };  // IMPORTANT: if you change this list, remember to also change the error messages in this class. //todo: not sure if ogg is actually only for audio or not. If not, find a way to know which one it currently is.
        dynamic _mediaElement = null;
        dynamic _mediaElement_ForAudioOnly_ForSimulatorOnly = null;

        string _nameOfAssemblyThatSetTheSourceUri; // Useful to convert relative URI to absolute URI.

        /// <summary>
        /// Gets or sets a value that indicates whether media will begin playback automatically
        /// when the Source property is set.
        /// </summary>
        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValue(AutoPlayProperty, value); }
        }
        /// <summary>
        /// Identifies the AutoPlay dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoPlayProperty =
            DependencyProperty.Register("AutoPlay", typeof(bool), typeof(MediaElement), new PropertyMetadata(true, AutoPlay_Changed));

        private static void AutoPlay_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MediaElement)d;
            if (control._mediaElement_ForAudioOnly_ForSimulatorOnly == null)
            {
                control.ManageDomBoolProperty_Changed(HTML_AUTOPLAY_PROPERTY_NAME, (bool)e.NewValue);
            }
            else //do nothing ? (because AutoPlay doesn't exist in wpf so we use Loaded of the wpf's MediaElement then Play)
            {
                //control._mediaElement_ForAudioOnly_ForSimulatorOnly.AutoPlay = control.AutoPlay;
            }
        }

        ///// <summary>
        ///// Gets a value that indicates whether media can be paused if the Pause method
        ///// is called.
        ///// </summary>
        //public bool CanPause { get; }
        ///// <summary>
        ///// Identifies the CanPause dependency property.
        ///// </summary>
        //public static DependencyProperty CanPauseProperty { get; }

        //// Returns:
        ////     The current state of this MediaElement. The state can be one of the following
        ////     (as defined in the MediaElementState enumeration): Buffering, Closed, Opening,
        ////     Paused, Playing, or Stopped. The default value is Closed.
        ///// <summary>
        ///// Gets the status of this MediaElement.
        ///// </summary>
        //public MediaElementState CurrentState { get; }
        ///// <summary>
        ///// Identifies the CurrentState dependency property.
        ///// </summary>
        //public static DependencyProperty CurrentStateProperty { get; }

        /// <summary>
        /// Gets a value that reports whether the current source media is an audio-only
        /// media file.
        /// </summary>
        public bool IsAudioOnly
        {
            get { return (bool)GetValue(IsAudioOnlyProperty); }
            private set { SetValue(IsAudioOnlyProperty, value); }
        }
        /// <summary>
        /// Identifies the IsAudioOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAudioOnlyProperty =
            DependencyProperty.Register("IsAudioOnly", typeof(bool), typeof(MediaElement), new PropertyMetadata(false));



        /// <summary>
        /// Gets or sets a value that describes whether the media source currently loaded
        /// in the media engine should automatically set the position to the media start
        /// after reaching its end.
        /// </summary>
        public bool IsLooping
        {
            get { return (bool)GetValue(IsLoopingProperty); }
            set { SetValue(IsLoopingProperty, value); }
        }
        /// <summary>
        /// Identifies the IsLooping dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoopingProperty =
            DependencyProperty.Register("IsLooping", typeof(bool), typeof(MediaElement), new PropertyMetadata(false, IsLooping_Changed));
        private static void IsLooping_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MediaElement)d;
            if (control._mediaElement_ForAudioOnly_ForSimulatorOnly == null)
            {
                control.ManageDomBoolProperty_Changed(HTML_ISLOOPING_PROPERTY_NAME, (bool)e.NewValue);
            }
            else
            {
                control._mediaElement_ForAudioOnly_ForSimulatorOnly.IsLooping = control.IsLooping;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the audio is muted.
        /// </summary>
        public bool IsMuted
        {
            get { return (bool)GetValue(IsMutedProperty); }
            set { SetValue(IsMutedProperty, value); }
        }
        /// <summary>
        /// Identifies the IsMuted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.Register("IsMuted", typeof(bool), typeof(MediaElement), new PropertyMetadata(false, IsMuted_Changed));

        private static void IsMuted_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MediaElement)d;
            if (control._mediaElement_ForAudioOnly_ForSimulatorOnly == null)
            {
                control.ManageDomBoolProperty_Changed(HTML_ISMUTED_PROPERTY_NAME, (bool)e.NewValue);
            }
            else
            {
                control._mediaElement_ForAudioOnly_ForSimulatorOnly.IsMuted = control.IsMuted;
            }
        }



        //// Returns:
        ////     The amount of time since the beginning of the media. The default is a TimeSpan
        ////     with value 0:0:0.
        ///// <summary>
        ///// Gets or sets the current position of progress through the media's playback
        ///// time.
        ///// </summary>
        //public TimeSpan Position
        //{
        //    get { return (TimeSpan)GetValue(PositionProperty); }
        //    set { SetValue(PositionProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the Position dependency property.
        ///// </summary>
        //public static readonly DependencyProperty PositionProperty =
        //    DependencyProperty.Register("Position", typeof(TimeSpan), typeof(MediaElement), new PropertyMetadata(new TimeSpan(), Position_Changed));

        //private static void Position_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = (MediaElement)d;
        //    control.ManagePosition_Changed();
        //}

        //void ManagePosition_Changed()
        //{
        //    if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _mediaElement != null)
        //    {
        //        INTERNAL_HtmlDomManager.SetDomElementAttribute(_mediaElement, "currentTime", Position.TotalSeconds, forceSimulatorExecuteImmediately: true);
        //    }
        //}

        /// <summary>
        /// Gets or sets a media source on the MediaElement.
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set
            {
#if ! BRIDGE
                // Get the assembly name of the calling method: //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method that is executed immediately after the one where the URI is defined! Be careful when moving the following line of code.
                string callerAssemblyName = Interop.IsRunningInTheSimulator ? Assembly.GetCallingAssembly().GetName().Name : INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#else
                
                // Get the assembly name of the calling method: //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method that is executed immediately after the one where the URI is defined! Be careful when moving the following line of code.
                string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#endif
                this._nameOfAssemblyThatSetTheSourceUri = callerAssemblyName;
                SetValue(SourceProperty, value);
            }
        }
        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(MediaElement), new PropertyMetadata(null, Source_Changed));

        private static void Source_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MediaElement)d;
            var newValue = (Uri)e.NewValue;

            // Always check that the control is in the Visual Tree before modifying its HTML representation
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(control))
            {
                string newUri = newValue.ToString();

                // If the new Source is an empty string, we avoid the error messages:
                if (!string.IsNullOrWhiteSpace(newUri))
                {
                    string tagString = "none";

                    string valueForHtml5SourceProperty = INTERNAL_UriHelper.ConvertToHtml5Path(newValue.ToString(), control);

                    string newExtensionLowercase = GetExtension(newUri).ToLower();

                    if (SupportedVideoTypes.Contains(newExtensionLowercase))
                    {
                        if (control.IsAudioOnly || control._mediaElement == null) //note: I chose to use IsAudioOnly here because using e.oldValue would make it recreate the video tag when it was already a video tag.
                        {
                            tagString = "video";
                            control.IsAudioOnly = false;
                        }
                    }
                    else if (SupportedAudioTypes.Contains(newExtensionLowercase))
                    {
                        if (!control.IsAudioOnly || control._mediaElement == null) //note: I chose to use IsAudioOnly here because using e.oldValue would make it recreate the audio tag when it was already a audio tag.
                        {
                            tagString = "audio";
                            control.IsAudioOnly = true;
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("ERROR: The MediaElement control only supports files of the following types: VIDEO: mp4, ogv, webm, 3gp - AUDIO: mp3, ogg - Note: for best browser compatibility, it is recommended to use only MP3 and MP4 files.");
                    }
                    if (tagString != "none") //tagString != "none" means that the new Uri has a different type (audio VS video) than the old one, so we need to (re)create the dom tag.
                    {
                        if (control._mediaElement != null)
                        {
                            INTERNAL_HtmlDomManager.RemoveFromDom(control._mediaElement); //note: there can be only one child element.
                        }
                        if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript || tagString == "video")
                        {
                            object element = null;
                            object outerDiv = control.INTERNAL_OuterDomElement;
                            INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle(tagString, outerDiv, control, out element);

                            control._mediaElement_ForAudioOnly_ForSimulatorOnly = null;

                            control._mediaElement = element;

                            control.Refresh(); //we refresh all the values of the element in the visual tree
                        }
                        else //when we are in the simulator, we don't want to use the <audio> tag, we will use a wpf one instead (because awesomium doesn't support .mp3 for example)
                        {
#if !CSHTML5NETSTANDARD
                            if (control._mediaElement_ForAudioOnly_ForSimulatorOnly == null)
                            {
                                control._mediaElement_ForAudioOnly_ForSimulatorOnly = INTERNAL_Simulator.WpfMediaElementFactory.Create((Action)control.SimulatorMediaElement_Loaded, (Action)control.SimulatorMediaElement_MediaEnded);
                            }
                            control._mediaElement_ForAudioOnly_ForSimulatorOnly.Source = new Uri(valueForHtml5SourceProperty);
                            control.Refresh_SimulatorOnly();
#endif
                            return;
                        }
                    }

                    // Update the "src" property of the <video> or <audio> tag
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(control._mediaElement, "src", valueForHtml5SourceProperty, forceSimulatorExecuteImmediately: true);
                }
                else
                {
                    if (control._mediaElement != null)
                    {
                        // Remove previous video/audio if any:
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(control._mediaElement, "src", "", forceSimulatorExecuteImmediately: true);
                    }
                }
            }
        }

        // Returns:
        //     The media's volume represented on a linear scale between 0 and 1. The default
        //     is 0.5.
        /// <summary>
        /// Gets or sets the media's volume.
        /// </summary>
        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        /// <summary>
        /// Identifies the Volume dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(MediaElement), new PropertyMetadata(0.5d, Volume_Changed));

        private static void Volume_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MediaElement)d;
            control.ManageVolume_Changed();
        }

        void ManageVolume_Changed()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                if (_mediaElement != null)
                {
                    INTERNAL_HtmlDomManager.SetDomElementProperty(_mediaElement, "volume", Volume);
                }
                else if (_mediaElement_ForAudioOnly_ForSimulatorOnly != null)
                {
                    _mediaElement_ForAudioOnly_ForSimulatorOnly.Pause();
                }
            }
        }

        ///// <summary>
        ///// Occurs when the value of the CurrentState property changes.
        ///// </summary>
        //public event RoutedEventHandler CurrentStateChanged;

        ///// <summary>
        ///// Occurs when the MediaElement is no longer playing audio or video.
        ///// </summary>
        //public event RoutedEventHandler MediaEnded;

        ///// <summary>
        ///// Occurs when there is an error associated with the media Source.
        ///// </summary>
        //public event ExceptionRoutedEventHandler MediaFailed;

        ///// <summary>
        ///// Occurs when the media stream has been validated and opened, and the file
        ///// headers have been read.
        ///// </summary>
        //public event RoutedEventHandler MediaOpened;

        ///// <summary>
        ///// Occurs when the value of the Volume property changes.
        ///// </summary>
        //public event RoutedEventHandler VolumeChanged;

        /// <summary>
        /// Returns an enumeration value that describes the likelihood that the current
        /// MediaElement and its client configuration can play that media source.
        /// </summary>
        /// <param name="type">A string that describes the desired type as a string.</param>
        /// <returns>
        /// A value of the enumeration that describes the likelihood that the current
        /// media engine can play the source.
        /// </returns>
        public MediaCanPlayResponse CanPlayType(string type)
        {
            string canplay = INTERNAL_HtmlDomManager.CallDomMethod(_mediaElement, "canPlayType", type).ToString();
            return ConvertHtmlCanPlayTypeResult(canplay);
        }

        MediaCanPlayResponse ConvertHtmlCanPlayTypeResult(string htmlResult)
        {
            switch (htmlResult)
            {
                case "maybe":
                    return MediaCanPlayResponse.Maybe;
                case "probably":
                    return MediaCanPlayResponse.Probably;
                default:
                    return MediaCanPlayResponse.NotSupported;
            }
        }

        /// <summary>
        /// Pauses media at the current position.
        /// </summary>
        public void Pause()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {

                if (_mediaElement != null)
                {
                    INTERNAL_HtmlDomManager.CallDomMethod(_mediaElement, "pause");
                }
                else if (_mediaElement_ForAudioOnly_ForSimulatorOnly != null)
                {
                    _mediaElement_ForAudioOnly_ForSimulatorOnly.Pause();
                }
            }
        }

        /// <summary>
        /// Plays media from the current position.
        /// </summary>
        public void Play()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                if (_mediaElement != null)
                {
                    INTERNAL_HtmlDomManager.CallDomMethod(_mediaElement, "play");
                }
                else if (_mediaElement_ForAudioOnly_ForSimulatorOnly != null)
                {
                    _mediaElement_ForAudioOnly_ForSimulatorOnly.Play();
                }
            }
        }

        ///// <summary>
        ///// Sets the Source property using the supplied stream.
        ///// </summary>
        ///// <param name="stream">The stream that contains the media to load.</param>
        ///// <param name="mimeType">
        ///// The MIME type of the media resource, expressed as the string form typically
        ///// seen in HTTP headers and requests.
        ///// </param>
        //public void SetSource(IRandomAccessStream stream, string mimeType);

        /// <summary>
        /// Stops and resets media to be played from the beginning.
        /// </summary>
        //public void Stop()
        //{
        //    //todo
        //}


        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = null;
            object outerDiv;
            dynamic outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out outerDiv);
            return outerDiv;
        }

        static string GetExtension(string uriString)
        {
            if (uriString != null)
            {
                int lastDotIndex = uriString.LastIndexOf('.');
                if (lastDotIndex > -1)
                {
                    return uriString.Substring(lastDotIndex + 1);
                }
            }
            return string.Empty;
        }

        public bool ShowControls
        {
            get { return (bool)GetValue(ShowControlsProperty); }
            set { SetValue(ShowControlsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowControls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowControlsProperty =
            DependencyProperty.Register("ShowControls", typeof(bool), typeof(MediaElement), new PropertyMetadata(false, ShowControls_Changed));

        private static void ShowControls_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (MediaElement)sender;
            control.ManageDomBoolProperty_Changed(HTML_SHOWCONTROLS_PROPERTY_NAME, (bool)e.NewValue);
        }

        void ManageDomBoolProperty_Changed(string htmlPropertyName, bool newValue)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _mediaElement != null)
            {
                if (newValue)
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(_mediaElement, htmlPropertyName, "true");
                }
                else
                {
                    INTERNAL_HtmlDomManager.RemoveDomElementAttribute(_mediaElement, htmlPropertyName, forceSimulatorExecuteImmediately: true);
                }
            }
        }

        void Refresh_SimulatorOnly()
        {
            _mediaElement_ForAudioOnly_ForSimulatorOnly.IsMuted = IsMuted;
            _mediaElement_ForAudioOnly_ForSimulatorOnly.Volume = Volume;
        }

        private void SimulatorMediaElement_Loaded()
        {
            //Todo: throw the Loaded event
            if (AutoPlay)
            {
                _mediaElement_ForAudioOnly_ForSimulatorOnly.Play();
            }
        }

        private void SimulatorMediaElement_MediaEnded()
        {
            //todo: throw the MediaEnded event
            if (IsLooping)
            {
                _mediaElement_ForAudioOnly_ForSimulatorOnly.Stop();
                _mediaElement_ForAudioOnly_ForSimulatorOnly.Play();
            }
        }

        void Refresh()
        {
            // We call these because we need to apply default values if the dependency properties are not explicitly set, or if we have recreated the audio/video dom tag due to a change of the source property:
            ManageDomBoolProperty_Changed(HTML_SHOWCONTROLS_PROPERTY_NAME, ShowControls); //ShowControls Property
            //ManageSourceChanged();
            ManageDomBoolProperty_Changed(HTML_AUTOPLAY_PROPERTY_NAME, AutoPlay); //Autoplay Property
            ManageDomBoolProperty_Changed(HTML_ISLOOPING_PROPERTY_NAME, IsLooping); //IsLooping Property
            ManageDomBoolProperty_Changed(HTML_ISMUTED_PROPERTY_NAME, IsMuted); //IsMuted Property
            //ManagePosition_Changed();
            ManageVolume_Changed();
        }


        #region non supported stuff

        //public Stereo3DVideoPackingMode ActualStereo3DVideoPackingMode { get; }
        //public static DependencyProperty ActualStereo3DVideoPackingModeProperty { get; }

        //// Returns:
        ////     The height portion of the native aspect ratio of the media. This value holds
        ////     meaning only when you compare it with the value for the AspectRatioWidth
        ////     property; the two properties together describe the aspect ratio.
        ///// <summary>
        ///// Gets the height portion of the native aspect ratio of the media.
        ///// </summary>
        //public int AspectRatioHeight { get; }
        ///// <summary>
        ///// Identifies the AspectRatioHeight dependency property.
        ///// </summary>
        //public static DependencyProperty AspectRatioHeightProperty { get; }

        //// Returns:
        ////     The width portion of the native aspect ratio of the media. This value holds
        ////     meaning only when you compare it with the value for the AspectRatioHeight
        ////     property; the two properties together describe the aspect ratio.
        ///// <summary>
        ///// Gets the width portion of the native aspect ratio of the media.
        ///// </summary>
        //public int AspectRatioWidth { get; }
        ///// <summary>
        ///// Identifies the AspectRatioWidth dependency property.
        ///// </summary>
        //public static DependencyProperty AspectRatioWidthProperty { get; }

        ///// <summary>
        ///// Gets or sets a value that describes the purpose of the audio information
        ///// in an audio stream.
        ///// </summary>
        //public AudioCategory AudioCategory { get; set; }
        ///// <summary>
        ///// Identifies the AudioCategory dependency property.
        ///// </summary>
        //public static DependencyProperty AudioCategoryProperty { get; }

        ///// <summary>
        ///// Gets or sets a value that describes the primary usage of the device that
        ///// is being used to play back audio.
        ///// </summary>
        //public AudioDeviceType AudioDeviceType { get; set; }
        ///// <summary>
        ///// Identifies the AudioDeviceType dependency property.
        ///// </summary>
        //public static DependencyProperty AudioDeviceTypeProperty { get; }

        //// Returns:
        ////     The number of audio streams that exist in the source media file. The default
        ////     value is 0.
        ///// <summary>
        ///// Gets the number of audio streams that exist in the current media file.
        ///// </summary>
        //public int AudioStreamCount { get; }
        ///// <summary>
        ///// Identifies the AudioStreamCount dependency property.
        ///// </summary>
        //public static DependencyProperty AudioStreamCountProperty { get; }

        //// Returns:
        ////     The index in the media file of the audio component that plays along with
        ////     the video component. The index can be unspecified, in which case the value
        ////     is null. The default value is null. If you're programming using or , the
        ////     type of this property is projected as int? (a nullable integer).
        ///// <summary>
        ///// Gets or sets the index of the audio stream that plays along with the video
        ///// component. The collection of audio streams is composed at run time and represents
        ///// all audio streams that are available in the media file.
        ///// </summary>
        //public int? AudioStreamIndex { get; set; }
        ///// <summary>
        ///// Identifies the AudioStreamIndex dependency property.
        ///// </summary>
        //public static DependencyProperty AudioStreamIndexProperty { get; }

        //// Returns:
        ////     The ratio of volume across speakers in the range between -1 and 1. The default
        ////     value is 0.
        ///// <summary>
        ///// Gets or sets a ratio of volume across stereo speakers.
        ///// </summary>
        //public double Balance { get; set; }
        ///// <summary>
        ///// Identifies the Balance dependency property.
        ///// </summary>
        //public static DependencyProperty BalanceProperty { get; }

        //// Returns:
        ////     The amount of buffering that is completed for media content. The value ranges
        ////     from 0 to 1. Multiply by 100 to obtain a percentage.
        ///// <summary>
        ///// Gets a value that indicates the current buffering progress.
        ///// </summary>
        //public double BufferingProgress { get; }

        //// Returns:
        ////     The identifier for the BufferingProgress dependency property.
        ///// <summary>
        ///// Identifies the BufferingProgress dependency property.
        ///// </summary>
        //public static DependencyProperty BufferingProgressProperty { get; }

        ///// <summary>
        ///// Gets a value that indicates whether media can be repositioned by setting
        ///// the value of the Position property.
        ///// </summary>
        //public bool CanSeek { get; }
        ///// <summary>
        ///// Identifies the CanSeek dependency property.
        ///// </summary>
        //public static DependencyProperty CanSeekProperty { get; }

        ///// <summary>
        ///// Gets or sets the default playback rate for the media engine. The playback
        ///// rate applies when the user isn't using fast forward or reverse.
        ///// </summary>
        //public double DefaultPlaybackRate { get; set; }
        ///// <summary>
        ///// Identifies the DefaultPlaybackRate dependency property.
        ///// </summary>
        //public static DependencyProperty DefaultPlaybackRateProperty { get; }

        //// Returns:
        ////     A value that indicates the amount of download completed for content that
        ////     is located on a remote server. The value ranges from 0 to 1. Multiply by
        ////     100 to obtain a percentage.
        ///// <summary>
        ///// Gets a value that indicates the amount of download completed for content
        ///// located on a remote server.
        ///// </summary>
        //public double DownloadProgress { get; }
        ///// <summary>
        ///// Identifies the DownloadProgress dependency property.
        ///// </summary>
        //public static DependencyProperty DownloadProgressProperty { get; }

        ///// <summary>
        ///// Gets the offset of download progress, which is relevant in seek-ahead scenarios.
        ///// </summary>
        //public double DownloadProgressOffset { get; }
        ///// <summary>
        ///// Identifies the DownloadProgressOffset dependency property.
        ///// </summary>
        //public static DependencyProperty DownloadProgressOffsetProperty { get; }

        ///// <summary>
        ///// Gets a value that reports whether the current source media is a stereo 3-D
        ///// video media file.
        ///// </summary>
        //public bool IsStereo3DVideo { get; }
        ///// <summary>
        ///// Identifies the IsStereo3DVideo dependency property.
        ///// </summary>
        //public static DependencyProperty IsStereo3DVideoProperty { get; }

        //// Returns:
        ////     The collection of timeline markers (represented as TimelineMarker objects)
        ////     associated with the currently loaded media file. The default value is an
        ////     empty collection.
        ///// <summary>
        ///// Gets the collection of timeline markers associated with the currently loaded
        ///// media file.
        ///// </summary>
        //public TimelineMarkerCollection Markers { get; }

        //// Returns:
        ////     The natural duration of the media. The default value is a Duration structure
        ////     that evaluates as Automatic, which is the value held if you query this property
        ////     before MediaOpened.
        ///// <summary>
        ///// Gets the duration of the media file currently opened.
        ///// </summary>
        //public Duration NaturalDuration { get; }
        ///// <summary>
        ///// Identifies the NaturalDuration dependency property.
        ///// </summary>
        //public static DependencyProperty NaturalDurationProperty { get; }

        //// Returns:
        ////     The height of the video that is associated with the media, in pixels. Audio
        ////     files will return 0. The default value is 0.
        ///// <summary>
        ///// Gets the height of the video associated with the media.
        ///// </summary>
        //public int NaturalVideoHeight { get; }
        ///// <summary>
        ///// Identifies the NaturalVideoHeight dependency property.
        ///// </summary>
        //public static DependencyProperty NaturalVideoHeightProperty { get; }

        //// Returns:
        ////     The width of the video associated with the media. The default value is 0.
        ///// <summary>
        ///// Gets the width of the video associated with the media.
        ///// </summary>
        //public int NaturalVideoWidth { get; }
        ///// <summary>
        ///// Identifies the NaturalVideoWidth dependency property.
        ///// </summary>
        //public static DependencyProperty NaturalVideoWidthProperty { get; }

        //// Returns:
        ////     The playback rate ratio for the media. A value of 1.0 is the normal playback
        ////     speed. Value can be negative to play backwards.
        ///// <summary>
        ///// Gets or sets the playback rate ratio for the media engine.
        ///// </summary>
        //public double PlaybackRate { get; set; }
        ///// <summary>
        ///// Identifies the PlaybackRate dependency property.
        ///// </summary>
        //public static DependencyProperty PlaybackRateProperty { get; }

        //// Returns:
        ////     A reference object that carries the "PlayTo" source information.
        ///// <summary>
        ///// Gets the information that is transmitted if the MediaElement is used for
        ///// a "PlayTo" scenario.
        ///// </summary>
        //public PlayToSource PlayToSource { get; }
        ///// <summary>
        ///// Identifies the PlayToSource dependency property.
        ///// </summary>
        //public static DependencyProperty PlayToSourceProperty { get; }

        //// Returns:
        ////     An image source for a transition ImageBrush that is applied to the MediaElement
        ////     content area.
        ///// <summary>
        ///// Gets or sets the image source that is used for a placeholder image during
        ///// MediaElement loading transition states.
        ///// </summary>
        //public ImageSource PosterSource { get; set; }
        ///// <summary>
        ///// Identifies the PosterSource dependency property.
        ///// </summary>
        //public static DependencyProperty PosterSourceProperty { get; }

        ///// <summary>
        ///// Gets or sets the dedicated object for media content protection that is associated
        ///// with this MediaElement.
        ///// </summary>
        //public MediaProtectionManager ProtectionManager { get; set; }
        ///// <summary>
        ///// Identifies the ProtectionManager dependency property.
        ///// </summary>
        //public static DependencyProperty ProtectionManagerProperty { get; }

        ///// <summary>
        ///// Gets or sets a value that configures the MediaElement for real-time communications
        ///// scenarios.
        ///// </summary>
        //public bool RealTimePlayback { get; set; }
        ///// <summary>
        ///// Identifies the RealTimePlayback dependency property.
        ///// </summary>
        //public static DependencyProperty RealTimePlaybackProperty { get; }

        ///// <summary>
        ///// Gets or sets an enumeration value that determines the stereo 3-D video frame-packing
        ///// mode for the current media source.
        ///// </summary>
        //public Stereo3DVideoPackingMode Stereo3DVideoPackingMode { get; set; }
        ///// <summary>
        ///// Identifies the Stereo3DVideoPackingMode dependency property.
        ///// </summary>
        //public static DependencyProperty Stereo3DVideoPackingModeProperty { get; }

        ///// <summary>
        ///// Gets or sets an enumeration value that determines the stereo 3-D video render
        ///// mode for the current media source.
        ///// </summary>
        //public Stereo3DVideoRenderMode Stereo3DVideoRenderMode { get; set; }
        ///// <summary>
        ///// Identifies the Stereo3DVideoRenderMode dependency property.
        ///// </summary>
        //public static DependencyProperty Stereo3DVideoRenderModeProperty { get; }

        ///// <summary>
        ///// Occurs when the BufferingProgress property changes.
        ///// </summary>
        //public event RoutedEventHandler BufferingProgressChanged;

        ///// <summary>
        ///// Occurs when the DownloadProgress property has changed.
        ///// </summary>
        //public event RoutedEventHandler DownloadProgressChanged;

        ///// <summary>
        ///// Occurs when a timeline marker is encountered during media playback.
        ///// </summary>
        //public event TimelineMarkerRoutedEventHandler MarkerReached;

        ///// <summary>
        ///// Occurs when PlaybackRate changes value.
        ///// </summary>
        //public event RateChangedRoutedEventHandler RateChanged;

        ///// <summary>
        ///// Occurs when the seek point of a requested seek operation is ready for playback.
        ///// </summary>
        //public event RoutedEventHandler SeekCompleted;

        ///// <summary>
        ///// Applies an audio effect to playback. Takes effect for the next source that
        ///// is set on this MediaElement.
        ///// </summary>
        ///// <param name="effectID">The identifier for the desired effect.</param>
        ///// <param name="effectOptional">
        ///// True if the effect shouldn't block playback when the effect can't be used
        ///// at run time. False if the effect should block playback when the effect can't
        ///// be used at run time.
        ///// </param>
        ///// <param name="effectConfiguration">
        ///// A property set that transmits property values to specific effects as selected
        ///// by effectID.
        ///// </param>
        //public void AddAudioEffect(string effectID, bool effectOptional, IPropertySet effectConfiguration);

        ///// <summary>
        ///// Applies a video effect to playback. Takes effect for the next source that
        ///// is set on this MediaElement.
        ///// </summary>
        ///// <param name="effectID">The identifier for the desired effect.</param>
        ///// <param name="effectOptional">
        ///// True if the effect shouldn't block playback when the effect can't be used
        ///// at run time. False if the effect should block playback when the effect can't
        ///// be used at run time.
        ///// </param>
        ///// <param name="effectConfiguration">
        ///// A property set that transmits property values to specific effects as selected
        ///// by effectID.
        ///// </param>
        //public void AddVideoEffect(string effectID, bool effectOptional, IPropertySet effectConfiguration);

        //public string GetAudioStreamLanguage(int? index);

        ////
        //// Summary:
        ///// <summary>
        ///// Removes all effects for the next source set for this MediaElement.
        ///// </summary>
        //public void RemoveAllEffects();

        #endregion
    }
}
