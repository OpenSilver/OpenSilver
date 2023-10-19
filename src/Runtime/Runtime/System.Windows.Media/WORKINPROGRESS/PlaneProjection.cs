
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Windows.Media.Media3D;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a perspective transform (a 3-D-like effect) on an object.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class PlaneProjection : Projection
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaneProjection"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public PlaneProjection()
        {
        }

        #endregion Constructor

        #region Dependency Properties

        #region Rotation

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.RotationX" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RotationXProperty =
           DependencyProperty.Register(
               "RotationX",
               typeof(double),
               typeof(PlaneProjection),
               new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the x-axis of rotation.
        /// </summary>
        /// <returns>
        /// The number of degrees to rotate the object around the x-axis of rotation. The default is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double RotationX
        {
            get { return (double)GetValue(RotationXProperty); }
            set { SetValue(RotationXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.RotationY" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RotationYProperty =
            DependencyProperty.Register(
                "RotationY",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the y-axis of rotation.
        /// </summary>
        /// <returns>
        /// The number of degrees to rotate the object around the y-axis of rotation. The default is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double RotationY
        {
            get { return (double)GetValue(RotationYProperty); }
            set { SetValue(RotationYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.RotationZ" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RotationZProperty =
            DependencyProperty.Register(
                "RotationZ",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the z-axis of rotation.
        /// </summary>
        /// <returns>
        /// The number of degrees to rotate the object around the z-axis of rotation. The default is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double RotationZ
        {
            get { return (double)GetValue(RotationZProperty); }
            set { SetValue(RotationZProperty, value); }
        }

        #endregion Rotation

        #region CenterOfRotation

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.CenterOfRotationX" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CenterOfRotationXProperty =
            DependencyProperty.Register(
                "CenterOfRotationX",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0.5d));

        /// <summary>
        /// Gets or sets the x-coordinate of the center of rotation of the object you rotate. 
        /// </summary>
        /// <returns>
        /// The x-coordinate of the center of rotation of the object you rotate. Typical values 
        /// are between 0 and 1 with a value of 0 corresponding to one edge of the object and 1 
        /// to the opposite edge. Values outside this range are allowed and move the center of 
        /// rotation accordingly. 
        /// The default is 0.5 (the center of object). 
        /// </returns>
        [OpenSilver.NotImplemented]
        public double CenterOfRotationX
        {
            get { return (double)this.GetValue(CenterOfRotationXProperty); }
            set { this.SetValue(CenterOfRotationXProperty, value); }
        }


        /// <summary>
        /// Identifies the <see cref="PlaneProjection.CenterOfRotationY" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CenterOfRotationYProperty =
            DependencyProperty.Register(
                "CenterOfRotationY",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0.5d));

        /// <summary>
        /// Gets or sets the y-coordinate of the center of rotation of the object you rotate.
        /// </summary>
        /// <returns>
        /// The y-coordinate of the center of rotation of the object you rotate. Typical values 
        /// are between 0 and 1 with a value of 0 corresponding to one edge of the object and 1 
        /// to the opposite edge. Values outside this range are allowed and move the center of 
        /// rotation accordingly. 
        /// The default is 0.5 (the center of object).
        /// </returns>
        [OpenSilver.NotImplemented]
        public double CenterOfRotationY
        {
            get { return (double)this.GetValue(CenterOfRotationYProperty); }
            set { this.SetValue(CenterOfRotationYProperty, value); }
        }


        /// <summary>
        /// Identifies the <see cref="PlaneProjection.CenterOfRotationZ" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CenterOfRotationZProperty =
            DependencyProperty.Register(
                "CenterOfRotationZ",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the z-coordinate of the center of rotation of the object you rotate.
        /// </summary>
        /// <returns>
        /// The z-coordinate of the center of rotation of the object you rotate. 
        /// The default is 0. Values greater than 0 correspond to coordinates out from the plane 
        /// of the object, and negative values correspond to coordinates behind the plane of the 
        /// object.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double CenterOfRotationZ
        {
            get { return (double)this.GetValue(CenterOfRotationZProperty); }
            set { this.SetValue(CenterOfRotationZProperty, value); }
        }

        #endregion CenterOfRotation

        #region Local Offset

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.LocalOffsetX" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LocalOffsetXProperty =
            DependencyProperty.Register(
                "LocalOffsetX",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the distance the object is translated along the x-axis of the plane of the object. 
        /// </summary>
        [OpenSilver.NotImplemented]
        public double LocalOffsetX
        {
            get { return (double)GetValue(LocalOffsetXProperty); }
            set { SetValue(LocalOffsetXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.LocalOffsetY" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LocalOffsetYProperty =
             DependencyProperty.Register(
                "LocalOffsetY",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the distance the object is translated along the y-axis of the plane of the object.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double LocalOffsetY
        {
            get { return (double)GetValue(LocalOffsetYProperty); }
            set { SetValue(LocalOffsetYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.LocalOffsetZ" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LocalOffsetZProperty =
             DependencyProperty.Register(
                "LocalOffsetZ",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the distance the object is translated along the z-axis of the plane of the object.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double LocalOffsetZ
        {
            get { return (double)GetValue(LocalOffsetZProperty); }
            set { SetValue(LocalOffsetZProperty, value); }
        }

        #endregion Local Offset

        #region Global Offset

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.GlobalOffsetX" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty GlobalOffsetXProperty =
            DependencyProperty.Register(
                "GlobalOffsetX",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the distance the object is translated along the x-axis of the screen. 
        /// </summary>
        [OpenSilver.NotImplemented]
        public double GlobalOffsetX
        {
            get { return (double)GetValue(GlobalOffsetXProperty); }
            set { SetValue(GlobalOffsetXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.GlobalOffsetY" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty GlobalOffsetYProperty =
            DependencyProperty.Register(
                "GlobalOffsetY",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the distance the object is translated along the y-axis of the screen. 
        /// </summary>
        [OpenSilver.NotImplemented]
        public double GlobalOffsetY
        {
            get { return (double)GetValue(GlobalOffsetYProperty); }
            set { SetValue(GlobalOffsetYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PlaneProjection.GlobalOffsetZ" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty GlobalOffsetZProperty =
            DependencyProperty.Register(
                "GlobalOffsetZ",
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the distance the object is translated along the z-axis of the screen.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double GlobalOffsetZ
        {
            get { return (double)GetValue(GlobalOffsetZProperty); }
            set { SetValue(GlobalOffsetZProperty, value); }
        }

        #endregion Global Offset

        #region Projection Matrix

        /// <summary>
        ///  Identifies the <see cref="PlaneProjection.ProjectionMatrix" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ProjectionMatrixProperty =
            DependencyProperty.Register(
                "ProjectionMatrix",
                typeof(Matrix3D),
                typeof(PlaneProjection),
                new PropertyMetadata(Matrix3D.Identity));

        /// <summary>
        /// Gets the projection matrix on the <see cref="PlaneProjection" />.
        /// The default value is <see cref="Matrix3D.Identity"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Matrix3D ProjectionMatrix
        {
            get { return (Matrix3D)this.GetValue(ProjectionMatrixProperty); }
            private set { this.SetValue(ProjectionMatrixProperty, value); }
        }

        #endregion Projection Matrix

        #endregion Dependency Properties
    }
}
