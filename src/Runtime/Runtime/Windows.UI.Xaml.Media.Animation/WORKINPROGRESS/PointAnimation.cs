#if WORKINPROGRESS
using System.Windows;
using System;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
	public sealed partial class PointAnimation : Timeline
	{
		public static readonly DependencyProperty FromProperty = DependencyProperty.Register(nameof(From), typeof(Nullable<Point>), typeof(PointAnimation), new PropertyMetadata());
		public static readonly DependencyProperty ToProperty = DependencyProperty.Register(nameof(To), typeof(Nullable<Point>), typeof(PointAnimation), new PropertyMetadata());
		public static readonly DependencyProperty ByProperty = DependencyProperty.Register(nameof(By), typeof(Nullable<Point>), typeof(PointAnimation), new PropertyMetadata());
		public Nullable<Point> From
		{
			get
			{
				return (Nullable<Point>)this.GetValue(PointAnimation.FromProperty);
			}

			set
			{
				this.SetValue(PointAnimation.FromProperty, value);
			}
		}

		public Nullable<Point> To
		{
			get
			{
				return (Nullable<Point>)this.GetValue(PointAnimation.ToProperty);
			}

			set
			{
				this.SetValue(PointAnimation.ToProperty, value);
			}
		}

		public Nullable<Point> By
		{
			get
			{
				return (Nullable<Point>)this.GetValue(PointAnimation.ByProperty);
			}

			set
			{
				this.SetValue(PointAnimation.ByProperty, value);
			}
		}

		public PointAnimation()
		{
		}
	}
}
#endif