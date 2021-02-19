#if WORKINPROGRESS
using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	public abstract partial class TileBrush : Brush
	{
		public static readonly DependencyProperty AlignmentXProperty = DependencyProperty.Register(nameof(AlignmentX), typeof(AlignmentX), typeof(TileBrush), new PropertyMetadata());
		public static readonly DependencyProperty AlignmentYProperty = DependencyProperty.Register(nameof(AlignmentY), typeof(AlignmentY), typeof(TileBrush), new PropertyMetadata());
		public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(TileBrush), new PropertyMetadata());
		public AlignmentX AlignmentX
		{
			get
			{
				return (AlignmentX)this.GetValue(TileBrush.AlignmentXProperty);
			}

			set
			{
				this.SetValue(TileBrush.AlignmentXProperty, value);
			}
		}

		public AlignmentY AlignmentY
		{
			get
			{
				return (AlignmentY)this.GetValue(TileBrush.AlignmentYProperty);
			}

			set
			{
				this.SetValue(TileBrush.AlignmentYProperty, value);
			}
		}

		public Stretch Stretch
		{
			get
			{
				return (Stretch)this.GetValue(TileBrush.StretchProperty);
			}

			set
			{
				this.SetValue(TileBrush.StretchProperty, value);
			}
		}

		protected TileBrush()
		{
		}
	}
}
#endif