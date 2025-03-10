
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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using OpenSilver;

namespace CSHTML5.Internal
{
    public static class INTERNAL_VisualTreeManager
    {
        internal static bool EnablePerformanceLogging;
        internal static bool EnableOptimizationWhereCollapsedControlsAreNotRendered = true;

        internal static void DetachPopupRoot(PopupRoot popupRoot)
        {
            Debug.Assert(popupRoot is not null);

            if (IsElementInVisualTree(popupRoot))
            {
                // Remove the element from the DOM:
                INTERNAL_HtmlDomManager.RemoveNodeNative(popupRoot.OuterDiv);

                // Detach Element  
                UnloadSubTree(popupRoot);
            }
            else
            {
                UnloadVisual(popupRoot);
            }
        }

        public static void DetachVisualChildIfNotNull(UIElement child, UIElement parent)
        {
#if PERFSTAT
            var t = Performance.now();
#endif
            if (child != null)
            {
                if (IsElementInVisualTree(child))
                {
                    // Verify that the child is really a child of the specified control:
                    if (parent.VisualChildrenInformation != null && parent.VisualChildrenInformation.Contains(child))
                    {
                        // Remove the element from the DOM:
                        INTERNAL_HtmlDomManager.RemoveFromDom(child.OuterDiv);

                        // Remove the element from the parent's children collection:
                        parent.VisualChildrenInformation.Remove(child);

                        //Detach Element  
                        UnloadSubTree(child);
                    }
                    else
                    {
                        throw new Exception(
                            string.Format("Cannot detach the element '{0}' because it is not a child of the element '{1}'.",
                                          child.GetType().ToString(),
                                          parent.GetType().ToString()));
                    }
                }
                else if (parent.VisualChildrenInformation != null && parent.VisualChildrenInformation.Contains(child))
                {
                    // Remove the element from the parent's children collection:
                    parent.VisualChildrenInformation.Remove(child);
                    UnloadVisual(child);
                }
            }
#if PERFSTAT
            Performance.Counter("DetachVisualChildIfNotNull", t);
#endif
        }

        private static void UnloadSubTree(UIElement element)
        {
            PropagateIsUnloading(element);
            UnloadVisualRec(element);

            static void PropagateIsUnloading(UIElement element)
            {
                element.IsUnloading = true;
                if (element.VisualChildrenInformation is not null)
                {
                    foreach (UIElement child in element.VisualChildrenInformation)
                    {
                        PropagateIsUnloading(child);
                    }
                }
            }

            static void UnloadVisualRec(UIElement element)
            {
                var children = element.VisualChildrenInformation;
                UnloadVisual(element);
                if (children is not null)
                {
                    foreach (UIElement child in children)
                    {
                        UnloadVisualRec(child);
                    }
                }
            }
        }

        private static void UnloadVisual(UIElement element)
        {
            element.IsUnloading = true;

            if (element.IsConnectedToLiveTree)
            {
                InputManager.Current.OnElementRemoved(element);

                // Call the "OnDetached" of the element. This is particularly useful for elements to
                // clear any references they have to DOM elements. For example, the Grid will use it
                // to set its _tableDiv to null.
                element.INTERNAL_OnDetachedFromVisualTree();

                //We detach the events from the dom element:
                element.INTERNAL_DetachFromDomEvents();

                // Call the "Unloaded" event: (note: in XAML, the "unloaded" event of the parent is called
                // before the "unloaded" event of the children)
                element.IsLoadedCache = false;

                if (element is FrameworkElement fe)
                {
                    fe.RaiseUnloadedEvent();
                    fe.UnloadResources();
                }

                INTERNAL_HtmlDomManager.RemoveFromGlobalStore(element.OuterDiv);
            }

            // Reset all visual-tree related information:
            element.IsConnectedToLiveTree = false;
            element.IsUnloading = false;
            element.OuterDiv = null;
            element.VisualChildrenInformation = null;
            element.IsRenderingSuspended = false;
        }

        public static void AttachVisualChildIfNotAlreadyAttached(UIElement child, UIElement parent, int index = -1)
        {
            // Modify the visual tree only if the parent element is itself in the visual tree:
            if (child != null && IsElementInVisualTree(parent))
            {
                // Ensure that the child is not already attached:
                if (!child.IsConnectedToLiveTree)
                {
                    string label = "";
                    if (EnablePerformanceLogging)
                    {
                        label = "Attach" + " - " + child.GetType().Name + " - " + child.GetHashCode().ToString();
                        Profiler.ConsoleTime(label);
                    }

                    AttachVisualChild_Private(child, parent);

                    if (EnablePerformanceLogging)
                    {
                        Profiler.ConsoleTimeEnd(label);
                    }
                }
                else if (child.InternalVisualParent is not null && !ReferenceEquals(child.InternalVisualParent, parent))
                {
                    throw new InvalidOperationException("The element already has a parent. An element cannot appear in multiple locations in the Visual Tree. Remove the element from the Visual Tree before adding it elsewhere.");
                }
                else
                {
                    // Nothing to do: the element is already attached to the specified parent.
                    return; //prevent from useless call to INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE().
                }
            }
        }

