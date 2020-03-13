#if WORKINPROGRESS

namespace System.Windows.Controls
{
	public partial class GridSplitter
	{
		//
		// Summary:
		//     Identifies the System.Windows.Controls.GridSplitter.PreviewStyle dependency property.
		//
		// Returns:
		//     An identifier for the System.Windows.Controls.GridSplitter.PreviewStyle dependency
		//     property.
		public static readonly DependencyProperty PreviewStyleProperty;
		
		//
		// Summary:
		//     Gets or sets the System.Windows.Style that is used for previewing changes.
		//
		// Returns:
		//     The style that is used to preview changes.
		public Style PreviewStyle { get; set; }
	}
}

#endif