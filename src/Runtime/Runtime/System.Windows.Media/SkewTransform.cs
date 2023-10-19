
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

using System.Collections.Generic;
using CSHTML5.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a two-dimensional skew.
    /// </summary>
    public sealed class SkewTransform : Transform
    {
        double _appliedCssAngleX;
        double _appliedCssAngleY;
        object _domElementToWhichTheCssAngleXWasApplied;
        object _domElementToWhichTheCssAngleYWasApplied;

        /// <summary>
        /// Gets or sets the x-axis skew angle, which is measured in degrees counterclockwise
        /// from the y-axis.
        /// </summary>
        public double AngleX
        {
            get { return (double)GetValue(AngleXProperty); }
            set { SetValue(AngleXProperty, value); }
        }

        /// <summary>
        /// Identifies the AngleX dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleXProperty =
            DependencyProperty.Register(
                nameof(AngleX), 
                typeof(double), 
                typeof(SkewTransform), 
                new PropertyMetadata(0d, OnAngleXChanged)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((SkewTransform)instance).INTERNAL_parent;
                        if (target != null)
                        {
                            return new CSSEquivalent()
                            {
                                DomElement = target.INTERNAL_OuterDomElement,
                                Value = (inst, value) =>
                                {
                                    return value + "deg";
                                },
                                Name = new List<string> { "skewX" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                                UIElement = target,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = true
                            };
                        }
                        return null;
                    }
                });

        private static void OnAngleXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SkewTransform)d).RaiseTransformChanged();
        }

        /// <summary>
        /// Gets or sets the y-axis skew angle, which is measured in degrees counterclockwise
        /// from the x-axis.
        /// </summary>
        public double AngleY
        {
            get { return (double)GetValue(AngleYProperty); }
            set { SetValue(AngleYProperty, value); }
        }

        /// <summary>
        /// Identifies the AngleY dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleYProperty =
            DependencyProperty.Register(
                nameof(AngleY), 
                typeof(double), 
                typeof(SkewTransform), 
                new PropertyMetadata(0d, OnAngleYChanged)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((SkewTransform)instance).INTERNAL_parent;
                        if (target != null)
                        {
                            return new CSSEquivalent()
                            {
                                DomElement = target.INTERNAL_OuterDomElement,
                                Value = (inst, value) =>
                                {
                                    return value + "deg";
                                },
                                Name = new List<string> { "skewY" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                                UIElement = target,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = true
                            };
                        }
                        return null;
                    }
                });

        private static void OnAngleYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SkewTransform)d).RaiseTransformChanged();
        }

        /// <summary>
        /// Identifies the <see cref="CenterX"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register(
                nameof(CenterX),
                typeof(double),
                typeof(SkewTransform),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the x-coordinate of the transform center.
        /// The default is 0.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double CenterX
        {
            get => (double)GetValue(CenterXProperty);
            set => SetValue(CenterXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CenterY"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register(
                nameof(CenterY),
                typeof(double),
                typeof(SkewTransform),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the y-coordinate of the transform center.
        /// The default is 0.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double CenterY
        {
            get => (double)GetValue(CenterYProperty);
            set => SetValue(CenterYProperty, value);
        }

        private void ApplyCSSChanges(double angleX, double angleY)
        {
            CSSEquivalent angleXcssEquivalent = AngleXProperty.GetMetadata(DependencyObjectType).GetCSSEquivalent(this);
            if (angleXcssEquivalent != null)
            {
                object domElementX = angleXcssEquivalent.DomElement;
                if ((angleX != _appliedCssAngleX) || 
                    (_domElementToWhichTheCssAngleXWasApplied != null && domElementX != _domElementToWhichTheCssAngleXWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
                {
                    INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                        angleXcssEquivalent.DomElement, 
                        angleXcssEquivalent.Name, 
                        angleXcssEquivalent.Value(this, angleX));
                    _appliedCssAngleX = angleX;
                    _domElementToWhichTheCssAngleXWasApplied = domElementX;
                }
            }

            CSSEquivalent angleYcssEquivalent = AngleYProperty.GetMetadata(DependencyObjectType).GetCSSEquivalent(this);
            if (angleYcssEquivalent != null)
            {
                object domElementY = angleYcssEquivalent.DomElement;
                if ((angleY != _appliedCssAngleY) || 
                    (_domElementToWhichTheCssAngleYWasApplied != null && domElementY != _domElementToWhichTheCssAngleYWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
                {
                    INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                        angleYcssEquivalent.DomElement, 
                        angleYcssEquivalent.Name, 
                        angleYcssEquivalent.Value(this, angleY));
                    _appliedCssAngleY = angleY;
                    _domElementToWhichTheCssAngleYWasApplied = domElementY;
                }
            }
        }

        internal override void INTERNAL_ApplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this.AngleX, this.AngleY);
            }
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(0, 0);
            }
        }

        internal override Matrix ValueInternal
        {
            get
            {
                Matrix matrix = new Matrix();

                double angleX = AngleX;
                double angleY = AngleY;
                double centerX = CenterX;
                double centerY = CenterY;

                bool hasCenter = centerX != 0 || centerY != 0;

                if (hasCenter)
                {
                    matrix.Translate(-centerX, -centerY);
                }

                matrix.Skew(angleX, angleY);

                if (hasCenter)
                {
                    matrix.Translate(centerX, centerY);
                }

                return matrix;
            }
        }

        internal override bool IsIdentity => AngleX == 0 && AngleY == 0;
    }
}