        static void AttachVisualChild_Private(UIElement child, UIElement parent)
        {
            //--------------------------------------------------------
            // PREPARE THE PARENT:
            //--------------------------------------------------------

            // Remember the information about the "VisualChildren"
            parent.VisualChildrenInformation ??= new HashSet<UIElement>();
            parent.VisualChildrenInformation.Add(child);

            //--------------------------------------------------------
            // CONTINUE WITH THE OTHER STEPS
            //--------------------------------------------------------
            
            AttachVisualChild_Private_MainSteps(
                child,
                parent);
        }

        static void AttachVisualChild_Private_MainSteps(UIElement child, UIElement parent)
        {
            //--------------------------------------------------------
            // PREPARE THE CHILD:
            //--------------------------------------------------------

            var childFE = child as FrameworkElement;

            child.IsConnectedToLiveTree = true;

            // Set the "ParentWindow" property so that the element knows where to display popups:
            child.ParentWindow = parent.ParentWindow;

            // Create and append the DOM structure of the Child:
            var outerDomElement = (INTERNAL_HtmlDomElementReference)child.CreateDomElement(parent.OuterDiv, out _);

            // For debugging purposes (to better read the output html), add a class to the outer DIV
            // that tells us the corresponding type of the element (Border, StackPanel, etc.):
            if (Features.DOM.AssignClass)
            {
                INTERNAL_HtmlDomManager.AddCSSClass(outerDomElement, child.GetType().ToString());
            }

            //--------------------------------------------------------
            // REMEMBER ALL INFORMATION FOR FUTURE USE:
            //--------------------------------------------------------

            // Remember the DIVs:
            child.OuterDiv = outerDomElement;

            //--------------------------------------------------------
            // HANDLE EVENTS:
            //--------------------------------------------------------

            // Register DOM events if any:
            child.INTERNAL_AttachToDomEvents();

            //--------------------------------------------------------
            // SET "ISLOADED" PROPERTY AND CALL "ONATTACHED" EVENT:
            //--------------------------------------------------------

            childFE?.LoadResources();

            // Tell the control that it is now present into the visual tree:
            child.IsLoadedCache = true;

            // Raise the "OnAttached" event:
            child.INTERNAL_OnAttachedToVisualTree(); // IMPORTANT: Must be done BEFORE "RaiseChangedEventOnAllDependencyProperties" (for example, the ItemsControl uses this to initialize its visual)

            // INTERNAL_OnAttachedToVisualTree will fire the Loaded event on children, so we need to make
            // sure that 'child' has not been disconnected from the visual tree in the process.
            // We check outer div rather than _isLoaded because 'child' may have been removed and added back,
            // in which case the code below would run twice.
            if (child.OuterDiv != outerDomElement)
            {
                return;
            }

            //--------------------------------------------------------
            // RENDER THE ELEMENTS BY APPLYING THE CSS PROPERTIES:
            //--------------------------------------------------------

            if (EnableOptimizationWhereCollapsedControlsAreNotRendered && child.IsInCollapsedTree)
            {
                child.IsRenderingSuspended = true;
                if (child.Visibility == Visibility.Collapsed)
                {
                    INTERNAL_HtmlDomManager.SetVisibility(child.OuterDiv, Visibility.Collapsed);
                }
            }
            else
            {
                child.RenderVisual();
            }

            //--------------------------------------------------------
            // RAISE THE "LOADED" EVENT:
            //--------------------------------------------------------

            // Raise the "Loaded" event: (note: in XAML, the "loaded" event of the children is called before the "loaded" event of the parent)
            childFE?.RaiseLoadedEvent();
        }

        public static bool IsElementInVisualTree(UIElement element) => element.IsConnectedToLiveTree && !element.IsUnloading;

        /// <summary>
        /// Returns the first child of the specified type (recursively).
        /// </summary>
        /// <typeparam name="T">The type to lookup.</typeparam>
        /// <param name="parent">The parent element.</param>
        /// <returns>The first child of the specified type.</returns>
        public static T GetChildOfType<T>(UIElement parent) where T : UIElement
        {
            if (parent == null)
                return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as UIElement;
                var result = child as T ?? GetChildOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
