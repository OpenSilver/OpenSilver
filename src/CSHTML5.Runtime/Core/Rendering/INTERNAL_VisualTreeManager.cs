

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
#endif

namespace CSHTML5.Internal
{
    public static class INTERNAL_VisualTreeManager
    {
        internal static bool EnablePerformanceLogging;
        internal static bool EnableOptimizationWhereCollapsedControlsAreNotLoaded;

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
                    dynamic optionalChildWrapper_OuterDomElement = parent.INTERNAL_VisualChildrenInformation[child].INTERNAL_OptionalChildWrapper_OuterDomElement;
                    if (optionalChildWrapper_OuterDomElement != null)
                        INTERNAL_HtmlDomManager.RemoveFromDom(optionalChildWrapper_OuterDomElement);

                    // Remove the element from the parent's children collection:
                    parent.INTERNAL_VisualChildrenInformation.Remove(child);

                    child.INTERNAL_SpanParentCell = null;

                    // Detach the element as well as all the children recursively:
                    DetachVisualChidrenRecursively(child);

                    //remove the Style from the child if it was an implicit Style:
                    FrameworkElement childAsFrameworkElement = child as FrameworkElement;
                    if (childAsFrameworkElement.INTERNAL_IsImplicitStyle)
                    {
                        childAsFrameworkElement.Style = null;
                        //no need to set childAsFrameworkElement.INTERNAL_IsImplicitStyle to false since it is done in the Style Setter.
                    }

                    INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE();
                }
                else
                {
                    throw new Exception(string.Format("Cannot detach the element '{0}' because it is not a child of the element '{1}'.", child.GetType().ToString(), (parent != null ? parent.GetType().ToString() : "null")));
                }
            }
#if PERFSTAT
            Performance.Counter("DetachVisualChildIfNotNull", t);
