
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

using System;
using System.Collections.Generic;
using CSHTML5.Internal;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Rotates an object clockwise about a specified point in a two-dimensional
    /// x-y coordinate system.
    /// </summary>
    public sealed class RotateTransform : Transform
    {
        double _appliedCssAngle;
        object _domElementToWhichTheCssWasApplied;

        /// <summary>
        /// Gets or sets the angle, in degrees, of clockwise rotation.
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// Identifies the Angle dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(
                nameof(Angle), 
                typeof(double), 
                typeof(RotateTransform), 
                new PropertyMetadata(0d, OnAngleChanged)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((RotateTransform)instance).INTERNAL_parent;
                        if (target != null)
                        {
                            return new CSSEquivalent()
                            {
                                DomElement = target.INTERNAL_OuterDomElement,
                                Value = (inst, value) =>
                                {
                                    return value + "deg";
                                },
                                Name = new List<string> { "rotateZ" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                                UIElement = target,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = true
                            };
                        }
                        return null;
                    }
                });

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotateTransform)d).RaiseTransformChanged();
        }

        private void ApplyCSSChanges(double angle)
        {
            CSSEquivalent anglecssEquivalent = AngleProperty.GetMetadata(DependencyObjectType).GetCSSEquivalent(this);
            if (anglecssEquivalent != null)
            {
                object domElement = anglecssEquivalent.DomElement;
                if (angle != _appliedCssAngle || (_domElementToWhichTheCssWasApplied != null && 
                    domElement != _domElementToWhichTheCssWasApplied)) // Optimization to avoid setting the transform if the value is 0 or if it is the same as the last time.
                {
                    INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                        anglecssEquivalent.DomElement, 
                        anglecssEquivalent.Name, 
                        anglecssEquivalent.Value(this, angle));
                    _appliedCssAngle = angle;
                    _domElementToWhichTheCssWasApplied = domElement;
                }
            }
        }

        internal override void INTERNAL_ApplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this.Angle);
            }
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(0);
            }
        }

        internal override Matrix ValueInternal
        {
            get
            {
                Matrix m = new Matrix();
                m.RotateAt(Angle, CenterX, CenterY);
                return m;
            }
        }

        internal override bool IsIdentity => Angle == 0;

        /// <summary>
        /// Gets or sets the x-coordinate of the rotation center point.
        /// The default is 0.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double CenterX
        {
            get => (double)GetValue(CenterXProperty);
            set => SetValue(CenterXProperty, value);
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CenterXProperty = 
            DependencyProperty.Register(
                nameof(CenterX), 
                typeof(double), 
                typeof(RotateTransform), 
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the y-coordinate of the rotation center point.
        /// The default is 0.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double CenterY
        {
            get => (double)GetValue(CenterYProperty);
            set => SetValue(CenterYProperty, value);
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CenterYProperty = 
            DependencyProperty.Register(
                nameof(CenterY), 
                typeof(double), 
                typeof(RotateTransform), 
                new PropertyMetadata(0.0));
    }
}