
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
    /// Translates (moves) an object in the two-dimensional x-y coordinate system.
    /// </summary>
    public sealed class TranslateTransform : Transform
    {
        double _appliedCssX;
        double _appliedCssY;
        object _domElementToWhichTheCssXWasApplied;
        object _domElementToWhichTheCssYWasApplied;

        /// <summary>
        /// Gets or sets the distance to translate along the x-axis.
        /// </summary>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Identifies the X dependency property
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(
                nameof(X), 
                typeof(double), 
                typeof(TranslateTransform), 
                new PropertyMetadata(0d, OnXChanged)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((TranslateTransform)instance).INTERNAL_parent;
                        if (target != null)
                        {
                            return new CSSEquivalent()
                            {
                                DomElement = target.INTERNAL_OuterDomElement,
                                Value = (inst, value) =>
                                {
                                    return value + "px";
                                },
                                Name = new List<string> { "translateX" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                                UIElement = target,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = true
                            };
                        }
                        return null;
                    }
                });

        private static void OnXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TranslateTransform)d).RaiseTransformChanged();
        }

        /// <summary>
        /// Gets or sets the distance to translate (move) an object along the y-axis.
        /// </summary>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        /// <summary>
        /// Identifies the Y dependency property
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(
                nameof(Y), 
                typeof(double), 
                typeof(TranslateTransform), 
                new PropertyMetadata(0d, OnYChanged)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((TranslateTransform)instance).INTERNAL_parent;
                        if (target != null)
                        {
                            return new CSSEquivalent()
                            {
                                DomElement = target.INTERNAL_OuterDomElement,
                                Value = (inst, value) =>
                                {
                                    return value + "px";
                                },
                                Name = new List<string> { "translateY" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                                UIElement = target,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = true
                            };
                        }
                        return null;
                    }
                });

        private static void OnYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TranslateTransform)d).RaiseTransformChanged();
        }

        private void ApplyCSSChanges(double x, double y)
        {
            CSSEquivalent translateXcssEquivalent = XProperty.GetMetadata(DependencyObjectType).GetCSSEquivalent(this);
            if (translateXcssEquivalent != null)
            {
                object domElementX = translateXcssEquivalent.DomElement;
                if ((x != _appliedCssX) || 
                    (_domElementToWhichTheCssXWasApplied != null && domElementX != _domElementToWhichTheCssXWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
                {
                    INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                        domElementX, 
                        translateXcssEquivalent.Name, 
                        translateXcssEquivalent.Value(this, x));
                    _appliedCssX = x;
                    _domElementToWhichTheCssXWasApplied = domElementX;
                }
            }

            CSSEquivalent translateYcssEquivalent = YProperty.GetMetadata(DependencyObjectType).GetCSSEquivalent(this);
            if (translateYcssEquivalent != null)
            {
                object domElementY = translateYcssEquivalent.DomElement;
                if ((y != _appliedCssY) || 
                    (_domElementToWhichTheCssYWasApplied != null && domElementY != _domElementToWhichTheCssYWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
                {
                    INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                        translateYcssEquivalent.DomElement, 
                        translateYcssEquivalent.Name, 
                        translateYcssEquivalent.Value(this, y));
                    _appliedCssY = x;
                    _domElementToWhichTheCssYWasApplied = domElementY;
                }
            }
        }

        internal override void INTERNAL_ApplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this.X, this.Y);
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
                Matrix matrix = Matrix.Identity;

                matrix.Translate(X, Y);

                return matrix;
            }
        }

        internal override bool IsIdentity => X == 0 && Y == 0;
    }
}
