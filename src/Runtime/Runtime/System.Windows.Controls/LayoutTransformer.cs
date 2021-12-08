﻿//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if MIGRATION
using System.Windows.Media;
#else
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    [TemplatePart(Name = "Presenter", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "TransformRoot", Type = typeof(Grid))]
    public class LayoutTransformer : ContentControl
    {
        /// <summary>
        /// Name of the TransformRoot template part.
        /// </summary>
        private const string TransformRootName = "TransformRoot";

        /// <summary>
        /// Name of the Presenter template part.
        /// </summary>
        private const string PresenterName = "Presenter";


        /// <summary>
        /// Gets or sets the layout transform to apply on the LayoutTransformer 
        /// control content.
        /// </summary>
        /// <remarks>
        /// Corresponds to UIElement.LayoutTransform.
        /// </remarks>
        public Transform LayoutTransform
        {
            get { return (Transform)GetValue(LayoutTransformProperty); }
            set { SetValue(LayoutTransformProperty, value); }
        }

        /// <summary>
        /// Identifies the LayoutTransform DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty LayoutTransformProperty = DependencyProperty.Register(
            "LayoutTransform", typeof(Transform), typeof(LayoutTransformer), new PropertyMetadata(LayoutTransformChanged));

        /// <summary>
        /// Gets the child element being transformed.
        /// </summary>
        private FrameworkElement Child
        {
            get
            {
                // Preferred child is the content; fall back to the presenter itself
                return (null != _contentPresenter) ?
                    (_contentPresenter.Content as FrameworkElement ?? _contentPresenter) :
                    null;
            }
        }

        // Note: AcceptableDelta and DecimalsAfterRound work around double arithmetic rounding issues on Silverlight.

        /// <summary>
        /// Acceptable difference between two doubles.
        /// </summary>
        private const double AcceptableDelta = 0.0001;

        /// <summary>
        /// Number of decimals to round the Matrix to.
        /// </summary>
        private const int DecimalsAfterRound = 4;

        /// <summary>
        /// Root element for performing transformations.
        /// </summary>
        private Panel _transformRoot;

        /// <summary>
        /// ContentPresenter element for displaying the content.
        /// </summary>
        private ContentPresenter _contentPresenter;

        /// <summary>
        /// RenderTransform/MatrixTransform applied to _transformRoot.
        /// </summary>
        private MatrixTransform _matrixTransform;

        /// <summary>
        /// Transformation matrix corresponding to _matrixTransform.
        /// </summary>
        private Matrix _transformation;

        /// <summary>
        /// Actual DesiredSize of Child element (the value it returned from its MeasureOverride method).
        /// </summary>
        private Size _childActualSize = Size.Empty;

        /// <summary>
        /// Initializes a new instance of the LayoutTransformer class.
        /// </summary>
        public LayoutTransformer()
        {
            // Associated default style
            DefaultStyleKey = typeof(LayoutTransformer);

            // Can't tab to LayoutTransformer
            IsTabStop = false;
#if MIGRATION
            // Disable layout rounding because its rounding of values confuses things
            UseLayoutRounding = false;
#endif
        }

        /// <summary>
        /// Builds the visual tree for the LayoutTransformer control when a new 
        /// template is applied.
        /// </summary>
#if MIGRATION
        public
#else
        protected
