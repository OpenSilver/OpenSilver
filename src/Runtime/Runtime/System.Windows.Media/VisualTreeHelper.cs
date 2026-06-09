
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls.Primitives;

namespace System.Windows.Media;

/// <summary>
/// Provides utility methods that perform common tasks involving nodes in a visual tree.
/// </summary>
public sealed class VisualTreeHelper
{
    /// <summary>
    /// Returns an object's root object in the visual tree.
    /// </summary>
    /// <param name="reference">
    /// The object to get the root object for.
    /// </param>
    /// <returns>
    /// The root object of the reference object in the visual tree.
    /// </returns>
    public static DependencyObject GetRoot(DependencyObject reference)
    {
        DependencyObject root = reference;

        DependencyObject parent;
        while ((parent = GetParent(root)) is not null)
        {
            root = parent;
        }

        return root;
    }

    // Variant of GetRoot that ignore the IsVisualTreeRoot flag
    internal static DependencyObject GetVisualRoot(DependencyObject reference)
    {
        DependencyObject root = reference;

        DependencyObject parent;
        while ((parent = GetParent(root)) is not null)
        {
            root = parent;
        }

        return root;

        static DependencyObject GetParent(DependencyObject reference)
        {
            return reference switch
            {
                UIElement uie => uie.InternalVisualParent,
                IInternalUIElement iuie => iuie.VisualParent,
                _ => null,
            };
        }
    }

    /// <summary>
    /// Returns a <see cref="DependencyObject"/> value that represents the parent of the visual object.
    /// </summary>
    /// <param name="reference">
    /// The visual whose parent is returned.
    /// </param>
    /// <returns>
    /// The parent of the visual.
    /// </returns>
    public static DependencyObject GetParent(DependencyObject reference)
    {
        return reference switch
        {
            UIElement uie => GetParent(uie),
            IInternalUIElement iuie => iuie.VisualParent,
            _ => null,
        };
    }

    /// <inheritdoc cref="GetParent(DependencyObject)" />
    public static DependencyObject GetParent(IDependencyObject reference)
    {
        return reference switch
        {
            UIElement uie => GetParent(uie),
            IInternalUIElement iuie => iuie.VisualParent,
            _ => null,
        };
    }

    /// <inheritdoc cref="GetParent(DependencyObject)" />
    public static DependencyObject GetParent(UIElement reference)
    {
        if (reference is null || reference.IsVisualTreeRoot)
        {
            return null;
        }
        return reference.InternalVisualParent;
    }

    /// <summary>
    /// Returns the number of children that the specified visual object contains.
    /// </summary>
    /// <param name="reference">
    /// The parent visual that is referenced as a <see cref="DependencyObject"/>.
    /// </param>
    /// <returns>
    /// The number of child visuals that the parent visual contains.
    /// </returns>
    public static int GetChildrenCount(DependencyObject reference)
    {
        if (reference is IInternalUIElement uie)
        {
            return uie.VisualChildrenCount;
        }

        throw new InvalidOperationException(string.Format(Strings.UIElement_NotAnUIElement, nameof(reference)));
    }

    /// <inheritdoc cref="GetChildrenCount(DependencyObject)" />
    public static int GetChildrenCount(UIElement reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return reference.InternalVisualChildrenCount;
    }

    /// <summary>
    /// Returns the child visual object from the specified collection index within a specified parent.
    /// </summary>
    /// <param name="reference">
    /// The parent visual, referenced as a <see cref="DependencyObject"/>.
    /// </param>
    /// <param name="childIndex">
    /// The index that represents the child visual that is contained by reference.
    /// </param>
    /// <returns>
    /// The child object as referenced by childIndex.
    /// </returns>
    public static DependencyObject GetChild(DependencyObject reference, int childIndex)
    {
        if (reference is IInternalUIElement uie)
        {
            return uie.GetVisualChild(childIndex);
        }

        throw new InvalidOperationException(string.Format(Strings.UIElement_NotAnUIElement, nameof(reference)));
    }

