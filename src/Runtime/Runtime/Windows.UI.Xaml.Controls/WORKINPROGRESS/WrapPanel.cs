#if WORKINPROGRESS

#if MIGRATION
using System.Collections.Generic;
using System.Linq;

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
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
	public static readonly DependencyProperty ItemHeightProperty =
		DependencyProperty.Register("ItemHeight",
										typeof(double),
										typeof(WrapPanel),
										new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
#else
	public static readonly DependencyProperty ItemHeightProperty =
			DependencyProperty.Register("ItemHeight",
										typeof(double),
										typeof(WrapPanel),
										new PropertyMetadata(double.NaN));
#endif

		//
		// Summary:
		//     Gets or sets a value that specifies the width of all items that are contained
		//     within a System.Windows.Controls.WrapPanel.
		//
		// Returns:
		//     A System.Double that represents the uniform width of all items that are contained
		//     within the System.Windows.Controls.WrapPanel. The default value is System.Double.NaN.
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
    public static readonly DependencyProperty ItemWidthProperty =
		    DependencyProperty.Register("ItemWidth",
										typeof(double),
										typeof(WrapPanel),
										new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
#else
	public static readonly DependencyProperty ItemWidthProperty =
			DependencyProperty.Register("ItemWidth",
										typeof(double),
										typeof(WrapPanel),
										new PropertyMetadata(double.NaN));
#endif
		private void MeasureChild(UIElement child, double availableMainLength, double availableCrossLength)
		{
			child.Measure(Orientation == Orientation.Horizontal ?
				new Size(availableMainLength, availableCrossLength) :
				new Size(availableCrossLength, availableMainLength));
		}
		private void ArrangeChild(UIElement child, double finalMainStart, double finalCrossStart, double finalMainLength, double finalCrossLength)
		{
			child.Arrange(Orientation == Orientation.Horizontal ?
				new Rect(finalMainStart, finalCrossStart, finalMainLength, finalCrossLength) :
				new Rect(finalCrossStart, finalMainStart, finalCrossLength, finalMainLength));
		}
		private double GetMainLength(Size size)
		{
			return Orientation == Orientation.Horizontal ? size.Width : size.Height;
		}

		private double GetCrossLength(Size size)
		{
			return Orientation == Orientation.Horizontal ? size.Height : size.Width;
		}
		// get groups of elements that should be arranged in the same row or column
		private IEnumerable<IEnumerable<UIElement>> GetElementGroups(Size size)
		{
			double mainLength = GetMainLength(size);

			List<IEnumerable<UIElement>> groups = new List<IEnumerable<UIElement>>();

			List<UIElement> currentGroup = new List<UIElement>();
			double currentGroupMainLength = 0;

			foreach (UIElement child in Children)
			{
				double childMainLength = GetMainLength(child.DesiredSize);

				if (currentGroupMainLength > 0 && currentGroupMainLength + childMainLength > mainLength)
				{
					groups.Add(currentGroup);

					// start a new group
					currentGroup = new List<UIElement>();
					currentGroupMainLength = 0;
				}

				currentGroupMainLength += childMainLength;
				currentGroup.Add(child);
			}

			groups.Add(currentGroup);
			return groups;
		}
		private Size CreateSize(double mainLength, double crossLength)
		{
			if (Orientation == Orientation.Horizontal)
			{
				return new Size(mainLength, crossLength);
			}

			return new Size(crossLength, mainLength);
		}
		protected override Size MeasureOverride(Size availableSize)
		{
			if (!this.Children.Any())
			{
				return Size.Zero;
			}

			foreach (UIElement child in Children)
			{
				MeasureChild(child, GetMainLength(availableSize), GetCrossLength(availableSize));
			}

			IEnumerable<IEnumerable<UIElement>> groups = GetElementGroups(availableSize);

			double mainLength = groups.Select(group => group.Select(child => GetMainLength(child.DesiredSize)).Sum()).Max();
			double crossLength = groups.Select(group => group.Select(child => GetCrossLength(child.DesiredSize)).Max()).Sum();

			return CreateSize(mainLength, crossLength);
		}
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (!this.Children.Any())
			{
				return Size.Zero;
			}

			IEnumerable<IEnumerable<UIElement>> groups = GetElementGroups(finalSize);

			double maxMainLength = 0;
			double totalCrossLength = 0;

			foreach (IEnumerable<UIElement> group in groups)
			{
				double groupMainLength = 0;
				double groupMaxCrossLength = group.Select(child => GetCrossLength(child.DesiredSize)).Max();

				foreach (UIElement child in group)
				{
					double childMainLength = GetMainLength(child.DesiredSize);

					ArrangeChild(child, groupMainLength, totalCrossLength, childMainLength, groupMaxCrossLength);

					groupMainLength += childMainLength;
				}

				maxMainLength = Math.Max(maxMainLength, groupMainLength);
				totalCrossLength += groupMaxCrossLength;
			}

			return CreateSize(maxMainLength, totalCrossLength);
		}
	}
}

#endif