#if WORKINPROGRESS
using System;
#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public partial class VirtualizingStackPanel : VirtualizingPanel, IScrollInfo
	{
		public static readonly DependencyProperty VirtualizationModeProperty = DependencyProperty.Register("VirtualizationMode", typeof(VirtualizationMode), typeof(VirtualizingStackPanel), null);
		public static readonly DependencyProperty IsVirtualizingProperty = DependencyProperty.Register("IsVirtualizing", typeof(bool), typeof(VirtualizingStackPanel), null);
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VirtualizingStackPanel), new PropertyMetadata());
		public Orientation Orientation
		{
			get
			{
				return (Orientation)this.GetValue(VirtualizingStackPanel.OrientationProperty);
			}

			set
			{
				this.SetValue(VirtualizingStackPanel.OrientationProperty, value);
			}
		}

		public ScrollViewer ScrollOwner
		{
			get;
			set;
		}

		public VirtualizingStackPanel()
		{
		}

		public Rect MakeVisible(UIElement visual, Rect rectangle)
		{
			throw new NotImplementedException();
		}

		// Summary:
		//     Returns the System.Windows.Controls.VirtualizationMode for the specified object.
		//
		// Parameters:
		//   element:
		//     The object from which the System.Windows.Controls.VirtualizationMode is read.
		//
		// Returns:
		//     One of the enumeration values that specifies whether the object uses container
		//     recycling.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     element is null.
		public static VirtualizationMode GetVirtualizationMode(DependencyObject element)
		{
			return (VirtualizationMode)element.GetValue(VirtualizationModeProperty);
		}

		//
		// Summary:
		//     Sets the System.Windows.Controls.VirtualizationMode on the specified object.
		//
		// Parameters:
		//   element:
		//     The element on which to set the System.Windows.Controls.VirtualizationMode.
		//
		//   value:
		//     One of the enumeration values that specifies whether element uses container recycling.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     element is null.
		public static void SetVirtualizationMode(DependencyObject element, VirtualizationMode value)
		{
			element.SetValue(VirtualizationModeProperty, value);
		}
	}
}
#endif