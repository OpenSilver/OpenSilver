
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Media;
using CSHTML5.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents an object that contains audio, video, or both.
    /// </summary>
    public sealed partial class MediaElement : FrameworkElement
    {
        private static readonly HashSet<string> SupportedVideoTypes = new() { "mp4", "ogv", "webm", "3gp" };

        // todo: not sure if ogg is actually only for audio or not.
        // If not, find a way to know which one it currently is.
        private static readonly HashSet<string> SupportedAudioTypes = new() { "mp3", "ogg" };

        private INTERNAL_HtmlDomElementReference _mediaElement;
        private JavaScriptCallback _mediaOpenedCallback;
        private JavaScriptCallback _mediaEndedCallback;
        private JavaScriptCallback _mediaFailedCallback;

        /// <summary>
        /// Occurs when the media stream has been validated and opened, and the file headers have been read.
        /// </summary>
        public event RoutedEventHandler MediaOpened;

        private void OnMediaOpened() => MediaOpened?.Invoke(this, new RoutedEventArgs());

        /// <summary>
        /// Occurs when the <see cref="MediaElement"/> is no longer playing audio or video.
        /// </summary>
        public event RoutedEventHandler MediaEnded;

        private void OnMediaEnded() => MediaEnded?.Invoke(this, new RoutedEventArgs());

        /// <summary>
        /// Occurs when there is an error associated with the media <see cref="Source"/>.
        /// </summary>
        public event EventHandler<ExceptionRoutedEventArgs> MediaFailed;

        private void OnMediaFailed() => MediaFailed?.Invoke(this, new ExceptionRoutedEventArgs());

        /// <summary>
        /// Gets or sets a value that indicates whether media will begin playback automatically
        /// when the Source property is set.
        /// </summary>
        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValueInternal(AutoPlayProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AutoPlay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoPlayProperty =
            DependencyProperty.Register(
                nameof(AutoPlay),
                typeof(bool),
                typeof(MediaElement),
                new PropertyMetadata(true)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((MediaElement)d).SetAutoPlayAttribute((bool)newValue),
                });

        private void SetBoolAttribute(string attributeName, bool value)
        {
            Debug.Assert(INTERNAL_VisualTreeManager.IsElementInVisualTree(this));

            if (_mediaElement != null)
            {
                if (value)
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(_mediaElement, attributeName, "true");
                }
                else
                {
                    INTERNAL_HtmlDomManager.RemoveAttribute(_mediaElement, attributeName);
                }
            }
        }

        private void SetAutoPlayAttribute(bool value)
        {
            const string AutoPlay = "autoplay";
            SetBoolAttribute(AutoPlay, value);
        }

        /// <summary>
        /// Gets a value that reports whether the current source media is an audio-only
        /// media file.
        /// </summary>
        public bool IsAudioOnly
        {
            get { return (bool)GetValue(IsAudioOnlyProperty); }
            private set { SetValueInternal(IsAudioOnlyPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsAudioOnlyPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsAudioOnly),
                typeof(bool),
                typeof(MediaElement),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsAudioOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAudioOnlyProperty = IsAudioOnlyPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets a value that describes whether the media source currently loaded
        /// in the media engine should automatically set the position to the media start
        /// after reaching its end.
        /// </summary>
        public bool IsLooping
        {
            get { return (bool)GetValue(IsLoopingProperty); }
            set { SetValueInternal(IsLoopingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsLooping"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoopingProperty =
            DependencyProperty.Register(
                nameof(IsLooping),
                typeof(bool),
                typeof(MediaElement),
                new PropertyMetadata(false)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((MediaElement)d).SetLoopAttribute((bool)newValue),
                });

        private void SetLoopAttribute(bool value)
        {
            const string Loop = "loop";
            SetBoolAttribute(Loop, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the audio is muted.
        /// </summary>
        public bool IsMuted
        {
            get { return (bool)GetValue(IsMutedProperty); }
            set { SetValueInternal(IsMutedProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="IsMuted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.Register(
                nameof(IsMuted),
                typeof(bool),
                typeof(MediaElement),
                new PropertyMetadata(false)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((MediaElement)d).SetMutedAttribute((bool)newValue),
                });

        private void SetMutedAttribute(bool value)
        {
            const string Muted = "muted";
            SetBoolAttribute(Muted, value);
        }

        /// <summary>
        /// Gets or sets a media source on the MediaElement.
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValueInternal(SourceProperty, value); }
        }
        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(Uri),
                typeof(MediaElement),
                new PropertyMetadata(null, OnSourceChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((MediaElement)d).SetMediaSource((Uri)newValue),
                });

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Uri source)
            {
                string uriString = source.ToString();
                if (!string.IsNullOrWhiteSpace(uriString))
                {
                    string extension = GetExtension(uriString).ToLower();
                    if (!SupportedVideoTypes.Contains(extension) && !SupportedAudioTypes.Contains(extension))
                    {
                        throw new NotSupportedException($"ERROR: The MediaElement control only supports files of the following types: VIDEO: {string.Join(", ", SupportedVideoTypes)} - AUDIO: {string.Join(", ", SupportedAudioTypes)} - Note: for best browser compatibility, it is recommended to use only MP3 and MP4 files.");
                    }
                }
            }
        }

        private void SetMediaSource(Uri source)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _mediaElement != null)
            {
                string sElement = OpenSilver.Interop.GetVariableStringForJS(_mediaElement);
                INTERNAL_HtmlDomManager.RemoveAttribute(_mediaElement, "src");
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sElement}.load();");
            }

            CreateMediaElement(OuterDiv, source);
        }

        /// <summary>
        /// Gets or sets the media's volume.
        /// </summary>
        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValueInternal(VolumeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Volume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register(
                nameof(Volume),
                typeof(double),
                typeof(MediaElement),
                new PropertyMetadata(0.5d)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((MediaElement)d).SetVolumeProperty((double)newValue),
                });

        private void SetVolumeProperty(double volume)
        {
            const string Volume = "volume";
            if (_mediaElement != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_mediaElement, Volume, volume);
            }
        }

        public bool ShowControls
        {
            get { return (bool)GetValue(ShowControlsProperty); }
            set { SetValueInternal(ShowControlsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowControls"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowControlsProperty =
            DependencyProperty.Register(
                nameof(ShowControls),
                typeof(bool),
                typeof(MediaElement),
                new PropertyMetadata(false)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((MediaElement)d).SetControlsAttribute((bool)newValue),
                });

        private void SetControlsAttribute(bool value)
        {
            const string Controls = "controls";
            SetBoolAttribute(Controls, value);
        }

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
            if (_mediaElement != null)
            {
                string sElement = OpenSilver.Interop.GetVariableStringForJS(_mediaElement);
                string sType = OpenSilver.Interop.GetVariableStringForJS(type);
                string canPlay = OpenSilver.Interop.ExecuteJavaScriptString($"{sElement}.canPlayType({sType});");
                return ToMediaCanPlayResponse(canPlay);
            }

            return MediaCanPlayResponse.NotSupported;
        }

        private static MediaCanPlayResponse ToMediaCanPlayResponse(string htmlResult) =>
            htmlResult switch
            {
                "maybe" => MediaCanPlayResponse.Maybe,
                "probably" => MediaCanPlayResponse.Probably,
                _ => MediaCanPlayResponse.NotSupported,
            };

        /// <summary>
        /// Pauses media at the current position.
        /// </summary>
        public void Pause()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                if (_mediaElement != null)
                {
                    string sElement = OpenSilver.Interop.GetVariableStringForJS(_mediaElement);
                    OpenSilver.Interop.ExecuteJavaScriptVoid($"{sElement}.pause();");
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
                    string sElement = OpenSilver.Interop.GetVariableStringForJS(_mediaElement);
                    OpenSilver.Interop.ExecuteJavaScriptVoid($"{sElement}.play();");
                }
            }
        }

        /// <summary>
        /// Stops and resets media to be played from the beginning.
        /// </summary>
        public void Stop()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                if (_mediaElement != null)
                {
                    string sElement = OpenSilver.Interop.GetVariableStringForJS(_mediaElement);
                    OpenSilver.Interop.ExecuteJavaScriptVoid($"{sElement}.pause(); {sElement}.currentTime = 0;");
                }
            }
        }

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            UnregisterEvent();
            _mediaElement = null;
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = null;
            var outerDiv = INTERNAL_HtmlDomManager.CreateDomLayoutElementAndAppendIt("div", parentRef, this, false);
            CreateMediaElement(outerDiv, Source);
            return outerDiv;
        }

        private void CreateMediaElement(INTERNAL_HtmlDomElementReference parentRef, Uri source)
        {
            string absoluteURI = string.Empty;

            if (source != null)
            {
                string uriString = source.ToString();
                if (!string.IsNullOrWhiteSpace(uriString))
                {
                    absoluteURI = INTERNAL_UriHelper.ConvertToHtml5Path(uriString, this);

                    string tagName = string.Empty;
                    string extension = GetExtension(uriString).ToLower();

                    if (SupportedVideoTypes.Contains(extension))
                    {
                        // note: I chose to use IsAudioOnly here because using e.oldValue would make
                        // it recreate the video tag when it was already a video tag.
                        if (IsAudioOnly || _mediaElement == null)
                        {
                            tagName = "video";
                            IsAudioOnly = false;
                        }
                    }
                    else if (SupportedAudioTypes.Contains(extension))
                    {
                        // note: I chose to use IsAudioOnly here because using e.oldValue would make
                        // it recreate the audio tag when it was already a audio tag.
                        if (!IsAudioOnly || _mediaElement == null)
                        {
                            tagName = "audio";
                            IsAudioOnly = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(tagName))
                    {
                        if (_mediaElement != null)
                        {
                            UnregisterEvent();
                            INTERNAL_HtmlDomManager.RemoveFromDom(_mediaElement);
                            _mediaElement = null;
                        }

                        _mediaElement = CreateHTMLMediaElement(tagName, parentRef);

                        if (!IsAudioOnly)
                        {
                            _mediaElement.Style.width = "100%";
                            _mediaElement.Style.height = "100%";
                        }

                        Refresh();
                    }
                }
            }

            if (_mediaElement != null && !string.IsNullOrEmpty(absoluteURI))
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_mediaElement, "src", absoluteURI, true);
            }
        }

        private INTERNAL_HtmlDomElementReference CreateHTMLMediaElement(string tagName, INTERNAL_HtmlDomElementReference parent)
        {
            var mediaElement = INTERNAL_HtmlDomManager.AppendDomElement(tagName, parent, this);

            string sElement = OpenSilver.Interop.GetVariableStringForJS(mediaElement);

            _mediaOpenedCallback = JavaScriptCallback.Create(OnMediaOpened);
            string mediaOpenedCallback = OpenSilver.Interop.GetVariableStringForJS(_mediaOpenedCallback);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sElement}.addEventListener('loadedmetadata', function(e) {{ {mediaOpenedCallback}(); }});");

            _mediaEndedCallback = JavaScriptCallback.Create(OnMediaEnded);
            string mediaEndedCallback = OpenSilver.Interop.GetVariableStringForJS(_mediaEndedCallback);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sElement}.addEventListener('ended', function(e) {{ {mediaEndedCallback}(); }});");

            _mediaFailedCallback = JavaScriptCallback.Create(OnMediaFailed);
            string mediaFailedCallback = OpenSilver.Interop.GetVariableStringForJS(_mediaFailedCallback);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sElement}.addEventListener('error', function(e) {{ {mediaFailedCallback}(); }});");

            return mediaElement;
        }

        private void UnregisterEvent()
        {
            _mediaOpenedCallback?.Dispose();
            _mediaOpenedCallback = null;
            _mediaEndedCallback?.Dispose();
            _mediaEndedCallback = null;
            _mediaFailedCallback?.Dispose();
            _mediaFailedCallback = null;
        }

        private static string GetExtension(string uriString)
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

        private void Refresh()
        {
            // We call these because we need to apply default values if the dependency
            // properties are not explicitly set, or if we have recreated the audio/video
            // dom tag due to a change of the source property:
            SetControlsAttribute(ShowControls);
            SetAutoPlayAttribute(AutoPlay);
            SetLoopAttribute(IsLooping);
            SetMutedAttribute(IsMuted);
            SetVolumeProperty(Volume);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new MediaElementAutomationPeer(this);
    }
}
