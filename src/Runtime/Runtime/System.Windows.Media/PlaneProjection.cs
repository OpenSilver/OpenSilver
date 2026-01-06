
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

using System.Globalization;
using System.Windows.Media.Media3D;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a perspective transform (a 3-D-like effect) on an object.
    /// </summary>
    public sealed class PlaneProjection : Projection
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaneProjection"/> class.
        /// </summary>
        public PlaneProjection()
        {
        }

        #endregion Constructor

        #region Dependency Properties

        #region Rotation

        /// <summary>
        /// Identifies the <see cref="RotationX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationXProperty =
           DependencyProperty.Register(
               nameof(RotationX),
               typeof(double),
               typeof(PlaneProjection),
               new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the x-axis of rotation.
        /// </summary>
        /// <returns>
        /// The number of degrees to rotate the object around the x-axis of rotation. The default is 0.
        /// </returns>
        public double RotationX
        {
            get { return (double)GetValue(RotationXProperty); }
            set { SetValueInternal(RotationXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RotationY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationYProperty =
            DependencyProperty.Register(
                nameof(RotationY),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the y-axis of rotation.
        /// </summary>
        /// <returns>
        /// The number of degrees to rotate the object around the y-axis of rotation. The default is 0.
        /// </returns>
        public double RotationY
        {
            get { return (double)GetValue(RotationYProperty); }
            set { SetValueInternal(RotationYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RotationZ"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationZProperty =
            DependencyProperty.Register(
                nameof(RotationZ),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the number of degrees to rotate the object around the z-axis of rotation.
        /// </summary>
        /// <returns>
        /// The number of degrees to rotate the object around the z-axis of rotation. The default is 0.
        /// </returns>
        public double RotationZ
        {
            get { return (double)GetValue(RotationZProperty); }
            set { SetValueInternal(RotationZProperty, value); }
        }

        #endregion Rotation

        #region CenterOfRotation

        /// <summary>
        /// Identifies the <see cref="CenterOfRotationX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterOfRotationXProperty =
            DependencyProperty.Register(
                nameof(CenterOfRotationX),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0.5d, OnProjectionPropertyChanged));

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
        public double CenterOfRotationX
        {
            get { return (double)GetValue(CenterOfRotationXProperty); }
            set { SetValueInternal(CenterOfRotationXProperty, value); }
        }


        /// <summary>
        /// Identifies the <see cref="CenterOfRotationY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterOfRotationYProperty =
            DependencyProperty.Register(
                nameof(CenterOfRotationY),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0.5d, OnProjectionPropertyChanged));

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
        public double CenterOfRotationY
        {
            get { return (double)GetValue(CenterOfRotationYProperty); }
            set { SetValueInternal(CenterOfRotationYProperty, value); }
        }


        /// <summary>
        /// Identifies the <see cref="CenterOfRotationZ"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterOfRotationZProperty =
            DependencyProperty.Register(
                nameof(CenterOfRotationZ),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the z-coordinate of the center of rotation of the object you rotate.
        /// </summary>
        /// <returns>
        /// The z-coordinate of the center of rotation of the object you rotate. 
        /// The default is 0. Values greater than 0 correspond to coordinates out from the plane 
        /// of the object, and negative values correspond to coordinates behind the plane of the 
        /// object.
        /// </returns>
        public double CenterOfRotationZ
        {
            get { return (double)GetValue(CenterOfRotationZProperty); }
            set { SetValueInternal(CenterOfRotationZProperty, value); }
        }

        #endregion CenterOfRotation

        #region Local Offset

        /// <summary>
        /// Identifies the <see cref="LocalOffsetX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocalOffsetXProperty =
            DependencyProperty.Register(
                nameof(LocalOffsetX),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the distance the object is translated along the x-axis of the plane of the object. 
        /// </summary>
        public double LocalOffsetX
        {
            get { return (double)GetValue(LocalOffsetXProperty); }
            set { SetValueInternal(LocalOffsetXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LocalOffsetY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocalOffsetYProperty =
             DependencyProperty.Register(
                nameof(LocalOffsetY),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the distance the object is translated along the y-axis of the plane of the object.
        /// </summary>
        public double LocalOffsetY
        {
            get { return (double)GetValue(LocalOffsetYProperty); }
            set { SetValueInternal(LocalOffsetYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LocalOffsetZ"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocalOffsetZProperty =
             DependencyProperty.Register(
                nameof(LocalOffsetZ),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the distance the object is translated along the z-axis of the plane of the object.
        /// </summary>
        public double LocalOffsetZ
        {
            get { return (double)GetValue(LocalOffsetZProperty); }
            set { SetValueInternal(LocalOffsetZProperty, value); }
        }

        #endregion Local Offset

        #region Global Offset

        /// <summary>
        /// Identifies the <see cref="GlobalOffsetX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GlobalOffsetXProperty =
            DependencyProperty.Register(
                nameof(GlobalOffsetX),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the distance the object is translated along the x-axis of the screen. 
        /// </summary>
        public double GlobalOffsetX
        {
            get { return (double)GetValue(GlobalOffsetXProperty); }
            set { SetValueInternal(GlobalOffsetXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="GlobalOffsetY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GlobalOffsetYProperty =
            DependencyProperty.Register(
                nameof(GlobalOffsetY),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the distance the object is translated along the y-axis of the screen. 
        /// </summary>
        public double GlobalOffsetY
        {
            get { return (double)GetValue(GlobalOffsetYProperty); }
            set { SetValueInternal(GlobalOffsetYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="GlobalOffsetZ"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GlobalOffsetZProperty =
            DependencyProperty.Register(
                nameof(GlobalOffsetZ),
                typeof(double),
                typeof(PlaneProjection),
                new PropertyMetadata(0d, OnProjectionPropertyChanged));

        /// <summary>
        /// Gets or sets the distance the object is translated along the z-axis of the screen.
        /// </summary>
        public double GlobalOffsetZ
        {
            get { return (double)GetValue(GlobalOffsetZProperty); }
            set { SetValueInternal(GlobalOffsetZProperty, value); }
        }

        #endregion Global Offset

        #region Projection Matrix

        private static readonly DependencyPropertyKey ProjectionMatrixPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ProjectionMatrix),
                typeof(Matrix3D),
                typeof(PlaneProjection),
                new PropertyMetadata(Matrix3D.Identity));

        /// <summary>
        ///  Identifies the <see cref="ProjectionMatrix"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProjectionMatrixProperty = ProjectionMatrixPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the projection matrix on the <see cref="PlaneProjection"/>.
        /// The default value is <see cref="Matrix3D.Identity"/>.
        /// </summary>
        public Matrix3D ProjectionMatrix
        {
            get { return (Matrix3D)GetValue(ProjectionMatrixProperty); }
            private set { SetValueInternal(ProjectionMatrixPropertyKey, value); }
        }

        #endregion Projection Matrix

        #endregion Dependency Properties

        private static void OnProjectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlaneProjection projection = (PlaneProjection)d;
            projection.UpdateProjectionMatrix();
            projection.RaiseChanged();
        }

        private void UpdateProjectionMatrix()
        {
            ProjectionMatrix = CalculateProjectionMatrix();
        }

        private Matrix3D CalculateProjectionMatrix()
        {
            double radX = RotationX * Math.PI / 180.0;
            double radY = RotationY * Math.PI / 180.0;
            double radZ = RotationZ * Math.PI / 180.0;

            double cosX = Math.Cos(radX);
            double sinX = Math.Sin(radX);
            double cosY = Math.Cos(radY);
            double sinY = Math.Sin(radY);
            double cosZ = Math.Cos(radZ);
            double sinZ = Math.Sin(radZ);

            Matrix3D rotX = new Matrix3D(
                1, 0, 0, 0,
                0, cosX, sinX, 0,
                0, -sinX, cosX, 0,
                0, 0, 0, 1);

            Matrix3D rotY = new Matrix3D(
                cosY, 0, -sinY, 0,
                0, 1, 0, 0,
                sinY, 0, cosY, 0,
                0, 0, 0, 1);

            Matrix3D rotZ = new Matrix3D(
                cosZ, sinZ, 0, 0,
                -sinZ, cosZ, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            Matrix3D rotation = rotZ * rotY * rotX;

            Matrix3D localOffset = new Matrix3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                LocalOffsetX, LocalOffsetY, LocalOffsetZ, 1);

            Matrix3D result = rotation * localOffset;

            result.OffsetX += GlobalOffsetX;
            result.OffsetY += GlobalOffsetY;
            result.OffsetZ += GlobalOffsetZ;

            return result;
        }

        internal override string GetCssTransform(double elementWidth, double elementHeight)
        {
            if (IsIdentity())
            {
                return string.Empty;
            }

            double centerX = elementWidth * CenterOfRotationX;
            double centerY = elementHeight * CenterOfRotationY;
            double centerZ = CenterOfRotationZ;

            var transforms = new System.Text.StringBuilder();

            if (GlobalOffsetX != 0 || GlobalOffsetY != 0 || GlobalOffsetZ != 0)
            {
                transforms.Append(string.Format(CultureInfo.InvariantCulture,
                    "translate3d({0}px,{1}px,{2}px) ",
                    Math.Round(GlobalOffsetX, 4),
                    Math.Round(GlobalOffsetY, 4),
                    Math.Round(GlobalOffsetZ, 4)));
            }

            if (centerX != 0 || centerY != 0 || centerZ != 0)
            {
                transforms.Append(string.Format(CultureInfo.InvariantCulture,
                    "translate3d({0}px,{1}px,{2}px) ",
                    Math.Round(centerX, 4),
                    Math.Round(centerY, 4),
                    Math.Round(centerZ, 4)));
            }

            if (LocalOffsetX != 0 || LocalOffsetY != 0 || LocalOffsetZ != 0)
            {
                transforms.Append(string.Format(CultureInfo.InvariantCulture,
                    "translate3d({0}px,{1}px,{2}px) ",
                    Math.Round(LocalOffsetX, 4),
                    Math.Round(LocalOffsetY, 4),
                    Math.Round(LocalOffsetZ, 4)));
            }

            if (RotationX != 0)
            {
                transforms.Append(string.Format(CultureInfo.InvariantCulture,
                    "rotateX({0}deg) ", Math.Round(RotationX, 4)));
            }

            if (RotationY != 0)
            {
                transforms.Append(string.Format(CultureInfo.InvariantCulture,
                    "rotateY({0}deg) ", Math.Round(RotationY, 4)));
            }

            if (RotationZ != 0)
            {
                transforms.Append(string.Format(CultureInfo.InvariantCulture,
                    "rotateZ({0}deg) ", Math.Round(RotationZ, 4)));
            }

            if (centerX != 0 || centerY != 0 || centerZ != 0)
            {
                transforms.Append(string.Format(CultureInfo.InvariantCulture,
                    "translate3d({0}px,{1}px,{2}px)",
                    Math.Round(-centerX, 4),
                    Math.Round(-centerY, 4),
                    Math.Round(-centerZ, 4)));
            }

            return transforms.ToString().Trim();
        }

        private bool IsIdentity()
        {
            return RotationX == 0 &&
                   RotationY == 0 &&
                   RotationZ == 0 &&
                   LocalOffsetX == 0 &&
                   LocalOffsetY == 0 &&
                   LocalOffsetZ == 0 &&
                   GlobalOffsetX == 0 &&
                   GlobalOffsetY == 0 &&
                   GlobalOffsetZ == 0;
        }
    }
}

