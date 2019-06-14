
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
    /// Rotates an object clockwise about a specified point in a two-dimensional
    /// x-y coordinate system.
    /// </summary>
    public sealed class RotateTransform : Transform
    {
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
            DependencyProperty.Register("Angle", typeof(double), typeof(RotateTransform), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    if (((RotateTransform)instance).INTERNAL_parent != null)
                    {
                        return new CSSEquivalent()
                        {
                            DomElement = ((RotateTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
                            Value = (inst, value) =>
                            {
                                return value + "deg";
                            },
                            Name = new List<string> { "rotateZ" }, //Note: the css use would be: transform = "scaleX(2)" but the velocity call must use: scaleX : 2
                            UIElement = ((RotateTransform)instance).INTERNAL_parent,
                            ApplyAlsoWhenThereIsAControlTemplate = true,
                            OnlyUseVelocity = true
                        };
                    }
                    return null;
                }
            });

#if WORKINPROGRESS
        private void ApplyCSSChanges(RotateTransform rotateTransform, double angle)
        {
            CSSEquivalent anglecssEquivalent = AngleProperty.GetTypeMetaData(typeof(TranslateTransform)).GetCSSEquivalent(rotateTransform);
            INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(anglecssEquivalent.DomElement, anglecssEquivalent.Name, anglecssEquivalent.Value(rotateTransform, angle));
        }

        internal override void INTERNAL_ApplyCSSChanges()
        {
            ApplyCSSChanges(this, this.Angle);
        }

        internal override void INTERNAL_UnapplyCSSChanges()
        {
            ApplyCSSChanges(this, 0);
        }
#endif

        void ApplyRotateTransform(double angle)
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
#if WORKINPROGRESS
                ApplyCSSChanges(this, angle);
#else
                object parentDom = this.INTERNAL_parent.INTERNAL_OuterDomElement;
                CSSEquivalent anglecssEquivalent = AngleProperty.GetTypeMetaData(typeof(TranslateTransform)).GetCSSEquivalent(this);
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(anglecssEquivalent.DomElement, anglecssEquivalent.Name, anglecssEquivalent.Value(this, angle));
#endif



                //dynamic domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this.INTERNAL_parent);

                //string value = "rotate(" + angle + "deg)"; //todo: make sure that the conversion from double to string is culture-invariant so that it uses dots instead of commas for the decimal separator.

                //try
                //{
                //    domStyle.transform = value;
                //}
                //catch
                //{
                //    //do nothing
                //}
                //try
                //{
                //    domStyle.msTransform = value;
                //}
                //catch
                //{
                //    //do nothing
                //}
                //try // Prevents crash in the simulator that uses IE.
                //{
                //    domStyle.WebkitTransform = value;
                //}
                //catch
                //{
                //    //do nothing
                //}
            }
        }

        // NOTE: CenterX and CenterY are currently not supported because in CSS there is only the "transformOrigin" property, which is used for the "UIElement.RenderTransformOrigin" property.


        internal override void INTERNAL_ApplyTransform()
        {
            this.ApplyRotateTransform(this.Angle);
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
#if WORKINPROGRESS
                INTERNAL_UnapplyCSSChanges();
#else
                object parentDom = this.INTERNAL_parent.INTERNAL_OuterDomElement;
                CSSEquivalent anglecssEquivalent = AngleProperty.GetTypeMetaData(typeof(TranslateTransform)).GetCSSEquivalent(this);
                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(anglecssEquivalent.DomElement, anglecssEquivalent.Name, anglecssEquivalent.Value(this, 0));
#endif
            }
        }

        protected override Point INTERNAL_TransformPoint(Point point)
        {
            throw new NotImplementedException("Please contact support@cshtml5.com");
        }
#if WORKINPROGRESS
#region Not supported yet

        /// <summary>
        ///     CenterX - double.  Default value is 0.0.
        /// </summary>
        public double CenterX
        {
            get
            {
                return (double)GetValue(CenterXProperty);
            }
            set
            {
                SetValue(CenterXProperty, value);
            }
        }

        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(double), typeof(RotateTransform), new PropertyMetadata(null));

        /// <summary>
        ///     CenterY - double.  Default value is 0.0.
        /// </summary>
        public double CenterY
        {
            get
            {
                return (double)GetValue(CenterYProperty);
            }
            set
            {
                SetValue(CenterYProperty, value);
            }
        }

        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(double), typeof(RotateTransform), new PropertyMetadata(null));


#endregion
#endif
    }
}