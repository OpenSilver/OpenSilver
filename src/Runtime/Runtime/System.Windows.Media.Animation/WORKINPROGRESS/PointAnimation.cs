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
    [OpenSilver.NotImplemented]
	public sealed partial class PointAnimation : Timeline
	{
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty FromProperty = DependencyProperty.Register(nameof(From), typeof(Nullable<Point>), typeof(PointAnimation), new PropertyMetadata());
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ToProperty = DependencyProperty.Register(nameof(To), typeof(Nullable<Point>), typeof(PointAnimation), new PropertyMetadata());
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ByProperty = DependencyProperty.Register(nameof(By), typeof(Nullable<Point>), typeof(PointAnimation), new PropertyMetadata());
        [OpenSilver.NotImplemented]
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

        [OpenSilver.NotImplemented]
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

        [OpenSilver.NotImplemented]
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

        [OpenSilver.NotImplemented]
		public PointAnimation()
		{
		}
	}
}
