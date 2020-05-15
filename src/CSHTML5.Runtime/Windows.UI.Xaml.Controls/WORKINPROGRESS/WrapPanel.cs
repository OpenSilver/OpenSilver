#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public partial class WrapPanel
	{
		// Summary:
		//     Gets or sets a value that specifies the height of all items that are contained
		//     within a System.Windows.Controls.WrapPanel.
		//
		// Returns:
		//     The System.Double that represents the uniform height of all items that are
		//     contained within the System.Windows.Controls.WrapPanel. The default value
		//     is System.Double.NaN.
		public double ItemHeight
		{
			get { return (double)GetValue(ItemHeightProperty); }
			set { SetValue(ItemHeightProperty, value); }
		}
		// Summary:
		//     Identifies the System.Windows.Controls.WrapPanel.ItemHeight  dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.WrapPanel.ItemHeight  dependency
		//     property.
		public static readonly DependencyProperty ItemHeightProperty =
			DependencyProperty.Register("ItemHeight",
										typeof(double),
										typeof(WrapPanel),
										new PropertyMetadata(double.NaN));

		//
		// Summary:
		//     Gets or sets a value that specifies the width of all items that are contained
		//     within a System.Windows.Controls.WrapPanel.
		//
		// Returns:
		//     A System.Double that represents the uniform width of all items that are contained
		//     within the System.Windows.Controls.WrapPanel. The default value is System.Double.NaN.
		public double ItemWidth
		{
			get { return (double)GetValue(ItemWidthProperty); }
			set { SetValue(ItemWidthProperty, value); }
		}
		//
		// Summary:
		//     Identifies the System.Windows.Controls.WrapPanel.ItemWidth  dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.WrapPanel.ItemWidth  dependency
		//     property.
		public static readonly DependencyProperty ItemWidthProperty =
			DependencyProperty.Register("ItemWidth",
										typeof(double),
										typeof(WrapPanel),
										new PropertyMetadata(double.NaN));
	}
}

#endif