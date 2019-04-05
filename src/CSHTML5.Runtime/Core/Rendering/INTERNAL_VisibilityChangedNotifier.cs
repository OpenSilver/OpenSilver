
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5;
using System;
using System.Collections.Generic;
#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internal
{
    /// <summary>
    /// The goal of this class is to allow elements to subscribe
    /// to the Visibility_Changed event of any of its ancestors.
    /// It is useful for example to work around the fact that
    /// drawing on a hidden html canvas (in the XAML "Path"
    /// control for example) is not possible, so if a html
    /// canvas is hidden, we subscribe to the Visibility_Changed
    /// event of its ancestors, so as to refresh it when it becomes
    /// visible.
    /// </summary>
    internal static class INTERNAL_VisibilityChangedNotifier
    {
        // Those two dictionaries are one the inverse of the other. They are kept in sync.
        static Dictionary<DependencyObject, HashSet2<DependencyObject>> ElementsToListeners = new Dictionary<DependencyObject, HashSet2<DependencyObject>>();
        static Dictionary<DependencyObject, HashSet2<DependencyObject>> ElementsToListened = new Dictionary<DependencyObject, HashSet2<DependencyObject>>();
        static Dictionary<DependencyObject, Action> ListenersToCallbacks = new Dictionary<DependencyObject, Action>();

        public static void StartListeningToAncestorsVisibilityChanged(UIElement listeningElement, Action onAncestorVisibilityChanged)
        {
            // Get the list of ancestors, including the element itself:
            List<DependencyObject> ancestorsAndSelf = new List<DependencyObject>();
            DependencyObject element = listeningElement; // Note: we include the element itself (for example, if a Path's Visibility is Collapsed, we do not draw it, but when it becomes Visible, we should draw it).
            while (element != null)
            {
                ancestorsAndSelf.Add(element);
                element = ((UIElement)element).INTERNAL_VisualParent;
            }

            // Subscribe to each ancestor:
            foreach (var ancestor in ancestorsAndSelf)
            {
                AddToDictionaryIfNotAlreadyThere(ElementsToListeners, ancestor, listeningElement);
                AddToDictionaryIfNotAlreadyThere(ElementsToListened, listeningElement, ancestor);
            }

            // Remember the callback:
            ListenersToCallbacks[listeningElement] = onAncestorVisibilityChanged;
        }

        public static void StopListeningToAncestorsVisibilityChanged(DependencyObject listeningElement)
        {
            // Get the list of elements that the element was listening to:
            HashSet2<DependencyObject> listenedElements = null;
            if (ElementsToListened.ContainsKey(listeningElement))
            {
                listenedElements = ElementsToListened[listeningElement];
            }

            // Unsubscribe from each listened element (the ancestors):
            if (listenedElements != null)
            {
                foreach (DependencyObject listenedElement in listenedElements)
                {
                    RemoveFromDictionaryIfFound(ElementsToListeners, listenedElement, listeningElement);
                }
            }

            // Remove the entry that stores the listened elements:
            if (listenedElements != null)
                ElementsToListened.Remove(listeningElement);

            // Remove the callback:
            if (ListenersToCallbacks.ContainsKey(listeningElement))
                ListenersToCallbacks.Remove(listeningElement);
        }

        public static void NotifyListenersThatVisibilityHasChanged(DependencyObject elementThatHasChanged)
        {
            if (ElementsToListeners.ContainsKey(elementThatHasChanged))
            {
                var listeners = ElementsToListeners[elementThatHasChanged];

                // We make a copy of the collection of listeners because the collection may be modified during the "foreach" statement below:
                var listenersCopy = new List<DependencyObject>(listeners);

                // Notify each listener:
                foreach (var listener in listenersCopy)
                {
                    if (ListenersToCallbacks.ContainsKey(listener))
                    {
                        var onAncestorVisibilityChanged = ListenersToCallbacks[listener];
                        if (onAncestorVisibilityChanged != null)
                            onAncestorVisibilityChanged();
                    }
                }
            }
        }

        static void AddToDictionaryIfNotAlreadyThere(Dictionary<DependencyObject, HashSet2<DependencyObject>> dictionary, DependencyObject key, DependencyObject value)
        {
            HashSet2<DependencyObject> list;
            if (dictionary.ContainsKey(key))
                list = dictionary[key];
            else
            {
                list = new HashSet2<DependencyObject>();
                dictionary[key] = list;
            }
            if (!list.Contains(value))
                list.Add(value);
        }

        static void RemoveFromDictionaryIfFound(Dictionary<DependencyObject, HashSet2<DependencyObject>> dictionary, DependencyObject key, DependencyObject value)
        {
            if (dictionary.ContainsKey(key))
            {
                HashSet2<DependencyObject> list = dictionary[key];
                if (list.Contains(value))
                    list.Remove(value);
            }
        }

        /// <summary>
        /// Determines whether an element is visible. It can be hidden if any of the
        /// ancestors have the "Visibility" property set to "Collapsed".
        /// </summary>
        /// <param name="element">UI Element</param>
        /// <returns>True if the element is visible in the visual tree, false otherwise.</returns>
#if !BRIDGE
        [JSReplacement("(!(typeof $element.INTERNAL_OuterDomElement === 'undefined' || $element.INTERNAL_OuterDomElement === null) && !document.doesElementInheritDisplayNone($element.INTERNAL_OuterDomElement))")]
#else
        [Template("(!(typeof {element}.INTERNAL_OuterDomElement === 'undefined' || {element}.INTERNAL_OuterDomElement === null) && !document.doesElementInheritDisplayNone({element}.INTERNAL_OuterDomElement))")]
#endif
        public static bool IsElementVisible(UIElement element)
        {
            // Below is the Simulator version (the JS version is above, cf. "JSReplacement" attribute).
            // In the Simulator version we check element.Visibility for best performance:
            while (element != null)
            {
                if (element.Visibility == Visibility.Collapsed)
                    return false;
                element = element.INTERNAL_VisualParent as UIElement;
            }
            return true;

            // Commented on 2018.02.02 in order to reduce the number of Interop (C# to JS) calls in the Simulator in order to improve performance.
            /*
            if (element.INTERNAL_OuterDomElement != null)
            {
                return !Convert.ToBoolean(Interop.ExecuteJavaScript("document.doesElementInheritDisplayNone($0)", element.INTERNAL_OuterDomElement));
                //// Alternative that doesn't work very well:
                //return !Convert.ToBoolean(Interop.ExecuteJavaScript("($0.offsetParent === null)", element.INTERNAL_OuterDomElement));
            }
            else
            {
                return false;
            }
             */
        }
    }
}
