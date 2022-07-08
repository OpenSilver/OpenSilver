

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
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;
using DotNetForHtml5.Core;
#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Provides utility methods that can used to traverse object relationships (along
    /// child object or parent object axes) in the visual tree.
    /// </summary>
    public sealed partial class VisualTreeHelper
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
        public static DependencyObject GetParent(DependencyObject reference) //todo: the original signature takes a "DependencyObject"
        {
            if (reference is UIElement uie)
            {
                return uie.INTERNAL_VisualParent;
            }

            return null;
        }

        /// <summary>
        /// Returns the number of children that exist in an object's child collection in the visual tree.
        /// </summary>
        /// <param name="reference">The source visual.</param>
        /// <returns>The number of visual children for the provided source visual.</returns>
        public static int GetChildrenCount(DependencyObject reference)
        {
            if (reference is UIElement uie)
            {
                return uie.VisualChildrenCount;
            }

            throw new InvalidOperationException("Reference is not a valid visual DependencyObject.");
        }

        /// <summary>
        /// Using the provided index, obtains a specific child object of the provided object by examining the visual tree. Please note that the current implementation does not guarantee that the index corresponds to the visual order of the controls.
        /// </summary>
        /// <param name="reference">The object that holds the child collection.</param>
        /// <param name="childIndex">The index of the requested child object in the reference child collection. Please note that the current implementation does not guarantee that the index corresponds to the visual order of the controls.</param>
        /// <returns>The child object as referenced by childIndex.</returns>
        public static DependencyObject GetChild(DependencyObject reference, int childIndex)
        {
            if (reference is UIElement uie)
            {
                return uie.GetVisualChild(childIndex);
            }
            
            throw new InvalidOperationException("Reference is not a valid visual DependencyObject.");
        }

        /// <summary>
        /// Retrieves an object that is located within a specified point of an object's coordinate space.
        /// </summary>
        /// <param name="intersectingPoint">The point to use as the determination point.</param>
        /// <returns>The UIElement object that is determined to be located
        /// in the visual tree composition at the specified point.</returns>
        internal static UIElement FindElementInHostCoordinates(Point intersectingPoint)
        {
            return INTERNAL_HtmlDomManager.FindElementInHostCoordinates_UsedBySimulatorToo(intersectingPoint.X, intersectingPoint.Y);
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
        /// An enumerable set of System.Windows.UIElement objects that are determined to
        /// be located in the visual tree composition at the specified point and within the
        /// specified subtee.
        /// </returns>
        public static IEnumerable<UIElement> FindElementsInHostCoordinates(Point intersectingPoint, UIElement subtree)
        {
            var list = new List<UIElement>();
            foreach (UIElement uie in INTERNAL_HtmlDomManager.FindElementsInHostCoordinates(intersectingPoint, subtree))
            {
                if (UIElement.EnablePointerEventsBase(uie))
                {
                    list.Add(uie);
                }
            }

            return list;
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

            var popupRoots = INTERNAL_PopupsManager.GetAllRootUIElements().OfType<PopupRoot>();

            foreach (var root in popupRoots)
            {
                if (root.INTERNAL_ParentWindow == window && root.INTERNAL_LinkedPopup.IsOpen
                                                         && root.INTERNAL_LinkedPopup.Child != null)
                {
                    result.Add(root.INTERNAL_LinkedPopup);
                }
            }

            return result;
        }

        /// <summary>
        /// Get the visual tree children of an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree children of an element.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualChildrenAndSelfIterator(element).Skip(1);
            return null;
        }

        /// <summary>
        /// Get the visual tree children of an element and the element itself.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The visual tree children of an element and the element itself.
        /// </returns>
        private static IEnumerable<DependencyObject> GetVisualChildrenAndSelfIterator(DependencyObject element)
        {
            yield return element;

            int count = GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                yield return GetChild(element, i);
            }
        }

        [OpenSilver.NotImplemented]
        public static IEnumerable<UIElement> FindElementsInHostCoordinates(Rect intersectingRect, UIElement subtree)
        {
            return null;
        }
    }
}