

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
    /// Represents a two-dimensional skew.
    /// </summary>
    public sealed partial class SkewTransform : Transform
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
            DependencyProperty.Register("AngleX", typeof(double), typeof(SkewTransform), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((SkewTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((SkewTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "deg";
                            },
                            Name = new List<string> { "skewX" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((SkewTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            });


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
            DependencyProperty.Register("AngleY", typeof(double), typeof(SkewTransform), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((SkewTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((SkewTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "deg";
                            },
                            Name = new List<string> { "skewY" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((SkewTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            });

        private void ApplyCSSChanges(SkewTransform skewTransform, double angleX, double angleY)
        {
            CSSEquivalent angleXcssEquivalent = AngleXProperty.GetTypeMetaData(typeof(SkewTransform)).GetCSSEquivalent(skewTransform);
            object domElementX = angleXcssEquivalent.DomElement;
            if (angleX != _appliedCssAngleX || (_domElementToWhichTheCssAngleXWasApplied != null && domElementX != _domElementToWhichTheCssAngleXWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(angleXcssEquivalent.DomElement, angleXcssEquivalent.Name, angleXcssEquivalent.Value(skewTransform, angleX));
                _appliedCssAngleX = angleX;
                _domElementToWhichTheCssAngleXWasApplied = domElementX;
            }

            CSSEquivalent angleYcssEquivalent = AngleYProperty.GetTypeMetaData(typeof(SkewTransform)).GetCSSEquivalent(skewTransform);
            object domElementY = angleYcssEquivalent.DomElement;
            if (angleY != _appliedCssAngleY || (_domElementToWhichTheCssAngleYWasApplied != null && domElementY != _domElementToWhichTheCssAngleYWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(angleYcssEquivalent.DomElement, angleYcssEquivalent.Name, angleYcssEquivalent.Value(skewTransform, angleY));
                _appliedCssAngleY = angleY;
                _domElementToWhichTheCssAngleYWasApplied = domElementY;
            }
        }

        internal override void INTERNAL_ApplyCSSChanges()
        {
            ApplyCSSChanges(this, this.AngleX, this.AngleY);
        }

        internal override void INTERNAL_UnapplyCSSChanges()
        {
            ApplyCSSChanges(this, 0, 0);
        }

        internal void ApplySkewTransform(double angleX, double angleY)
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this, angleX, angleY);

                //dynamic domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this.INTERNAL_parent);

                //string value = "skew(" + angleX + "deg, " + angleY + "deg)"; //todo: make sure that the conversion from double to string is culture-invariant so that it uses dots instead of commas for the decimal separator.
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


        // NOTE: CenterX and CenterY are currently not supported because in CSS there is only the "transformOrigin" property, which is used for the "UIElement.RenderTransformOrigin" property.
        // However, they are still present as stubs in the WorkInProgress version or SkewTransform


        internal override void INTERNAL_ApplyTransform()
        {
            this.ApplySkewTransform(this.AngleX, this.AngleY);
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
