using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>
    /// Control that implements support for transformations as if applied by
    /// LayoutTransform (which does not exist in Silverlight).
    /// </summary>
    [System.Windows.Markup.ContentProperty("Child")]
    internal class LayoutTransformControl : Control
    {
        /// <summary>Identifies the ContentProperty.</summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Child), typeof(FrameworkElement), typeof(LayoutTransformControl), new PropertyMetadata(new PropertyChangedCallback(LayoutTransformControl.ChildChanged)));
        /// <summary>Identifies the TransformProperty dependency property.</summary>
        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register(nameof(Transform), typeof(Transform), typeof(LayoutTransformControl), new PropertyMetadata(new PropertyChangedCallback(LayoutTransformControl.TransformChanged)));
        /// <summary>Actual DesiredSize of Child element.</summary>
        private Size _childActualSize = Size.Empty;
        /// <summary>
        /// Value used to work around double arithmetic rounding issues in
        /// Silverlight.
        /// </summary>
        private const double AcceptableDelta = 0.0001;
        /// <summary>
        /// Value used to work around double arithmetic rounding issues in
        /// Silverlight.
        /// </summary>
        private const int DecimalsAfterRound = 4;
        /// <summary>Host panel for Child element.</summary>
        private Panel _layoutRoot;
        /// <summary>
        /// RenderTransform/MatrixTransform applied to layout root.
        /// </summary>
        private MatrixTransform _matrixTransform;
        /// <summary>
        /// Transformation matrix corresponding to matrix transform.
        /// </summary>
        private Matrix _transformation;

        /// <summary>
        /// Gets or sets the single child of the LayoutTransformControl.
        /// </summary>
        /// <remarks>
        /// Corresponds to Windows Presentation Foundation's Decorator.Child
        /// property.
        /// </remarks>
        public FrameworkElement Child
        {
            get
            {
                return (FrameworkElement)this.GetValue(LayoutTransformControl.ContentProperty);
            }
            set
            {
                this.SetValue(LayoutTransformControl.ContentProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the Transform of the LayoutTransformControl.
        /// </summary>
        /// <remarks>Corresponds to UIElement.RenderTransform.</remarks>
        public Transform Transform
        {
            get
            {
                return (Transform)this.GetValue(LayoutTransformControl.TransformProperty);
            }
            set
            {
                this.SetValue(LayoutTransformControl.TransformProperty, (object)value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the LayoutTransformControl class.
        /// </summary>
        public LayoutTransformControl()
        {
            this.IsTabStop = false;
            this.UseLayoutRounding = false;
            // Moved the template to generic.xaml
            //this.Template = (ControlTemplate)XamlReader.Load("<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><Grid x:Name='LayoutRoot' Background='{TemplateBinding Background}'><Grid.RenderTransform><MatrixTransform x:Name='MatrixTransform'/></Grid.RenderTransform></Grid></ControlTemplate>");
            DefaultStyleKey = typeof(LayoutTransformControl);
        }

        /// <summary>Called whenever the control's template changes.</summary>
        public override void OnApplyTemplate()
        {
            FrameworkElement child = this.Child;
            this.Child = (FrameworkElement)null;
            base.OnApplyTemplate();
            this._layoutRoot = this.GetTemplateChild("LayoutRoot") as Panel;
            this._matrixTransform = this.GetTemplateChild("MatrixTransform") as MatrixTransform;
            this.Child = child;
            this.TransformUpdated();
        }

        /// <summary>Handle changes to the child dependency property.</summary>
        /// <param name="o">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private static void ChildChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutTransformControl)o).OnChildChanged((FrameworkElement)e.NewValue);
        }

        /// <summary>Updates content when the child property is changed.</summary>
        /// <param name="newContent">The new child.</param>
        private void OnChildChanged(FrameworkElement newContent)
        {
            if (null == this._layoutRoot)
                return;
            this._layoutRoot.Children.Clear();
            if (null != newContent)
                this._layoutRoot.Children.Add((UIElement)newContent);
            this.InvalidateMeasure();
        }

        /// <summary>Handles changes to the Transform DependencyProperty.</summary>
        /// <param name="o">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private static void TransformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutTransformControl)o).OnTransformChanged((Transform)e.NewValue);
        }

        /// <summary>
        /// Processes the transform when the transform is changed.
        /// </summary>
        /// <param name="newValue">The transform to process.</param>
        private void OnTransformChanged(Transform newValue)
        {
            this.ProcessTransform(newValue);
        }

        /// <summary>
        /// Notifies the LayoutTransformControl that some aspect of its
        /// Transform property has changed.
        /// </summary>
        /// <remarks>
        /// Call this to update the LayoutTransform in cases where
        /// LayoutTransformControl wouldn't otherwise know to do so.
        /// </remarks>
        public void TransformUpdated()
        {
            this.ProcessTransform(this.Transform);
        }

        /// <summary>
        /// Processes the current transform to determine the corresponding
        /// matrix.
        /// </summary>
        /// <param name="transform">The transform to use to determine the
        /// matrix.</param>
        private void ProcessTransform(Transform transform)
        {
            this._transformation = LayoutTransformControl.RoundMatrix(this.GetTransformMatrix(transform), 4);
            if (null != this._matrixTransform)
                this._matrixTransform.Matrix = this._transformation;
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Walks the Transform and returns the corresponding matrix.
        /// </summary>
        /// <param name="transform">The transform to create a matrix for.</param>
        /// <returns>The matrix calculated from the transform.</returns>
        private Matrix GetTransformMatrix(Transform transform)
        {
            if (null != transform)
            {
                TransformGroup transformGroup = transform as TransformGroup;
                if (null != transformGroup)
                {
                    Matrix matrix1 = Matrix.Identity;
                    foreach (Transform child in (PresentationFrameworkCollection<Transform>)transformGroup.Children)
                        matrix1 = LayoutTransformControl.MatrixMultiply(matrix1, this.GetTransformMatrix(child));
                    return matrix1;
                }
                RotateTransform rotateTransform = transform as RotateTransform;
                if (null != rotateTransform)
                {
                    double num1 = 2.0 * Math.PI * rotateTransform.Angle / 360.0;
                    double m12 = Math.Sin(num1);
                    double num2 = Math.Cos(num1);
                    return new Matrix(num2, m12, -m12, num2, 0.0, 0.0);
                }
                ScaleTransform scaleTransform = transform as ScaleTransform;
                if (null != scaleTransform)
                    return new Matrix(scaleTransform.ScaleX, 0.0, 0.0, scaleTransform.ScaleY, 0.0, 0.0);
                SkewTransform skewTransform = transform as SkewTransform;
                if (null != skewTransform)
                {
                    double angleX = skewTransform.AngleX;
                    return new Matrix(1.0, 2.0 * Math.PI * skewTransform.AngleY / 360.0, 2.0 * Math.PI * angleX / 360.0, 1.0, 0.0, 0.0);
                }
                MatrixTransform matrixTransform = transform as MatrixTransform;
                if (null != matrixTransform)
                    return matrixTransform.Matrix;
            }
            return Matrix.Identity;
        }

        /// <summary>
        /// Provides the behavior for the "Measure" pass of layout.
        /// </summary>
        /// <param name="availableSize">The available size that this element can
        /// give to child elements. Infinity can be specified as a value to
        /// indicate that the element will size to whatever content is
        /// available.</param>
        /// <returns>The size that this element determines it needs during
        /// layout, based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this._layoutRoot == null || null == this.Child)
                return Size.Empty;
            this._layoutRoot.Measure(!(this._childActualSize == Size.Empty) ? this._childActualSize : this.ComputeLargestTransformedSize(availableSize));
            double x = 0.0;
            double y = 0.0;
            Size desiredSize = this._layoutRoot.DesiredSize;
            double width = desiredSize.Width;
            desiredSize = this._layoutRoot.DesiredSize;
            double height = desiredSize.Height;
            Rect rect = LayoutTransformControl.RectTransform(new Rect(x, y, width, height), this._transformation);
            return new Size(rect.Width, rect.Height);
        }

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of layout.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this
        /// element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        /// <remarks>
        /// Using the WPF paramater name finalSize instead of Silverlight's
        /// finalSize for clarity.
        /// </remarks>
        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement child = this.Child;
            if (this._layoutRoot == null || null == child)
                return finalSize;
            Size a = this.ComputeLargestTransformedSize(finalSize);
            if (LayoutTransformControl.IsSizeSmaller(a, this._layoutRoot.DesiredSize))
                a = this._layoutRoot.DesiredSize;
            Rect rect = LayoutTransformControl.RectTransform(new Rect(0.0, 0.0, a.Width, a.Height), this._transformation);
            this._layoutRoot.Arrange(new Rect(-rect.Left + (finalSize.Width - rect.Width) / 2.0, -rect.Top + (finalSize.Height - rect.Height) / 2.0, a.Width, a.Height));
            if (LayoutTransformControl.IsSizeSmaller(a, child.RenderSize) && Size.Empty == this._childActualSize)
            {
                this._childActualSize = new Size(child.ActualWidth, child.ActualHeight);
                this.InvalidateMeasure();
            }
            else
                this._childActualSize = Size.Empty;
            return finalSize;
        }

        /// <summary>
        /// Computes the largest usable size after applying the transformation
        /// to the specified bounds.
        /// </summary>
        /// <param name="arrangeBounds">The size to arrange within.</param>
        /// <returns>The size required.</returns>
        private Size ComputeLargestTransformedSize(Size arrangeBounds)
        {
            Size size = Size.Empty;
            bool flag1 = double.IsInfinity(arrangeBounds.Width);
            if (flag1)
                arrangeBounds.Width = arrangeBounds.Height;
            bool flag2 = double.IsInfinity(arrangeBounds.Height);
            if (flag2)
                arrangeBounds.Height = arrangeBounds.Width;
            double m11 = this._transformation.M11;
            double m12 = this._transformation.M12;
            double m21 = this._transformation.M21;
            double m22 = this._transformation.M22;
            double num1 = Math.Abs(arrangeBounds.Width / m11);
            double num2 = Math.Abs(arrangeBounds.Width / m21);
            double num3 = Math.Abs(arrangeBounds.Height / m12);
            double num4 = Math.Abs(arrangeBounds.Height / m22);
            double num5 = num1 / 2.0;
            double num6 = num2 / 2.0;
            double num7 = num3 / 2.0;
            double num8 = num4 / 2.0;
            double num9 = -(num2 / num1);
            double num10 = -(num4 / num3);
            if (0.0 == arrangeBounds.Width || 0.0 == arrangeBounds.Height)
                size = new Size(0.0, 0.0);
            else if (flag1 && flag2)
                size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            else if (!LayoutTransformControl.MatrixHasInverse(this._transformation))
                size = new Size(0.0, 0.0);
            else if (0.0 == m12 || 0.0 == m21)
            {
                double num11 = flag2 ? double.PositiveInfinity : num4;
                double num12 = flag1 ? double.PositiveInfinity : num1;
                if (0.0 == m12 && 0.0 == m21)
                    size = new Size(num12, num11);
                else if (0.0 == m12)
                {
                    double height = Math.Min(num6, num11);
                    size = new Size(num12 - Math.Abs(m21 * height / m11), height);
                }
                else if (0.0 == m21)
                {
                    double width = Math.Min(num7, num12);
                    size = new Size(width, num11 - Math.Abs(m12 * width / m22));
                }
            }
            else if (0.0 == m11 || 0.0 == m22)
            {
                double num11 = flag2 ? double.PositiveInfinity : num3;
                double num12 = flag1 ? double.PositiveInfinity : num2;
                if (0.0 == m11 && 0.0 == m22)
                    size = new Size(num11, num12);
                else if (0.0 == m11)
                {
                    double height = Math.Min(num8, num12);
                    size = new Size(num11 - Math.Abs(m22 * height / m12), height);
                }
                else if (0.0 == m22)
                {
                    double width = Math.Min(num5, num11);
                    size = new Size(width, num12 - Math.Abs(m11 * width / m21));
                }
            }
            else if (num6 <= num10 * num5 + num4)
                size = new Size(num5, num6);
            else if (num8 <= num9 * num7 + num2)
            {
                size = new Size(num7, num8);
            }
            else
            {
                double width = (num4 - num2) / (num9 - num10);
                size = new Size(width, num9 * width + num2);
            }
            return size;
        }

        /// <summary>
        /// Return true if Size a is smaller than Size b in either dimension.
        /// </summary>
        /// <param name="a">The left size.</param>
        /// <param name="b">The right size.</param>
        /// <returns>A value indicating whether the left size is smaller than
        /// the right.</returns>
        private static bool IsSizeSmaller(Size a, Size b)
        {
            return a.Width + 0.0001 < b.Width || a.Height + 0.0001 < b.Height;
        }

        /// <summary>
        /// Rounds the non-offset elements of a matrix to avoid issues due to
        /// floating point imprecision.
        /// </summary>
        /// <param name="matrix">The matrix to round.</param>
        /// <param name="decimalsAfterRound">The number of decimals after the
        /// round.</param>
        /// <returns>The rounded matrix.</returns>
        private static Matrix RoundMatrix(Matrix matrix, int decimalsAfterRound)
        {
            return new Matrix(Math.Round(matrix.M11, decimalsAfterRound), Math.Round(matrix.M12, decimalsAfterRound), Math.Round(matrix.M21, decimalsAfterRound), Math.Round(matrix.M22, decimalsAfterRound), matrix.OffsetX, matrix.OffsetY);
        }

        /// <summary>
        /// Implement Windows Presentation Foundation's Rect.Transform on
        /// Silverlight.
        /// </summary>
        /// <param name="rectangle">The rectangle to transform.</param>
        /// <param name="matrix">The matrix to use to transform the rectangle.</param>
        /// <returns>The transformed rectangle.</returns>
        private static Rect RectTransform(Rect rectangle, Matrix matrix)
        {
            Point point1 = matrix.Transform(new Point(rectangle.Left, rectangle.Top));
            Point point2 = matrix.Transform(new Point(rectangle.Right, rectangle.Top));
            Point point3 = matrix.Transform(new Point(rectangle.Left, rectangle.Bottom));
            Point point4 = matrix.Transform(new Point(rectangle.Right, rectangle.Bottom));
            double x = Math.Min(Math.Min(point1.X, point2.X), Math.Min(point3.X, point4.X));
            double y = Math.Min(Math.Min(point1.Y, point2.Y), Math.Min(point3.Y, point4.Y));
            double num1 = Math.Max(Math.Max(point1.X, point2.X), Math.Max(point3.X, point4.X));
            double num2 = Math.Max(Math.Max(point1.Y, point2.Y), Math.Max(point3.Y, point4.Y));
            return new Rect(x, y, num1 - x, num2 - y);
        }

        /// <summary>
        /// Implements Windows Presentation Foundation's Matrix.Multiply on
        /// Silverlight.
        /// </summary>
        /// <param name="matrix1">The left matrix.</param>
        /// <param name="matrix2">The right matrix.</param>
        /// <returns>The product of the two matrices.</returns>
        private static Matrix MatrixMultiply(Matrix matrix1, Matrix matrix2)
        {
            return new Matrix(matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21, matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22, matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21, matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22, matrix1.OffsetX * matrix2.M11 + matrix1.OffsetY * matrix2.M21 + matrix2.OffsetX, matrix1.OffsetX * matrix2.M12 + matrix1.OffsetY * matrix2.M22 + matrix2.OffsetY);
        }

        /// <summary>
        /// Implements Windows Presentation Foundation's Matrix.HasInverse on
        /// Silverlight.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>True if matrix has an inverse.</returns>
        private static bool MatrixHasInverse(Matrix matrix)
        {
            return 0.0 != matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
        }
    }
}
