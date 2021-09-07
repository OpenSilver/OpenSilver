﻿

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;

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
    /// Applies multiple transform operations to an object.
    /// </summary>
    public sealed partial class CompositeTransform : Transform
    {
        /// <summary>
        /// Gets or sets the x-axis scale factor. You can use this property to stretch
        /// or shrink an object horizontally. The default is 1.
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
                typeof(CompositeTransform), 
                new PropertyMetadata(1d)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((CompositeTransform)instance).INTERNAL_parent;
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

        /// <summary>
        /// Gets or sets the y-axis scale factor. You can use this property to stretch
        /// or shrink an object vertically. The default is 1.
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
                typeof(CompositeTransform), 
                new PropertyMetadata(1d)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((CompositeTransform)instance).INTERNAL_parent;
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

        /// <summary>
        /// Gets or sets the x-axis skew angle, which is measured in degrees counterclockwise
        /// from the y-axis. A skew transform can be useful for creating the illusion
        /// of three-dimensional depth in a two-dimensional object. The default is 0.
        /// </summary>
        public double SkewX
        {
            get { return (double)GetValue(SkewXProperty); }
            set { SetValue(SkewXProperty, value); }
        }

        /// <summary>
        /// Identifies the SkewX dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewXProperty =
            DependencyProperty.Register(
                nameof(SkewX), 
                typeof(double), 
                typeof(CompositeTransform), 
                new PropertyMetadata(0d)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((CompositeTransform)instance).INTERNAL_parent;
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

        /// <summary>
        /// Gets or sets the y-axis skew angle, which is measured in degrees counterclockwise
        /// from the x-axis. A skew transform can be useful for creating the illusion
        /// of three-dimensional depth in a two-dimensional object. The default is 0.
        /// </summary>
        public double SkewY
        {
            get { return (double)GetValue(SkewYProperty); }
            set { SetValue(SkewYProperty, value); }
        }

        /// <summary>
        /// Identifies the SkewY dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewYProperty =
            DependencyProperty.Register(
                nameof(SkewY), 
                typeof(double), 
                typeof(CompositeTransform), 
                new PropertyMetadata(0d)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((CompositeTransform)instance).INTERNAL_parent;
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

        /// <summary>
        /// Gets or sets the angle, in degrees, of clockwise rotation. The default is 0.
        /// </summary>
        public double Rotation
        {
            get { return (double)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }
        /// <summary>
        /// Identifies the Rotation dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register(
                nameof(Rotation), 
                typeof(double), 
                typeof(CompositeTransform), 
                new PropertyMetadata(0d)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((CompositeTransform)instance).INTERNAL_parent;
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

        /// <summary>
        /// Gets or sets the distance to translate along the x-axis.
        /// </summary>
        public double TranslateX
        {
            get { return (double)GetValue(TranslateXProperty); }
            set { SetValue(TranslateXProperty, value); }
        }

        /// <summary>
        /// Identifies the TranslateX dependency property. The default is 0.
        /// </summary>
        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register(
                nameof(TranslateX), 
                typeof(double), 
                typeof(CompositeTransform), 
                new PropertyMetadata(0d)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((CompositeTransform)instance).INTERNAL_parent;
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

        /// <summary>
        /// Gets or sets the distance to translate (move) an object along the y-axis. The default is 0.
        /// </summary>
        public double TranslateY
        {
            get { return (double)GetValue(TranslateYProperty); }
            set { SetValue(TranslateYProperty, value); }
        }
        /// <summary>
        /// Identifies the TranslateY dependency property
        /// </summary>
        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register(
                nameof(TranslateY), 
                typeof(double), 
                typeof(CompositeTransform), 
                new PropertyMetadata(0d)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((CompositeTransform)instance).INTERNAL_parent;
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

        private void ApplyCSSChanges(double scaleX, double scaleY, double skewX, double skewY, double rotation, double translateX, double translateY)
        {
            //-------------
            // In XAML, order is always:
            // 1. Scale
            // 2. Skew
            // 3. Rotate
            // 4. Translate
            //
            // Below we do in reverse order because in CSS the right-most operation is done first.
            //-------------

            //TranslateX:
            CSSEquivalent translateXcssEquivalent = TranslateXProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(this);
            if (translateXcssEquivalent != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                    translateXcssEquivalent.DomElement, 
                    translateXcssEquivalent.Name, 
                    translateXcssEquivalent.Value(this, translateX));
            }

            //TranslateY:
            CSSEquivalent translateYcssEquivalent = TranslateYProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(this);
            if (translateYcssEquivalent != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                    translateYcssEquivalent.DomElement, 
                    translateYcssEquivalent.Name, 
                    translateYcssEquivalent.Value(this, translateY)); 
            }

            //Rotation:
            CSSEquivalent rotationcssEquivalent = RotationProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(this);
            if (rotationcssEquivalent != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                    rotationcssEquivalent.DomElement, 
                    rotationcssEquivalent.Name, 
                    rotationcssEquivalent.Value(this, rotation)); 
            }

            //SkewX:
            CSSEquivalent skewXcssEquivalent = SkewXProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(this);
            if (skewXcssEquivalent != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                    skewXcssEquivalent.DomElement, 
                    skewXcssEquivalent.Name, 
                    skewXcssEquivalent.Value(this, skewX)); 
            }

            //SkewY:
            CSSEquivalent skewYcssEquivalent = SkewYProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(this);
            if (skewYcssEquivalent != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                    skewYcssEquivalent.DomElement, 
                    skewYcssEquivalent.Name, 
                    skewYcssEquivalent.Value(this, skewY)); 
            }

            //ScaleX:
            CSSEquivalent scaleXcssEquivalent = ScaleXProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(this);
            if (scaleXcssEquivalent != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                    scaleXcssEquivalent.DomElement, 
                    scaleXcssEquivalent.Name, 
                    scaleXcssEquivalent.Value(this, scaleX)); 
            }

            //ScaleY:
            CSSEquivalent scaleYcssEquivalent = ScaleYProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(this);
            if (scaleYcssEquivalent != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(
                    scaleYcssEquivalent.DomElement, 
                    scaleYcssEquivalent.Name, 
                    scaleYcssEquivalent.Value(this, scaleY)); 
            }
        }

        internal override void INTERNAL_ApplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this.ScaleX, this.ScaleY, this.SkewX, this.SkewY, this.Rotation, this.TranslateX, this.TranslateY);
            }
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(1, 1, 0, 0, 0, 0, 0);
            }
        }

        internal override Matrix Value
        {
            get
            {
                double centerX = this.CenterX;
                double centerY = this.CenterY;
                bool hasCenter = centerX != 0 || centerY != 0;

                // 1. Scale
                Matrix transform = new Matrix();
                transform.ScaleAt(ScaleX, ScaleY, centerX, centerY);

                // 2. Skew
                if (hasCenter)
                {
                    transform.Translate(-centerX, -centerY);
                }

                transform.Skew(SkewX, SkewY);

                if (hasCenter)
                {
                    transform.Translate(centerX, centerY);
                }

                // 3. Rotate
                transform.RotateAt(Rotation, centerX, centerY);

                // 4. Translate
                transform.Translate(TranslateX, TranslateY);

                return transform;
            }
        }
    }
}
