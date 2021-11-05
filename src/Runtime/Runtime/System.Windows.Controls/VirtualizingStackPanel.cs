using System;
using System.Collections.Specialized;
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
		private const double LineDelta = 14.7;
		private const double Wheelitude = 3;

		public static readonly DependencyProperty VirtualizationModeProperty = 
			DependencyProperty.RegisterAttached("VirtualizationMode", typeof(VirtualizationMode), typeof(VirtualizingStackPanel), new PropertyMetadata(VirtualizationMode.Recycling));
        
		public static readonly DependencyProperty IsVirtualizingProperty = 
			DependencyProperty.RegisterAttached("IsVirtualizing", typeof(bool), typeof(VirtualizingStackPanel), new PropertyMetadata(false));
        
		public static readonly DependencyProperty OrientationProperty = 
			DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VirtualizingStackPanel), new PropertyMetadata(Orientation.Vertical, VirtualizingStackPanel.OrientationChanged));
        
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

		public VirtualizingStackPanel()
		{
		}

		//
		// Attached Property Accessor Methods
		//
		public static bool GetIsVirtualizing(DependencyObject o)
		{
			if (o == null)
				throw new ArgumentNullException("o");

			return (bool)o.GetValue(VirtualizingStackPanel.IsVirtualizingProperty);
		}

		internal static void SetIsVirtualizing(DependencyObject o, bool value)
		{
			if (o == null)
				throw new ArgumentNullException("o");

			o.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, value);
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

		static void OrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((VirtualizingStackPanel)sender).InvalidateMeasure();
		}

		bool canHorizontallyScroll;
		bool canVerticallyScroll;

		//
		// Helper Methods
		//
		void RemoveUnusedContainers(int first, int count)
		{
			IRecyclingItemContainerGenerator generator = ItemContainerGenerator as IRecyclingItemContainerGenerator;
			ItemsControl owner = ItemsControl.GetItemsOwner(this);
			VirtualizationMode mode = GetVirtualizationMode(this);
			CleanUpVirtualizedItemEventArgs args;
			int last = first + count - 1;
			GeneratorPosition pos;
			int item;

			//Console.WriteLine ("VSP.RemoveUnusedContainers ({0}, {1});", first, count);
			pos = new GeneratorPosition(Children.Count - 1, 0);
			while (pos.Index >= 0)
			{
				item = generator.IndexFromGeneratorPosition(pos);

				if (item < first || item > last)
				{
					//Console.WriteLine ("\tRemoving item[{0}] (child #{1})", item, pos.Index);
					args = new CleanUpVirtualizedItemEventArgs(Children[pos.Index], owner.Items[item]);
					OnCleanUpVirtualizedItem(args);

					if (!args.Cancel)
					{
						RemoveInternalChildRange(pos.Index, 1);

						if (mode == VirtualizationMode.Recycling)
							generator.Recycle(pos, 1);
						else
							generator.Remove(pos, 1);
					}
				}

				pos.Index--;
			}
		}

		//
		// Method Overrides
		//
		protected override Size MeasureOverride(Size constraint)
		{
			ItemsControl owner = ItemsControl.GetItemsOwner(this);
			Size measured = new Size(0, 0);
			bool invalidate = false;
			int nvisible = 0;
			int beyond = 0;
			int index;

			if (Orientation == Orientation.Horizontal)
				index = (int)HorizontalOffset;
			else
				index = (int)VerticalOffset;

			// Ensure we always touch ItemContainerGenerator as by accessing this property
			// we hook up to some events on it.
			IItemContainerGenerator generator = ItemContainerGenerator;
			if (owner.Items.Count > 0)
			{
				GeneratorPosition start;
				int insertAt;

				Size childAvailable = constraint;
				if (CanHorizontallyScroll || Orientation == Orientation.Horizontal)
					childAvailable.Width = double.PositiveInfinity;
				if (CanVerticallyScroll || Orientation == Orientation.Vertical)
					childAvailable.Height = double.PositiveInfinity;

				// Next, prepare and measure the extents of our viewable items...
				start = generator.GeneratorPositionFromIndex(index);
				insertAt = (start.Offset == 0) ? start.Index : start.Index + 1;

				using (generator.StartAt(start, GeneratorDirection.Forward, true))
				{
					bool isNewlyRealized;

					for (int i = index; i < owner.Items.Count && beyond < 2; i++, insertAt++)
					{
						// Generate the child container
						UIElement child = (UIElement)generator.GenerateNext(out isNewlyRealized);
						if (isNewlyRealized || insertAt >= Children.Count || Children[insertAt] != child)
						{
							// Add newly created children to the panel
							if (insertAt < Children.Count)
							{
								InsertInternalChild(insertAt, child);
							}
							else
							{
								AddInternalChild(child);
							}

							generator.PrepareItemContainer(child);
						}

						// Call Measure() on the child to both force layout and also so
						// that we can figure out when to stop adding children (e.g. when
						// we go beyond the viewable area)
						child.Measure(childAvailable);
						Size size = child.DesiredSize;
						nvisible++;

						if (Orientation == Orientation.Vertical)
						{
							measured.Width = Math.Max(measured.Width, size.Width);
							measured.Height += size.Height;

							if (measured.Height > constraint.Height)
								beyond++;
						}
						else
						{
							measured.Height = Math.Max(measured.Height, size.Height);
							measured.Width += size.Width;

							if (measured.Width > constraint.Width)
								beyond++;
						}
					}
				}
			}

			VirtualizingStackPanel.SetIsVirtualizing(owner, true);
			// FIXME: this if-check is a workaround for a bug
			// exposed by NBC Olympics but should not normally be
			// here.
			if (nvisible > 0)
				RemoveUnusedContainers(index, nvisible);

			nvisible -= beyond;

			// Update our Extent and Viewport values
			if (Orientation == Orientation.Vertical)
			{
				if (ExtentHeight != owner.Items.Count)
				{
					ExtentHeight = owner.Items.Count;
					invalidate = true;
				}

				if (ExtentWidth != measured.Width)
				{
					ExtentWidth = measured.Width;
					invalidate = true;
				}

				if (ViewportHeight != nvisible)
				{
					ViewportHeight = nvisible;
					invalidate = true;
				}

				if (ViewportWidth != constraint.Width)
				{
					ViewportWidth = constraint.Width;
					invalidate = true;
				}
			}
			else
			{
				if (ExtentHeight != measured.Height)
				{
					ExtentHeight = measured.Height;
					invalidate = true;
				}

				if (ExtentWidth != owner.Items.Count)
				{
					ExtentWidth = owner.Items.Count;
					invalidate = true;
				}

				if (ViewportHeight != constraint.Height)
				{
					ViewportHeight = constraint.Height;
					invalidate = true;
				}

				if (ViewportWidth != nvisible)
				{
					ViewportWidth = nvisible;
					invalidate = true;
				}
			}

			if (invalidate && ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();

			return measured;
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			Size arranged = arrangeSize;

			if (Orientation == Orientation.Vertical)
				arranged.Height = 0;
			else
				arranged.Width = 0;

			// Arrange our children
			foreach (UIElement child in Children)
			{
				Size size = child.DesiredSize;
				if (Orientation == Orientation.Vertical)
				{
					size.Width = arrangeSize.Width;
					Rect childFinal = new Rect(-HorizontalOffset, arranged.Height, size.Width, size.Height);

					if (childFinal.IsEmpty)
						child.Arrange(new Rect());
					else
						child.Arrange(childFinal);

					arranged.Width = Math.Max(arranged.Width, size.Width);
					arranged.Height += size.Height;
				}
				else
				{
					size.Height = arrangeSize.Height;
					Rect childFinal = new Rect(arranged.Width, -VerticalOffset, size.Width, size.Height);

					if (childFinal.IsEmpty)
						child.Arrange(new Rect());
					else
						child.Arrange(childFinal);

					arranged.Width += size.Width;
					arranged.Height = Math.Max(arranged.Height, size.Height);
				}
			}

			if (Orientation == Orientation.Vertical)
				arranged.Height = Math.Max(arranged.Height, arrangeSize.Height);
			else
				arranged.Width = Math.Max(arranged.Width, arrangeSize.Width);

			return arranged;
		}

		protected override void OnClearChildren()
		{
			base.OnClearChildren();

			HorizontalOffset = 0;
			VerticalOffset = 0;

			InvalidateMeasure();

			if (ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();
		}

		protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
		{
			base.OnItemsChanged(sender, args);
			IItemContainerGenerator generator = ItemContainerGenerator;
			ItemsControl owner = ItemsControl.GetItemsOwner(this);
			int index, offset, viewable;

			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					// The following logic is meant to keep the current viewable items in view
					// after adjusting for added items.
					index = generator.IndexFromGeneratorPosition(args.Position);
					if (Orientation == Orientation.Horizontal)
						offset = (int)HorizontalOffset;
					else
						offset = (int)VerticalOffset;

					if (index <= offset)
					{
						// items have been added earlier in the list than what is viewable
						offset += args.ItemCount;
					}

					if (Orientation == Orientation.Horizontal)
						SetHorizontalOffset(offset);
					else
						SetVerticalOffset(offset);
					break;
				case NotifyCollectionChangedAction.Remove:
					// The following logic is meant to keep the current viewable items in view
					// after adjusting for removed items.
					index = generator.IndexFromGeneratorPosition(args.Position);
					if (Orientation == Orientation.Horizontal)
					{
						offset = (int)HorizontalOffset;
						viewable = (int)ViewportWidth;
					}
					else
					{
						viewable = (int)ViewportHeight;
						offset = (int)VerticalOffset;
					}

					if (index < offset)
					{
						// items earlier in the list than what is viewable have been removed
						offset = Math.Max(offset - args.ItemCount, 0);
					}

					// adjust for items removed in the current view and/or beyond the current view
					offset = Math.Min(offset, owner.Items.Count - viewable);
					offset = Math.Max(offset, 0);

					if (Orientation == Orientation.Horizontal)
						SetHorizontalOffset(offset);
					else
						SetVerticalOffset(offset);

					RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
					break;
				case NotifyCollectionChangedAction.Replace:
					RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
					break;
				case NotifyCollectionChangedAction.Reset:
					// DO NOTHING
					break;
			}

			InvalidateMeasure();

			if (ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();
		}

		//
		// Methods
		//
		protected virtual void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
		{
			CleanUpVirtualizedItemEventHandler handler = CleanUpVirtualizedItemEvent;

			if (handler != null)
				handler(this, e);
		}

		//
		// Events
		//
		public event CleanUpVirtualizedItemEventHandler CleanUpVirtualizedItemEvent;

		#region "IScrollInfo"
		public bool CanHorizontallyScroll
		{
			get { return canHorizontallyScroll; }
			set
			{
				canHorizontallyScroll = value;
				InvalidateMeasure();
			}
		}

		public bool CanVerticallyScroll
		{
			get { return canVerticallyScroll; }
			set
			{
				canVerticallyScroll = value;
				InvalidateMeasure();
			}
		}

		public double ExtentWidth
		{
			get; private set;
		}

		public double ExtentHeight
		{
			get; private set;
		}

		public double HorizontalOffset
		{
			get; private set;
		}

		public double VerticalOffset
		{
			get; private set;
		}

		public double ViewportWidth
		{
			get; private set;
		}

		public double ViewportHeight
		{
			get; private set;
		}

		public ScrollViewer ScrollOwner { get; set; }

		//
		// Note: When scrolling along the stacking orientation,
		// Silverlight will perform logical scrolling. That
		// is, to say, it will scroll by items and not pixels.
		//

		public virtual void LineDown()
		{
			if (Orientation == Orientation.Horizontal)
				SetVerticalOffset(VerticalOffset + LineDelta);
			else
				SetVerticalOffset(VerticalOffset + 1);
		}

		public virtual void LineLeft()
		{
			if (Orientation == Orientation.Vertical)
				SetHorizontalOffset(HorizontalOffset - LineDelta);
			else
				SetHorizontalOffset(HorizontalOffset - 1);
		}

		public virtual void LineRight()
		{
			if (Orientation == Orientation.Vertical)
				SetHorizontalOffset(HorizontalOffset + LineDelta);
			else
				SetHorizontalOffset(HorizontalOffset + 1);
		}

		public virtual void LineUp()
		{
			if (Orientation == Orientation.Horizontal)
				SetVerticalOffset(VerticalOffset - LineDelta);
			else
				SetVerticalOffset(VerticalOffset - 1);
		}

		public Rect MakeVisible(UIElement visual, Rect rectangle)
		{
			Rect exposed = new Rect(0, 0, 0, 0);

			foreach (UIElement child in Children)
			{
				if (child == visual)
				{
					if (Orientation == Orientation.Vertical)
					{
						if (rectangle.X != HorizontalOffset)
							SetHorizontalOffset(rectangle.X);

						exposed.Width = Math.Min(child.RenderSize.Width, ViewportWidth);
						exposed.Height = child.RenderSize.Height;
						exposed.X = HorizontalOffset;
					}
					else
					{
						if (rectangle.Y != VerticalOffset)
							SetVerticalOffset(rectangle.Y);

						exposed.Height = Math.Min(child.RenderSize.Height, ViewportHeight);
						exposed.Width = child.RenderSize.Width;
						exposed.Y = VerticalOffset;
					}

					return exposed;
				}

				if (Orientation == Orientation.Vertical)
					exposed.Y += child.RenderSize.Height;
				else
					exposed.X += child.RenderSize.Width;
			}

			throw new ArgumentException("Visual is not a child of this Panel");
		}

		public virtual void MouseWheelDown()
		{
			if (Orientation == Orientation.Horizontal)
				SetVerticalOffset(VerticalOffset + LineDelta * Wheelitude);
			else
				SetVerticalOffset(VerticalOffset + Wheelitude);
		}

		public virtual void MouseWheelLeft()
		{
			if (Orientation == Orientation.Vertical)
				SetHorizontalOffset(HorizontalOffset - LineDelta * Wheelitude);
			else
				SetHorizontalOffset(HorizontalOffset - Wheelitude);
		}

		public virtual void MouseWheelRight()
		{
			if (Orientation == Orientation.Vertical)
				SetHorizontalOffset(HorizontalOffset + LineDelta * Wheelitude);
			else
				SetHorizontalOffset(HorizontalOffset + Wheelitude);
		}

		public virtual void MouseWheelUp()
		{
			if (Orientation == Orientation.Horizontal)
				SetVerticalOffset(VerticalOffset - LineDelta * Wheelitude);
			else
				SetVerticalOffset(VerticalOffset - Wheelitude);
		}

		public virtual void PageDown()
		{
			SetVerticalOffset(VerticalOffset + ViewportHeight);
		}

		public virtual void PageLeft()
		{
			SetHorizontalOffset(HorizontalOffset - ViewportWidth);
		}

		public virtual void PageRight()
		{
			SetHorizontalOffset(HorizontalOffset + ViewportWidth);
		}

		public virtual void PageUp()
		{
			SetVerticalOffset(VerticalOffset - ViewportHeight);
		}

		public void SetHorizontalOffset(double offset)
		{
			if (offset < 0 || ViewportWidth >= ExtentWidth)
				offset = 0;
			else if (offset + ViewportWidth >= ExtentWidth)
				offset = ExtentWidth - ViewportWidth;

			if (HorizontalOffset == offset)
				return;

			HorizontalOffset = offset;

			if (Orientation == Orientation.Horizontal)
				InvalidateMeasure();
			else
				InvalidateArrange();

			if (ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();
		}

		public void SetVerticalOffset(double offset)
		{
			if (offset < 0 || ViewportHeight >= ExtentHeight)
				offset = 0;
			else if (offset + ViewportHeight >= ExtentHeight)
				offset = ExtentHeight - ViewportHeight;

			if (VerticalOffset == offset)
				return;

			VerticalOffset = offset;

			if (Orientation == Orientation.Vertical)
				InvalidateMeasure();
			else
				InvalidateArrange();

			if (ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();
		}
		#endregion "IScrollInfo"
	}
}
