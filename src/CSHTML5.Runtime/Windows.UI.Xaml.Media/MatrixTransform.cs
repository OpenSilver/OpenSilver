

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
using CSHTML5.Internal;
using System.Collections.Generic;
using System.Globalization;

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
    public sealed partial class MatrixTransform : Transform
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

        private void ApplyCSSChanges(MatrixTransform matrixTransform)
        {
            throw new NotImplementedException();
        }

        internal override void INTERNAL_ApplyCSSChanges()
        {
            ApplyCSSChanges(this);
        }

        internal override void INTERNAL_UnapplyCSSChanges()
        {
            ApplyCSSChanges(this);
        }

        internal override void INTERNAL_ApplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                dynamic domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this.INTERNAL_parent);

                var matrix = this.Matrix;

                string value = "matrix("
                    + matrix.M11.ToString(CultureInfo.InvariantCulture) + ","
                    + matrix.M12.ToString(CultureInfo.InvariantCulture) + ","
                    + matrix.M21.ToString(CultureInfo.InvariantCulture) + ","
                    + matrix.M22.ToString(CultureInfo.InvariantCulture) + ","
                    + matrix.OffsetX.ToString(CultureInfo.InvariantCulture) + ","
                    + matrix.OffsetY.ToString(CultureInfo.InvariantCulture)
                    + ")";

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
