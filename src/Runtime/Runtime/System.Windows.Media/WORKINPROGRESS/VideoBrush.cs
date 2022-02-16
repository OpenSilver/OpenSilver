
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

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Paints an area with video content.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class VideoBrush : TileBrush
    {
        /// <summary>
        /// Initializes a new instance of the VideoBrush class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public VideoBrush() { }

        /// <summary>
        /// The identifier for the <see cref="SourceName"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SourceNameProperty =
            DependencyProperty.Register(
                nameof(SourceName),
                typeof(string),
                typeof(VideoBrush),
                new PropertyMetadata(string.Empty));

        /// <summary>
        ///  The name of the <see cref="MediaElement"/> to use as the source of the <see cref="VideoBrush"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public string SourceName
        {
            get => (string)GetValue(SourceNameProperty);
            set => SetValue(SourceNameProperty, value);
        }

        /// <summary>
        /// Sets the source of the <see cref="VideoBrush"/> using a media file source from an intermediary <see cref="MediaElement"/> control.
        /// </summary>
        /// <param name="source">
        /// The intermediary <see cref="MediaElement"/> control used as the source for the <see cref="VideoBrush"/>.
        /// </param>
        [OpenSilver.NotImplemented]
        public void SetSource(MediaElement source) { }
    }
}
