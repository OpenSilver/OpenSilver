﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
                }
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
                }
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
    }
}
