
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
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OpenSilver;
using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    public static class INTERNAL_VisualTreeManager
    {
        internal static bool EnablePerformanceLogging;
        internal static bool EnableOptimizationWhereCollapsedControlsAreNotRendered = true;

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
                    if (parent.VisualChildrenInformation != null
                        && parent.VisualChildrenInformation.ContainsKey(child))
                    {
                        // Remove the element from the DOM:
                        INTERNAL_HtmlDomManager.RemoveFromDom(child.OuterDiv);

                        // Remove the parent-specific wrapper around the child in the DOM (if any):
                        var optionalChildWrapper_OuterDomElement = parent.VisualChildrenInformation[child].WrapperDiv;
                        if (optionalChildWrapper_OuterDomElement != null)
                            INTERNAL_HtmlDomManager.RemoveFromDom(optionalChildWrapper_OuterDomElement);

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
                else if (parent.VisualChildrenInformation != null
                        && parent.VisualChildrenInformation.ContainsKey(child))
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
                    foreach (UIElement child in element.VisualChildrenInformation.Keys)
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
                    foreach (UIElement child in children.Keys)
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
                element._isLoaded = false;

                if (element is FrameworkElement fe)
                {
                    fe.RaiseUnloadedEvent();
                    fe.UnloadResources();
                }

                INTERNAL_HtmlDomManager.RemoveFromGlobalStore(element.OuterDiv);
                INTERNAL_HtmlDomManager.RemoveFromGlobalStore(element.InnerDiv);
            }

            // Reset all visual-tree related information:
            element.IsConnectedToLiveTree = false;
            element.IsUnloading = false;
            element.OuterDiv = null;
            element.InnerDiv = null;
            element.VisualChildrenInformation = null;
            element.RenderingIsDeferred = false;
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void MoveVisualChildInSameParent(UIElement child, UIElement parent, int newIndex, int oldIndex)
        {
            if (oldIndex < 0)
            {
                // setting oldIndex to -1 means we don't know the previous
                // position of the child. We have to iterate through all the
                // child to find it.
                MoveVisualChildInSameParent(child, parent, newIndex);
                return;
            }

            if (parent.VisualChildrenInformation.ContainsKey(child))
            {
                VisualChildInformation visualChildInformation = parent.VisualChildrenInformation[child];
                var domElementToMove = visualChildInformation.WrapperDiv ?? child.OuterDiv;

                //Not sure if this test is needed but at least we won't 
                // break anything if the element is not in the Visual tree
                if (domElementToMove != null)
                {
                    object domElementWhereToPlaceChildStuff = (parent.GetDomElementWhereToPlaceChild(child) ?? parent.InnerDiv);

                    object movedChild = OpenSilver.Interop.ExecuteJavaScript(
                        "$0.children[$1]",
                        domElementWhereToPlaceChildStuff,
                        oldIndex);


                    if (!Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("$0 == $1", movedChild, domElementToMove)))
                    {
                        throw new InvalidOperationException(string.Format("index '{0}' does match index of the element about to be moved.", oldIndex));
                    }

                    object nextSibling = OpenSilver.Interop.ExecuteJavaScript(
                        "$0.children[$1]",
                        domElementWhereToPlaceChildStuff,
                        newIndex);

                    if (nextSibling != null)
                    {
                        OpenSilver.Interop.ExecuteJavaScript(
                            "$0.insertBefore($1, $2)",
                            domElementWhereToPlaceChildStuff,
                            domElementToMove,
                            nextSibling);
                    }
                    else
                    {
                        OpenSilver.Interop.ExecuteJavaScript(
                            "$0.appendChild($1)",
                            domElementWhereToPlaceChildStuff,
                            domElementToMove);
                    }
                }
            }
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void MoveVisualChildInSameParent(UIElement child, UIElement parent, int index)
        {
            if (parent.VisualChildrenInformation.ContainsKey(child))
            {
                VisualChildInformation visualChildInformation = parent.VisualChildrenInformation[child];
                var domElementToMove = visualChildInformation.WrapperDiv;
                if (domElementToMove == null)
                    domElementToMove = child.OuterDiv;

                if (domElementToMove != null) //Not sure if this test is needed but at least we won't break anything if the element is not in the Visual tree
                {
                    object domElementWhereToPlaceChildStuff = (parent.GetDomElementWhereToPlaceChild(child) ?? parent.InnerDiv);
                    //todo: see if there is a way to know the index of the domElement in its parent without looping through the list (where we find i in the js below).
                    OpenSilver.Interop.ExecuteJavaScript(@"
var actualIndex = $1;
var i = 0;
while (i < actualIndex && $0.children[i]!=$2) { 
    ++i;
}
if(i < actualIndex) {
    ++actualIndex; //to compensate the fact that the item that will be moved was before the next sibling
}
var nextSibling = $0.children[$1];
if(nextSibling != undefined) {
    $0.insertBefore($2, nextSibling);
} else {
    $0.appendChild($2);
}", domElementWhereToPlaceChildStuff, index, domElementToMove);
                }
            }
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

                    AttachVisualChild_Private(child, parent, index);

                    if (EnablePerformanceLogging)
                    {
                        Profiler.ConsoleTimeEnd(label);
                    }
                }
                else if (child.VisualParent is not null && !ReferenceEquals(child.VisualParent, parent))
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

        static void AttachVisualChild_Private(UIElement child, UIElement parent, int index)
        {
            //
            // THIS IS WHAT THE FINAL STRUCTURE IN THE DOM TREE WILL LOOK LIKE:
            //
            //     domElementWhereToPlaceChildStuff
            //     --- [wrapperForChild]
            //     --- --- [innerDivOfWrapperForChild]
            //     --- --- --- [additionalOutsideDivForMargins, aka BoxSizing]
            //     --- --- --- --- outerDomElement
            //

            //--------------------------------------------------------
            // PREPARE THE PARENT:
            //--------------------------------------------------------

#if PERFSTAT
            var t0 = Performance.now();
#endif

            // Prepare the parent DOM structure so that it is ready to contain the child (for example, in case of a grid, we need to (re)create the rows and columns where to place the elements).
            //parent.INTERNAL_UpdateDomStructureIfNecessary();

#pragma warning disable CS0618
            object domElementWhereToPlaceChildStuff = (parent.GetDomElementWhereToPlaceChild(child) ?? parent.InnerDiv);
#pragma warning restore CS0618

            // A "wrapper for child" is sometimes needed between the child and the parent (for example in case of a grid).
            // It is usually one or more DIVs that fit in-between the child and the parent, and that are used to position
            // the child within the parent.
#pragma warning disable CS0618
            object wrapperForChild = parent.CreateDomChildWrapper(domElementWhereToPlaceChildStuff, out object innerDivOfWrapperForChild, index);
#pragma warning restore CS0618
            bool doesParentRequireToCreateAWrapperForEachChild = wrapperForChild is not null && innerDivOfWrapperForChild is not null;

            // Remember the information about the "VisualChildren"
            parent.VisualChildrenInformation ??= new Dictionary<UIElement, VisualChildInformation>();
            parent.VisualChildrenInformation.Add(child, new VisualChildInformation(wrapperForChild));

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Prepare the parent", t0);
#endif

            //--------------------------------------------------------
            // CONTINUE WITH THE OTHER STEPS
            //--------------------------------------------------------
            
            AttachVisualChild_Private_MainSteps(
                child,
                parent,
                index,
                doesParentRequireToCreateAWrapperForEachChild,
                innerDivOfWrapperForChild,
                domElementWhereToPlaceChildStuff,
                wrapperForChild);
        }

        static void AttachVisualChild_Private_MainSteps(UIElement child,
            UIElement parent,
            int index,
            bool doesParentRequireToCreateAWrapperForEachChild,
            object innerDivOfWrapperForChild,
            object domElementWhereToPlaceChildStuff,
            object wrapperForChild)
        {
            //--------------------------------------------------------
            // PREPARE THE CHILD:
            //--------------------------------------------------------

#if PERFSTAT
            var t2 = Performance.now();
#endif

            // Determine where to place the child:
            object whereToPlaceTheChild = (doesParentRequireToCreateAWrapperForEachChild
                    ? innerDivOfWrapperForChild
                    : domElementWhereToPlaceChildStuff);

            child.IsConnectedToLiveTree = true;

            // Set the "ParentWindow" property so that the element knows where to display popups:
            child.ParentWindow = parent.ParentWindow;

            // Create and append the DOM structure of the Child:
            object domElementWhereToPlaceGrandChildren = null;
            INTERNAL_HtmlDomElementReference outerDomElement;
            if (child.HtmlRepresentation == null)
            {
                if (child is Control control && control.HasTemplate)
                {
                    outerDomElement = (INTERNAL_HtmlDomElementReference)control.CreateDomElementForControlTemplate(whereToPlaceTheChild, out domElementWhereToPlaceGrandChildren);
                }
                else
                {
                    outerDomElement = (INTERNAL_HtmlDomElementReference)child.CreateDomElement(whereToPlaceTheChild, out domElementWhereToPlaceGrandChildren);
                }
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                outerDomElement = (INTERNAL_HtmlDomElementReference)INTERNAL_HtmlDomManager.CreateDomFromStringAndAppendIt(child.HtmlRepresentation, whereToPlaceTheChild, child);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            // For debugging purposes (to better read the output html), add a class to the outer DIV
            // that tells us the corresponding type of the element (Border, StackPanel, etc.):
            if (Features.DOM.AssignClass)
            {
                INTERNAL_HtmlDomManager.AddCSSClass(outerDomElement, child.GetType().ToString());
            }

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Prepare the child", t2);
#endif

            //--------------------------------------------------------
            // REMEMBER ALL INFORMATION FOR FUTURE USE:
            //--------------------------------------------------------

            // Remember the DIVs:
            child.OuterDiv = outerDomElement;
            child.InnerDiv = (INTERNAL_HtmlDomElementReference)domElementWhereToPlaceGrandChildren;

            //--------------------------------------------------------
            // HANDLE EVENTS:
            //--------------------------------------------------------

            // Register DOM events if any:
            child.INTERNAL_AttachToDomEvents();

            //--------------------------------------------------------
            // SET "ISLOADED" PROPERTY AND CALL "ONATTACHED" EVENT:
            //--------------------------------------------------------

            if (child is FrameworkElement)
            {
                ((FrameworkElement)child).LoadResources();
            }

            // Tell the control that it is now present into the visual tree:
            child._isLoaded = true;

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

            // Defer rendering when the control is not visible to when becomes visible (note: when this option is enabled, we do not apply the CSS properties of the UI elements that are not visible. Those property are applied later, when the control becomes visible. This option results in improved performance.)
            bool enableDeferredRenderingOfCollapsedControls = EnableOptimizationWhereCollapsedControlsAreNotRendered;

            if (enableDeferredRenderingOfCollapsedControls && !child.IsVisible)
            {
                child.RenderingIsDeferred = true;
                if (child.Visibility == Visibility.Collapsed)
                {
                    INTERNAL_HtmlDomManager.SetVisible(child.OuterDiv, false);
                }
            }
            else
            {
                RenderElementsAndRaiseChangedEventOnAllDependencyProperties(child);
            }

            //--------------------------------------------------------
            // RAISE THE "LOADED" EVENT:
            //--------------------------------------------------------
#if PERFSTAT
            var t11 = Performance.now();
#endif

            // Raise the "Loaded" event: (note: in XAML, the "loaded" event of the children is called before the "loaded" event of the parent)
            if (child is FrameworkElement fe)
            {
                fe.RaiseLoadedEvent();
            }

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Raise Loaded event", t11);
#endif
        }
        public static bool IsElementInVisualTree(UIElement element) => element.IsConnectedToLiveTree && !element.IsUnloading;

        internal static void RenderElementsAndRaiseChangedEventOnAllDependencyProperties(UIElement uie)
        {
            //--------------------------------------------------------------
            // RAISE "PROPERTYCHANGED" FOR ALL THE PROPERTIES THAT HAVE 
            // A VALUE THAT HAS BEEN SET, INCLUDING ATTACHED PROPERTIES, 
            // AND CALL THE "METHOD TO UPDATE DOM"
            //--------------------------------------------------------------

            // This is used to force a redraw of all the properties that are 
            // set on the object (including Attached Properties!). For 
            // example, if a Border has a colored background, this is the 
            // moment when that color will be applied. Properties that have 
            // no value set by the user are not concerned (their default 
            // state is rendered elsewhere).

            if (uie.EffectiveValuesCount > 0)
            {
                // we copy the Dictionary so that the foreach doesn't break when 
                // we modify a DependencyProperty inside the Changed of another 
                // one (which causes it to be added to the Dictionary).
                // we exclude properties where source is set to default because
                // it means they have been set at some point, and unset afterward,
                // so we should not call the PropertyChanged callback.

                Storage[] storages = ArrayPool<Storage>.Shared.Rent(uie.EffectiveValuesCount);
                int length = 0;
                foreach (KeyValuePair<int, Storage> kvp in uie.EffectiveValues)
                {
                    if (kvp.Value.Entry.FullValueSource == (FullValueSource)BaseValueSourceInternal.Default)
                    {
                        continue;
                    }

                    storages[length++] = kvp.Value;
                }

                Span<Storage> span = storages.AsSpan(0, length);
                try
                {
                    foreach (Storage storage in span)
                    {
                        DependencyProperty dp = DependencyProperty.RegisteredPropertyList[storage.PropertyIndex];
                        if (dp.GetMetadata(uie.DependencyObjectType) is not PropertyMetadata metadata)
                        {
                            continue;
                        }
                        
                        object value = null;
                        bool valueWasRetrieved = false;

                        //--------------------------------------------------
                        // Call "MethodToUpdateDom"
                        //--------------------------------------------------
                        if (metadata.MethodToUpdateDom != null)
                        {
                            if (!valueWasRetrieved)
                            {
                                value = DependencyObjectStore.GetEffectiveValue(storage.Entry, RequestFlags.FullyResolved);
                                valueWasRetrieved = true;
                            }

                            // Call the "Method to update DOM"
                            metadata.MethodToUpdateDom(uie, value);
                        }

                        if (metadata.MethodToUpdateDom2 != null)
                        {
                            if (!valueWasRetrieved)
                            {
                                value = DependencyObjectStore.GetEffectiveValue(storage.Entry, RequestFlags.FullyResolved);
                                valueWasRetrieved = true;
                            }

                            // DependencyProperty.UnsetValue for the old value signify that
                            // the old value should be ignored.
                            metadata.MethodToUpdateDom2(
                                uie,
                                DependencyProperty.UnsetValue,
                                value);
                        }

                        //--------------------------------------------------
                        // Call PropertyChanged
                        //--------------------------------------------------

                        if (metadata.PropertyChangedCallback != null
#pragma warning disable CS0618 // Type or member is obsolete
                            && metadata.CallPropertyChangedWhenLoadedIntoVisualTree != WhenToCallPropertyChangedEnum.Never)
#pragma warning restore CS0618 // Type or member is obsolete
                        {
                            if (!valueWasRetrieved)
                            {
                                value = DependencyObjectStore.GetEffectiveValue(storage.Entry, RequestFlags.FullyResolved);
                                valueWasRetrieved = true;
                            }
                            
                            // Raise the "PropertyChanged" event
                            metadata.PropertyChangedCallback(
                                uie,
                                new DependencyPropertyChangedEventArgs(value, value, dp, metadata));
                        }
                    }
                }
                finally
                {
                    ArrayPool<Storage>.Shared.Return(storages, false);
                    span.Clear();
                }
            }

            if (uie.IsHitTestable)
            {
                uie.SetPointerEvents(true);
            }
        }

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
