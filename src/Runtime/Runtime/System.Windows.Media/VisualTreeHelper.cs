

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
            if (reference is UIElement)
                return ((UIElement)reference).INTERNAL_VisualParent;
            else
                return null;
        }

        /// <summary>
        /// Returns the number of children that exist in an object's child collection in the visual tree.
        /// </summary>
        /// <param name="reference">The source visual.</param>
        /// <returns>The number of visual children for the provided source visual.</returns>
        public static int GetChildrenCount(DependencyObject reference)
        {
            List<DependencyObject> children;

            if (reference is UIElement)
            {
                //-------------------------------------------------------------
                // If the reference object is currently loaded into the 
                // visual tree, we look at the "INTERNAL_VisualChildrenInformation"
                // property. Alternatively, we attempt to find the [ContentProperty]
                // attribute, which usually contains the name of the dependency
                // property that contains the children of the object.
                //-------------------------------------------------------------

                if (((UIElement)reference)._isLoaded)
                {
                    var visualChildrenInformation = ((UIElement)reference).INTERNAL_VisualChildrenInformation;
                    if (visualChildrenInformation != null)
                    {
                        return visualChildrenInformation.Count;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else if (TryGetChildrenUsingContentProperty(reference, out children)) //todo-perfs: the current implementation is O(N), so it may lead to O(N^2) algorithms.
                {
                    return children.Count;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                throw new InvalidOperationException("The argument is null, or is not a valid UIElement.");
            }
        }

        /// <summary>
        /// Using the provided index, obtains a specific child object of the provided object by examining the visual tree. Please note that the current implementation does not guarantee that the index corresponds to the visual order of the controls.
        /// </summary>
        /// <param name="reference">The object that holds the child collection.</param>
        /// <param name="childIndex">The index of the requested child object in the reference child collection. Please note that the current implementation does not guarantee that the index corresponds to the visual order of the controls.</param>
        /// <returns>The child object as referenced by childIndex.</returns>
        public static DependencyObject GetChild(DependencyObject reference, int childIndex)
        {
            //todo: the current implementation does not guarantee that the index corresponds to the visual order of the controls.

            //todo-perfs: the current implementation is O(N), so it may lead to O(N^2) algorithms.

            List<DependencyObject> children;

            if (reference is UIElement)
            {
                //-------------------------------------------------------------
                // If the reference object is currently loaded into the 
                // visual tree, we look at the "INTERNAL_VisualChildrenInformation"
                // property. Alternatively, we attempt to find the [ContentProperty]
                // attribute, which usually contains the name of the dependency
                // property that contains the children of the object.
                //-------------------------------------------------------------

                if (((UIElement)reference)._isLoaded)
                {
                    var visualChildrenInformation = ((UIElement)reference).INTERNAL_VisualChildrenInformation;
                    if (visualChildrenInformation != null)
                    {
                        var keys = visualChildrenInformation.Keys.ToArray();

                        //todo: the current implementation does not guarantee that the index corresponds to the visual order of the controls.

                        if (childIndex >= keys.Length)
                            throw new Exception(string.Format("The index '{0}' is out of bounds.", childIndex.ToString()));

                        return visualChildrenInformation[keys[childIndex]].INTERNAL_UIElement;
                    }
                    else
                    {
                        throw new Exception(string.Format("The index '{0}' is out of bounds.", childIndex.ToString()));
                    }
                }
                else if (TryGetChildrenUsingContentProperty(reference, out children))
                {
                    if (childIndex >= children.Count)
                        throw new Exception(string.Format("The index '{0}' is out of bounds.", childIndex.ToString()));

                    return children[childIndex];
                }
                else
                {
                    throw new Exception(string.Format("The index '{0}' is out of bounds.", childIndex.ToString()));
                }
            }
            else
            {
                throw new InvalidOperationException("The argument is null, or is not a valid UIElement.");
            }
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
        /// Retrieves a set of objects that are located within a specified point of an object's coordinate space. Note: the current implementation only returns the top-most element.
        /// </summary>
        /// <param name="intersectingPoint">The point to use as the determination point.</param>
        /// <param name="subtree">The object to search within.</param>
        /// <returns>An enumerable set of UIElement objects that are determined to be located
        /// in the visual tree composition at the specified point and within the specified
        /// subtree.  Note: the current implementation only returns the top-most element.</returns>
        public static IEnumerable<UIElement> FindElementsInHostCoordinates(Point intersectingPoint, UIElement subtree)
        {
            var elementAtCoordinates = FindElementInHostCoordinates(intersectingPoint);

            if (elementAtCoordinates != null)
            {
                if (subtree == null)
                {
                    return new List<UIElement>() { elementAtCoordinates };
                }
                else
                {
                    //-------------
                    // Here, it means that neither the result nor the subtree are null.
                    // So we verify that the result is in the subtree.
                    //-------------

                    // Walk up the visual tree until we find the subtree root:
                    UIElement current = elementAtCoordinates as UIElement;
                    while (current != null)
                    {
                        if (current == subtree)
                        {
                            break;
                        }
                        else
                        {
                            current = current.INTERNAL_VisualParent as UIElement;
                        }
                    }

                    bool elementIsInSubtree = (current != null);

                    if (elementIsInSubtree)
                    {
                        return new List<UIElement>() { elementAtCoordinates };
                    }
                    else
                    {
                        return new List<UIElement>();
                    }
                }
            }
            else
            {
                return new List<UIElement>();
            }
        }

        private static bool TryGetChildrenUsingContentProperty(DependencyObject depObj, out List<DependencyObject> children)
        {
            Type depObjType = depObj.GetType();

            // Attempt to find the attribute [ContentProperty("Children")] on the object:
#if !BRIDGE
            ContentPropertyAttribute contentPropertyAttribute
                = (ContentPropertyAttribute)Attribute.GetCustomAttribute(depObjType, typeof(ContentPropertyAttribute));
#else
            
            ContentPropertyAttribute contentPropertyAttribute
                = (ContentPropertyAttribute)GetCustomAttribute(depObjType.GetCustomAttributes(true), typeof(ContentPropertyAttribute));
#endif
            
            if (contentPropertyAttribute != null)
            {
                // Get the name of the dependency property that contains the children:
                string contentPropertyName = contentPropertyAttribute.Name;

                // Attempt to find the property using Reflection:
                PropertyInfo propertyInfo = depObjType.GetProperty(contentPropertyName);

                //BRIDGETODO 
                //Verify that GetGetMethod() == GetMethod
#if !BRIDGE
                if (propertyInfo != null && propertyInfo.CanRead && propertyInfo.GetGetMethod() != null)
#else
                if (propertyInfo != null && propertyInfo.CanRead && propertyInfo.GetMethod != null)
#endif
                {
                    // Attempt to read the value of the content property:
                    object value = propertyInfo.GetValue(depObj);

                    children = new List<DependencyObject>();

                    // If the content of the content property is a single item, we return it. Otherwise we return the whole collection:
                    if (value is UIElement)
                    {
                        children.Add((DependencyObject)value);
                    }
                    else if (value is IEnumerable && (!(value is string)))
                    {
                        foreach (var item in (IEnumerable)value)
                        {
                            if (!(item is UIElement))
                            {
                                children = null;

                                //--------
                                // ERROR: enumerable does not contain dependency objects
                                //--------
                                return false;
                            }
                            else
                            {
                                children.Add((DependencyObject)item);
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    children = null;
                    return false;
                }
            }
            else
            {
                children = null;
                return false;
            }
        }
    
        private static Attribute GetCustomAttribute(object[] arrayOfAttribute, Type typeWanted)
        {
            int k = 0;
            
            while (k < arrayOfAttribute.Length)
            {
                if (arrayOfAttribute[k].GetType() == typeWanted)
                    return (Attribute)arrayOfAttribute[k];
                ++k;
            }
            return null; // no attribute found
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