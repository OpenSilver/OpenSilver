
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
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
    /// Creates an arbitrary affine matrix transformation that is used to manipulate objects or coordinate systems in a two-dimensional plane.
    /// </summary>
    public sealed class MatrixTransform : Transform
    {
        /// <summary>
        /// Gets or sets the Matrix that defines this transformation. The default is an
        /// identity Matrix. An identity matrix has a value of 1 in coefficients [1,1],
        /// [2,2], and [3,3]; and a value of 0 in the rest of the coefficients.
        /// </summary>
        public Matrix Matrix
        {
            get { return (Matrix)GetValue(MatrixProperty); }
            set { SetValue(MatrixProperty, value); }
        }

        /// <summary>
        /// Identifies the Matrix dependency property.
        /// </summary>
        public static readonly DependencyProperty MatrixProperty =
            DependencyProperty.Register("Matrix", typeof(Matrix), typeof(MatrixTransform), new PropertyMetadata(Matrix.Identity));
            //{
            //    GetCSSEquivalent = (instance) =>
            //    {
            //        if (((GeneralTransform)instance).INTERNAL_parent != null)
            //        {
            //            return new CSSEquivalent()
            //            {
            //                DomElement = ((GeneralTransform)instance).INTERNAL_parent.INTERNAL_OuterDomElement,
            //                Value = (inst, value) =>
            //                {
            //                    return "1.06, 1.84, 0.54, 2.8, 466px, 482px";
            //                },
            //                Name = new List<string> { "transform" },
            //                UIElement = ((GeneralTransform)instance).INTERNAL_parent,
            //                ApplyAlsoWhenThereIsAControlTemplate = true,
            //                OnlyUseVelocity = false
            //            };
            //        }
            //        else
            //            return null;
            //    }
            //});

        internal override void INTERNAL_ApplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                dynamic domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this.INTERNAL_parent);

                var matrix = this.Matrix;

                string value = "matrix("
                    + matrix.M11.ToString() + ","
                    + matrix.M12.ToString() + ","
                    + matrix.M21.ToString() + ","
                    + matrix.M22.ToString() + ","
                    + matrix.OffsetX.ToString() + ","
                    + matrix.OffsetY.ToString()
                    + ")"; //todo: make sure that the conversion from double to string is culture-invariant so that it uses dots instead of commas for the decimal separator.

                try
                {
                    domStyle.transform = value;
                }
                catch
                {
                }
                try
                {
                    domStyle.msTransform = value;
                }
                catch
                {
                }
                try // Prevents crash in the simulator that uses IE.
                {
                    domStyle.WebkitTransform = value;
                }
                catch
                {
                }

            }
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                dynamic domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this.INTERNAL_parent);

                string value = "";

                try
                {
                    domStyle.transform = value;
                }
                catch
                {
                }
                try
                {
                    domStyle.msTransform = value;
                }
                catch
                {
                }
                try // Prevents crash in the simulator that uses IE.
                {
                    domStyle.WebkitTransform = value;
                }
                catch
                {
                }
            }
        }

        protected override Point INTERNAL_TransformPoint(Point point)
        {
            throw new NotImplementedException("Please contact support@cshtml5.com");
        }
    }
}
