
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
using System.Collections.Generic;
using System.Globalization;
using CSHTML5.Internal;
using OpenSilver.Internal;

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
                new PropertyMetadata(Matrix.Identity, OnMatrixChanged)
                {
                    GetCSSEquivalent = static (instance) =>
                    {
                        var target = ((MatrixTransform)instance).INTERNAL_parent;
                        if (target != null)
                        {
                            return new CSSEquivalent
                            {
                                DomElement = target.INTERNAL_OuterDomElement,
                                Value = (inst, value) => MatrixToHtmlString((Matrix)value),
                                Name = new List<string>(1) { "transform" },
                                UIElement = target,
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                                OnlyUseVelocity = false
                            };
                        }
                        return null;
                    }
                });

        private static void OnMatrixChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MatrixTransform)d).RaiseTransformChanged();
        }

        internal override Matrix ValueInternal => this.Matrix;

        private void ApplyCSSChanges(Matrix matrix)
        {
            UIElement owner = INTERNAL_parent;
            if (owner is not null)
            {
                INTERNAL_HtmlDomManager.SetCSSStyleProperty(
                    owner.INTERNAL_OuterDomElement,
                    "transform",
                    MatrixToHtmlString(matrix));
            }
        }

        internal static string MatrixToHtmlString(Matrix m)
        {
            return $"matrix({m.M11.ToInvariantString()}, {m.M12.ToInvariantString()}, {m.M21.ToInvariantString()}, {m.M22.ToInvariantString()}, {m.OffsetX.ToInvariantString()}, {m.OffsetY.ToInvariantString()})";
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
