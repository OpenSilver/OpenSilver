#if WORKINPROGRESS
using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a perspective transform (a 3-D-like effect) on an object.
    /// </summary>
    public sealed partial class PlaneProjection : Projection
    {
        public PlaneProjection()
        {
        }

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the z-axis of rotation.
        /// </summary>
        /// <returns>
        /// The number of degrees to rotate the object around the z-axis of rotation. The default is 0.
        /// </returns>
        public double RotationZ
        {
            get { return (double)GetValue(RotationZProperty); }
            set { SetValue(RotationZProperty, value); }
        }

        public static readonly DependencyProperty RotationZProperty =
            DependencyProperty.Register("RotationZ", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0d));


        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the x-axis of rotation.
        /// </summary>
        /// <returns>
        /// The number of degrees to rotate the object around the x-axis of rotation. The default is 0.
        /// </returns>


        public double RotationX
        {
            get { return (double)GetValue(RotationXProperty); }
            set { SetValue(RotationXProperty, value); }
        }

        public static readonly DependencyProperty RotationXProperty =
            DependencyProperty.Register("RotationX", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0d));


        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the y-axis of rotation.
        /// </summary>
        /// <returns>
        /// The number of degrees to rotate the object around the y-axis of rotation. The default is 0.
        /// </returns>

        public double RotationY
        {
            get { return (double)GetValue(RotationYProperty); }
            set { SetValue(RotationYProperty, value); }
        }

        public static readonly DependencyProperty RotationYProperty =
            DependencyProperty.Register("RotationY", typeof(double), typeof(PlaneProjection), new PropertyMetadata(0d));
    }
}
#endif