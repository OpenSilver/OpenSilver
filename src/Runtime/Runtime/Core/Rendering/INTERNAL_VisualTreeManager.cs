
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
        static INTERNAL_VisualTreeManager()
        {
            LayoutManager = new LayoutManagerInternal();
        }

        internal static LayoutManagerInternal LayoutManager { get; }

        internal class MeasureQueue
        {
            private Queue<UIElement> _queue;

            public MeasureQueue()
            {
                this._queue = new Queue<UIElement>();
            }

            internal bool UpdateInProgress
            {
                get;
                set;
            }

            public void Add(UIElement uie)
            {
                this._queue.Enqueue(uie);
                if (!this.UpdateInProgress)
                {
                    this.UpdateInProgress = true;
                    try
                    {
                        while (this._queue.Count > 0)
                        {
                            UIElement e = this._queue.Dequeue();

                            e.MeasureInProgress = true;
                            try
                            {
                                e.MeasureCore();
                            }
                            finally
                            {
                                e.MeasureInProgress = false;
                            }

                            e.MeasureDirty = false;
                        }
                    }
                    finally
                    {
                        this.UpdateInProgress = false;
                    }
                }
            }
        }

        internal class LayoutManagerInternal
        {
            private MeasureQueue _measureQueue;

            internal bool MeasureUpdateInProgress
            {
                get
                {
                    if (this._measureQueue != null)
                    {
                        return this._measureQueue.UpdateInProgress;
                    }
                    return false;
                }
            }

            internal MeasureQueue MeasureQueue
            {
                get
                {
                    if (this._measureQueue == null)
                    {
                        this._measureQueue = new MeasureQueue();
                    }
                    return this._measureQueue;
                }
            }
        }

        internal static bool EnablePerformanceLogging;
        internal static bool EnableOptimizationWhereCollapsedControlsAreNotRendered = true;
        internal static bool EnableOptimizationWhereCollapsedControlsAreNotLoaded = false;
        internal static bool EnableOptimizationWhereCollapsedControlsAreLoadedLast = false;

        public static void DetachVisualChildIfNotNull(UIElement child, UIElement parent)
        {
#if PERFSTAT
            var t = Performance.now();
#endif
            if (child != null && IsElementInVisualTree(child)) //todo: doublecheck that "IsElementInVisualTree" is a good thing here.
            {
                // Verify that the child is really a child of the specified control:
                if (parent.INTERNAL_VisualChildrenInformation != null
                    && parent.INTERNAL_VisualChildrenInformation.ContainsKey(child))
                {
                    // Remove the element from the DOM:
                    string stringForDebugging = !IsRunningInJavaScript() ? "Removing " + child.GetType().ToString() : null;
                    INTERNAL_HtmlDomManager.RemoveFromDom(child.INTERNAL_AdditionalOutsideDivForMargins, stringForDebugging);

                    // Remove the parent-specific wrapper around the child in the DOM (if any):
                    var optionalChildWrapper_OuterDomElement = parent.INTERNAL_VisualChildrenInformation[child].INTERNAL_OptionalChildWrapper_OuterDomElement;
                    if (optionalChildWrapper_OuterDomElement != null)
                        INTERNAL_HtmlDomManager.RemoveFromDom(optionalChildWrapper_OuterDomElement);

                    // Remove the element from the parent's children collection:
                    parent.INTERNAL_VisualChildrenInformation.Remove(child);

                    child.INTERNAL_SpanParentCell = null;

                    //Detach Element  
                    DetachVisualChidren(child);


                    INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE();

                    parent.InvalidateMeasure();
                    parent.InvalidateArrange();
                }
                else
                {
                    throw new Exception(
                        string.Format("Cannot detach the element '{0}' because it is not a child of the element '{1}'.",
                                      child.GetType().ToString(),
                                      parent.GetType().ToString()));
                }
            }
#if PERFSTAT
            Performance.Counter("DetachVisualChildIfNotNull", t);
#endif
        }

        static void DetachVisualChidren(UIElement element)
        {
            Queue<UIElement> elementsQueue = new Queue<UIElement>();
            elementsQueue.Enqueue(element);

            while (elementsQueue.Count > 0)
            {
                var elementToDetach = elementsQueue.Dequeue();
                if (elementToDetach.INTERNAL_VisualChildrenInformation != null)
                {
                    foreach (var pair in elementToDetach.INTERNAL_VisualChildrenInformation)
                    {
                        elementsQueue.Enqueue(pair.Value.INTERNAL_UIElement);
                    }
                }
                DetachElement(elementToDetach);
            }
        }

        static void DetachElement(UIElement element)
        {
            if (element.IsPointerOver)
            {
                element.RaiseMouseLeave();
            }

            // Call the "OnDetached" of the element. This is particularly useful for elements to clear any references they have to DOM elements. For example, the Grid will use it to set its _tableDiv to null.
            element.INTERNAL_OnDetachedFromVisualTree();

            //We detach the events from the dom element:
            element.INTERNAL_DetachFromDomEvents();

            // Call the "Unloaded" event: (note: in XAML, the "unloaded" event of the parent is called before the "unloaded" event of the children)
            element._isLoaded = false;
            if (element is FrameworkElement)
            {
                // Detach resizeSensor
                ((FrameworkElement)element).DetachResizeSensorFromDomElement();

                // Initialize measure & arrange status
                ((FrameworkElement)element).ClearMeasureAndArrangeValidation();

                ((FrameworkElement)element).INTERNAL_RaiseUnloadedEvent();
            }

            // Reset all visual-tree related information:
            element.IsConnectedToLiveTree = false;
            element.INTERNAL_OuterDomElement = null;
            element.INTERNAL_InnerDomElement = null;
            element.INTERNAL_VisualChildrenInformation = null;
            element.INTERNAL_AdditionalOutsideDivForMargins = null;
            element.INTERNAL_DeferredRenderingWhenControlBecomesVisible = null;
            element.INTERNAL_DeferredLoadingWhenControlBecomesVisible = null;
        }
        public static void MoveVisualChildInSameParent(UIElement child, UIElement parent, int newIndex, int oldIndex)
        {
            if (oldIndex < 0)
            {
                // setting oldIndex to -1 means we don't know the previous
                // position of the child. We have to iterate through all the
                // child to find it.
                MoveVisualChildInSameParent(child, parent, newIndex);

                parent.InvalidateMeasure();
                parent.InvalidateArrange();
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

                    parent.InvalidateMeasure();
                    parent.InvalidateArrange();
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
                else if (!ReferenceEquals(child.INTERNAL_VisualParent, parent))
                {
                    throw new InvalidOperationException("The element already has a parent. An element cannot appear in multiple locations in the Visual Tree. Remove the element from the Visual Tree before adding it elsewhere.");
                }
                else
                {
                    // Nothing to do: the element is already attached to the specified parent.
                    return; //prevent from useless call to INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE().
                }

                INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE();

                child.InvalidateMeasure();
                child.InvalidateArrange();
                parent.InvalidateMeasure();
                parent.InvalidateArrange();
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

            // A "wrapper for child" is sometimes needed between the child and the parent (for example in case of a grid). It is usually one or more DIVs that fit in-between the child and the parent, and that are used to position the child within the parent.
            object innerDivOfWrapperForChild;
            object wrapperForChild = parent.CreateDomChildWrapper(domElementWhereToPlaceChildStuff, out innerDivOfWrapperForChild, index);
            bool comparison1 = (wrapperForChild == null); // Note: we need due to a bug of JSIL where translation fails if we do not use this temp variable.
            bool comparison2 = (innerDivOfWrapperForChild == null); // Note: we need due to a bug of JSIL where translation fails if we do not use this temp variable.
            bool doesParentRequireToCreateAWrapperForEachChild = (!comparison1 && !comparison2); // Note: The result is "True" for complex structures such as tables, false otherwise (cf. documentation in "INTERNAL_VisualChildInformation" class).

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Prepare the parent", t0);
#endif

            //--------------------------------------------------------
            // CONTINUE WITH THE OTHER STEPS (AND DEFER SOME OF THEM WHEN THE ELEMENT IS COLLAPSED):
            //--------------------------------------------------------

            // Defer loading to when the control becomes visible if the option to not load collapsed controls is enabled:
            if (EnableOptimizationWhereCollapsedControlsAreNotLoaded && child.Visibility == Visibility.Collapsed)
            {
                child.INTERNAL_DeferredLoadingWhenControlBecomesVisible = () =>
                {
                    AttachVisualChild_Private_MainSteps(
                        child,
                        parent,
                        index,
                        doesParentRequireToCreateAWrapperForEachChild,
                        innerDivOfWrapperForChild,
                        domElementWhereToPlaceChildStuff,
                        wrapperForChild);
                    Grid grid = parent as Grid;
                    if (grid != null)
                    {
                        grid.LocallyManageChildrenChanged();
                    }
                };
            }
            else if (EnableOptimizationWhereCollapsedControlsAreLoadedLast && child.Visibility == Visibility.Collapsed)
            {
                child.INTERNAL_DeferredLoadingWhenControlBecomesVisible = () =>
                {
                    AttachVisualChild_Private_MainSteps(
                        child,
                        parent,
                        index,
                        doesParentRequireToCreateAWrapperForEachChild,
                        innerDivOfWrapperForChild,
                        domElementWhereToPlaceChildStuff,
                        wrapperForChild);
                    Grid grid = parent as Grid;
                    if (grid != null)
                    {
                        grid.LocallyManageChildrenChanged();
                    }
                };
                INTERNAL_DispatcherHelpers.QueueAction(async () =>
                {
                    await Task.Delay(100); // This ensures that the hidden controls are really loaded after everything else (including the shapes and other visible elements which rendering is also deferred via a "Dispatcher.BeginInvoke" call).
                    Action deferredLoadingWhenControlBecomesVisible = child.INTERNAL_DeferredLoadingWhenControlBecomesVisible;
                    if (deferredLoadingWhenControlBecomesVisible != null) // Note: it may have become "null" if the visibility was changed to "Visible" before the dispatched was called, in which case the element is loaded earlier (cf. "Visibility_Changed" handler)
                    {
                        child.INTERNAL_DeferredLoadingWhenControlBecomesVisible = null;
                        deferredLoadingWhenControlBecomesVisible();
                    }
                });
            }
            else
            {
                AttachVisualChild_Private_MainSteps(
                    child,
                    parent,
                    index,
                    doesParentRequireToCreateAWrapperForEachChild,
                    innerDivOfWrapperForChild,
                    domElementWhereToPlaceChildStuff,
                    wrapperForChild);
            }
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
            // CREATE THE DIV FOR THE MARGINS (OPTIONAL):
            //--------------------------------------------------------

#if PERFSTAT
            var t1 = Performance.now();
#endif

            // Determine if an additional DIV for handling margins is needed:
            object additionalOutsideDivForMargins = null;
            var margin = ((FrameworkElement)child).Margin;
            bool containsNegativeMargins = (margin.Left < 0d || margin.Top < 0d || margin.Right < 0d || margin.Bottom < 0d);
            bool isADivForMarginsNeeded = !(parent is Canvas) // Note: In a Canvas, we don't want to add the additional DIV because there are no margins and we don't want to interfere with the pointer events by creating an additional DIV.
                                            && !(child is Inline); // Note: inside a TextBlock we do not want the HTML DIV because we want to create HTML SPAN elements only (otherwise there would be unwanted line returns).

            if (isADivForMarginsNeeded && (parent.IsCustomLayoutRoot || parent.IsUnderCustomLayout))
                isADivForMarginsNeeded = false;

            if (isADivForMarginsNeeded)
            {
                // Determine where to place it:
                object whereToPlaceDivForMargins =
                    (doesParentRequireToCreateAWrapperForEachChild
                    ? innerDivOfWrapperForChild
                    : domElementWhereToPlaceChildStuff);

                // Create and append the DIV for handling margins and append:
                additionalOutsideDivForMargins = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", whereToPlaceDivForMargins, parent, index); //todo: check if the third parameter should be the child or the parent (make something with margins and put a mouseenter in the parent then see if the event is triggered).

                // Style the DIV for handling margins:
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(additionalOutsideDivForMargins);
                style.boxSizing = "border-box";
                if (child is FrameworkElement &&
                    (((FrameworkElement)child).HorizontalAlignment == HorizontalAlignment.Stretch && double.IsNaN(((FrameworkElement)child).Width)
                    && !(child is Image && ((Image)child).Stretch == Stretch.None)))
                {
                    if (!containsNegativeMargins)
                        style.width = "100%";
                }
                if (child is FrameworkElement &&
                    (((FrameworkElement)child).VerticalAlignment == VerticalAlignment.Stretch && double.IsNaN(((FrameworkElement)child).Height)
                    && !(child is Image && ((Image)child).Stretch == Stretch.None)))
                {
                    style.height = "100%";
                } 
            }

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Create the DIV for the margin", t1);
#endif

            //--------------------------------------------------------
            // PREPARE THE CHILD:
            //--------------------------------------------------------

#if PERFSTAT
            var t2 = Performance.now();
#endif

            // Determine where to place the child:
            object whereToPlaceTheChild = (isADivForMarginsNeeded
                ? additionalOutsideDivForMargins : (doesParentRequireToCreateAWrapperForEachChild
                    ? innerDivOfWrapperForChild
                    : domElementWhereToPlaceChildStuff));

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

            // Initialize the "Width" and "Height" of the child DOM structure:
            if (child is FrameworkElement)
                FrameworkElement.INTERNAL_InitializeOuterDomElementWidthAndHeight(((FrameworkElement)child), outerDomElement);

            // Update the DOM structure of the Child (for example, if the child is a Grid, this will render its rows and columns):
            child.INTERNAL_UpdateDomStructureIfNecessary();

            // For debugging purposes (to better read the output html), add a class to the outer DIV that tells us the corresponding type of the element (Border, StackPanel, etc.):
            INTERNAL_HtmlDomManager.SetDomElementAttribute(outerDomElement, "class", child.GetType().ToString());

            // Set Visibility hidden when rendering with CustomLayout
            if (child.IsCustomLayoutRoot && child.Visibility == Visibility.Visible)
            {
                INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDomElement).visibility = "hidden";
                child.isFirstRendering = true;
            }

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Prepare the child", t2);
#endif

            //--------------------------------------------------------
            // REMEMBER ALL INFORMATION FOR FUTURE USE:
            //--------------------------------------------------------

#if PERFSTAT
            var t3 = Performance.now();
#endif

            // Remember the DIVs:
            child.INTERNAL_OuterDomElement = outerDomElement;
            child.INTERNAL_InnerDomElement = domElementWhereToPlaceGrandChildren;
            child.INTERNAL_AdditionalOutsideDivForMargins = additionalOutsideDivForMargins ?? outerDomElement;
            child.INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny = doesParentRequireToCreateAWrapperForEachChild ? innerDivOfWrapperForChild : null;

            // Remember the information about the "VisualChildren":
            if (parent.INTERNAL_VisualChildrenInformation == null)
                parent.INTERNAL_VisualChildrenInformation = new Dictionary<UIElement, INTERNAL_VisualChildInformation>();
            parent.INTERNAL_VisualChildrenInformation.Add(child,
                new INTERNAL_VisualChildInformation()
                {
                    INTERNAL_UIElement = child,
                    INTERNAL_OptionalChildWrapper_OuterDomElement = wrapperForChild,
                    INTERNAL_OptionalChildWrapper_ChildWrapperInnerDomElement = innerDivOfWrapperForChild
                });

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Remember all info for future use", t3);
#endif

            //--------------------------------------------------------
            // HANDLE SPECIAL CASES:
            //--------------------------------------------------------

#if PERFSTAT
            var t4 = Performance.now();
#endif

            // If we are inside a canvas, we set the position to "absolute":
            if (parent is Canvas)
            {
                INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDomElement).position = child is StackPanel ? "relative" : "absolute"; //todo: test if this works properly
            }

            UIElement.SetPointerEvents(child);

            // Reset the flag that tells if we have already applied the RenderTransformOrigin (this is useful to ensure that the default RenderTransformOrigin is (0,0) like in normal XAML, instead of (0.5,0.5) like in CSS):
            child.INTERNAL_RenderTransformOriginHasBeenApplied = false;

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Handle special cases", t4);
#endif

            //--------------------------------------------------------
            // HANDLE EVENTS:
            //--------------------------------------------------------

