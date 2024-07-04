
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
using DotNetForHtml5.Core;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Provides utility methods that can used to traverse object relationships (along
    /// child object or parent object axes) in the visual tree.
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
            while ((parent = GetParent(root)) != null)
            {
                root = parent;
            }

            return root;
        }

        /// <summary>
        /// Returns an object's parent object in the visual tree.
        /// </summary>
        /// <param name="reference">The object for which to get the parent object.</param>
        /// <returns>The parent object of the reference object in the visual tree.</returns>
        public static DependencyObject GetParent(DependencyObject reference)
        {
            if (reference is IInternalUIElement uie)
            {
                return uie.VisualParent;
            }

            return null;
        }

        /// <summary>
        /// Returns an object's parent object in the visual tree.
        /// </summary>
        /// <param name="reference">The object for which to get the parent object.</param>
        /// <returns>The parent object of the reference object in the visual tree.</returns>
        public static DependencyObject GetParent(IDependencyObject reference)
        {
            if (reference is IInternalUIElement uie)
            {
                return uie.VisualParent;
            }

            return null;
        }

        /// <summary>
        /// Returns an object's parent object in the visual tree.
        /// </summary>
        /// <param name="reference">
        /// The object for which to get the parent object.
        /// </param>
        /// <returns>
        /// The parent object of the reference object in the visual tree.
        /// </returns>
        public static DependencyObject GetParent(UIElement reference) => reference?.VisualParent;

        /// <summary>
        /// Returns the number of children that exist in an object's child collection in the visual tree.
        /// </summary>
        /// <param name="reference">The source visual.</param>
        /// <returns>The number of visual children for the provided source visual.</returns>
        public static int GetChildrenCount(DependencyObject reference)
        {
            if (reference is IInternalUIElement uie)
            {
                return uie.VisualChildrenCount;
            }

            throw new InvalidOperationException("Reference is not a valid visual DependencyObject.");
        }

        /// <summary>
        /// Returns the number of children that exist in an object's child collection in the visual tree.
        /// </summary>
        /// <param name="reference">
        /// The source visual.
        /// </param>
        /// <returns>
        /// The number of visual children for the provided source visual.
        /// </returns>
        public static int GetChildrenCount(UIElement reference)
        {
            if (reference is null)
            {
                throw new InvalidOperationException("Reference is not a valid visual DependencyObject.");
            }

            return reference.VisualChildrenCount;
        }

        /// <summary>
        /// Using the provided index, obtains a specific child object of the provided object by examining the visual tree. Please note that the current implementation does not guarantee that the index corresponds to the visual order of the controls.
        /// </summary>
        /// <param name="reference">The object that holds the child collection.</param>
        /// <param name="childIndex">The index of the requested child object in the reference child collection. Please note that the current implementation does not guarantee that the index corresponds to the visual order of the controls.</param>
        /// <returns>The child object as referenced by childIndex.</returns>
        public static DependencyObject GetChild(DependencyObject reference, int childIndex)
        {
            if (reference is IInternalUIElement uie)
            {
                return uie.GetVisualChild(childIndex);
            }
            
            throw new InvalidOperationException("Reference is not a valid visual DependencyObject.");
        }

        public static DependencyObject GetChild(UIElement reference, int childIndex)
        {
            if (reference is null)
            {
                throw new InvalidOperationException("Reference is not a valid visual DependencyObject.");
            }

            return reference.GetVisualChild(childIndex);
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
                int childrenCount = element.VisualChildrenCount;
                for (int i = childrenCount - 1; i >= 0; i--)
                {
                    UIElement child = element.GetVisualChild(i);
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
                        $"document.elementsFromPointOpenSilver({intersectingPoint.X.ToInvariantString()}, {intersectingPoint.Y.ToInvariantString()});"));

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
                foreach (PopupRoot root in PopupsManager.GetActivePopupRoots())
                {
                    if (root.ParentWindow == window &&
                        root.ParentPopup.IsOpen &&
                        root.ParentPopup.Child != null)
                    {
                        result.Add(root.ParentPopup);
                    }
                }
            }

            return result;
        }
    }
}