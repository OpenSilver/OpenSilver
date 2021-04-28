
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

#if WORKINPROGRESS

using System.Windows.Markup;

#if MIGRATION
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

#else
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Documents;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	[ContentProperty("Blocks")]
    [OpenSilver.NotImplemented]
	public partial class RichTextBox : Control
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="RichTextBox"/> class.
		/// </summary>
        [OpenSilver.NotImplemented]
		public RichTextBox()
        {
			this.DefaultStyleKey = typeof(RichTextBox);
        }

		#endregion Constructor

		#region Public Events

		/// <summary>
		/// Occurs when the content changes in a <see cref="RichTextBox"/>.
		/// </summary>
        [OpenSilver.NotImplemented]
		public event ContentChangedEventHandler ContentChanged;

		/// <summary>
		/// Occurs when the text selection has changed.
		/// </summary>
        [OpenSilver.NotImplemented]
		public event RoutedEventHandler SelectionChanged;

		#endregion Public Events

		#region Public Properties

		private ScrollBarVisibility _verticalScrollBarVisibility = ScrollBarVisibility.Auto;

		/// <summary>
		/// Gets or sets the visibility of the vertical scroll bar.
		/// The default is <see cref="ScrollBarVisibility.Auto"/>.
		/// </summary>
        [OpenSilver.NotImplemented]
		public ScrollBarVisibility VerticalScrollBarVisibility
		{
			get { return this._verticalScrollBarVisibility; }
			set { this._verticalScrollBarVisibility = value; }
		}


		private ScrollBarVisibility _horizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
		
		/// <summary>
		/// Gets or sets the visibility of the horizontal scroll bar.
		/// </summary>
		/// <returns>
		/// The visibility of the horizontal scroll bar. The default is <see cref="ScrollBarVisibility.Hidden"/>.
		/// </returns>
        [OpenSilver.NotImplemented]
		public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return this._horizontalScrollBarVisibility; }
            set { this._horizontalScrollBarVisibility = value; }
        }

		/// <summary>
		/// Gets or sets a XAML representation of the content in the <see cref="RichTextBox"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="String"/> object that is a XAML representation of the content in the <see cref="RichTextBox"/>.
		/// </returns>
        [OpenSilver.NotImplemented]
		public string Xaml
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a value that represents the offset in pixels from the top of the content
		/// to the baseline of the first paragraph. The baseline of the paragraph is the
		/// baseline of the first line in it.
		/// </summary>
		/// <returns>
		/// The computed baseline for the first paragraph, or 0 if the <see cref="RichTextBox"/>
		/// is empty.
		/// </returns>
        [OpenSilver.NotImplemented]
		public double BaselineOffset { get; }

		/// <summary>
		/// Gets the <see cref="TextSelection"/> in the <see cref="RichTextBox"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="TextSelection"/> that represents the selected text in
		/// the <see cref="RichTextBox"/>.
		/// </returns>
        [OpenSilver.NotImplemented]
		public TextSelection Selection { get; }

		/// <summary>
		/// Gets a <see cref="TextPointer"/> that indicates the end of content
		/// in the <see cref="RichTextBox"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="TextPointer"/> that indicates the end of content in the
		/// <see cref="RichTextBox"/>.
		/// </returns>
        [OpenSilver.NotImplemented]
		public TextPointer ContentEnd { get; }

		/// <summary>
		/// Gets a <see cref="TextPointer"/> that indicates the start of content
		/// in the <see cref="RichTextBox"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="TextPointer"/> that indicates the start of content in
		/// the <see cref="RichTextBox"/>.
		/// </returns>
        [OpenSilver.NotImplemented]
		public TextPointer ContentStart { get; }

		/// <summary>
		/// Gets the contents of the <see cref="RichTextBox"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="BlockCollection"/> that contains the contents of the
		/// <see cref="RichTextBox"/>.
		/// </returns>
        [OpenSilver.NotImplemented]
		public BlockCollection Blocks { get; }

		#region Dependency Properties

		/// <summary>
		/// Identifies the <see cref="RichTextBox.IsReadOnly"/> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty IsReadOnlyProperty =
			DependencyProperty.Register(
				"IsReadOnly",
				typeof(bool),
				typeof(RichTextBox),
				new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a value that determines whether the user can change the text in
		/// the <see cref="RichTextBox"/>.
		/// The default is false.
		/// </summary>
        [OpenSilver.NotImplemented]
		public bool IsReadOnly
		{
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="RichTextBox.LineHeight"/> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty LineHeightProperty =
			DependencyProperty.Register(
				"LineHeight",
				typeof(double),
				typeof(RichTextBox),
#if WORKINPROGRESS
				new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
#else
				new PropertyMetadata(0d));
#endif

		/// <summary>
		/// Gets or sets the height of each line of content.
		/// </summary>
		/// <returns>
		/// The height of each line in pixels. A value of 0 indicates that the line height
		/// is determined automatically from the current font characteristics. The default
		/// is 0.
		/// </returns>
        [OpenSilver.NotImplemented]
		public double LineHeight
		{
			get { return (double)this.GetValue(LineHeightProperty); }
			set { this.SetValue(LineHeightProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="RichTextBox.AcceptsReturn"/> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty AcceptsReturnProperty =
			DependencyProperty.Register(
				"AcceptsReturn",
				typeof(bool),
				typeof(RichTextBox),
				new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a value that determines whether the <see cref="RichTextBox"/>
		/// allows and displays the newline or return characters when the ENTER or RETURN
		/// keys are pressed.
		/// </summary>
		/// <returns>
		/// true if the <see cref="RichTextBox"/> allows newline characters; otherwise,
		/// false. The default is true.
		/// </returns>
        [OpenSilver.NotImplemented]
		public bool AcceptsReturn
		{
            get { return (bool)this.GetValue(AcceptsReturnProperty); }
            set { this.SetValue(AcceptsReturnProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="RichTextBox.CaretBrush"/> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty CaretBrushProperty =
			DependencyProperty.Register(
				"CaretBrush",
				typeof(Brush),
				typeof(RichTextBox),
				new PropertyMetadata((object)null));

		/// <summary>
		/// Gets or sets the brush that is used to render the vertical bar that indicates
		/// the insertion point.
		/// </summary>
		/// <returns>
		/// A brush that is used to render the vertical bar that indicates the insertion
		/// point.
		/// </returns>
        [OpenSilver.NotImplemented]
		public Brush CaretBrush
		{
            get { return (Brush)this.GetValue(CaretBrushProperty); }
            set { this.SetValue(CaretBrushProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="RichTextBox.LineStackingStrategy"/> dependency
		/// property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty LineStackingStrategyProperty =
			DependencyProperty.Register(
				"LineStackingStrategy",
				typeof(LineStackingStrategy),
				typeof(RichTextBox),
				new PropertyMetadata(LineStackingStrategy.MaxHeight));

		/// <summary>
		/// Gets or sets a value that indicates how a line box is determined for each line
		/// of text in the <see cref="RichTextBox"/>.
		/// </summary>
		/// <returns>
		/// A value that indicates how a line box is determined for each line of text in
		/// the <see cref="RichTextBox"/>. The default is <see cref="LineStackingStrategy.MaxHeight"/>.
		/// </returns>
        [OpenSilver.NotImplemented]
		public LineStackingStrategy LineStackingStrategy
		{
            get { return (LineStackingStrategy)this.GetValue(LineStackingStrategyProperty); }
            set { this.SetValue(LineStackingStrategyProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="RichTextBox.TextAlignment"/> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty TextAlignmentProperty =
			DependencyProperty.Register(
				"TextAlignment",
				typeof(TextAlignment),
				typeof(RichTextBox),
				new PropertyMetadata(TextAlignment.Left));

		/// <summary>
		/// Gets or sets how the text should be aligned in the <see cref="RichTextBox"/>.
		/// </summary>
		/// <returns>
		/// One of the <see cref="TextAlignment"/> enumeration values. The default is Left.
		/// </returns>
        [OpenSilver.NotImplemented]
		public TextAlignment TextAlignment
		{
            get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
            set { this.SetValue(TextAlignmentProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="RichTextBox.TextWrapping"/> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty TextWrappingProperty =
			DependencyProperty.Register(
				"TextWrapping",
				typeof(TextWrapping),
				typeof(RichTextBox),
				new PropertyMetadata(TextWrapping.Wrap));

		/// <summary>
		/// Gets or sets how text wrapping occurs if a line of text extends beyond the available
		/// width of the <see cref="RichTextBox"/>.
		/// </summary>
		/// <returns>
		/// One of the <see cref="TextWrapping"/> values. The default is <see cref="TextWrapping.Wrap"/>.
		/// </returns>
        [OpenSilver.NotImplemented]
		public TextWrapping TextWrapping
        {
            get { return (TextWrapping)this.GetValue(TextWrappingProperty); }
            set { this.SetValue(TextWrappingProperty, value); }
        }

		#endregion Dependency Properties

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Returns a <see cref="TextPointer"/> that indicates the closest insertion
		/// position for the specified point.
		/// </summary>
		/// <param name="point">
		/// A point in the coordinate space of the <see cref="RichTextBox"/> for
		/// which the closest insertion position is retrieved.
		/// </param>
		/// <returns>
		/// A <see cref="TextPointer"/> that indicates the closest insertion position
		/// for the specified point.
		/// </returns>
        [OpenSilver.NotImplemented]
		public TextPointer GetPositionFromPoint(Point point)
		{
			return null;
		}

		/// <summary>
		/// Selects the entire contents in the <see cref="RichTextBox"/>.
		/// </summary>
        [OpenSilver.NotImplemented]
		public void SelectAll()
		{
		}

		#endregion Public Methods

		#region Protected Methods

#if false
		/// <summary>
		/// Returns a <see cref="RichTextBoxAutomationPeer"/> for use by
		/// the Silverlight automation infrastructure.
		/// </summary>
		/// <returns>
		/// A <see cref="RichTextBoxAutomationPeer"/> for the <see cref="RichTextBox"/>
		/// object.
		/// </returns>
        [OpenSilver.NotImplemented]
		protected override AutomationPeer OnCreateAutomationPeer()
        {
			return null;
        }
#endif

		/// <summary>
		/// Called before the <see cref="UIElement.GotFocus"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
		protected override void OnGotFocus(RoutedEventArgs e)
        {
        }

		/// <summary>
		/// Called when the <see cref="UIElement.KeyDown"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
		protected override void OnKeyDown(KeyEventArgs e)
#else
		protected override void OnKeyDown(KeyRoutedEventArgs e)
#endif
		{
		}

		/// <summary>
		/// Called before the <see cref="UIElement.KeyUp"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
		protected override void OnKeyUp(KeyEventArgs e)
#else
		protected override void OnKeyUp(KeyRoutedEventArgs e)
#endif
		{
		}

		/// <summary>
		/// Called before the <see cref="UIElement.LostFocus"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
		protected override void OnLostFocus(RoutedEventArgs e)
        {
        }

		/// <summary>
		/// Provides handling for the <see cref="UIElement.LostMouseCapture"/> event.
		/// </summary>
		/// <param name="e">
		/// A <see cref="MouseEventArgs"/> that contains the event data.
		/// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
		protected override void OnLostMouseCapture(MouseEventArgs e)
#else
		protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
#endif
		{
		}

		/// <summary>
		/// Called before the <see cref="UIElement.MouseEnter"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
		protected override void OnMouseEnter(MouseEventArgs e)
#else
		protected override void OnPointerEntered(PointerRoutedEventArgs e)
#endif
		{
		}

		/// <summary>
		/// Called before the <see cref="UIElement.MouseLeave"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
		protected internal override void OnMouseLeave(MouseEventArgs e)
#else
		protected internal override void OnPointerExited(PointerRoutedEventArgs e)
#endif
		{
		}

		/// <summary>
		/// Called before the <see cref="UIElement.MouseLeftButtonDown"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
		protected override void OnPointerPressed(PointerRoutedEventArgs e)
#endif
		{
		}

		/// <summary>
		/// Called before the <see cref="UIElement.MouseLeftButtonUp"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
		protected override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
		{
		}

		/// <summary>
		/// Called before the <see cref="UIElement.MouseMove"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
		protected override void OnMouseMove(MouseEventArgs e)
#else
		protected override void OnPointerMoved(PointerRoutedEventArgs e)
#endif
		{
		}

		/// <summary>
		/// Called before the <see cref="UIElement.TextInput"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
		protected override void OnTextInput(TextCompositionEventArgs e)
        {
        }

		/// <summary>
		/// Called before the <see cref="UIElement.TextInputStart"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
		protected override void OnTextInputStart(TextCompositionEventArgs e)
        {
        }

		/// <summary>
		/// Called before the <see cref="UIElement.TextInputUpdate"/> event occurs.
		/// </summary>
		/// <param name="e">
		/// The data for the event.
		/// </param>
        [OpenSilver.NotImplemented]
		protected override void OnTextInputUpdate(TextCompositionEventArgs e)
        {
        }

#endregion Protected Methods
	}
}
#endif