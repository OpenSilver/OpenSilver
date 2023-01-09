
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using OpenSilver.Internal;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Paints an area with an image.
    /// </summary>
	public sealed class ImageBrush : TileBrush
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBrush"/> class.
        /// </summary>
        public ImageBrush() { }

        /// <summary>
        /// Identifies the <see cref="ImageSource"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(ImageSource),
                typeof(ImageBrush),
                null);

        /// <summary>
        /// Gets or sets the image displayed by this <see cref="ImageBrush"/>.
        /// </summary>
        /// <returns>
        /// The image displayed by this <see cref="ImageBrush"/>.
        /// </returns>
		public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// Occurs when there is an error associated with image retrieval or format.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event EventHandler<ExceptionRoutedEventArgs> ImageFailed;

        /// <summary>
        /// Occurs when the image source is downloaded and decoded with no failure. You can
        /// use this event to determine the size of an image before rendering it.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event EventHandler<RoutedEventArgs> ImageOpened;

        internal async override Task<string> GetDataStringAsync(UIElement parent)
        {
            ImageSource source = ImageSource;
            if (source != null)
            {
                string url = await source.GetDataStringAsync(parent);
                if (!string.IsNullOrEmpty(url))
                {
                    string opacity = (1.0 - Opacity).ToInvariantString();
                    string positionX = ConvertAlignmentX(AlignmentX);
                    string positionY = ConvertAlignmentX(AlignmentY);
                    string stretch = ConvertStretch(Stretch);
                    return $"linear-gradient(to right, rgba(255, 255, 255, {opacity}) 0 100%), url({url}) {positionX} {positionY} / {stretch} no-repeat";
                }
            }

            return string.Empty;
        }

        private static string ConvertAlignmentX(AlignmentX alignmentX)
            => alignmentX switch
            {
                AlignmentX.Left => "left",
                AlignmentX.Right => "right",
                _ => "center",
            };

        private static string ConvertAlignmentX(AlignmentY alignmentY)
            => alignmentY switch
            {
                AlignmentY.Bottom => "bottom",
                AlignmentY.Top => "top",
                _ => "center",
            };

        private static string ConvertStretch(Stretch stretch)
            => stretch switch
            {
                Stretch.None => "auto",
                Stretch.Uniform => "contain",
                Stretch.UniformToFill => "cover",
                _ => "100% 100%",
            };

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<object> ConvertToCSSValues(DependencyObject parent)
        {
            var task = GetDataStringAsync(parent as UIElement);

            // For any source except WriteableBitmap, the url is retrieved
            // synchronously. In case we are dealing with a WriteableBitmap,
            // we can only return the source if the WriteableBitmap has already
            // been fully loaded.
            if (task.IsCompleted)
            {
                return new List<object>(1) { task.Result };
            }

            return new List<object>();
        }
    }
}
