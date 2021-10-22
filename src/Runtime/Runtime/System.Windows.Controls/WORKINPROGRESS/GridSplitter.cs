#if MIGRATION

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
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty PreviewStyleProperty =
			DependencyProperty.Register("PreviewStyle",
										typeof(Style),
										typeof(GridSplitter),
										null);

		//
		// Summary:
		//     Gets or sets the System.Windows.Style that is used for previewing changes.
		//
		// Returns:
		//     The style that is used to preview changes.
        [OpenSilver.NotImplemented]
		public Style PreviewStyle
		{
			get { return (Style)GetValue(PreviewStyleProperty); }
			set { SetValue(PreviewStyleProperty, value); }
		}
	}
}
#endif