#if PERFSTAT
            var t5 = Performance.now();
#endif

            // Register DOM events if any:
            child.INTERNAL_AttachToDomEvents();

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Handle events", t5);
#endif

            //--------------------------------------------------------
            // SET "ISLOADED" PROPERTY AND CALL "ONATTACHED" EVENT:
            //--------------------------------------------------------

            // Tell the control that it is now present into the visual tree:
            child._isLoaded = true;

            // Raise the "OnAttached" event:
            child.INTERNAL_OnAttachedToVisualTree(); // IMPORTANT: Must be done BEFORE "RaiseChangedEventOnAllDependencyProperties" (for example, the ItemsControl uses this to initialize its visual)

            //--------------------------------------------------------
            // RENDER THE ELEMENTS BY APPLYING THE CSS PROPERTIES:
            //--------------------------------------------------------

            // Defer rendering when the control is not visible to when becomes visible (note: when this option is enabled, we do not apply the CSS properties of the UI elements that are not visible. Those property are applied later, when the control becomes visible. This option results in improved performance.)
            bool enableDeferredRenderingOfCollapsedControls =
                EnableOptimizationWhereCollapsedControlsAreNotRendered
                || EnableOptimizationWhereCollapsedControlsAreLoadedLast
                || EnableOptimizationWhereCollapsedControlsAreNotLoaded;

            if (enableDeferredRenderingOfCollapsedControls && !child.IsVisible)
            {
                child.INTERNAL_DeferredRenderingWhenControlBecomesVisible = () =>
                {
                    RenderElementsAndRaiseChangedEventOnAllDependencyProperties(child);
                    child.ClearMeasureAndArrangeValidation();
                };
            }
            else
            {
                RenderElementsAndRaiseChangedEventOnAllDependencyProperties(child);
            }

            //--------------------------------------------------------
            // HANDLE BINDING:
            //--------------------------------------------------------

