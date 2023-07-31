
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
using System.Linq;
using System.Threading.Tasks;
#if OPENSILVER
using OpenSilver; 
#endif

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
#endif

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
                    if (parent.INTERNAL_VisualChildrenInformation != null
                        && parent.INTERNAL_VisualChildrenInformation.ContainsKey(child))
                    {
                        // Remove the element from the DOM:
                        string stringForDebugging = !IsRunningInJavaScript() ? "Removing " + child.GetType().ToString() : null;
                        INTERNAL_HtmlDomManager.RemoveFromDom(child.INTERNAL_OuterDomElement, stringForDebugging);

                        // Remove the parent-specific wrapper around the child in the DOM (if any):
                        var optionalChildWrapper_OuterDomElement = parent.INTERNAL_VisualChildrenInformation[child].INTERNAL_OptionalChildWrapper_OuterDomElement;
                        if (optionalChildWrapper_OuterDomElement != null)
                            INTERNAL_HtmlDomManager.RemoveFromDom(optionalChildWrapper_OuterDomElement);

                        // Remove the element from the parent's children collection:
                        parent.INTERNAL_VisualChildrenInformation.Remove(child);

                        //Detach Element  
                        DetachVisualChildren(child);

                        INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE();
                    }
                    else
                    {
                        throw new Exception(
                            string.Format("Cannot detach the element '{0}' because it is not a child of the element '{1}'.",
                                          child.GetType().ToString(),
                                          parent.GetType().ToString()));
                    }
                }
                else if (parent.INTERNAL_VisualChildrenInformation != null
                        && parent.INTERNAL_VisualChildrenInformation.ContainsKey(child))
                {
                    // Remove the element from the parent's children collection:
                    parent.INTERNAL_VisualChildrenInformation.Remove(child);
                    DetachElement(child);
                }
            }
#if PERFSTAT
            Performance.Counter("DetachVisualChildIfNotNull", t);
