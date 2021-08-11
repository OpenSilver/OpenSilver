

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
        #region Constructors

        public MatrixTransform()
        {
        }

        internal MatrixTransform(Matrix matrix)
        {
            Matrix = matrix;
        }

        #endregion Constructors

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
            DependencyProperty.Register(
                nameof(Matrix),
                typeof(Matrix),
                typeof(MatrixTransform),
                new PropertyMetadata(Matrix.Identity)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        var target = ((MatrixTransform)instance).INTERNAL_parent;
                        if (target != null)
                        {
                            return new CSSEquivalent()
                            {
                                DomElement = target.INTERNAL_OuterDomElement,
                                Value = (inst, value) =>
                                {
                                    var m = (Matrix)value;
                                    return string.Format(CultureInfo.InvariantCulture,
                                        "matrix({0}, {1}, {2}, {3}, {4}, {5})",
                                        m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
                                },
                                Name = new List<string>(1) { "transform" },
                                UIElement = target,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = false
                            };
                        }
                        return null;
                    }
                });

        internal override Matrix Value => this.Matrix;

        private void ApplyCSSChanges(Matrix matrix)
        {
            CSSEquivalent cssEquivalent = MatrixProperty.GetTypeMetaData(typeof(MatrixTransform)).GetCSSEquivalent(this);
            if (cssEquivalent != null)
            {
                object domElement = cssEquivalent.DomElement;
                INTERNAL_HtmlDomManager.SetDomElementStyleProperty(
                    cssEquivalent.DomElement, 
                    cssEquivalent.Name, 
                    cssEquivalent.Value(this, matrix));
            }
        }

        internal override void INTERNAL_ApplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this.Matrix);
            }
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(Matrix.Identity);
            }
        }
    }
}
