
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
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Media;

namespace System.Windows;

public partial class UIElement
{
    /// <summary>
    /// Returns a transform that can be used to transform coordinates from the <see cref="UIElement"/> to the 
    /// specified visual object.
    /// </summary>
    /// <param name="visual">
    /// The <see cref="UIElement"/> to which the coordinates are transformed.
    /// </param>
    /// <returns>
    /// A value of type <see cref="GeneralTransform"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The visual objects are not related.
    /// </exception>
    public GeneralTransform TransformToVisual(UIElement visual) => new MatrixTransform(InternalTransformToVisual(visual));

    internal Matrix InternalTransformToVisual(UIElement visual)
    {
        if (visual is null)
        {
            return InternalTransformToAncestor(null);
        }

        if (FindCommonVisualAncestor(visual) is not UIElement ancestor)
        {
            return TransformToVisualNative(visual);
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

    /// <summary>
    /// Returns a transform that can be used to transform coordinates from the <see cref="UIElement"/> to the 
    /// specified ancestor of the visual object.
    /// </summary>
    /// <param name="ancestor">
    /// The <see cref="UIElement"/> to which the coordinates are transformed.
    /// </param>
    /// <returns>
    /// A value of type <see cref="GeneralTransform"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The visual objects are not related.
    /// </exception>
    public GeneralTransform TransformToAncestor(UIElement ancestor) => new MatrixTransform(InternalTransformToAncestor(ancestor));

    internal Matrix InternalTransformToAncestor(UIElement ancestor)
    {
        TrySimpleTransformToAncestor(ancestor, false, out Matrix m);
        return m;
    }

    /// <summary>
    /// Returns a transform that can be used to transform coordinates from the <see cref="UIElement"/> to the 
    /// specified visual object descendant.
    /// </summary>
    /// <param name="descendant">
    /// The <see cref="UIElement"/> to which the coordinates are transformed.
    /// </param>
    /// <returns>
    /// A value of type <see cref="GeneralTransform"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descendant"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The visual objects are not related.
    /// </exception>
    public GeneralTransform TransformToDescendant(UIElement descendant)
    {
        ArgumentNullException.ThrowIfNull(descendant);

        return new MatrixTransform(InternalTransformToDescendant(descendant));
    }

    internal Matrix InternalTransformToDescendant(UIElement descendant)
    {
        Debug.Assert(descendant is not null);
        descendant.TrySimpleTransformToAncestor(this, true, out Matrix m);
        return m;
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

            if (g.VisualTransform is Transform transform)
            {
                Matrix cm = transform.Matrix;
                MatrixUtil.MultiplyMatrix(ref m, ref cm);
            }

            m.Translate(g.VisualOffset.X, g.VisualOffset.Y);

            g = GetVisualAncestor(g);
        }

        if (g != ancestor)
        {
            throw new InvalidOperationException(inverse ? Strings.UIElement_NotADescendant : Strings.UIElement_NotAnAncestor);
        }

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
    }

    private bool TryGetBorderOffsets(UIElement uie, out Point offsets)
    {
        if (uie is IBorderElement border)
        {
            Thickness thickness = border.BorderThickness;
            offsets = new Point(thickness.Left, thickness.Top);
            return true;
        }

        offsets = default;
        return false;
    }

    private Matrix TransformToVisualNative(UIElement otherVisual)
    {
        Debug.Assert(otherVisual is not null);

        if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this) ||
            !INTERNAL_VisualTreeManager.IsElementInVisualTree(otherVisual))
        {
            return Matrix.Identity;
        }

        Vector offets = Vector.Parse(
            OpenSilver.Interop.ExecuteJavaScriptString(
                $"osjs.transformToVisual('{OuterDiv.Uid}', '{otherVisual.OuterDiv.Uid}')"));

        return new Matrix(1, 0, 0, 1, offets.X, offets.Y);
    }

    private void SetVisualFlagsToRoot(VisualFlags flag, bool value)
    {
        UIElement current = this;

        do
        {
            current.WriteVisualFlag(flag, value);
            current = GetVisualAncestor(current);
        }
        while (current is not null);
    }

    private UIElement FindFirstAncestorWithFlagsAnd(VisualFlags flag)
    {
        UIElement current = this;

        do
        {
            if (current.ReadVisualFlag(flag))
            {
                // The other UIElement crossed through this UIElement's parent chain. Hence this is our
                // common ancestor.
                return current;
            }

            current = GetVisualAncestor(current);
        }
        while (current is not null);

        return null;
    }

    private UIElement FindCommonVisualAncestor(UIElement otherVisual)
    {
        ArgumentNullException.ThrowIfNull(otherVisual);

        // Since we can't rely on code running in the CLR, we need to first make sure
        // that the FindCommonAncestor flag is not set. It is enought to ensure this
        // on one path to the root Visual.

        SetVisualFlagsToRoot(VisualFlags.FindCommonAncestor, false);

        // Walk up the other visual's parent chain and set the FindCommonAncestor flag.
        otherVisual.SetVisualFlagsToRoot(VisualFlags.FindCommonAncestor, true);

        // Now see if the other Visual's parent chain crosses our parent chain.
        return FindFirstAncestorWithFlagsAnd(VisualFlags.FindCommonAncestor);
    }

    private static UIElement GetVisualAncestor(UIElement uie)
    {
        Debug.Assert(uie is not null);

        // PopupRoots are contained inside a Window, but are not considered as visual children.
        // This method helps reconnecting the popup root to its window.
        //
        // (1) Get the regular visual parent
        // (2) Get the containing window (if different from the element itself)

        if (uie.InternalVisualParent is UIElement parent)
        {
            return parent;
        }

        Window window = Window.GetWindow(uie);
        if (window != uie)
        {
            return window;
        }

        return null;
    }
}
