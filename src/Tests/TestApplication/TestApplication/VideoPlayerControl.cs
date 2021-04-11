using System;
#if SLMIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace TestApplication
{
    public class VideoPlayerControl : FrameworkElement
    {
        public VideoPlayerControl()
        {
            // Specify the HTML representation of the control:
            CSharpXamlForHtml5.DomManagement.SetHtmlRepresentation(this, @"<video controls autoplay/>");

            //todo: replace with the following code, so that on non-compatible browser we display an error message:
            //          CSharpXamlForHtml5.DomManagement.SetHtmlRepresentation(this, @"
            //<div>
            //  <video autoplay>
            //    Your browser does not support HTML5 video.
            //  </video>
            //</div>"); // Note: the text above will appear only on browsers that do not support HTML5 video.

            Loaded += VideoPlayerControl_Loaded;
            Unloaded += VideoPlayerControl_Unloaded;
        }

        void VideoPlayerControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var control = (VideoPlayerControl)sender;

            CSharpXamlForHtml5.DomManagement.GetDomElementFromControl(control).removeEventListener("error", (Action<object>)OnMediaFailed);
        }

        void VideoPlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            var control = (VideoPlayerControl)sender;

            CSharpXamlForHtml5.DomManagement.GetDomElementFromControl(control).addEventListener("error", (Action<object>)OnMediaFailed);
        }


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
        /// DependencyProperty for VideoPlayerControl AutoPlay property.
        /// </summary>
        public static readonly DependencyProperty AutoPlayProperty =
            DependencyProperty.Register("AutoPlay", typeof(bool), typeof(VideoPlayerControl), new PropertyMetadata(true, AutoPlay_Changed));

        private static void AutoPlay_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (VideoPlayerControl)sender;
            var newValue = (bool)e.NewValue;

            if (CSharpXamlForHtml5.DomManagement.IsControlInVisualTree(control))
            {
                if (newValue)
                {
                    CSharpXamlForHtml5.DomManagement.GetDomElementFromControl(control).autoplay = "true"; //any value should work actually, the very existence of the "controls" attribute makes it display the built-in controls.
                }
                else
                {
                    CSharpXamlForHtml5.DomManagement.GetDomElementFromControl(control).removeAttribute("autoplay");
                }
            }
        }

        /// <summary>
        /// Gets/Sets the Source on this VideoPlayerControl.
        ///
        /// The Source property is the Uri of the media to be played.
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for VideoPlayerControl Source property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(VideoPlayerControl), new PropertyMetadata(null, Source_Changed));

        /// <summary>
        /// Raised when source is changed
        /// </summary>
        static void Source_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (VideoPlayerControl)sender;
            var newValue = (Uri)e.NewValue;

            // Always check that the control is in the Visual Tree before modifying its HTML representation
            if (CSharpXamlForHtml5.DomManagement.IsControlInVisualTree(control))
            {
                // Verify that the URI is supported:
                string uriLowerCase = newValue.ToString().ToLower();
                if (!uriLowerCase.StartsWith(@"http:/") && !uriLowerCase.StartsWith(@"https:/"))
                {
                    MessageBox.Show("ERROR: In this version, video file location must start with http or https. Please change the URI of the video to play.");
                    return;
                }
                if (!uriLowerCase.EndsWith(@".mp4"))
                {
                    MessageBox.Show("ERROR: To support the biggest number of browsers, only .MP4 video files can be used. Please change the video format to MP4.");
                    return;
                }

                // Update the "src" property of the <video> tag
                CSHTML5.Interop.ExecuteJavaScript("$0.src = $1", CSHTML5.Interop.GetDiv(control), newValue.ToString());
            }
        }


        /// <summary>
        /// Gets or sets a value that indicates whether the player shows the built-in controls (Play, Progression, etc.) or not.
        /// </summary>
        public bool ShowControls
        {
            get { return (bool)GetValue(ShowControlsProperty); }
            set { SetValue(ShowControlsProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for VideoPlayerControl ShowControls property.
        /// </summary>
        public static readonly DependencyProperty ShowControlsProperty =
            DependencyProperty.Register("ShowControls", typeof(bool), typeof(VideoPlayerControl), new PropertyMetadata(true, ShowControls_Changed));

        private static void ShowControls_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (VideoPlayerControl)sender;
            var newValue = (bool)e.NewValue;

            if (CSharpXamlForHtml5.DomManagement.IsControlInVisualTree(control))
            {
                if (newValue)
                {
                    CSharpXamlForHtml5.DomManagement.GetDomElementFromControl(control).controls = "true"; //any value should work actually, the very existence of the "controls" attribute makes it display the built-in controls.
                }
                else
                {
                    CSharpXamlForHtml5.DomManagement.GetDomElementFromControl(control).removeAttribute("controls");
                }
            }
        }

        private void OnMediaFailed(object e)
        {
            MessageBox.Show("Unable to load: " + Source.OriginalString);
        }

        public event ExceptionRoutedEventHandler MediaFailed;

    }
}
