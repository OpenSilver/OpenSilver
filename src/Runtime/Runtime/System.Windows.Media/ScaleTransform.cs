
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
    /// Scales an object in the two-dimensional x-y coordinate system.
    /// </summary>
    public sealed class ScaleTransform : Transform
    {
        double _appliedCssScaleX = 1d;
        double _appliedCssScaleY = 1d;
        object _domElementToWhichTheCssScaleXWasApplied;
        object _domElementToWhichTheCssScaleYWasApplied;

        /// <summary>
        /// Gets or sets the x-axis scale factor.
        /// </summary>
        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        /// <summary>
        /// Identifies the ScaleX dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register(
                nameof(ScaleX), 
                typeof(double), 
                typeof(ScaleTransform), 
                new PropertyMetadata(1d, OnScaleXChanged)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((ScaleTransform)instance).INTERNAL_parent;
                        if (target != null)
                        {
                            return new CSSEquivalent()
                            {
                                DomElement = target.INTERNAL_OuterDomElement,
                                Value = (inst, value) =>
                                {
                                    return value;
                                },
                                Name = new List<string> { "scaleX" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                                UIElement = target,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = true
                            };
                        }
                        return null;
                    }
                });

        private static void OnScaleXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScaleTransform)d).RaiseTransformChanged();
        }

        /// <summary>
        /// Gets or sets the y-axis scale factor.
        /// </summary>
        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        /// <summary>
        /// Identifies the ScaleY dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register(
                nameof(ScaleY), 
                typeof(double), 
                typeof(ScaleTransform), 
                new PropertyMetadata(1d, OnScaleYChanged)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((ScaleTransform)instance).INTERNAL_parent;
                        if (target != null)
                        {
                            return new CSSEquivalent()
                            {
                                DomElement = target.INTERNAL_OuterDomElement,
                                Value = (inst, value) =>
                                    {
                                        return value;
                                    },
                                Name = new List<string> { "scaleY" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                                UIElement = target,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = true
                            };
                        }
                        return null;
                    }
                });

        private static void OnScaleYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScaleTransform)d).RaiseTransformChanged();
        }

        private void ApplyCSSChanges(double scaleX, double scaleY)
        {
            CSSEquivalent scaleXcssEquivalent = ScaleXProperty.GetMetadata(DependencyObjectType).GetCSSEquivalent(this);
            if (scaleXcssEquivalent != null)
            {
                object domElementX = scaleXcssEquivalent.DomElement;
                if (scaleX != _appliedCssScaleX || (_domElementToWhichTheCssScaleXWasApplied != null && 
                    domElementX != _domElementToWhichTheCssScaleXWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
                {
                    INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                        scaleXcssEquivalent.DomElement, 
                        scaleXcssEquivalent.Name, 
                        scaleXcssEquivalent.Value(this, scaleX));
                    _appliedCssScaleX = scaleX;
                    _domElementToWhichTheCssScaleXWasApplied = domElementX;
                }
            }

            CSSEquivalent scaleYcssEquivalent = ScaleYProperty.GetMetadata(DependencyObjectType).GetCSSEquivalent(this);
            if (scaleYcssEquivalent != null)
            {
                object domElementY = scaleYcssEquivalent.DomElement;
                if ((scaleY != _appliedCssScaleY) || 
                    (_domElementToWhichTheCssScaleYWasApplied != null && domElementY != _domElementToWhichTheCssScaleYWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
                {
                    INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                        scaleYcssEquivalent.DomElement, 
                        scaleYcssEquivalent.Name, 
                        scaleYcssEquivalent.Value(this, scaleY));
                    _appliedCssScaleY = scaleY;
                    _domElementToWhichTheCssScaleYWasApplied = domElementY;
                }
            }
        }

        internal override void INTERNAL_ApplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this.ScaleX, this.ScaleY);
            }
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(1, 1);
            }
        }

        internal override Matrix ValueInternal
        {
            get
            {
                Matrix m = new Matrix();
                m.ScaleAt(ScaleX, ScaleY, CenterX, CenterY);
                return m;
            }
        }

        internal override bool IsIdentity => ScaleX == 1 && ScaleY == 1;

        /// <summary>
        /// Identifies the <see cref="ScaleTransform.CenterX"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CenterXProperty = 
            DependencyProperty.Register(
                nameof(CenterX), 
                typeof(double), 
                typeof(ScaleTransform), 
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the x-coordinate of the center point of this <see cref="ScaleTransform"/>.
        /// The default is 0.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double CenterX
        {
            get { return (double)this.GetValue(CenterXProperty); }
            set { this.SetValue(CenterXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ScaleTransform.CenterY"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CenterYProperty = 
            DependencyProperty.Register(
                nameof(CenterY), 
                typeof(double), 
                typeof(ScaleTransform),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the y-coordinate of the center point of this <see cref="ScaleTransform"/>.
        /// The default is 0.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double CenterY
        {
            get { return (double)this.GetValue(CenterYProperty); }
            set { this.SetValue(CenterYProperty, value); }
        }
    }
}
