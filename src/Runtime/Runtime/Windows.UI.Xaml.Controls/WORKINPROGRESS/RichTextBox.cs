
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
#else
using Windows.Foundation;
using Windows.UI.Xaml.Documents;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	[ContentProperty("Blocks")]
	public partial class RichTextBox : Control
	{
		//
		// Summary:
		//     Identifies the System.Windows.Controls.RichTextBox.IsReadOnly dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.RichTextBox.IsReadOnly dependency
		//     property.
		public static readonly DependencyProperty IsReadOnlyProperty =
			DependencyProperty.Register("IsReadOnly",
										typeof(bool),
										typeof(RichTextBox),
										new PropertyMetadata(false));

		public string Xaml
		{
			get;
			set;
		}
		
		//
		// Summary:
		//     Occurs when the content changes in a System.Windows.Controls.RichTextBox.
		public event ContentChangedEventHandler ContentChanged;
		
		//
		// Summary:
		//     Gets or sets a value that determines whether the System.Windows.Controls.RichTextBox
		//     allows and displays the newline or return characters when the ENTER or RETURN
		//     keys are pressed.
		//
		// Returns:
		//     true if the System.Windows.Controls.RichTextBox allows newline characters; otherwise,
		//     false. The default is true.
		public bool AcceptsReturn { get; set; }
		
		//
		// Summary:
		//     Gets or sets how text wrapping occurs if a line of text extends beyond the available
		//     width of the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     One of the System.Windows.TextWrapping values. The default is System.Windows.TextWrapping.Wrap.
		public TextWrapping TextWrapping { get; set; }
		
		//
		// Summary:
		//     Gets the System.Windows.Documents.TextSelection in the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     A System.Windows.Documents.TextSelection that represents the selected text in
		//     the System.Windows.Controls.RichTextBox.
		public TextSelection Selection { get; }
		
		//
		// Summary:
		//     Gets a System.Windows.Documents.TextPointer that indicates the start of content
		//     in the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     A System.Windows.Documents.TextPointer that indicates the start of content in
		//     the System.Windows.Controls.RichTextBox.
		public TextPointer ContentStart { get; }
		
		//
		// Summary:
		//     Gets the contents of the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     A System.Windows.Documents.BlockCollection that contains the contents of the
		//     System.Windows.Controls.RichTextBox.
		public BlockCollection Blocks { get; }
		
		//
		// Summary:
		//     Returns a System.Windows.Documents.TextPointer that indicates the closest insertion
		//     position for the specified point.
		//
		// Parameters:
		//   point:
		//     A point in the coordinate space of the System.Windows.Controls.RichTextBox for
		//     which the closest insertion position is retrieved.
		//
		// Returns:
		//     A System.Windows.Documents.TextPointer that indicates the closest insertion position
		//     for the specified point.
		public TextPointer GetPositionFromPoint(Point point)
		{
			return default(TextPointer);
		}
		
		//
		// Summary:
		//     Gets or sets a value that determines whether the user can change the text in
		//     the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     true if the System.Windows.Controls.RichTextBox is read-only; otherwise, false.
		//     The default is false.
		public bool IsReadOnly
		{
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}

		private ScrollBarVisibility _verticalScrollBarVisibility = ScrollBarVisibility.Auto;

		/// <summary>
		/// Gets or sets the visibility of the vertical scroll bar.
		/// The default is <see cref="ScrollBarVisibility.Auto"/>.
		/// </summary>
		public ScrollBarVisibility VerticalScrollBarVisibility
		{
			get { return this._verticalScrollBarVisibility; }
			set { this._verticalScrollBarVisibility = value; }
		}

		/// <summary>
		/// Identifies the <see cref="RichTextBox.LineHeight"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LineHeightProperty =
			DependencyProperty.Register(
				"LineHeight",
				typeof(double),
				typeof(RichTextBox),
				new PropertyMetadata(0d));

		/// <summary>
		/// Gets or sets the height of each line of content.
		/// </summary>
		/// <returns>
		/// The height of each line in pixels. A value of 0 indicates that the line height
		/// is determined automatically from the current font characteristics. The default
		/// is 0.
		/// </returns>
		public double LineHeight
		{
			get { return (double)this.GetValue(LineHeightProperty); }
			set { this.SetValue(LineHeightProperty, value); }
		}
	}
}
#endif