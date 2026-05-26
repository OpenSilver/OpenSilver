
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

using OpenSilver.Internal;

namespace System.Windows.Media
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
				new PropertyMetadata(AlignmentX.Center, OnPropertyChanged),
                ValidateEnums.IsAlignmentXValid);

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
            set => SetValueInternal(AlignmentXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AlignmentY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlignmentYProperty =
			DependencyProperty.Register(
				nameof(AlignmentY),
				typeof(AlignmentY),
				typeof(TileBrush),
				new PropertyMetadata(AlignmentY.Center, OnPropertyChanged),
                ValidateEnums.IsAlignmentYValid);

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
            set => SetValueInternal(AlignmentYProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Stretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
			DependencyProperty.Register(
				nameof(Stretch),
				typeof(Stretch),
				typeof(TileBrush),
				new PropertyMetadata(Stretch.Fill, OnPropertyChanged),
                ValidateEnums.IsStretchValid);

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
			set => SetValueInternal(StretchProperty, value);
		}

        /// <summary>
        /// Identifies the <see cref="ViewboxUnits"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewboxUnitsProperty =
            DependencyProperty.Register(
                nameof(ViewboxUnits),
                typeof(BrushMappingMode),
                typeof(TileBrush),
                new PropertyMetadata(BrushMappingMode.RelativeToBoundingBox, OnPropertyChanged),
                ValidateEnums.IsBrushMappingModeValid);

        /// <summary>
        /// Gets or sets a value that specifies whether the <see cref="Viewbox"/> value is relative 
        /// to the bounding box of the <see cref="TileBrush"/> contents or whether the value is absolute.
        /// </summary>
        /// <returns>
        /// A value that indicates whether the <see cref="Viewbox"/> value is relative to the bounding 
        /// box of the <see cref="TileBrush"/> contents or whether it is an absolute value. The default 
        /// value is <see cref="BrushMappingMode.RelativeToBoundingBox"/>.
        /// </returns>
        public BrushMappingMode ViewboxUnits
        {
            get => (BrushMappingMode)GetValue(ViewboxUnitsProperty);
            set => SetValueInternal(ViewboxUnitsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Viewbox"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewboxProperty =
            DependencyProperty.Register(
                nameof(Viewbox),
                typeof(Rect),
                typeof(TileBrush),
                new PropertyMetadata(new Rect(0, 0, 1, 1), OnPropertyChanged));

        /// <summary>
        /// Gets or sets the position and dimensions of the content in a <see cref="TileBrush"/> 
        /// tile.
        /// </summary>
        /// <returns>
        /// The position and dimensions of the <see cref="TileBrush"/> content. The default value 
        /// is a rectangle (<see cref="Rect"/>) that has a <see cref="Rect.TopLeft"/> of (0,0), and 
        /// a <see cref="Rect.Width"/> and <see cref="Rect.Height"/> of 1.
        /// </returns>
        public Rect Viewbox
        {
            get => (Rect)GetValue(ViewboxProperty);
            set => SetValueInternal(ViewboxProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ViewportUnits"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportUnitsProperty =
            DependencyProperty.Register(
                nameof(ViewportUnits),
                typeof(BrushMappingMode),
                typeof(TileBrush),
                new PropertyMetadata(BrushMappingMode.RelativeToBoundingBox, OnPropertyChanged));

        /// <summary>
        /// Gets or sets a <see cref="BrushMappingMode"/> enumeration that specifies whether the 
        /// value of the <see cref="Viewport"/>, which indicates the size and position of the 
        /// <see cref="TileBrush"/> base tile, is relative to the size of the output area.
        /// </summary>
        /// <returns>
        /// Indicates whether the value of the <see cref="Viewport"/>, which describes the size and 
        /// position of the <see cref="TileBrush"/> tiles, is relative to the size of the whole output 
        /// area. The default value is <see cref="BrushMappingMode.RelativeToBoundingBox"/>.
        /// </returns>
        public BrushMappingMode ViewportUnits
        {
            get => (BrushMappingMode)GetValue(ViewportUnitsProperty);
            set => SetValueInternal(ViewportUnitsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Viewport"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register(
                nameof(Viewport),
                typeof(Rect),
                typeof(TileBrush),
                new PropertyMetadata(new Rect(0, 0, 1, 1), OnPropertyChanged));

        /// <summary>
        /// Gets or sets the position and dimensions of the base tile for a <see cref="TileBrush"/>.
        /// </summary>
        /// <returns>
        /// The position and dimensions of the base tile for a <see cref="TileBrush"/>. The default 
        /// value is a rectangle (<see cref="Rect"/>) with a <see cref="Rect.TopLeft"/> of (0,0) and 
        /// a <see cref="Rect.Width"/> and <see cref="Rect.Height"/> of 1.
        /// </returns>
        public Rect Viewport
        {
            get => (Rect)GetValue(ViewportProperty);
            set => SetValueInternal(ViewportProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TileMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TileModeProperty =
            DependencyProperty.Register(
                nameof(TileMode),
                typeof(TileMode),
                typeof(TileBrush),
                new PropertyMetadata(TileMode.None, OnPropertyChanged),
                ValidateEnums.IsTileModeValid);

        /// <summary>
        /// Gets or sets a value that specifies how a <see cref="TileBrush"/> fills the area that you 
        /// are painting if the base tile is smaller than the output area.
        /// </summary>
        /// <returns>
        /// A value that specifies how the <see cref="TileBrush"/> tiles fill the output area when the 
        /// base tile, which is specified by the <see cref="Viewport"/> property, is smaller than the 
        /// output area. The default value is <see cref="TileMode.None"/>.
        /// </returns>
        public TileMode TileMode
        {
            get => (TileMode)GetValue(TileModeProperty);
            set => SetValueInternal(TileModeProperty, value);
        }
    }
}
