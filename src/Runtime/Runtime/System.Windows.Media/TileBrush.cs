
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
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Base class that describes a way to paint a region.
    /// </summary>
    public abstract class TileBrush : Brush
	{
        /// <summary>
        /// Provides initialization for base class values when called by the constructor
        /// of a derived class.
        /// </summary>
        protected TileBrush() { }

        /// <summary>
		/// Identifies the <see cref="AlignmentX"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty AlignmentXProperty =
			DependencyProperty.Register(
				nameof(AlignmentX),
				typeof(AlignmentX),
				typeof(TileBrush),
				new PropertyMetadata(AlignmentX.Center));

        /// <summary>
        /// Gets or sets the horizontal alignment of content in the <see cref="TileBrush"/>
        /// base file.
        /// </summary>
        /// <returns>
        /// A value that specifies the horizontal position of <see cref="TileBrush"/> content 
        /// in its base tile. The default value is <see cref="AlignmentX.Center"/>.
        /// </returns>
        public AlignmentX AlignmentX
        {
            get => (AlignmentX)GetValue(AlignmentXProperty);
            set => SetValue(AlignmentXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AlignmentY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlignmentYProperty =
			DependencyProperty.Register(
				nameof(AlignmentY),
				typeof(AlignmentY),
				typeof(TileBrush),
				new PropertyMetadata(AlignmentY.Center));

        /// <summary>
        /// Gets or sets the vertical alignment of content in the <see cref="TileBrush"/>
        /// base file.
        /// </summary>
        /// <returns>
        /// A value that specifies the vertical position of <see cref="TileBrush"/> content 
        /// in its base tile. The default value is <see cref="AlignmentY.Center"/>.
        /// </returns>
        public AlignmentY AlignmentY
        {
            get => (AlignmentY)GetValue(AlignmentYProperty);
            set => SetValue(AlignmentYProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Stretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
			DependencyProperty.Register(
				nameof(Stretch),
				typeof(Stretch),
				typeof(TileBrush),
				new PropertyMetadata(Stretch.Fill));

        /// <summary>
        /// Gets or sets a value that specifies how the content of this <see cref="TileBrush"/>
        /// stretches to fit its tiles.
        /// </summary>
        /// <returns>
        /// A value that specifies how this <see cref="TileBrush"/> content is projected
        /// onto its base tile. The default value is <see cref="Stretch.Fill"/>.
        /// </returns>
		public Stretch Stretch
		{
			get => (Stretch)GetValue(StretchProperty);
			set => SetValue(StretchProperty, value);
		}
	}
}