#endif
        }

        static void DetachVisualChidrenRecursively(UIElement element)
        {
            // Debug:
            //System.Diagnostics.Debug.WriteLine("Detached: " + element.GetType().Name);


            if (element._pointerExitedEventManager != null)
            {
                //todo: use element.INTERNAL_IsPointerInside for the test then set it to false.
                //bool wasPointerInElement = Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0.data_IsMouseInside", element.INTERNAL_OuterDomElement));
                //if (wasPointerInElement)
                if (element.INTERNAL_isPointerInside)
                {

#if MIGRATION
                    //element.ProcessOnMouseLeave(CSHTML5.Interop.ExecuteJavaScript("window.lastPointerPosition.event"));
                    MouseEventArgs eventArgs = new MouseEventArgs();
                    element.OnMouseLeave(eventArgs);
                    element.OnMouseLeave_ForHandledEventsToo(eventArgs);
#else
                    PointerRoutedEventArgs eventArgs = new PointerRoutedEventArgs();
                    element.OnPointerExited(eventArgs);//CSHTML5.Interop.ExecuteJavaScript("window.lastPointerPosition.event"));
                    element.OnPointerExited_ForHandledEventsToo(eventArgs);
#endif
                }
                element.isAlreadySubscribedToMouseEnterAndLeave = false;
                element.INTERNAL_isPointerInside = false;
            }

            //we reset the inherited properties since the element is no longer attached to its parent:
            element.ResetInheritedProperties();

            // Call the "OnDetached" of the element. This is particularly useful for elements to clear any references they have to DOM elements. For example, the Grid will use it to set its _tableDiv to null.
            element.INTERNAL_OnDetachedFromVisualTree();

            //We detach the events from the dom element:
            element.INTERNAL_DetachFromDomEvents();

            // Call the "Unloaded" event: (note: in XAML, the "unloaded" event of the parent is called before the "unloaded" event of the children)
            element._isLoaded = false;
            if (element is FrameworkElement)
            {
                ((FrameworkElement)element).INTERNAL_RaiseUnloadedEvent();
            }

            // Traverse all elements recursively:
            if (element.INTERNAL_VisualChildrenInformation != null)
            {
                foreach (INTERNAL_VisualChildInformation childInfo in
#if BRIDGE
                    INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(element.INTERNAL_VisualChildrenInformation)
#else
                    element.INTERNAL_VisualChildrenInformation.Values
#endif
                    )
                {
                    DetachVisualChidrenRecursively(childInfo.INTERNAL_UIElement);
                }
            }

            // Reset all visual-tree related information:
            element.INTERNAL_OuterDomElement = null;
            element.INTERNAL_InnerDomElement = null;
            element.INTERNAL_VisualParent = null;
            element.INTERNAL_VisualChildrenInformation = null;
            element.INTERNAL_AdditionalOutsideDivForMargins = null;
            element.INTERNAL_DeferredLoadingWhenControlBecomesVisible = null;
        }

        public static void AttachVisualChildIfNotAlreadyAttached(UIElement child, UIElement parent)
        {
            // Modify the visual tree only if the parent element is itself in the visual tree:
            if (child != null && IsElementInVisualTree(parent))
            {
                // Debug:
                //System.Diagnostics.Debug.WriteLine("Attached: " + child.GetType().Name);

                // Ensure that the child is not already attached:
                if (child.INTERNAL_VisualParent == null)
                {
#if OLD_CODE_TO_OPTIMIZE_SIMULATOR_PERFORMANCE && !BRIDGE // Obsolete since Beta 13.4 on 2018.01.31 because we now use the Dispatcher instead (cf. the class "INTERNAL_SimulatorExecuteJavaScript")
                    StartTransactionToOptimizeSimulatorPerformance();
#endif
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

#if OLD_CODE_TO_OPTIMIZE_SIMULATOR_PERFORMANCE && !BRIDGE // Obsolete since Beta 13.4 on 2018.01.31 because we now use the Dispatcher instead (cf. the class "INTERNAL_SimulatorExecuteJavaScript")
                    EndTransactionToOptimizeSimulatorPerformance();
#endif
                }
                else if (!object.ReferenceEquals(child.INTERNAL_VisualParent, parent))
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
        /// <returns>True if there are implicit styles (including from the parents)</returns>
        internal static bool UpdateElementInheritedImplicitStyles(FrameworkElement element, FrameworkElement parent)
        {
            if (parent != null)
            {
                //Note: the order (of the three ifs below) is important because an style defined lower in the visual tree should have priority over one (with the same key) defined higher.
                if (parent.Resources != null && parent.Resources.INTERNAL_HasImplicitStyles)
                {
                    element.INTERNAL_InheritedImplicitStyles = new List<ResourceDictionary>();
                    element.INTERNAL_InheritedImplicitStyles.Add(parent.Resources);
                    if (parent.INTERNAL_InheritedImplicitStyles != null)
                    {
                        foreach (ResourceDictionary resourceDictionary in parent.INTERNAL_InheritedImplicitStyles)
                        {
                            element.INTERNAL_InheritedImplicitStyles.Add(resourceDictionary);
                        }
                    }
                    return true;
                }
                if (parent.INTERNAL_InheritedImplicitStyles != null)
                {
                    element.INTERNAL_InheritedImplicitStyles = parent.INTERNAL_InheritedImplicitStyles;
                    return true;
                }
            }

            element.INTERNAL_InheritedImplicitStyles = null; //this is in case the element had a parent that defined implicit styles then moved to another parent that doesn't: we need to remove the former list.
            return false; //result is false here only if 1) the direct parent didn't define an implicit Style, 2) the parent didn't inherit implicit Styles from its parents.
        }

        static void AttachVisualChild_Private(UIElement child, UIElement parent)
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
            // APPLY IMPLICIT AND DEFAULT STYLES:
            //--------------------------------------------------------

#if PERFSTAT
            var t = Performance.now();
#endif

            //See if the child has a Style, if not, try to find an implicitStyle for its Type.
            FrameworkElement childAsFrameworkElement = child as FrameworkElement;
            if (childAsFrameworkElement != null)
            {
                if (childAsFrameworkElement.Style == null)
                {
                    //-----------------------------
                    // APPLY IMPLICIT STYLE IF ANY:
                    //-----------------------------

                    Style style = null;
                    //Note if anyone wonders: if type B inherits from type A, an Implicit Style defined for type A is ignored by elements of type B (tested in an SL project).
                    if (UpdateElementInheritedImplicitStyles(childAsFrameworkElement, (FrameworkElement)parent))
                    {
                        Type childType = child.GetType();
                        foreach (ResourceDictionary resourceDictionary in childAsFrameworkElement.INTERNAL_InheritedImplicitStyles) // Note: "INTERNAL_InheritedImplicitStyles" is not null because the method above returned True.
                        {
                            if (resourceDictionary.ContainsKey(childType))
                            {
                                style = resourceDictionary[childType] as Style;
                                break;
                            }
                        }
                    }
                    if (style != null)
                    {
                        childAsFrameworkElement.Style = style;
                        childAsFrameworkElement.INTERNAL_IsImplicitStyle = true; //Note: it is important that we do this AFTER setting the Style property to the ImplicitStyle because the setter of the Style Property sets it to false to avoid removing styles that are not implicit.
                    }
                }

                //Add the effect of the setters of the default Style on the child:
                Style childDefaultStyle = childAsFrameworkElement.INTERNAL_defaultStyle;
                if (childDefaultStyle != null)
                {
                    //----------------------------
                    // APPLY DEFAULT STYLE IF ANY:
                    //----------------------------

                    //get the list of setters of the currently applied style on the element and of the default style:
                    Dictionary<DependencyProperty, Setter> normalStyleDictionary = null;
                    bool childHasStyle = childAsFrameworkElement.Style != null;
                    if (childHasStyle)
                    {
                        normalStyleDictionary = childAsFrameworkElement.Style.GetDictionaryOfSettersFromStyle();
                    }
                    Dictionary<DependencyProperty, Setter> defaultStyleDictionary = childDefaultStyle.GetDictionaryOfSettersFromStyle();

                    //for all the setters the default style has, that are not on properties already affected by the normal style, we apply them like a normal style (we basically do the same as what we do on each setter in FrameworkElement.Style_Changed):
                    foreach (DependencyProperty prop in
#if BRIDGE
                        INTERNAL_BridgeWorkarounds.GetDictionaryKeys_SimulatorCompatible(defaultStyleDictionary)
#else
                        defaultStyleDictionary.Keys
#endif
                        )
                    {
                        if (!childHasStyle || !normalStyleDictionary.ContainsKey(prop))
                        {
                            Setter setter = defaultStyleDictionary[prop];

                            childDefaultStyle.SetterValueChanged -= childAsFrameworkElement.StyleSetterValueChanged;
                            childDefaultStyle.SetterValueChanged += childAsFrameworkElement.StyleSetterValueChanged;

                            INTERNAL_PropertyStorage storage;
                            INTERNAL_PropertyStore.TryGetStorage(childAsFrameworkElement, setter.Property, true/*create*/, out storage);
                            INTERNAL_PropertyStore.SetLocalStyleValue(storage, setter.Value);
                        }
                    }
                }
            }

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Apply implicit and default styles", t);
#endif

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
            object wrapperForChild = parent.CreateDomChildWrapper(domElementWhereToPlaceChildStuff, out innerDivOfWrapperForChild);
            bool comparison1 = (wrapperForChild == null); // Note: we need due to a bug of JSIL where translation fails if we do not use this temp variable.
            bool comparison2 = (innerDivOfWrapperForChild == null); // Note: we need due to a bug of JSIL where translation fails if we do not use this temp variable.
            bool doesParentRequireToCreateAWrapperForEachChild = (!comparison1 && !comparison2); // Note: The result is "True" for complex structures such as tables, false otherwise (cf. documentation in "INTERNAL_VisualChildInformation" class).

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Prepare the parent", t0);
#endif

            //--------------------------------------------------------
            // CONTINUE WITH THE OTHER STEPS (OR DEFER TO WHEN THE ELEMENT IS VISIBLE, IF THE OPTIMIZATION TO NOT LOAD COLLAPSED CONTROLS IS ENABLED):
            //--------------------------------------------------------

            if (!EnableOptimizationWhereCollapsedControlsAreNotLoaded || child.Visibility != Visibility.Collapsed)
            {
                AttachVisualChild_Private_FinalStepsOnlyIfControlIsVisible(
                    child,
                    parent,
                    doesParentRequireToCreateAWrapperForEachChild,
                    innerDivOfWrapperForChild,
                    domElementWhereToPlaceChildStuff,
                    wrapperForChild);
            }
            else
            {
                child.INTERNAL_DeferredLoadingWhenControlBecomesVisible = () =>
                {
                    AttachVisualChild_Private_FinalStepsOnlyIfControlIsVisible(
                        child,
                        parent,
                        doesParentRequireToCreateAWrapperForEachChild,
                        innerDivOfWrapperForChild,
                        domElementWhereToPlaceChildStuff,
                        wrapperForChild);
                };
            }
        }
        static void AttachVisualChild_Private_FinalStepsOnlyIfControlIsVisible(
            UIElement child,
            UIElement parent,
            bool doesParentRequireToCreateAWrapperForEachChild,
            object innerDivOfWrapperForChild,
            object domElementWhereToPlaceChildStuff,
            object wrapperForChild
            )
        {
#if REWORKLOADED
            if (INTERNAL_VisualTreeOperation.Current.Root == null)
            {
                INTERNAL_VisualTreeOperation.Current.Root = parent;
            }
#endif
            //#if CSHTML5BLAZOR && DEBUG
            //            string childIndentity = child + (child != null ? " (" + child.GetHashCode() + ")" : "");
            //            string parentIndentity = parent + (parent != null ? " (" + parent.GetHashCode() + ")" : "");
            //            Console.WriteLine("OPEN SILVER DEBUG: VisualTreeManager : AttachVisualChild_Private_FinalStepsOnlyIfControlIsVisible: " + childIndentity + " attached to " + parentIndentity);
            //#endif

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
#if ONLY_ADD_DIV_FOR_MARGINS_WHEN_MARGINS_NOT_ZERO
            bool isADivForMarginsNeeded = !(parent is Canvas) && !(child is Inline) && child is FrameworkElement;  // Note: In a Canvas, we don't want to add the additional DIV because there are no margins and we don't want to interfere with the pointer events by creating an additional DIV.
            if (isADivForMarginsNeeded)
            {
                var horizontalAlign = ((FrameworkElement)child).HorizontalAlignment;
                var verticalAlign = ((FrameworkElement)child).VerticalAlignment;
                isADivForMarginsNeeded = !(margin.Left == 0 && margin.Top == 0 && margin.Right == 0 && margin.Bottom == 0 && horizontalAlign == HorizontalAlignment.Stretch && verticalAlign == VerticalAlignment.Stretch);
            }
#else
            bool isADivForMarginsNeeded = !(parent is Canvas) // Note: In a Canvas, we don't want to add the additional DIV because there are no margins and we don't want to interfere with the pointer events by creating an additional DIV.
                                            && !(child is Inline); // Note: inside a TextBlock we do not want the HTML DIV because we want to create HTML SPAN elements only (otherwise there would be unwanted line returns).
#endif
            if (isADivForMarginsNeeded)
            {
                // Determine where to place it:
                object whereToPlaceDivForMargins =
                    (doesParentRequireToCreateAWrapperForEachChild
                    ? innerDivOfWrapperForChild
                    : domElementWhereToPlaceChildStuff);

                // Create and append the DIV for handling margins and append:
                additionalOutsideDivForMargins = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", whereToPlaceDivForMargins, parent); //todo: check if the third parameter should be the child or the parent (make something with margins and put a mouseenter in the parent then see if the event is triggered).

                // Style the DIV for handling margins:
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(additionalOutsideDivForMargins);
                style.boxSizing = "border-box";
                if (child is FrameworkElement &&
                    (((FrameworkElement)child).HorizontalAlignment == HorizontalAlignment.Stretch && double.IsNaN(((FrameworkElement)child).Width)))
                {
                    if (!containsNegativeMargins)
                        style.width = "100%";
                }
                if (child is FrameworkElement &&
                    (((FrameworkElement)child).VerticalAlignment == VerticalAlignment.Stretch && double.IsNaN(((FrameworkElement)child).Height)))
                    style.height = "100%";
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

            // Set the "Parent" property of the Child (IMPORTANT: we need to do that before child.CreateDomElement because the type of the parent is used to display the child correctly):
            child.INTERNAL_VisualParent = parent;

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
                INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDomElement).position = "absolute"; //todo: test if this works properly
            }

#if REVAMPPOINTEREVENTS
            UIElement.INTERNAL_UpdateCssPointerEvents(child);
#else
            // If the current element is inside a Grid, we need to explicitly set its CSS property "PointerEvents=auto" because its parent child wrapper has "PointerEvents=none" in order to prevent its child wrappers from overlapping each other and thus preventing clicks on some children.
            if (parent is Grid && child is FrameworkElement) //todo: generalize this code so that we have no hard-reference on the Grid control, and third-party controls can use the same mechanics.
            {
                var frameworkElement = ((FrameworkElement)child);
                FrameworkElement.INTERNAL_UpdateCssPointerEventsPropertyBasedOnIsHitTestVisibleAndIsEnabled(frameworkElement, frameworkElement.IsHitTestVisible, frameworkElement.IsEnabled);
            }
#endif

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
            // HANDLE INHERITED PROPERTIES:
            //--------------------------------------------------------

#if PERFSTAT
            var t6 = Performance.now();
#endif

            // Get the Inherited properties and pass them to the direct children:
            foreach (DependencyProperty dependencyProperty in
#if BRIDGE
                INTERNAL_BridgeWorkarounds.GetDictionaryKeys_SimulatorCompatible(parent.INTERNAL_AllInheritedProperties)
#else
                parent.INTERNAL_AllInheritedProperties.Keys
#endif
                )
            {
                bool recursively = false; // We don't want a recursion here because the "Attach" method is already recursive due to the fact that we raise property changed on the Children property, which causes to reattach the subtree.
                INTERNAL_PropertyStorage storage = parent.INTERNAL_AllInheritedProperties[dependencyProperty];
                PropertyMetadata typeMetadata = dependencyProperty.GetTypeMetaData(storage.Owner.GetType());
                child.SetInheritedValue(dependencyProperty, INTERNAL_PropertyStore.GetValue(storage, typeMetadata), recursively);
            }

#if PERFSTAT
            Performance.Counter("Handle inherited properties", t6);
#endif

            //--------------------------------------------------------
            // SET "ISLOADED" PROPERTY AND CALL "ONATTACHED" EVENT:
            //--------------------------------------------------------

            // Tell the control that it is now present into the visual tree:
            child._isLoaded = true;

            // Raise the "OnAttached" event:
            child.INTERNAL_OnAttachedToVisualTree(); // IMPORTANT: Must be done BEFORE "RaiseChangedEventOnAllDependencyProperties" (for example, the ItemsControl uses this to initialize its visual)


            //--------------------------------------------------------
            // RECURSIVELY ATTACH THE CHILDREN, AND RENDER THE ELEMENTS BY RAISING PROPERTYCHANGED EVENT ON ALL THE DEPENDENCY PROPERTIES THAT ARE SET:
            //--------------------------------------------------------

            RenderElementsAndRaiseChangedEventOnAllDependencyProperties(child); // Note: this causes a recursion of the AttachVisualChild method because the "Children" properties of the controls get refreshed.
            //element.INTERNAL_Render();


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

            // For GotFocus and LostFocus to work, the DIV specified by "INTERNAL_OptionalSpecifyDomElementConcernedByFocus"
            // (or the OuterDomElement otherwise) needs to have the "tabIndex" attribute set. Therefore we need to always set
            // it (unless IsTabStop is False) to its current value (default is Int32.MaxValue). At the time when this code was
            // written, there was no way to automatically call the "OnChanged" on a dependency property if no value was set.

#if !REWORKLOADED
            // IMPORTANT: This needs to be done AFTER the "OnApplyTemplate" (for example, the TextBox sets the "INTERNAL_OptionalSpecifyDomElementConcernedByFocus" in the "OnApplyTemplate").
            if (isChildAControl)
            {
                if (!(child is ContentPresenter) && !(child is TextBlock)) //ContentPresenter should not count in tabbing, as well as TextBlock (especially since TextBlock is not supposed to be a Control).
                {
                    Control.TabIndexProperty_MethodToUpdateDom(child, ((Control)child).TabIndex);
                }
            }
#endif

            //--------------------------------------------------------
            // RAISE THE "SIZECHANGED" EVENT:
            //--------------------------------------------------------
#if !REWORKLOADED
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

            child.StartManagingPointerPositionForPointerExitedEventIfNeeded();

#if PERFSTAT
            Performance.Counter("VisualTreeManager: Raise Loaded event", t11);
#endif

#else
            INTERNAL_VisualTreeOperation.Current.Enqueue(child);
#endif
        }

        public static bool IsElementInVisualTree(UIElement child)
        {
            return (child.INTERNAL_VisualParent != null || child is Window || child is PopupRoot); //todo: replace "INTERNAL_VisualParent" with a check of the "_isLoaded" property? (it may work better with bindings, see for example the issue on March 22, where a "Binding" on ListBox.ItemsSource caused the selection to not work properly: it was fixed with a workaround to avoid possible regressions)
        }

        static void RenderElementsAndRaiseChangedEventOnAllDependencyProperties(DependencyObject dependencyObject)
        {

            //#if CSHTML5BLAZOR && DEBUG
            //            string prettyPrintDependencyObject = dependencyObject + (dependencyObject != null ? "(" + dependencyObject.GetHashCode().ToString() +")" : "");
            //            Console.WriteLine("OPENSILVER DEBUG: VisualTreeManager: RenderElementsAndRaiseChangedEventOnAllDependencyProperties: dependencyObject:" + prettyPrintDependencyObject);
            //#endif

            // To prevent raising the property multiple times, we keep track of the properties for which we already raised the "PropertyChanged" event:
            //Dictionary<DependencyProperty, DependencyProperty> propertiesForWhichTheEventHasAlreadyBeenRaised = new Dictionary<DependencyProperty, DependencyProperty>(); //todo: replace with HashSet<DependencyProperty> when available.

            //------------------------------------------------------------------------------------------------------------------
            // RAISE "PROPERTYCHANGED" FOR ALL THE PROPERTIES THAT HAVE A VALUE THAT HAS BEEN SET, INCLUDING ATTACHED PROPERTIES, AND CALL THE "METHOD TO UPDATE DOM":
            //------------------------------------------------------------------------------------------------------------------

            // This is used to force a redraw of all the properties that are set on the object (including Attached Properties!). For example, if a Border has a colored background, this is the moment when that color will be applied. Properties that have no value set by the user are not concerned (their default state is rendered elsewhere).
#if PERFSTAT
            var t0 = Performance.now();
#endif
            var list = dependencyObject.INTERNAL_PropertyStorageDictionary.ToList(); //we copy the Dictionary so that the foreach doesn't break when we modify a DependencyProperty inside the Changed of another one (which causes it to be added to the Dictionary).
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
                //#if CSHTML5BLAZOR && DEBUG
                //                    string prettyPrintProperty = property.Name + (property != null ? "(" + property.GetHashCode().ToString() + ")" : "");
                //                    Console.WriteLine("OPENSILVER DEBUG: VisualTreeManager: RenderElementsAndRaiseChangedEventOnAllDependencyProperties:"
                //                        + " dependencyObject:" + prettyPrintDependencyObject
                //                        + " property:" + prettyPrintProperty
                //                        + " MSG: " + (propertyMetadata != null ? "has" : "has not") + " a propertyMetadata");
                //#endif
                if (propertyMetadata != null)
                {
                    INTERNAL_PropertyStorage storage = propertiesAndTheirStorage.Value;
                    object value = null;
                    bool valueWasRetrieved = false;

                    //--------------------
                    // Call "Apply CSS", which uses "GetCSSEquivalent/s":
                    //--------------------
                    if (propertyMetadata.GetCSSEquivalent != null || propertyMetadata.GetCSSEquivalents != null)
                    {
                        if (!valueWasRetrieved)
                        {
                            value = storage.ActualValue;
                            valueWasRetrieved = true;
                        }

                        INTERNAL_PropertyStore.ApplyCssChanges(value, value, propertyMetadata, storage.Owner);
                    }

                    //--------------------
                    // Call "MethodToUpdateDom":
                    //--------------------
                    if (propertyMetadata.MethodToUpdateDom != null)
                    {
                        if (!valueWasRetrieved)
                        {
                            value = storage.ActualValue;
                            valueWasRetrieved = true;
                        }

                        // Call the "Method to update DOM":
                        propertyMetadata.MethodToUpdateDom(storage.Owner, value);
                    }

                    //--------------------
                    // Call PropertyChanged:
                    //--------------------
                    //#if CSHTML5BLAZOR && DEBUG
                    //                        string prettyPrintPropertyMetadata = propertyMetadata + (propertyMetadata != null ? propertyMetadata.GetHashCode().ToString() : "");
                    //                        Console.WriteLine("OPENSILVER DEBUG: VisualTreeManager: RenderElementsAndRaiseChangedEventOnAllDependencyProperties:"
                    //                            + " dependencyObject:" + prettyPrintDependencyObject
                    //                            + " property:" + prettyPrintProperty
                    //                            + " propertyMetadata:" + propertyMetadata
                    //                            + " MSG: " + (propertyMetadata.PropertyChangedCallback != null ? "has" : "has not") + " a PropertyChangedCallback" );
                    //#endif
                    if (propertyMetadata.PropertyChangedCallback != null
                        && propertyMetadata.CallPropertyChangedWhenLoadedIntoVisualTree != WhenToCallPropertyChangedEnum.Never)
                    {
                        if (!valueWasRetrieved)
                        {
                            value = storage.ActualValue;
                            valueWasRetrieved = true;
                        }

                        //#if CSHTML5BLAZOR && DEBUG
                        //                            Console.WriteLine("OPENSILVER DEBUG: VisualTreeManager: RenderElementsAndRaiseChangedEventOnAllDependencyProperties:" 
                        //                                + " dependencyObject:" + prettyPrintDependencyObject 
                        //                                + " property:" + prettyPrintProperty 
                        //                                + " propertyMetadata:" + propertyMetadata
                        //                                + " MSG: is raising PropertyChangedCallback)");
                        //#endif
                        // Raise the "PropertyChanged" event:
                        propertyMetadata.PropertyChangedCallback(storage.Owner, new DependencyPropertyChangedEventArgs(value, value, property));

                        // Remember that we raised the event, to avoid raising it again in the code below:
                        //propertiesForWhichTheEventHasAlreadyBeenRaised.Add(property, property); //commented since the "code below" is commented
                    }
                }


#if PERFSTAT
                Performance.Counter("VisualTreeManager: RaisePropertyChanged for property '" + property.Name + "'", t1);
#endif
            }
            /*
            //------------------------------------------------------------------------------------
            // RAISE "PROPERTYCHANGED" FOR ALL THE OTHER PROPERTIES THAT REQUIRE IT
            //------------------------------------------------------------------------------------

            // When a dependency property is not set, we usually do not raise the "PropertyChanged" event, unless the property has its "CallPropertyChangedWhenLoadedIntoVisualTree" property set to "Always" in its TypeMetadata:
            var type = dependencyObject.GetType();
            // Iterate through all the inheritance parents of the type (we use a Stack to revert the order):
            Stack<Type> typeAndItsInheritanceParents = new Stack<Type>();
            while (type != null)
            {
                typeAndItsInheritanceParents.Push(type);
                type = type.BaseType;
            }
            while (typeAndItsInheritanceParents.Count > 0)
            {
                type = typeAndItsInheritanceParents.Pop();
                if (INTERNAL_TypeToDependencyPropertiesThatRequirePropertyChanged.TypeToDependencyPropertiesThatRequirePropertyChanged.ContainsKey(type))
                {
                    // Raise the "PropertyChanged" for all the DependencyProperties of the object that require it:
                    foreach (DependencyProperty dependencyProperty in INTERNAL_TypeToDependencyPropertiesThatRequirePropertyChanged.TypeToDependencyPropertiesThatRequirePropertyChanged[type])
                    {
                        if (!propertiesForWhichTheEventHasAlreadyBeenRaised.ContainsKey(dependencyProperty)
                            && dependencyProperty.TypeMetadata.PropertyChangedCallback != null)
                        {
                            // Get the value:
                            object value;
                            if (dependencyObject.INTERNAL_PropertyStorageDictionary.ContainsKey(dependencyProperty))
                                value = dependencyObject.GetValue(dependencyProperty);
                            else if (dependencyProperty.TypeMetadata != null)
                                value = dependencyProperty.TypeMetadata.DefaultValue;
                            else
                                value = null;

                            // Raise the "PropertyChanged":
                            dependencyProperty.TypeMetadata.PropertyChangedCallback(dependencyObject, new DependencyPropertyChangedEventArgs(value, value, dependencyProperty));
                        }
                    }
                }
            }
             */
        }

