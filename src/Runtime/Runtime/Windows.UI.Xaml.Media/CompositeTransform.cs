

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
            DependencyProperty.Register("ScaleX", typeof(double), typeof(CompositeTransform), new PropertyMetadata(1d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((CompositeTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((CompositeTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value;
                            },
                            Name = new List<string> { "scaleX" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((CompositeTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                }
            });

        private static void ScaleX_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var compositeTransform = (CompositeTransform)d;
            compositeTransform.INTERNAL_ApplyTransform();
        }


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
            DependencyProperty.Register("ScaleY", typeof(double), typeof(CompositeTransform), new PropertyMetadata(1d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((CompositeTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((CompositeTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value;
                            },
                            Name = new List<string> { "scaleY" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((CompositeTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                }
            });

        private static void ScaleY_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var compositeTransform = (CompositeTransform)d;
            compositeTransform.INTERNAL_ApplyTransform();
        }



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
            DependencyProperty.Register("SkewX", typeof(double), typeof(CompositeTransform), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((CompositeTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((CompositeTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "deg";
                            },
                            Name = new List<string> { "skewX" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((CompositeTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                }
            });

        private static void SkewX_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var compositeTransform = (CompositeTransform)d;
            compositeTransform.INTERNAL_ApplyTransform();
        }

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
            DependencyProperty.Register("SkewY", typeof(double), typeof(CompositeTransform), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((CompositeTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((CompositeTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "deg";
                            },
                            Name = new List<string> { "skewY" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((CompositeTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                }
            });

        private static void SkewY_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var compositeTransform = (CompositeTransform)d;
            compositeTransform.INTERNAL_ApplyTransform();
        }

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
            DependencyProperty.Register("Rotation", typeof(double), typeof(CompositeTransform), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((CompositeTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((CompositeTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "deg";
                            },
                            Name = new List<string> { "rotateZ" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((CompositeTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                }
            });
        private static void Rotation_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var compositeTransform = (CompositeTransform)d;
            compositeTransform.INTERNAL_ApplyTransform();
        }


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
            DependencyProperty.Register("TranslateX", typeof(double), typeof(CompositeTransform), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((CompositeTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((CompositeTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "px";
                            },
                            Name = new List<string> { "translateX" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((CompositeTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                }
            });

        private static void TranslateX_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var compositeTransform = (CompositeTransform)d;
            compositeTransform.INTERNAL_ApplyTransform();
        }


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
            DependencyProperty.Register("TranslateY", typeof(double), typeof(CompositeTransform), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((CompositeTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((CompositeTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "px";
                            },
                            Name = new List<string> { "translateY" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((CompositeTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                }
            });
        private static void TranslateY_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var compositeTransform = (CompositeTransform)d;
            compositeTransform.INTERNAL_ApplyTransform();
        }

        private void ApplyCSSChanges(CompositeTransform compositeTransform, double scaleX, double scaleY, double skewX, double skewY, double rotation, double translateX, double translateY)
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
            CSSEquivalent translateXcssEquivalent = TranslateXProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(compositeTransform);
            INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(translateXcssEquivalent.DomElement, translateXcssEquivalent.Name, translateXcssEquivalent.Value(compositeTransform, translateX));
            //TranslateY:
            CSSEquivalent translateYcssEquivalent = TranslateYProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(compositeTransform);
            INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(translateYcssEquivalent.DomElement, translateYcssEquivalent.Name, translateYcssEquivalent.Value(compositeTransform, translateY));
            //Rotation:
            CSSEquivalent rotationcssEquivalent = RotationProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(compositeTransform);
            INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(rotationcssEquivalent.DomElement, rotationcssEquivalent.Name, rotationcssEquivalent.Value(compositeTransform, rotation));
            //SkewX:
            CSSEquivalent skewXcssEquivalent = SkewXProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(compositeTransform);
            INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(skewXcssEquivalent.DomElement, skewXcssEquivalent.Name, skewXcssEquivalent.Value(compositeTransform, skewX));
            //SkewY:
            CSSEquivalent skewYcssEquivalent = SkewYProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(compositeTransform);
            INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(skewYcssEquivalent.DomElement, skewYcssEquivalent.Name, skewYcssEquivalent.Value(compositeTransform, skewY));
            //ScaleX:
            CSSEquivalent scaleXcssEquivalent = ScaleXProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(compositeTransform);
            INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(scaleXcssEquivalent.DomElement, scaleXcssEquivalent.Name, scaleXcssEquivalent.Value(compositeTransform, scaleX));
            //ScaleY:
            CSSEquivalent scaleYcssEquivalent = ScaleYProperty.GetTypeMetaData(typeof(CompositeTransform)).GetCSSEquivalent(compositeTransform);
            INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(scaleYcssEquivalent.DomElement, scaleYcssEquivalent.Name, scaleYcssEquivalent.Value(compositeTransform, scaleY));

        }

        internal override void INTERNAL_ApplyCSSChanges()
        {
            ApplyCSSChanges(this, this.ScaleX, this.ScaleY, this.SkewX, this.SkewY, this.Rotation, this.TranslateX, this.TranslateY);
        }

        internal override void INTERNAL_UnapplyCSSChanges()
        {
            ApplyCSSChanges(this, 1, 1, 0, 0, 0, 0, 0);
        }

        internal void ApplyCompositeTransform(double scaleX, double scaleY, double skewX, double skewY, double rotation, double translateX, double translateY)
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this, scaleX, scaleY, skewX, skewY, rotation, translateX, translateY);
            }
        }


        // NOTE: CenterX and CenterY are currently not supported because in CSS there is only the "transformOrigin" property, which is used for the "UIElement.RenderTransformOrigin" property.
        // However, they are still present as stubs in the WorkInProgress version or CompositeTransform


        internal override void INTERNAL_ApplyTransform()
        {
            this.ApplyCompositeTransform(this.ScaleX, this.ScaleY, this.SkewX, this.SkewY, this.Rotation, this.TranslateX, this.TranslateY);
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                INTERNAL_UnapplyCSSChanges();
            }
        }

        protected override Point INTERNAL_TransformPoint(Point point)
        {
            throw new NotImplementedException("Please contact support@cshtml5.com");
        }

#if WORKINPROGRESS
        public override GeneralTransform Inverse
        {
            get { return null; }
        }

        public override Rect TransformBounds(Rect rect)
        {
            return new Rect();
        }

        public override bool TryTransform(Point inPoint, out Point outPoint)
        {
            outPoint = new Point();
            return false;
        }
#endif
    }
}
