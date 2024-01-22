using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace System.Windows.Media
{
	/// <summary>
	///  Enables you to apply a <see cref="Matrix3D"/> to an object.
	/// </summary>
	[ContentProperty(nameof(ProjectionMatrix))]
    [OpenSilver.NotImplemented]
	public sealed class Matrix3DProjection : Projection
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="Matrix3DProjection"/> class.
		/// </summary>
        [OpenSilver.NotImplemented]
		public Matrix3DProjection()
		{
		}

		/// <summary>
		/// Identifies the <see cref="Matrix3DProjection.ProjectionMatrix"/> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ProjectionMatrixProperty =
			DependencyProperty.Register(
				"ProjectionMatrix", 
				typeof(Matrix3D), 
				typeof(Matrix3DProjection), 
				new PropertyMetadata(Matrix3D.Identity));

		/// <summary>
		/// Gets or sets the <see cref="Matrix3D"/> that is used for the projection
		/// that is applied to the object.
		/// </summary>
        [OpenSilver.NotImplemented]
		public Matrix3D ProjectionMatrix
		{
			get { return (Matrix3D)GetValue(ProjectionMatrixProperty); }
			set { SetValueInternal(ProjectionMatrixProperty, value); }
		}
	}
}
