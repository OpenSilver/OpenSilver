

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
    /// Scales an object in the two-dimensional x-y coordinate system.
    /// </summary>
    public sealed partial class ScaleTransform : Transform
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
            DependencyProperty.Register("ScaleX", typeof(double), typeof(ScaleTransform), new PropertyMetadata(1d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((ScaleTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((ScaleTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                                {
                                    return value;
                                },
                            Name = new List<string> { "scaleX" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((ScaleTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            });



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
            DependencyProperty.Register("ScaleY", typeof(double), typeof(ScaleTransform), new PropertyMetadata(1d)
            {
                GetCSSEquivalent = (instance) =>
                    {
                        if (((ScaleTransform)instance).INTERNAL_parent != null)
                        {
                            return new CSSEquivalent()
                            {
                                DomElement = ((ScaleTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                                Value = (inst, value) =>
                                    {
                                        return value;
                                    },
                                Name = new List<string> { "scaleY" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                                UIElement = ((ScaleTransform)instance).INTERNAL_parent,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = true
                            };
                        }
                        return null;
                    },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            });

        private void ApplyCSSChanges(ScaleTransform scaleTransform, double scaleX, double scaleY)
        {
            CSSEquivalent scaleXcssEquivalent = ScaleXProperty.GetTypeMetaData(typeof(ScaleTransform)).GetCSSEquivalent(scaleTransform);
            object domElementX = scaleXcssEquivalent.DomElement;
            if (scaleX != _appliedCssScaleX || (_domElementToWhichTheCssScaleXWasApplied != null && domElementX != _domElementToWhichTheCssScaleXWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(scaleXcssEquivalent.DomElement, scaleXcssEquivalent.Name, scaleXcssEquivalent.Value(scaleTransform, scaleX));
                _appliedCssScaleX = scaleX;
                _domElementToWhichTheCssScaleXWasApplied = domElementX;
            }

            CSSEquivalent scaleYcssEquivalent = ScaleYProperty.GetTypeMetaData(typeof(ScaleTransform)).GetCSSEquivalent(scaleTransform);
            object domElementY = scaleYcssEquivalent.DomElement;
            if (scaleY != _appliedCssScaleY || (_domElementToWhichTheCssScaleYWasApplied != null && domElementY != _domElementToWhichTheCssScaleYWasApplied)) // Optimization to avoid setting the transform if the value is (0,0) or if it is the same as the last time.
            {
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(scaleYcssEquivalent.DomElement, scaleYcssEquivalent.Name, scaleYcssEquivalent.Value(scaleTransform, scaleY));
                _appliedCssScaleY = scaleY;
                _domElementToWhichTheCssScaleYWasApplied = domElementY;
            }
        }

        internal override void INTERNAL_ApplyCSSChanges()
        {
            ApplyCSSChanges(this, this.ScaleX, this.ScaleY);
        }

        internal override void INTERNAL_UnapplyCSSChanges()
        {
            ApplyCSSChanges(this, 1, 1);
        }

        internal void ApplyScaleTransform(double scaleX, double scaleY)
        {

            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this, scaleX, scaleY);

                //string value = "scale(" + scaleX + ", " + scaleY + ")"; //todo: make sure that the conversion from double to string is culture-invariant so that it uses dots instead of commas for the decimal separator.

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

        internal override void INTERNAL_ApplyTransform()
        {
            this.ApplyScaleTransform(this.ScaleX, this.ScaleY);
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
        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(double), typeof(ScaleTransform), null);
        public double CenterX
        {
            get { return (double)this.GetValue(CenterXProperty); }
            set { this.SetValue(CenterXProperty, value); }
        }

        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(double), typeof(ScaleTransform), null);

        public double CenterY
        {
            get { return (double)this.GetValue(CenterYProperty); }
            set { this.SetValue(CenterYProperty, value); }
        }

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