    /// <inheritdoc cref="GetChild(DependencyObject, int)" />
    public static DependencyObject GetChild(UIElement reference, int childIndex)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return reference.InternalGetVisualChild(childIndex);
    }

    /// <summary>
    /// Returns the offset of the <see cref="UIElement"/>.
    /// </summary>
    /// <param name="reference">
    /// The <see cref="UIElement"/> whose offset is returned.
    /// </param>
    /// <returns>
    /// A <see cref="Vector"/> that represents the offset value of the <see cref="UIElement"/>.
    /// </returns>
    public static Vector GetOffset(UIElement reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return reference.VisualOffset;
    }

    /// <summary>
    /// Return the clip region of the specified <see cref="UIElement"/> as a <see cref="Geometry"/> value.
    /// </summary>
    /// <param name="reference">
    /// The <see cref="UIElement"/> whose clip region value is returned.
    /// </param>
    /// <returns>
    /// The clip region value of the <see cref="UIElement"/> returned as a <see cref="Geometry"/> type.
    /// </returns>
    public static Geometry GetClip(UIElement reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return reference.VisualClip;
    }

    /// <summary>
    /// Returns a <see cref="Transform"/> value for the <see cref="UIElement"/>.
    /// </summary>
    /// <param name="reference">
    /// The <see cref="UIElement"/> whose transform value is returned.
    /// </param>
    /// <returns>
    /// The transform value of the <see cref="UIElement"/>, or null if reference does not have a 
    /// transform defined.
    /// </returns>
    public static Transform GetTransform(UIElement reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return reference.VisualTransform;
    }

    /// <summary>
    /// Retrieves a set of objects that are located within a specified point of an object's
    /// coordinate space.
    /// </summary>
    /// <param name="intersectingPoint">
    /// The point to use as the determination point.
    /// </param>
    /// <param name="subtree">
    /// The object to search within.
    /// </param>
    /// <returns>
    /// An enumerable set of <see cref="UIElement"/> objects that are determined to
    /// be located in the visual tree composition at the specified point and within the
    /// specified subtee.
    /// </returns>
    public static IEnumerable<UIElement> FindElementsInHostCoordinates(Point intersectingPoint, UIElement subtree)
    {
        var list = new List<UIElement>();

        Window window = subtree switch
        {
            Window w => w,
            null => Window.Current,
            _ => null,
        };

        var hitTestResults = HitTestNative(intersectingPoint);

        if (window is not null)
        {
            foreach (Popup popup in ((IEnumerable<Popup>)GetOpenPopups(window)).Reverse())
            {
                if (popup.Child is not UIElement popupRoot)
                {
                    continue;
                }

                list.AddRange(FindElementsInHostCoordinatesImpl(intersectingPoint, popupRoot, hitTestResults));
            }

            subtree = window;
        }

        if (subtree is not null)
        {
            list.AddRange(FindElementsInHostCoordinatesImpl(intersectingPoint, subtree, hitTestResults));
        }

        return list;

        static IEnumerable<UIElement> FindElementsInHostCoordinatesImpl(Point intersectingPoint, UIElement element, HashSet<UIElement> hitTestResults)
        {
            Debug.Assert(element is not null);

            bool includeAllElements = false;

            foreach (UIElement child in GetChildren(element).OrderByDescending(Canvas.GetZIndex))
            {
                foreach (UIElement uie in FindElementsInHostCoordinatesImpl(intersectingPoint, child, hitTestResults))
                {
                    includeAllElements = true;
                    yield return uie;
                }
            }

            if (includeAllElements || hitTestResults.Contains(element))
            {
                yield return element;
            }
        }

        static IEnumerable<UIElement> GetChildren(UIElement element)
        {
            int childrenCount = element.InternalVisualChildrenCount;
            for (int i = childrenCount - 1; i >= 0; i--)
            {
                UIElement child = element.InternalGetVisualChild(i);
                if (child is null or Inline)
                {
                    continue;
                }

                yield return child;
            }
        }

        static HashSet<UIElement> HitTestNative(Point intersectingPoint)
        {
            string[] ids = JsonSerializer.Deserialize<string[]>(
                OpenSilver.Interop.ExecuteJavaScriptString(
                    $"osjs.elementsFromPoint({intersectingPoint.X.ToInvariantString()}, {intersectingPoint.Y.ToInvariantString()})"));

            var hashset = new HashSet<UIElement>();
            foreach (string id in ids)
            {
                if (INTERNAL_HtmlDomManager.GetElementById(id) is UIElement uie)
                {
                    hashset.Add(uie);
                }
            }

            return hashset;
        }
    }

    /// <summary>
    /// Retrieves a set of objects that are located within a specified <see cref="Rect"/>
    /// of an object's coordinate space.
    /// </summary>
    /// <param name="intersectingRect">
    /// The <see cref="Rect"/> to use as the determination area.
    /// </param>
    /// <param name="subtree">
    /// The object to search within.
    /// </param>
    /// <returns>
    /// An enumerable set of <see cref="UIElement"/> objects that are determined to
    /// be located in the visual tree composition at the specified point and within the
    /// specified subtee.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static IEnumerable<UIElement> FindElementsInHostCoordinates(Rect intersectingRect, UIElement subtree)
    {
        return new List<UIElement>();
    }

    /// <summary>
    /// Gets all open <see cref="Popup"/> controls.
    /// </summary>
    /// <returns>A list of all open <see cref="Popup"/> controls.</returns>
    public static List<Popup> GetOpenPopups()
    {
        return GetOpenPopups(Window.Current);
    }

    /// <summary>
    /// Gets all open <see cref="Popup"/> controls for the specified <see cref="Window"/>.
    /// </summary>
    /// <param name="window">The <see cref="Window"/> to get <see cref="Popup"/> for.</param>
    /// <returns>A list of all open <see cref="Popup"/> controls.</returns>
    public static List<Popup> GetOpenPopups(Window window)
    {
        var result = new List<Popup>();

        if (window is not null)
        {
            foreach (PopupRoot root in PopupService.ActivePopups)
            {
                if (root.ParentWindow == window &&
                    root.Popup.IsOpen &&
                    root.Popup.Child != null)
                {
                    result.Add(root.Popup);
                }
            }
        }

        return result;
    }
}