#endif
        }

        private static void DetachVisualChildren(UIElement element)
        {
            PropagateIsUnloading(element);

            var queue = new Queue<UIElement>();
            queue.Enqueue(element);

            while (queue.Count > 0)
            {
                UIElement e = queue.Dequeue();
                if (e.INTERNAL_VisualChildrenInformation is not null)
                {
                    foreach (UIElement child in e.INTERNAL_VisualChildrenInformation.Keys)
                    {
                        queue.Enqueue(child);
                    }
                }

                DetachElement(e);
            }

            static void PropagateIsUnloading(UIElement element)
            {
                element.IsUnloading = true;
                if (element.INTERNAL_VisualChildrenInformation is not null)
                {
                    foreach (UIElement child in element.INTERNAL_VisualChildrenInformation.Keys)
                    {
                        PropagateIsUnloading(child);
                    }
                }
            }
        }

        private static void DetachElement(UIElement element)
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

                INTERNAL_HtmlDomManager.RemoveFromGlobalStore(element.INTERNAL_OuterDomElement as INTERNAL_HtmlDomElementReference);
                INTERNAL_HtmlDomManager.RemoveFromGlobalStore(element.INTERNAL_InnerDomElement as INTERNAL_HtmlDomElementReference);
            }

            // Reset all visual-tree related information:
            element.IsConnectedToLiveTree = false;
            element.IsUnloading = false;
            element.INTERNAL_OuterDomElement = null;
            element.INTERNAL_InnerDomElement = null;
            element.INTERNAL_VisualChildrenInformation = null;
            element.RenderingIsDeferred = false;
        }

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

            if (parent.INTERNAL_VisualChildrenInformation.ContainsKey(child))
            {
                INTERNAL_VisualChildInformation visualChildInformation = parent.INTERNAL_VisualChildrenInformation[child];
                var domElementToMove = visualChildInformation.INTERNAL_OptionalChildWrapper_OuterDomElement ?? child.INTERNAL_OuterDomElement;

                //Not sure if this test is needed but at least we won't 
                // break anything if the element is not in the Visual tree
                if (domElementToMove != null)
                {
                    object domElementWhereToPlaceChildStuff = (parent.GetDomElementWhereToPlaceChild(child) ?? parent.INTERNAL_InnerDomElement);

                    object movedChild = CSHTML5.Interop.ExecuteJavaScript(
                        "$0.children[$1]",
                        domElementWhereToPlaceChildStuff,
                        oldIndex);


                    if (!Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0 == $1", movedChild, domElementToMove)))
                    {
                        throw new InvalidOperationException(string.Format("index '{0}' does match index of the element about to be moved.", oldIndex));
                    }

                    object nextSibling = CSHTML5.Interop.ExecuteJavaScript(
                        "$0.children[$1]",
                        domElementWhereToPlaceChildStuff,
                        newIndex);

                    if (nextSibling != null)
                    {
                        CSHTML5.Interop.ExecuteJavaScript(
                            "$0.insertBefore($1, $2)",
                            domElementWhereToPlaceChildStuff,
                            domElementToMove,
                            nextSibling);
                    }
                    else
                    {
                        CSHTML5.Interop.ExecuteJavaScript(
                            "$0.appendChild($1)",
                            domElementWhereToPlaceChildStuff,
                            domElementToMove);
                    }
                }
            }
        }

        public static void MoveVisualChildInSameParent(UIElement child, UIElement parent, int index)
        {
            if (parent.INTERNAL_VisualChildrenInformation.ContainsKey(child))
            {
                INTERNAL_VisualChildInformation visualChildInformation = parent.INTERNAL_VisualChildrenInformation[child];
                var domElementToMove = visualChildInformation.INTERNAL_OptionalChildWrapper_OuterDomElement;
                if (domElementToMove == null)
                    domElementToMove = child.INTERNAL_OuterDomElement;

                if (domElementToMove != null) //Not sure if this test is needed but at least we won't break anything if the element is not in the Visual tree
                {
                    object domElementWhereToPlaceChildStuff = (parent.GetDomElementWhereToPlaceChild(child) ?? parent.INTERNAL_InnerDomElement);
                    //todo: see if there is a way to know the index of the domElement in its parent without looping through the list (where we find i in the js below).
                    Interop.ExecuteJavaScript(@"
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
                else if (child.INTERNAL_VisualParent is not null && !ReferenceEquals(child.INTERNAL_VisualParent, parent))
                {
                    throw new InvalidOperationException("The element already has a parent. An element cannot appear in multiple locations in the Visual Tree. Remove the element from the Visual Tree before adding it elsewhere.");
                }
                else
                {
                    // Nothing to do: the element is already attached to the specified parent.
                    return; //prevent from useless call to INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE().
                }

                INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE();
            }
        }

#if OLD_CODE_TO_OPTIMIZE_SIMULATOR_PERFORMANCE // Obsolete since Beta 13.4 on 2018.01.31 because we now use the Dispatcher instead (cf. the class "INTERNAL_SimulatorExecuteJavaScript")

#if !BRIDGE
        [JSReplacement("")]
#else
        [External]
#endif
        static void StartTransactionToOptimizeSimulatorPerformance()
        {
            INTERNAL_SimulatorPerformanceOptimizer.StartTransaction();
        }

#if !BRIDGE
        [JSReplacement("")]
#else
        [External]
#endif
        static void EndTransactionToOptimizeSimulatorPerformance()
        {
            INTERNAL_SimulatorPerformanceOptimizer.EndTransaction();
        }
#endif

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

            object domElementWhereToPlaceChildStuff = (parent.GetDomElementWhereToPlaceChild(child) ?? parent.INTERNAL_InnerDomElement);

            // A "wrapper for child" is sometimes needed between the child and the parent (for example in case of a grid).
            // It is usually one or more DIVs that fit in-between the child and the parent, and that are used to position
            // the child within the parent.
            object wrapperForChild = parent.CreateDomChildWrapper(domElementWhereToPlaceChildStuff, out object innerDivOfWrapperForChild, index);
            bool doesParentRequireToCreateAWrapperForEachChild = wrapperForChild is not null && innerDivOfWrapperForChild is not null;

            // Remember the information about the "VisualChildren"
            parent.INTERNAL_VisualChildrenInformation ??= new Dictionary<UIElement, INTERNAL_VisualChildInformation>();
            parent.INTERNAL_VisualChildrenInformation.Add(child,
                new INTERNAL_VisualChildInformation()
                {
                    INTERNAL_OptionalChildWrapper_OuterDomElement = wrapperForChild,
                });

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
            child.INTERNAL_ParentWindow = parent.INTERNAL_ParentWindow;

            // Create and append the DOM structure of the Child:
            object domElementWhereToPlaceGrandChildren = null;
            object outerDomElement;
            bool isChildAControl = child is Control;
            if (child.INTERNAL_HtmlRepresentation == null)
            {
                bool hasTemplate = isChildAControl && ((Control)child).HasTemplate;
                if (hasTemplate)
                {
                    outerDomElement = ((Control)child).CreateDomElementForControlTemplate(whereToPlaceTheChild, out domElementWhereToPlaceGrandChildren);
                }
                else
                {
                    outerDomElement = child.CreateDomElement(whereToPlaceTheChild, out domElementWhereToPlaceGrandChildren);
                }
            }
            else
            {
                outerDomElement = INTERNAL_HtmlDomManager.CreateDomFromStringAndAppendIt(child.INTERNAL_HtmlRepresentation, whereToPlaceTheChild, child);
            }

            // For debugging purposes (to better read the output html), add a class to the outer DIV
            // that tells us the corresponding type of the element (Border, StackPanel, etc.):
            INTERNAL_HtmlDomManager.AddCSSClass(outerDomElement, child.GetType().ToString());

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Prepare the child", t2);
#endif

            //--------------------------------------------------------
            // REMEMBER ALL INFORMATION FOR FUTURE USE:
            //--------------------------------------------------------

            // Remember the DIVs:
            child.INTERNAL_OuterDomElement = outerDomElement;
            child.INTERNAL_InnerDomElement = domElementWhereToPlaceGrandChildren;

            //--------------------------------------------------------
            // HANDLE SPECIAL CASES:
            //--------------------------------------------------------

            UIElement.SetPointerEvents(child);

            // Reset the flag that tells if we have already applied the RenderTransformOrigin (this is useful to ensure that the default RenderTransformOrigin is (0,0) like in normal XAML, instead of (0.5,0.5) like in CSS):
            child.INTERNAL_RenderTransformOriginHasBeenApplied = false;

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
                    INTERNAL_HtmlDomManager.AddCSSClass(child.INTERNAL_OuterDomElement, "uielement-collapsed");
                }
            }
            else
            {
                RenderElementsAndRaiseChangedEventOnAllDependencyProperties(child);
            }

            if (isChildAControl && child is not TextBlock && child is not TextElement)
            {
                ((Control)child).UpdateSystemFocusVisuals();
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

        internal static void RenderElementsAndRaiseChangedEventOnAllDependencyProperties(DependencyObject dependencyObject)
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

#if PERFSTAT
            var t0 = Performance.now();
#endif
            // we copy the Dictionary so that the foreach doesn't break when 
            // we modify a DependencyProperty inside the Changed of another 
            // one (which causes it to be added to the Dictionary).
            // we exclude properties where source is set to default because
            // it means they have been set at some point, and unset afterward,
            // so we should not call the PropertyChanged callback.
            var list = dependencyObject.INTERNAL_PropertyStorageDictionary
                .Where(s => s.Value.Entry.BaseValueSourceInternal > BaseValueSourceInternal.Default)
                .ToList();
#if PERFSTAT
            Performance.Counter("VisualTreeManager: Copy list of properties", t0);
#endif

            foreach (KeyValuePair<DependencyProperty, INTERNAL_PropertyStorage> propertiesAndTheirStorage in list)
            {
                // Read the value:
                DependencyProperty property = propertiesAndTheirStorage.Key;

#if PERFSTAT
                var t1 = Performance.now();
#endif

                PropertyMetadata propertyMetadata = property.GetMetadata(dependencyObject.DependencyObjectType);

                if (propertyMetadata != null)
                {
                    INTERNAL_PropertyStorage storage = propertiesAndTheirStorage.Value;
                    object value = null;
                    bool valueWasRetrieved = false;

                    //--------------------------------------------------
                    // Call "Apply CSS", which uses "GetCSSEquivalent/s"
                    //--------------------------------------------------

                    if (propertyMetadata.GetCSSEquivalent != null || propertyMetadata.GetCSSEquivalents != null)
                    {
                        if (!valueWasRetrieved)
                        {
                            value = INTERNAL_PropertyStore.GetEffectiveValue(storage.Entry);
                            valueWasRetrieved = true;
                        }

                        INTERNAL_PropertyStore.ApplyCssChanges(value, value, propertyMetadata, dependencyObject);
                    }

                    //--------------------------------------------------
                    // Call "MethodToUpdateDom"
                    //--------------------------------------------------
                    if (propertyMetadata.MethodToUpdateDom != null)
                    {
                        if (!valueWasRetrieved)
                        {
                            value = INTERNAL_PropertyStore.GetEffectiveValue(storage.Entry);
                            valueWasRetrieved = true;
                        }

                        // Call the "Method to update DOM"
                        propertyMetadata.MethodToUpdateDom(dependencyObject, value);
                    }

                    if (propertyMetadata.MethodToUpdateDom2 != null)
                    {
                        if (!valueWasRetrieved)
                        {
                            value = INTERNAL_PropertyStore.GetEffectiveValue(storage.Entry);
                            valueWasRetrieved = true;
                        }

                        // DependencyProperty.UnsetValue for the old value signify that
                        // the old value should be ignored.
                        propertyMetadata.MethodToUpdateDom2(
                            dependencyObject,
                            DependencyProperty.UnsetValue,
                            value);
                    }

                    //--------------------------------------------------
                    // Call PropertyChanged
                    //--------------------------------------------------

                    if (propertyMetadata.PropertyChangedCallback != null
                        && propertyMetadata.CallPropertyChangedWhenLoadedIntoVisualTree != WhenToCallPropertyChangedEnum.Never)
                    {
                        if (!valueWasRetrieved)
                        {
                            value = INTERNAL_PropertyStore.GetEffectiveValue(storage.Entry);
                            valueWasRetrieved = true;
                        }

                        // Raise the "PropertyChanged" event
                        propertyMetadata.PropertyChangedCallback(
                            dependencyObject,
                            new DependencyPropertyChangedEventArgs(value, value, property));
                    }
                }


#if PERFSTAT
                Performance.Counter("VisualTreeManager: RaisePropertyChanged for property '" + property.Name + "'", t1);
#endif
            }
        }

#if BRIDGE
        [Bridge.Template("true")]
#endif
        static bool IsRunningInJavaScript()
        {
            return false;
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