#if PERFSTAT
            var t9 = Performance.now();
#endif

            child.INTERNAL_UpdateBindingsSource();

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Handle binding", t9);
#endif

            //--------------------------------------------------------
            // HANDLE TABINDEX:
            //--------------------------------------------------------

            // For GotFocus and LostFocus to work, the DIV specified by UIElement.GetFocusTarget() needs to have the "tabIndex"
            // attribute set. Therefore we need to always set it (unless IsTabStop is False) to its current value (default is
            // Int32.MaxValue). At the time when this code was written, there was no way to automatically call the "OnChanged"
            // on a dependency property if no value was set.

            if (isChildAControl)
            {
                if (!(child is TextBlock)) // TextBlock should not count in tabbing (TextBlock is not supposed to be a Control).
                {
                    Control.TabIndexProperty_MethodToUpdateDom(child, ((Control)child).TabIndex);
                }
            }

            //--------------------------------------------------------
            // APPLY THE VISIBILITY:
            //--------------------------------------------------------

            Visibility childVisibility = child.Visibility;
            if (childVisibility == Visibility.Collapsed)
                UIElement.INTERNAL_ApplyVisibility(child, childVisibility);

            //--------------------------------------------------------
            // RAISE THE "SIZECHANGED" EVENT:
            //--------------------------------------------------------
