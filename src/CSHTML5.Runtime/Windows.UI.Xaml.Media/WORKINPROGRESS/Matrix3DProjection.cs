#if WORKINPROGRESS
using System.Windows;
using System;
using System.Windows.Media.Media3D;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	public sealed partial class Matrix3DProjection : Projection
	{
		public static readonly DependencyProperty ProjectionMatrixProperty = DependencyProperty.Register("ProjectionMatrixProperty", typeof(Matrix3D), typeof(Matrix3DProjection), new PropertyMetadata());
		public Matrix3D ProjectionMatrix
		{
			get
			{
				return (Matrix3D)this.GetValue(Matrix3DProjection.ProjectionMatrixProperty);
			}

			set
			{
				this.SetValue(Matrix3DProjection.ProjectionMatrixProperty, value);
			}
		}

		public Matrix3DProjection()
		{
		}
	}
}
#endif