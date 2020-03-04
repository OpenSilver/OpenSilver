#if WORKINPROGRESS

using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
	public static partial class VisualTreeExtensions
	{
		public static Rect? GetBoundsRelativeTo(this FrameworkElement element, UIElement otherElement)
		{
			return default(Rect?);
		}
		
		public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject element)
		{
			return default(IEnumerable<DependencyObject>);
		}
		
		public static IEnumerable<DependencyObject> GetVisualAncestorsAndSelf(this DependencyObject element)
		{
			return default(IEnumerable<DependencyObject>);
		}

		public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject element)
		{
			return default(IEnumerable<DependencyObject>);
		}
		
		public static IEnumerable<DependencyObject> GetVisualChildrenAndSelf(this DependencyObject element)
		{
			return default(IEnumerable<DependencyObject>);
		}
		
		public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject element)
		{
			return default(IEnumerable<DependencyObject>);
		}
		
		public static IEnumerable<DependencyObject> GetVisualDescendantsAndSelf(this DependencyObject element)
		{
			return default(IEnumerable<DependencyObject>);
		}
		
		public static IEnumerable<DependencyObject> GetVisualSiblings(this DependencyObject element)
		{
			return default(IEnumerable<DependencyObject>);
		}
		
		public static IEnumerable<DependencyObject> GetVisualSiblingsAndSelf(this DependencyObject element)
		{
			return default(IEnumerable<DependencyObject>);
		}
		
		public static void InvokeOnLayoutUpdated(this FrameworkElement element, Action action)
		{
			
		}
	}
}

#endif