#if PERFSTAT
            var t10 = Performance.now();
#endif

            // Raise the "SizeChanged" event: (note: in XAML, the "SizeChanged" event is called before the "Loaded" event)
            // IMPORTANT: This event needs to be raised AFTER the "OnApplyTemplate" and AFTER the "IsLoaded=true" (for example, it is used in the ScrollBar implementation).
            if (child is FrameworkElement)
            {
                ((FrameworkElement)child).INTERNAL_SizeChangedWhenAttachedToVisualTree();
            }

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Raise size changed event", t10);
#endif

            //--------------------------------------------------------
            // RAISE THE "LOADED" EVENT:
            //--------------------------------------------------------
#if PERFSTAT
            var t11 = Performance.now();
#endif

            // Raise the "Loaded" event: (note: in XAML, the "loaded" event of the children is called before the "loaded" event of the parent)
            if (child is FrameworkElement)
            {
                ((FrameworkElement)child).INTERNAL_RaiseLoadedEvent();
            }

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Raise Loaded event", t11);
#endif
        }

        public static bool IsElementInVisualTree(UIElement child)
        {
            return (child.IsConnectedToLiveTree || child is Window || child is PopupRoot); //todo: replace "INTERNAL_VisualParent" with a check of the "_isLoaded" property? (it may work better with bindings, see for example the issue on March 22, where a "Binding" on ListBox.ItemsSource caused the selection to not work properly: it was fixed with a workaround to avoid possible regressions)
        }

        static void RenderElementsAndRaiseChangedEventOnAllDependencyProperties(DependencyObject dependencyObject)
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

                PropertyMetadata propertyMetadata = property.GetTypeMetaData(dependencyObject.GetType());

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

                        INTERNAL_PropertyStore.ApplyCssChanges(value, value, propertyMetadata, storage.Owner);
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
                        propertyMetadata.MethodToUpdateDom(storage.Owner, value);
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
                        propertyMetadata.PropertyChangedCallback(storage.Owner, new DependencyPropertyChangedEventArgs(value, value, property));
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
            if (parent == null || parent.INTERNAL_VisualChildrenInformation == null)
                return null;

            foreach (var childInfo in parent.INTERNAL_VisualChildrenInformation.Select(x => x.Value))
            {
                var child = childInfo.INTERNAL_UIElement;
                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
