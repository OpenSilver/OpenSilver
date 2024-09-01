
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

using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using CSHTML5.Internal;

namespace System.Windows;

public partial class UIElement
{
    /// <summary>
    /// Returns a transform object that can be used to transform coordinates from
    /// the UIElement to the specified object.
    /// </summary>
    /// <param name="visual">
    /// The object to compare to the current object for purposes of obtaining the
    /// transform.
    /// </param>
    /// <returns>
    /// The transform information as an object. Call methods on this object to get
    /// a practical transform.
    /// </returns>
    public GeneralTransform TransformToVisual(UIElement visual) => new MatrixTransform(GetRelativeTransform(visual));

    internal Matrix GetRelativeTransform(UIElement visual)
    {
        if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this) ||
            (visual is not null && !INTERNAL_VisualTreeManager.IsElementInVisualTree(visual)))
        {
            return Matrix.Identity;
        }

        visual ??= Window.GetWindow(this);

        if (visual is null)
        {
            return Matrix.Identity;
        }

        if (FindCommonVisualAncestor(visual) is not UIElement ancestor)
        {
            return GetRelativeTransformNative(visual);
        }

        TrySimpleTransformToAncestor(ancestor, false, out Matrix m0);

        // combine the transforms
        // if both transforms are simple Matrix transforms, just multiply them and
        // return the result.
        if (visual.TrySimpleTransformToAncestor(ancestor, true, out Matrix m1))
        {
            MatrixUtil.MultiplyMatrix(ref m0, ref m1);
            return m0;
        }

        return m0;
    }

    private bool TrySimpleTransformToAncestor(UIElement ancestor, bool inverse, out Matrix simpleTransform)
    {
        UIElement g = this;
        Matrix m = Matrix.Identity;

        // This while loop will walk up the visual tree until we encounter the ancestor.
        // As it does so, it will accumulate the descendent->ancestor transform.

        while (g != ancestor)
        {
            // In SL/WPF borders are only visual effects and do not directly affect the layout.
            // Borders have to take them into account during the Arrange phase, so the offset
            // created by the BorderThickness is taking into account when computing VisualOffset.
            // In css, border-width is taken into account in the layout, which means we can't add
            // the offsets created by BorderThickness in the VisualOffset. That is why we manually
            // add it here.
            if (TryGetBorderOffsets(g, out Point offsets))
            {
                m.Translate(offsets.X, offsets.Y);
            }

            if (g.GetValue(RenderTransformProperty) is Transform transform)
            {
                Point origin = g.GetRenderTransformOrigin();
                bool hasOrigin = origin.X != 0d || origin.Y != 0d;
                if (hasOrigin)
                {
                    m.Translate(-origin.X, -origin.Y);
                }

                Matrix cm = transform.Matrix;
                MatrixUtil.MultiplyMatrix(ref m, ref cm);

                if (hasOrigin)
                {
                    m.Translate(origin.X, origin.Y);
                }
            }

            m.Translate(g.VisualOffset.X, g.VisualOffset.Y);

            if (GetAncestor(g) is not UIElement parent)
            {
                break;
            }

            g = parent;
        }

        Debug.Assert(g == ancestor, inverse ?
            "The specified Visual is not a descendant of this Visual." :
            "The specified Visual is not an ancestor of this Visual.");

        if (inverse)
        {
            if (!m.HasInverse)
            {
                simpleTransform = new Matrix();
                return false; // inversion failed, so simple transform failed.
            }

            m.Invert();
        }

        simpleTransform = m;
        return true; // simple transform succeeded

        static UIElement GetAncestor(UIElement uie)
        {
            Debug.Assert(uie is not null);

            // We try to get the ancestor in 3 differents ways. We cannot only rely on
            // the visual tree because popups create a disconnection in the visual tree.
            // This method helps "reconnect" the visual tree.
            //
            // (1) Get the regular visual parent
            // (2) Get the an informal visual parent when the element is the root of a popup
            // (3) Get the containing window (if different from the element itself)

            if (uie.VisualParent is UIElement parent)
            {
                return parent;
            }

            if (uie is FrameworkElement fe && fe.Parent is Popup popup)
            {
                return popup.PopupRoot?.HiddenVisualParent;
            }

            Window window = Window.GetWindow(uie);
            if (window != uie)
            {
                return window;
            }

            return null;
        }
    }

    private bool TryGetBorderOffsets(UIElement uie, out Point offsets)
    {
        if (uie is Border border)
        {
            Thickness borders = border.BorderThickness;
            offsets = new Point(borders.Left, borders.Top);
            return true;
        }

        offsets = default;
        return false;
    }

    private Matrix GetRelativeTransformNative(UIElement otherVisual)
    {
        Debug.Assert(otherVisual is not null);
        Debug.Assert(INTERNAL_VisualTreeManager.IsElementInVisualTree(this));
        Debug.Assert(INTERNAL_VisualTreeManager.IsElementInVisualTree(otherVisual));

        string sOuterDivOfControl = OpenSilver.Interop.GetVariableStringForJS(OuterDiv);
        string sOuterDivOfReferenceVisual = OpenSilver.Interop.GetVariableStringForJS(otherVisual.OuterDiv);
        string s = OpenSilver.Interop.ExecuteJavaScriptString(
            $"({sOuterDivOfControl}.getBoundingClientRect().left - {sOuterDivOfReferenceVisual}.getBoundingClientRect().left) + '|' + ({sOuterDivOfControl}.getBoundingClientRect().top - {sOuterDivOfReferenceVisual}.getBoundingClientRect().top)");
        int index = s.IndexOf('|');
        double offsetLeft = double.Parse(s.Substring(0, index), CultureInfo.InvariantCulture);
        double offsetTop = double.Parse(s.Substring(index + 1), CultureInfo.InvariantCulture);

        return new Matrix(1, 0, 0, 1, offsetLeft, offsetTop);
    }
}
