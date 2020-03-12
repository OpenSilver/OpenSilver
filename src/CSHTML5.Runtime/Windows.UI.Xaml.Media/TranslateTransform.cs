

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
    /// Translates (moves) an object in the two-dimensional x-y coordinate system.
    /// </summary>
    public sealed partial class TranslateTransform : Transform
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
            DependencyProperty.Register("X", typeof(double), typeof(TranslateTransform), new PropertyMetadata(0d)//, X_Changed));
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((TranslateTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((TranslateTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "px";
                            },
                            Name = new List<string> { "translateX" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((TranslateTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            });

        private static void X_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var translateTransform = (TranslateTransform)d;
            double newX = (double)e.NewValue;
            translateTransform.ApplyTranslateTransform(newX, translateTransform.Y);
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
            DependencyProperty.Register("Y", typeof(double), typeof(TranslateTransform), new PropertyMetadata(0d)//, Y_Changed));
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((TranslateTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((TranslateTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "px";
                            },
                            Name = new List<string> { "translateY" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((TranslateTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            });

        //private static void Y_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var translateTransform = (TranslateTransform)d;
        //    double newY = (double)e.NewValue;
        //    translateTransform.ApplyTranslateTransform(translateTransform.X, newY);
        //}

        private void ApplyCSSChanges(TranslateTransform translateTransform, double x, double y)
        {
            CSSEquivalent translateXcssEquivalent = XProperty.GetTypeMetaData(typeof(TranslateTransform)).GetCSSEquivalent(translateTransform);
            object domElementX = translateXcssEquivalent.DomElement;
            if (x != _appliedCssX || (_domElementToWhichTheCssXWasApplied != null && domElementX != _domElementToWhichTheCssXWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(domElementX, translateXcssEquivalent.Name, translateXcssEquivalent.Value(translateTransform, x));
                _appliedCssX = x;
                _domElementToWhichTheCssXWasApplied = domElementX;
            }

            CSSEquivalent translateYcssEquivalent = YProperty.GetTypeMetaData(typeof(TranslateTransform)).GetCSSEquivalent(translateTransform);
            object domElementY = translateYcssEquivalent.DomElement;
            if (y != _appliedCssY || (_domElementToWhichTheCssYWasApplied != null && domElementY != _domElementToWhichTheCssYWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(translateYcssEquivalent.DomElement, translateYcssEquivalent.Name, translateYcssEquivalent.Value(translateTransform, y));
                _appliedCssY = x;
                _domElementToWhichTheCssYWasApplied = domElementX;
            }
        }

        internal override void INTERNAL_ApplyCSSChanges()
        {
            ApplyCSSChanges(this, this.X, this.Y);
        }

        internal override void INTERNAL_UnapplyCSSChanges()
        {
            ApplyCSSChanges(this, 0, 0);
        }

        internal void ApplyTranslateTransform(double x, double y)
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this, x, y);

                //dynamic domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this.INTERNAL_parent);

                //string value = "translate(" + x + "px, " + y + "px)"; //todo: make sure that the conversion from double to string is culture-invariant so that it uses dots instead of commas for the decimal separator.

                //try
                //{
                //    domStyle.transform = value;
                //}
                //catch
                //{
                //}
                //try
                //{
                //    domStyle.msTransform = value;
                //}
                //catch
                //{
                //}
                //try // Prevents crash in the simulator that uses IE.
                //{
                //    domStyle.WebkitTransform = value;
                //}
                //catch
                //{
                //}
            }
        }

        internal override void INTERNAL_ApplyTransform()
        {
            this.ApplyTranslateTransform(this.X, this.Y);
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
            return new Point(point.X + X, point.Y + Y);
        }

#if WORKINPROGRESS
        public override GeneralTransform Inverse
        {
            get { return null; }
        }

        public override Rect TransformBounds(Rect rect)
        {
            return Rect.Empty;
        }

        public override bool TryTransform(Point inPoint, out Point outPoint)
        {
            outPoint = new Point();
            return false;
        }
#endif
    }
}