#if !BRIDGE
        [JSReplacement("true")]
#else
        [Template("true")]
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

            foreach (INTERNAL_VisualChildInformation visualChildInformation in
#if BRIDGE
                INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(parent.INTERNAL_VisualChildrenInformation)
#else
                parent.INTERNAL_VisualChildrenInformation.Values
#endif
                )
            {
                var child = visualChildInformation.INTERNAL_UIElement;
                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }

#if REWORKLOADED
    internal sealed class INTERNAL_VisualTreeOperation
    {
        private readonly Queue<UIElement> visualElements;

        static INTERNAL_VisualTreeOperation()
        {
            Current = new INTERNAL_VisualTreeOperation();
        }

        private INTERNAL_VisualTreeOperation()
        {
            this.visualElements = new Queue<UIElement>();
        }

        public static INTERNAL_VisualTreeOperation Current { get; private set; }

        public UIElement Root { get; set; }

        public void Enqueue(UIElement uiE)
        {
            this.visualElements.Enqueue(uiE);
        }

        public void Complete()
        {
            while (this.visualElements.Count > 0)
            {
                UIElement uiE = this.visualElements.Dequeue();
                if (uiE._isLoaded)
                {
                    uiE.INTERNAL_OnVisualParentChanged();
                    uiE.StartManagingPointerPositionForPointerExitedEventIfNeeded();
                }
            }
            this.Root = null;
        }
    }
#endif
}
