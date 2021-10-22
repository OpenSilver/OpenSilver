using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    [OpenSilver.NotImplemented]
	public abstract partial class TileBrush : Brush
	{
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty AlignmentXProperty = DependencyProperty.Register(nameof(AlignmentX), typeof(AlignmentX), typeof(TileBrush), new PropertyMetadata());
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty AlignmentYProperty = DependencyProperty.Register(nameof(AlignmentY), typeof(AlignmentY), typeof(TileBrush), new PropertyMetadata());
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(TileBrush), new PropertyMetadata());
        [OpenSilver.NotImplemented]
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

        [OpenSilver.NotImplemented]
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

        [OpenSilver.NotImplemented]
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

        [OpenSilver.NotImplemented]
		protected TileBrush()
		{
		}
	}
}