#endif
         override void OnApplyTemplate()
        {
            // Apply new template
            base.OnApplyTemplate();
            // Find template parts
            _transformRoot = GetTemplateChild(TransformRootName) as Grid;
            _contentPresenter = GetTemplateChild(PresenterName) as ContentPresenter;
            _matrixTransform = new MatrixTransform();
            if (null != _transformRoot)
            {
                _transformRoot.RenderTransform = _matrixTransform;
            }
            // Apply the current transform
            ApplyLayoutTransform();
        }

        /// <summary>
        /// Handles changes to the Transform DependencyProperty.
        /// </summary>
        /// <param name="o">Source of the change.</param>
        /// <param name="e">Event args.</param>
        private static void LayoutTransformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // Casts are safe because Silverlight is enforcing the types
            ((LayoutTransformer)o).ProcessTransform((Transform)e.NewValue);
        }

        /// <summary>
        /// Applies the layout transform on the LayoutTransformer control content.
        /// </summary>
        /// <remarks>
        /// Only used in advanced scenarios (like animating the LayoutTransform). 
        /// Should be used to notify the LayoutTransformer control that some aspect 
        /// of its Transform property has changed. 
        /// </remarks>
        public void ApplyLayoutTransform()
        {
            ProcessTransform(LayoutTransform);
        }

        /// <summary>
        /// Processes the Transform to determine the corresponding Matrix.
        /// </summary>
        /// <param name="transform">Transform to process.</param>
        private void ProcessTransform(Transform transform)
        {
            // Get the transform matrix and apply it
            _transformation = RoundMatrix(GetTransformMatrix(transform), DecimalsAfterRound);
            if (null != _matrixTransform)
            {
                _matrixTransform.Matrix = _transformation;
            }
            // New transform means re-layout is necessary
            InvalidateMeasure();
        }

        /// <summary>
        /// Walks the Transform(Group) and returns the corresponding Matrix.
        /// </summary>
        /// <param name="transform">Transform(Group) to walk.</param>
        /// <returns>Computed Matrix.</returns>
        private Matrix GetTransformMatrix(Transform transform)
        {
            if (null != transform)
            {
                // WPF equivalent of this entire method:
                // return transform.Value;

                // Process the TransformGroup
                TransformGroup transformGroup = transform as TransformGroup;
                if (null != transformGroup)
                {
                    Matrix groupMatrix = Matrix.Identity;
                    foreach (Transform child in transformGroup.Children)
                    {
                        groupMatrix = MatrixMultiply(groupMatrix, GetTransformMatrix(child));
                    }
                    return groupMatrix;
                }

                // Process the RotateTransform
                RotateTransform rotateTransform = transform as RotateTransform;
                if (null != rotateTransform)
                {
                    double angle = rotateTransform.Angle;
                    double angleRadians = (2 * Math.PI * angle) / 360;
                    double sine = Math.Sin(angleRadians);
                    double cosine = Math.Cos(angleRadians);
                    return new Matrix(cosine, sine, -sine, cosine, 0, 0);
                }

                // Process the ScaleTransform
                ScaleTransform scaleTransform = transform as ScaleTransform;
                if (null != scaleTransform)
                {
                    double scaleX = scaleTransform.ScaleX;
                    double scaleY = scaleTransform.ScaleY;
                    return new Matrix(scaleX, 0, 0, scaleY, 0, 0);
                }

                // Process the SkewTransform
                SkewTransform skewTransform = transform as SkewTransform;
                if (null != skewTransform)
                {
                    double angleX = skewTransform.AngleX;
                    double angleY = skewTransform.AngleY;
                    double angleXRadians = (2 * Math.PI * angleX) / 360;
                    double angleYRadians = (2 * Math.PI * angleY) / 360;
                    return new Matrix(1, angleYRadians, angleXRadians, 1, 0, 0);
                }

                // Process the MatrixTransform
                MatrixTransform matrixTransform = transform as MatrixTransform;
                if (null != matrixTransform)
                {
                    return matrixTransform.Matrix;
                }

                // TranslateTransform has no effect in LayoutTransform
            }

            // Fall back to no-op transformation
            return Matrix.Identity;
        }

        /// <summary>
        /// Provides the behavior for the "Measure" pass of layout.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            FrameworkElement child = Child;
            if ((null == _transformRoot) || (null == child))
            {
                // No content, no size
                return Size.Empty;
            }

            Size measureSize;
            if (_childActualSize == Size.Empty)
            {
                // Determine the largest size after the transformation
                measureSize = ComputeLargestTransformedSize(availableSize);
            }
            else
            {
                // Previous measure/arrange pass determined that Child.DesiredSize was larger than believed
                measureSize = _childActualSize;
            }

            // Perform a mesaure on the _transformRoot (containing Child)
            _transformRoot.Measure(measureSize);

            // WPF equivalent of _childActualSize technique (much simpler, but doesn't work on Silverlight 2)
            // // If the child is going to render larger than the available size, re-measure according to that size
            // child.Arrange(new Rect());
            // if (child.RenderSize != child.DesiredSize)
            // {
            //     _transformRoot.Measure(child.RenderSize);
            // }

            // Transform DesiredSize to find its width/height
            Rect transformedDesiredRect = RectTransform(new Rect(0, 0, _transformRoot.DesiredSize.Width, _transformRoot.DesiredSize.Height), _transformation);
            Size transformedDesiredSize = new Size(transformedDesiredRect.Width, transformedDesiredRect.Height);

            // Return result to allocate enough space for the transformation
            return transformedDesiredSize;
        }

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of layout.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        /// <remarks>
        /// Using the WPF paramater name finalSize instead of Silverlight's finalSize for clarity
        /// </remarks>
        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement child = Child;
            if ((null == _transformRoot) || (null == child))
            {
                // No child, use whatever was given
                return finalSize;
            }

            // Determine the largest available size after the transformation
            Size finalSizeTransformed = ComputeLargestTransformedSize(finalSize);
            if (IsSizeSmaller(finalSizeTransformed, _transformRoot.DesiredSize))
            {
                // Some elements do not like being given less space than they asked for (ex: TextBlock)
                // Bump the working size up to do the right thing by them
                finalSizeTransformed = _transformRoot.DesiredSize;
            }

            // Transform the working size to find its width/height
            Rect transformedRect = RectTransform(new Rect(0, 0, finalSizeTransformed.Width, finalSizeTransformed.Height), _transformation);
            // Create the Arrange rect to center the transformed content
            Rect finalRect = new Rect(
                -transformedRect.Left + ((finalSize.Width - transformedRect.Width) / 2),
                -transformedRect.Top + ((finalSize.Height - transformedRect.Height) / 2),
                finalSizeTransformed.Width,
                finalSizeTransformed.Height);

            // Perform an Arrange on _transformRoot (containing Child)
            _transformRoot.Arrange(finalRect);

            // This is the first opportunity under Silverlight to find out the Child's true DesiredSize
            if (IsSizeSmaller(finalSizeTransformed, child.RenderSize) && (Size.Empty == _childActualSize))
            {
                // Unfortunately, all the work so far is invalid because the wrong DesiredSize was used
                // Make a note of the actual DesiredSize
                _childActualSize = new Size(child.ActualWidth, child.ActualHeight);
                // Force a new measure/arrange pass
                InvalidateMeasure();
            }
            else
            {
                // Clear the "need to measure/arrange again" flag
                _childActualSize = Size.Empty;
            }

            return finalSize;
        }

        /// <summary>
        /// Compute the largest usable size (greatest area) after applying the transformation to the specified bounds.
        /// </summary>
        /// <param name="arrangeBounds">Arrange bounds.</param>
        /// <returns>Largest Size possible.</returns>
        private Size ComputeLargestTransformedSize(Size arrangeBounds)
        {

            // Computed largest transformed size
            Size computedSize = Size.Empty;

            // Detect infinite bounds and constrain the scenario
            bool infiniteWidth = double.IsInfinity(arrangeBounds.Width);
            if (infiniteWidth)
            {
                arrangeBounds.Width = arrangeBounds.Height;
            }
            bool infiniteHeight = double.IsInfinity(arrangeBounds.Height);
            if (infiniteHeight)
            {
                arrangeBounds.Height = arrangeBounds.Width;
            }

            // Capture the matrix parameters
            double a = _transformation.M11;
            double b = _transformation.M12;
            double c = _transformation.M21;
            double d = _transformation.M22;

            // Compute maximum possible transformed width/height based on starting width/height
            // These constraints define two lines in the positive x/y quadrant
            double maxWidthFromWidth = Math.Abs(arrangeBounds.Width / a);
            double maxHeightFromWidth = Math.Abs(arrangeBounds.Width / c);
            double maxWidthFromHeight = Math.Abs(arrangeBounds.Height / b);
            double maxHeightFromHeight = Math.Abs(arrangeBounds.Height / d);

            // The transformed width/height that maximize the area under each segment is its midpoint
            // At most one of the two midpoints will satisfy both constraints
            double idealWidthFromWidth = maxWidthFromWidth / 2;
            double idealHeightFromWidth = maxHeightFromWidth / 2;
            double idealWidthFromHeight = maxWidthFromHeight / 2;
            double idealHeightFromHeight = maxHeightFromHeight / 2;

            // Compute slope of both constraint lines
            double slopeFromWidth = -(maxHeightFromWidth / maxWidthFromWidth);
            double slopeFromHeight = -(maxHeightFromHeight / maxWidthFromHeight);

            if ((0 == arrangeBounds.Width) || (0 == arrangeBounds.Height))
            {
                // Check for empty bounds
                computedSize = new Size(arrangeBounds.Width, arrangeBounds.Height);
            }
            else if (infiniteWidth && infiniteHeight)
            {
                // Check for completely unbound scenario
                computedSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            }
            else if (!MatrixHasInverse(_transformation))
            {
                // Check for singular matrix
                computedSize = new Size(0, 0);
            }
            else if ((0 == b) || (0 == c))
            {
                // Check for 0/180 degree special cases
                double maxHeight = (infiniteHeight ? double.PositiveInfinity : maxHeightFromHeight);
                double maxWidth = (infiniteWidth ? double.PositiveInfinity : maxWidthFromWidth);
                if ((0 == b) && (0 == c))
                {
                    // No constraints
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (0 == b)
                {
                    // Constrained by width
                    double computedHeight = Math.Min(idealHeightFromWidth, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs((c * computedHeight) / a),
                        computedHeight);
                }
                else if (0 == c)
                {
                    // Constrained by height
                    double computedWidth = Math.Min(idealWidthFromHeight, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs((b * computedWidth) / d));
                }
            }
            else if ((0 == a) || (0 == d))
            {
                // Check for 90/270 degree special cases
                double maxWidth = (infiniteHeight ? double.PositiveInfinity : maxWidthFromHeight);
                double maxHeight = (infiniteWidth ? double.PositiveInfinity : maxHeightFromWidth);
                if ((0 == a) && (0 == d))
                {
                    // No constraints
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (0 == a)
                {
                    // Constrained by width
                    double computedHeight = Math.Min(idealHeightFromHeight, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs((d * computedHeight) / b),
                        computedHeight);
                }
                else if (0 == d)
                {
                    // Constrained by height
                    double computedWidth = Math.Min(idealWidthFromWidth, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs((a * computedWidth) / c));
                }
            }
            else if (idealHeightFromWidth <= ((slopeFromHeight * idealWidthFromWidth) + maxHeightFromHeight))
            {
                // Check the width midpoint for viability (by being below the height constraint line)
                computedSize = new Size(idealWidthFromWidth, idealHeightFromWidth);
            }
            else if (idealHeightFromHeight <= ((slopeFromWidth * idealWidthFromHeight) + maxHeightFromWidth))
            {
                // Check the height midpoint for viability (by being below the width constraint line)
                computedSize = new Size(idealWidthFromHeight, idealHeightFromHeight);
            }
            else
            {
                // Neither midpoint is viable; use the intersection of the two constraint lines instead
                // Compute width by setting heights equal (m1*x+c1=m2*x+c2)
                double computedWidth = (maxHeightFromHeight - maxHeightFromWidth) / (slopeFromWidth - slopeFromHeight);
                // Compute height from width constraint line (y=m*x+c; using height would give same result)
                computedSize = new Size(
                    computedWidth,
                    (slopeFromWidth * computedWidth) + maxHeightFromWidth);
            }

            return computedSize;
        }

        /// <summary>
        /// Returns true if Size a is smaller than Size b in either dimension.
        /// </summary>
        /// <param name="a">Second Size.</param>
        /// <param name="b">First Size.</param>
        /// <returns>True if Size a is smaller than Size b in either dimension.</returns>
        private static bool IsSizeSmaller(Size a, Size b)
        {
            // WPF equivalent of following code:
            // return ((a.Width < b.Width) || (a.Height < b.Height));
            return ((a.Width + AcceptableDelta < b.Width) || (a.Height + AcceptableDelta < b.Height));
        }

        /// <summary>
        /// Rounds the non-offset elements of a Matrix to avoid issues due to floating point imprecision.
        /// </summary>
        /// <param name="matrix">Matrix to round.</param>
        /// <param name="decimals">Number of decimal places to round to.</param>
        /// <returns>Rounded Matrix.</returns>
        private static Matrix RoundMatrix(Matrix matrix, int decimals)
        {
            return new Matrix(
                Math.Round(matrix.M11, decimals),
                Math.Round(matrix.M12, decimals),
                Math.Round(matrix.M21, decimals),
                Math.Round(matrix.M22, decimals),
                matrix.OffsetX,
                matrix.OffsetY);
        }

        /// <summary>
        /// Implements WPF's Rect.Transform on Silverlight.
        /// </summary>
        /// <param name="rect">Rect to transform.</param>
        /// <param name="matrix">Matrix to transform with.</param>
        /// <returns>Bounding box of transformed Rect.</returns>
        private static Rect RectTransform(Rect rect, Matrix matrix)
        {
            // WPF equivalent of following code:
            // Rect rectTransformed = Rect.Transform(rect, matrix);
            Point leftTop = matrix.Transform(new Point(rect.Left, rect.Top));
            Point rightTop = matrix.Transform(new Point(rect.Right, rect.Top));
            Point leftBottom = matrix.Transform(new Point(rect.Left, rect.Bottom));
            Point rightBottom = matrix.Transform(new Point(rect.Right, rect.Bottom));
            double left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            double top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            double right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            double bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));
            Rect rectTransformed = new Rect(left, top, right - left, bottom - top);
            return rectTransformed;
        }

        /// <summary>
        /// Implements WPF's Matrix.Multiply on Silverlight.
        /// </summary>
        /// <param name="matrix1">First matrix.</param>
        /// <param name="matrix2">Second matrix.</param>
        /// <returns>Multiplication result.</returns>
        private static Matrix MatrixMultiply(Matrix matrix1, Matrix matrix2)
        {
            // WPF equivalent of following code:
            // return Matrix.Multiply(matrix1, matrix2);
            return new Matrix(
                (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21),
                (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22),
                (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21),
                (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22),
                ((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) + matrix2.OffsetX,
                ((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) + matrix2.OffsetY);
        }

        /// <summary>
        /// Implements WPF's Matrix.HasInverse on Silverlight.
        /// </summary>
        /// <param name="matrix">Matrix to check for inverse.</param>
        /// <returns>True if the Matrix has an inverse.</returns>
        private static bool MatrixHasInverse(Matrix matrix)
        {
            // WPF equivalent of following code:
            // return matrix.HasInverse;
            return (0 != ((matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21)));
        }

    }

}
