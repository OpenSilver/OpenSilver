#if WORKINPROGRESS
#if MIGRATION

using System.Windows.Media;

namespace System.Windows.Controls
{
	[TemplatePart(Name = "Presenter", Type = typeof(ContentPresenter))]
	[TemplatePart(Name = "TransformRoot", Type = typeof(Grid))]
	public sealed partial class LayoutTransformer : ContentControl
	{
		public static readonly DependencyProperty LayoutTransformProperty;

		public LayoutTransformer()
		{
			
		}

		public Transform LayoutTransform { get; set; }

		public void ApplyLayoutTransform()
		{
			
		}

		public override void OnApplyTemplate()
		{
			
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			return default(Size);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return default(Size);
		}
	}
}

#endif
#endif