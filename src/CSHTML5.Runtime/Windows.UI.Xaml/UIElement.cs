

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
using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
#if MIGRATION
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// UIElement is a base class for most of the objects that have visual appearance
    /// and can process basic input in a user interface.
    /// </summary>
    public abstract partial class UIElement : DependencyObject
    {
        //internal bool INTERNAL_hasCapture = false;

        internal DependencyObject INTERNAL_VisualParent { get; set; } // This is used to determine if the item is in the Visual Tree: null means that the item is not in the visual tree, not null otherwise.
        internal Window INTERNAL_ParentWindow { get; set; } // This is a reference to the window where this control is presented. It is useful for example to know where to display the popups. //todo-perfs: replace all these properties with fields?
        internal
#if CSHTML5NETSTANDARD //todo: after testing with JSIL, make it "object" in all cases.
            object
#else
            dynamic
# endif
            INTERNAL_OuterDomElement
        { get; set; } // This is the main DIV of the HTML representation of the control //todo-perfs: replace all these properties with fields?
        internal object INTERNAL_InnerDomElement { get; set; } // This is used to add visual children to the DOM (optionally wrapped into additional code, c.f. "INTERNAL_VisualChildInformation")
        internal object INTERNAL_AdditionalOutsideDivForMargins { get; set; } // This is used to define the margins and to remove the div used for the margins when we remove this element.
        internal object INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny { get; set; } // This is non-null only if the parent has a "ChildWrapper", that is, a DIV that it creates for each of its children. If it is the case, we store the "inner div" of that child wrapper. It is useful for alignment purposes (cf. alignment methods in the FrameworkElement class).
        internal
#if CSHTML5NETSTANDARD //todo: after testing with JSIL, make it "object" in all cases.
            object
#else
            dynamic
#endif
            INTERNAL_OptionalSpecifyDomElementConcernedByFocus
        { get; set; } // This is optional. When set, it means that the "GotFocus", "LostFocus", "KeyDown", "KeyUp", etc. events of the specified DOM element are used instead of the those of the outer DOM element. An example is the "TextBox", which has many DIVs but we only listen to the inner DIV for the focus and key events. Similarly, setting the focus on the control will call the JS method "focus()" on this DOM element if any.
        internal object INTERNAL_OptionalSpecifyDomElementConcernedByIsEnabled { get; set; } // This is optional. When set, it means that the "FrameworkElement.ManageIsEnabled" method sets the "disabled" property on this specified DOM element rather than on the INTERNAL_OuterDomElement. An example is the "CheckBox", which specifies a different DOM element for its "disabled" state.
        internal object INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth { get; set; } // This is optional. When set, it means that the "FrameworkElement.MinHeight" and "MinWidth" properties are applied on this specified DOM element rather than on the INTERNAL_OuterDomElement. An example is the "TextBox", for which applying MinHeight to the outer DOM does not make the inner DOM bigger.
        internal INTERNAL_CellDefinition INTERNAL_SpanParentCell = null; //this is used to know where we put the element when in a cell of a grid that is overlapped (due to the span or presence of another element that was put there previously), which causes it to be "sucked" into the basic cell of the previousl "placed" child.
        internal string INTERNAL_HtmlRepresentation { get; set; } // This can be used instead of overriding the "CreateDomElement" method to specify the appearance of the control.
        internal Dictionary<UIElement, INTERNAL_VisualChildInformation> INTERNAL_VisualChildrenInformation { get; set; } //todo-performance: verify that JavaScript output is a performant dictionary too, otherwise change structure.
        internal bool INTERNAL_RenderTransformOriginHasBeenApplied = false; // This is useful to ensure that the default RenderTransformOrigin is (0,0) like in normal XAML, instead of (0.5,0.5) like in CSS.
        internal ToolTip INTERNAL_AssignedToolTip;
        internal Popup INTERNAL_ValidationErrorPopup;
        //Note: the two following fields are only used in the PointerRoutedEventArgs class to determine how many clicks have been made on this UIElement in a short amount of time.
        internal int INTERNAL_clickCount; //this is used in the PointerPressed event to fill the ClickCount Property.
        internal int INTERNAL_lastClickDate; //this is used in the PointerPressed event to fill the ClickCount Property.
        public string XamlSourcePath; //this is used by the Simulator to tell where this control is defined. It is non-null only on root elements, that is, elements which class has "InitializeComponent" method. This member is public because it needs to be accessible via reflection.
        internal bool _isLoaded;
        internal Action INTERNAL_DeferredLoadingWhenControlBecomesVisible;

        /// <summary>
        /// Dictionary that helps link the validationErrors to the BindingExpressions for managing the errors.
        /// </summary>
        internal Dictionary<BindingExpression, ValidationError> INTERNAL_ValidationErrorsDictionary;



        #region Special code for RadioButtons

        private string _childrenRadioButtonDefaultName = null;
        internal string INTERNAL_ChildrenRadioButtonDefaultName //this is used to define a name for the radio buttons contained inside this UIElement that have their GroupName property not defined. Mandatory because RadioButtons without GroupName inside a same UIElement are considered to be part of a same group.
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_childrenRadioButtonDefaultName))
                {
                    _childrenRadioButtonDefaultName = GenerateRadioButtonsDefaultName();
                }
                return _childrenRadioButtonDefaultName;
            }
        }

        private static Random _random;
        private static string GenerateRadioButtonsDefaultName()
        {
            if (_random == null)
            {
                _random = new Random();
            }
            int i = _random.Next();
            return "RadioButtonDefaultGroupName" + i.ToString();
        }

        #endregion

        internal virtual object GetDomElementToSetContentString()
        {
            return INTERNAL_InnerDomElement;
        }

        #region ClipToBounds

        /// <summary>
        /// Gets or sets a value indicating whether to clip the content of this element
        /// (or content coming from the child elements of this element) to fit into the
        /// size of the containing element. This is a dependency property.
        /// </summary>
        public bool ClipToBounds
        {
            get { return (bool)GetValue(ClipToBoundsProperty); }
            set { SetValue(ClipToBoundsProperty, value); }
        }
        public static readonly DependencyProperty ClipToBoundsProperty =
            DependencyProperty.Register("ClipToBounds", typeof(bool), typeof(UIElement), new PropertyMetadata(false) { MethodToUpdateDom = ClipToBounds_MethodToUpdateDom,
            CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void ClipToBounds_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            //todo: doesn't work with vertical overflow if the display is table or table-cell
            UIElement uiElement = (UIElement)d;
            dynamic outerDomElement = uiElement.INTERNAL_OuterDomElement;
            dynamic style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDomElement);
            if ((bool)newValue)
            {
                style.overflow = "hidden"; //todo: ? if this is a ScrollViewer do not do anything?
                style.tableLayout = "fixed"; //Note: this might break stuff somewhere else
            }
            else
            {
                style.overflow = "auto";
                style.tableLayout = "auto";
            }
        }

        #endregion


        /// <summary>
        /// When overriden, creates the dom elements designed to represent an instance of an UIElement and defines the place where its child(ren) will be added.
        /// </summary>
        /// <param name="parentRef">The parent of the UIElement</param>
        /// <param name="domElementWhereToPlaceChildren">The dom element where the UIElement's children will be added.</param>
        /// <returns>The "root" dom element of the UIElement.</returns>
        public abstract object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren);

        /// <summary>
        /// When overriden, creates a dom wrapper for each child that is added to the UIElement.
        /// </summary>
        /// <param name="parentRef"></param>
        /// <param name="domElementWhereToPlaceChild"></param>
        /// <returns></returns>
        public virtual object CreateDomChildWrapper(object parentRef, out object domElementWhereToPlaceChild)
        {
            // This method is optional (cf. documentation in "INTERNAL_VisualChildInformation" class). It should return null if not used.
            domElementWhereToPlaceChild = null;
            return null;
        }

        public virtual dynamic GetDomElementWhereToPlaceChild(UIElement child) // Note: if overridden, it supercedes the "INTERNAL_InnerDomElement" property.
        {
            return null;
        }

        public dynamic GetChildsWrapper(UIElement child)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_VisualTreeManager.IsElementInVisualTree(child))
            {
                return INTERNAL_VisualChildrenInformation[child].INTERNAL_OptionalChildWrapper_OuterDomElement;
            }
            else
            {
                return null;
            }
        }


        #region Effect

        public Effect Effect //todo: we may add the support for multiple effects on the same UIElement since it is possible in html (but not in wpf). If we try to, it will require some changes in the Effects already implemented and some work to make it work properly in the simulator.
        {
            get { return (Effect)GetValue(EffectProperty); }
            set { SetValue(EffectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Effect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EffectProperty =
            DependencyProperty.Register("Effect", typeof(Effect), typeof(UIElement), new PropertyMetadata(null, Effect_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void Effect_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            if (e.OldValue != null)
            {
                ((Effect)e.OldValue).SetParentUIElement(null);
            }

            if (e.NewValue != null)
            {
                ((Effect)e.NewValue).SetParentUIElement(element);
            }
        }

        #endregion


        #region RenderTransform and RenderTransformOrigin

        /// <summary>
        /// Gets or sets transform information that affects the rendering position of
        /// a UIElement.
        /// </summary>
        public Transform RenderTransform
        {
            get { return (Transform)GetValue(RenderTransformProperty); }
            set { SetValue(RenderTransformProperty, value); }
        }
        /// <summary>
        /// Identifies the RenderTransform dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderTransformProperty =
            DependencyProperty.Register("RenderTransform", typeof(Transform), typeof(UIElement), new PropertyMetadata(null, RenderTransform_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static void RenderTransform_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;
            Transform newValue = (Transform)e.NewValue;
            if (e.OldValue != null)
            {
                ((Transform)e.OldValue).INTERNAL_UnapplyTransform();
                ((Transform)e.OldValue).INTERNAL_parent = null;
            }
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
            {
                if (newValue != null)
                {
                    newValue.INTERNAL_parent = uiElement;
                    newValue.INTERNAL_ApplyTransform();

                    // Ensure that the default RenderTransformOrigin is (0,0) like in normal XAML, instead of (0.5,0.5) like in CSS:
                    if (!uiElement.INTERNAL_RenderTransformOriginHasBeenApplied)
                        ApplyRenderTransformOrigin(uiElement, new Point(0d, 0d));
                }
                else
                {
                    dynamic domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(uiElement);

                    try
                    {
                        domStyle.transform = "";
                    }
                    catch
                    {
                    }
                    try
                    {
                        domStyle.msTransform = "";
                    }
                    catch
                    {
                    }
                    try // Prevents crash in the simulator that uses IE.
                    {
                        domStyle.WebkitTransform = "";
                    }
                    catch
                    {
                    }
                }
            }
        }


        public Point RenderTransformOrigin
        {
            get { return (Point)GetValue(RenderTransformOriginProperty); }
            set { SetValue(RenderTransformOriginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RenderTransformOrigin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderTransformOriginProperty =
            DependencyProperty.Register("RenderTransformOrigin", typeof(Point), typeof(UIElement), new PropertyMetadata(new Point(0d, 0d), RenderTransformOrigin_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void RenderTransformOrigin_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
            {
                Point newValue;
                if (e.NewValue is Point)
                {
                    newValue = (Point)e.NewValue;
                }
                else
                {
                    newValue = new Point(0d, 0d);
                }
                ApplyRenderTransformOrigin(uiElement, newValue);
            }
        }

        #endregion

        static void ApplyRenderTransformOrigin(UIElement uiElement, Point newValue)
        {
            dynamic domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(uiElement);
            string transformOriginValue = (newValue.X * 100).ToString() + "% " + (newValue.Y * 100).ToString() + "%"; //todo: make sure that "ToString" is culture-independent (so we use dot instead of comma for decimal separator).
            try
            {
                domStyle.transformOrigin = transformOriginValue;
            }
            catch
            {
            }
            try
            {
                domStyle.webkitTransformOrigin = transformOriginValue;
            }
            catch
            {
            }
            try // Prevents crash in the simulator that uses IE.
            {
                domStyle.msTransformOrigin = transformOriginValue;
            }
            catch
            {
            }
            uiElement.INTERNAL_RenderTransformOriginHasBeenApplied = true;
        }


        #region UseLayoutRounding

        /// <summary>
        /// Gets or sets a value that determines whether rendering for the object and
        /// its visual subtree should use rounding behavior that aligns rendering to
        /// whole pixels.
        /// </summary>
        public bool UseLayoutRounding
        {
            get { return (bool)GetValue(UseLayoutRoundingProperty); }
            set { SetValue(UseLayoutRoundingProperty, value); }
        }

        /// <summary>
        /// Identifies the UseLayoutRounding dependency property.
        /// </summary>
        public static readonly DependencyProperty UseLayoutRoundingProperty =
            DependencyProperty.Register("UseLayoutRounding", typeof(bool), typeof(UIElement), new PropertyMetadata(false)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        //------
        // NOTE: The "UseLayoutRounding" is currently not supported, but we provide it anyway because it's a pain for end-users to remove the option in all their XAML elements and the benefit of not including it is not significant.
        //------

        #endregion


        #region Visibility

        /// <summary>
        /// Gets or sets the visibility of a UIElement. A UIElement that is not visible
        /// is not rendered and does not communicate its desired size to layout.
        /// </summary>
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }
        /// <summary>
        /// Identifies the Visibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(UIElement), new PropertyMetadata(Visibility.Visible, Visibility_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        string _previousValueOfDisplayCssProperty = "block";
        static void Visibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;
            Visibility newValue = (Visibility)e.NewValue;

            // Finish loading the element if it was not loaded yet because it was Collapsed (and optimization was enabled in the Settings):
            if (uiElement.INTERNAL_DeferredLoadingWhenControlBecomesVisible != null
                && newValue != Visibility.Collapsed)
            {
                Action deferredLoadingWhenControlBecomesVisible = uiElement.INTERNAL_DeferredLoadingWhenControlBecomesVisible;
                uiElement.INTERNAL_DeferredLoadingWhenControlBecomesVisible = null;
                deferredLoadingWhenControlBecomesVisible();
            }
            else
            {
                // Set the CSS to make the DOM element visible/collapsed:
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
                {
#if REVAMPPOINTEREVENTS
                    INTERNAL_UpdateCssPointerEvents(uiElement);
#endif
                    // Get a reference to the most outer DOM element to show/hide:
                    dynamic mostOuterDomElement = null;
                    if (uiElement.INTERNAL_VisualParent is UIElement)
                        mostOuterDomElement = ((UIElement)uiElement.INTERNAL_VisualParent).INTERNAL_VisualChildrenInformation[uiElement].INTERNAL_OptionalChildWrapper_OuterDomElement; // Note: this is useful for example inside a Grid, where we want to hide the whole child wrapper in order to ensure that it doesn't capture mouse clicks thus preventing users from clicking on other elements in the Grid.
                    if (mostOuterDomElement == null)
                        mostOuterDomElement = uiElement.INTERNAL_AdditionalOutsideDivForMargins ?? uiElement.INTERNAL_OuterDomElement;
                    dynamic style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(mostOuterDomElement);

                    // Apply the visibility:
                    if (newValue == Visibility.Collapsed)
                    {
                        // Remember the current value of the CSS property "display" so that we can later revert to it:
                        string previousValueOfDisplayCssProperty = style.display;
                        if (previousValueOfDisplayCssProperty != "none")
                            uiElement._previousValueOfDisplayCssProperty = previousValueOfDisplayCssProperty;

                        // Hide the DOM element (or its wrapper if any):
                        style.display = "none";
                    }
                    else
                    {
                        // Show the DOM element (or its wrapper if any) by reverting the CSS property "display" to its previous value:
                        if (style.display == "none")
                            style.display = uiElement._previousValueOfDisplayCssProperty;

                        // The alignment was not calculated when the object was hidden, so we need to calculate it now:
                        if (uiElement is FrameworkElement && uiElement.INTERNAL_VisualParent != null) // Note: The "INTERNAL_VisualParent" can be "null" for example if we are changing the visibility of a "PopupRoot" control.
                        {
                            FrameworkElement.INTERNAL_ApplyHorizontalAlignmentAndWidth((FrameworkElement)uiElement, ((FrameworkElement)uiElement).HorizontalAlignment); //todo-perfs: only call the relevant portion of the code?
                            FrameworkElement.INTERNAL_ApplyVerticalAlignmentAndHeight((FrameworkElement)uiElement, ((FrameworkElement)uiElement).VerticalAlignment); //todo-perfs: only call the relevant portion of the code?
                        }
                    }
                    INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE();

                    // Notify any listeners that the visibility has changed (this can be useful for example to redraw the Path control when it becomes visible, due to the fact that drawing on a hidden HTML canvas is not persisted):
                    INTERNAL_VisibilityChangedNotifier.NotifyListenersThatVisibilityHasChanged(uiElement);
                }
            }
        }

        #endregion


        #region Opacity

        /// <summary>
        /// Gets or sets the degree of the object's opacity.
        /// A value between 0 and 1.0 that declares the opacity factor, with 1.0 meaning
        /// full opacity and 0 meaning transparent. The default value is 1.0.
        /// </summary>
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }
        /// <summary>
        /// Identifies the Opacity dependency property.
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(UIElement), new PropertyMetadata(1.0)
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Value = (inst, value) => { return (Math.Floor((double)value * 1000) / 1000).ToString(); }, // Note: We multiply by 1000 and then divide by 1000 so as to only keep 3 decimals at the most.
                        Name = new List<string> { "opacity" },
                        ApplyAlsoWhenThereIsAControlTemplate = true,
                    };
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never
            }
            );

        #endregion

        #region IsHitTestVisible

        /// <summary>
        /// Gets or sets whether the contained area of this UIElement can return true
        /// values for hit testing.
        /// </summary>
        public bool IsHitTestVisible
        {
            get { return (bool)GetValue(IsHitTestVisibleProperty); }
            set { SetValue(IsHitTestVisibleProperty, value); }
        }
        /// <summary>
        /// Identifies the IsHitTestVisible property.
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(UIElement), new PropertyMetadata(true) { MethodToUpdateDom = IsHitTestVisible_MethodToUpdateDom, Inherits = true });

        private static void IsHitTestVisible_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            UIElement element = (UIElement)d;
#if REVAMPPOINTEREVENTS
            INTERNAL_UpdateCssPointerEvents(element);
#else
             INTERNAL_UpdateCssPointerEventsPropertyBasedOnIsHitTestVisibleAndIsEnabled(element,
                isHitTestVisible: (bool)newValue,
                isEnabled: element is FrameworkElement ? ((FrameworkElement)element).IsEnabled : true);
#endif

        }

#if REVAMPPOINTEREVENTS
        internal bool INTERNAL_ArePointerEventsEnabled
        {
            get
            {
                return INTERNAL_ManagePointerEventsAvailability();
            }
        }

        internal virtual bool INTERNAL_ManagePointerEventsAvailability()
        {
            return false;
        }
#endif

        #endregion

#if REVAMPPOINTEREVENTS
        internal static void INTERNAL_UpdateCssPointerEvents(UIElement element)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                dynamic style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(element.INTERNAL_OuterDomElement);
                if (element.INTERNAL_ArePointerEventsEnabled)
                {
                    style.pointerEvents = "auto"; //Note: MDN lies: pointer-events: "auto" is not the same as pointer-event not set.
                }
                else
                {
                    style.pointerEvents = "none";
                }
            }
        }
#else

        internal static void INTERNAL_UpdateCssPointerEventsPropertyBasedOnIsHitTestVisibleAndIsEnabled(UIElement element, bool isHitTestVisible, bool isEnabled)
        {
            //todo: at the moment, the "IsEnabled" property is implemented with the CSS property "PointerEvents=none" (just like "IsHitTestVisible"). However, this is not good because "PointerEvents=none" makes the element transparent to click, meaning that the user's click will go to the element that is under it. Instead, the click event should be "absorbed" and lost (or bubbled up? but not go behind).

            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                dynamic style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(element.INTERNAL_OuterDomElement);
                if (isHitTestVisible && isEnabled)
                {
                    style.pointerEvents = "auto";
                }
                else
                {
                    style.pointerEvents = "none";
                }
            }
        }
#endif


        internal bool INTERNAL_IsChildOf(UIElement element)
        {
            UIElement currentParent = this;
            while (currentParent.INTERNAL_VisualParent != null)
            {
                if (currentParent.INTERNAL_VisualParent == element) //note: we check on the visual parent directly because we don't want to return true if the element itself (this) is the parameter element.
                {
                    return true;
                }
                currentParent = (UIElement)currentParent.INTERNAL_VisualParent;
            }
            return false;
        }


        #region AllowDrop

        /// <summary>
        /// Gets or sets a value that determines whether this UIElement
        /// can be a drop target for purposes of drag-and-drop operations.
        /// </summary>
        public bool AllowDrop
        {
            get { return (bool)GetValue(AllowDropProperty); }
            set { SetValue(AllowDropProperty, value); }
        }

        /// <summary>
        /// Identifies the AllowDrop dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowDropProperty =
            DependencyProperty.Register("AllowDrop", typeof(bool), typeof(UIElement), new PropertyMetadata(false)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        #endregion

        internal virtual void INTERNAL_UpdateDomStructureIfNecessary()
        {
            // Used to update the DOM structure (for example, in case of a grid, we need to (re)create the rows and columns where to place the child elements).
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("true")]

#else
        [Template("true")]
#endif
        private static bool IsRunningInJavaScript() //must be static for Bridge "Template" to work properly
        {
            return false;
        }


        #region CapturePointer, ReleasePointerCapture, IsPointerCaptured, and OnLostMouseCapture

        /// <summary>
        /// Sets pointer capture to a UIElement.
        /// </summary>
        /// <param name="value">The pointer object reference.</param>
        /// <returns>True if the object has pointer capture; otherwise, false.</returns>
#if MIGRATION
        public bool CaptureMouse()
#else
        public bool CapturePointer(Pointer value = null)
#endif
        {
#if MIGRATION
            Pointer value = null;
#endif
            return CapturePointer(value, this.INTERNAL_OuterDomElement);
        }

        private bool CapturePointer(Pointer value, dynamic element) //note: when the pointer is already captured, trying to capture it again does absolutely nothing (even after releasing the first one)
        {
            // We set the events on document then reroute these events to the UIElement.
            if (Pointer.INTERNAL_captured == null)
            {
                Pointer.INTERNAL_captured = this;

                if (IsRunningInJavaScript())
                {
#if !BRIDGE
                    JSIL.Verbatim.Expression(@"
     document.onmouseup = function(e) {
        if(e.doNotReroute == undefined)
        {
               document.reroute(e, $0);
        }
    }
     document.onmouseover = function(e) {
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, $0);
        }
    }       
    document.onmousedown = function(e) {
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, $0);
        }
    }                               
     document.onmouseout = function(e) {   
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, $0);
        }
    }                                      
     document.onmousemove = function(e) {  
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, $0);
        }
    }                                      
     document.onclick = function(e) {      
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, $0);
        }
    }                                      
     document.oncontextmenu = function(e) {
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, $0);
        }
    }                                      
     document.ondblclick = function(e) {   
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, $0);
        }
    }
", element);
#else
                    Script.Write(@"
     document.onmouseup = function(e) {
        if(e.doNotReroute == undefined)
        {
               document.reroute(e, {0});
        }
    }
     document.onmouseover = function(e) {
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, {0});
        }
    }       
    document.onmousedown = function(e) {
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, {0});
        }
    }                               
     document.onmouseout = function(e) {   
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, {0});
        }
    }                                      
     document.onmousemove = function(e) {  
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, {0});
        }
    }                                      
     document.onclick = function(e) {      
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, {0});
        }
    }                                      
     document.oncontextmenu = function(e) {
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, {0});
        }
    }                                      
     document.ondblclick = function(e) {   
       if(e.doNotReroute == undefined)
        {
               document.reroute(e, {0});
        }
    }
", element);
#endif

                    //javacript reroute function: taken from http://blog.stchur.com/blogcode/event-rerouting/
                    //the following function is added in the cshtml5.js file
                    //function reroute(e, elem, shiftKey)
                    //{
                    //    shiftKey = shiftKey || false;
                    //
                    //    var evt;
                    //    if (typeof document.dispatchEvent !== 'undefined')
                    //    {
                    //        evt = document.createEvent('MouseEvents');
                    //        evt.initMouseEvent(
                    //           e.type				// event type
                    //           ,e.bubbles			// can bubble?
                    //           ,e.cancelable		// cancelable?
                    //           ,window				// the event's abstract view (should always be window)
                    //           ,e.detail			// mouse click count (or event "detail")
                    //           ,e.screenX			// event's screen x coordinate
                    //           ,e.screenY			// event's screen y coordinate
                    //           ,e.pageX				// event's client x coordinate
                    //           ,e.pageY				// event's client y coordinate
                    //           ,e.ctrlKey			// whether or not CTRL was pressed during event
                    //           ,e.altKey			// whether or not ALT was pressed during event
                    //           ,shiftKey			// whether or not SHIFT was pressed during event
                    //           ,e.metaKey			// whether or not the meta key was pressed during event
                    //           ,e.button			// indicates which button (if any) caused the mouse event (1 = primary button)
                    //           ,e.relatedTarget		// relatedTarget (only applicable for mouseover/mouseout events)
                    //        );
                    //        elem.dispatchEvent(evt);
                    //    }
                    //    else if (typeof document.fireEvent !== 'undefined')
                    //    {
                    //        evt = document.createEventObject(e);
                    //        evt.shiftKey = shiftKey;
                    //        elem.fireEvent('on' + e.type, evt);
                    //    }
                    //}

                }
                else
                {
                    string javaScriptCodeToExecute = string.Format(@"
     document.onmouseup = function(e) {{
        if(e.doNotReroute == undefined)
        {{
               var element = document.getElementById(""{0}"");
               document.reroute(e, element);
        }}
    }}
     document.onmouseover = function(e) {{
       if(e.doNotReroute == undefined)
        {{
               var element = document.getElementById(""{0}"");
               document.reroute(e, element);
        }}
    }} 
    document.onmousedown = function(e) {{
       if(e.doNotReroute == undefined)
        {{
               var element = document.getElementById(""{0}"");
               document.reroute(e, element);
        }}
    }}                       
     document.onmouseout = function(e) {{   
       if(e.doNotReroute == undefined)
        {{
               var element = document.getElementById(""{0}"");
               document.reroute(e, element);
        }}
    }}                            
     document.onmousemove = function(e) {{
       if(e.doNotReroute == undefined)
        {{
               var element = document.getElementById(""{0}"");
               document.reroute(e, element);
        }}
    }}                                    
     document.onclick = function(e) {{   
       if(e.doNotReroute == undefined)
        {{
               var element = document.getElementById(""{0}"");
               document.reroute(e, element);
        }}
    }}                                     
     document.oncontextmenu = function(e) {{
       if(e.doNotReroute == undefined)
        {{
               var element = document.getElementById(""{0}"");
               document.reroute(e, element);
        }}
    }}                                      
     document.ondblclick = function(e) {{   
       if(e.doNotReroute == undefined)
        {{
               var element = document.getElementById(""{0}"");
               document.reroute(e, element);
        }}
    }}

", ((INTERNAL_HtmlDomElementReference)element).UniqueIdentifier);
                    INTERNAL_HtmlDomManager.ExecuteJavaScriptWithResult(javaScriptCodeToExecute);
                }
                return true;
            }
            else if (Pointer.INTERNAL_captured == this)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the pointer is captured to this element.
        /// </summary>
#if MIGRATION
        public bool IsMouseCaptured
#else
        public bool IsPointerCaptured
#endif
        {
            get
            {
                return (Pointer.INTERNAL_captured == this);
            }
        }

        /// <summary>
        /// Releases pointer captures for capture of one specific pointer by this UIElement.
        /// </summary>
        /// <param name="value">
        /// The pointer reference. Use either saved references from previous captures,
        /// or pointer event data, to obtain this reference.
        /// </param>
#if MIGRATION
        public void ReleaseMouseCapture()
#else
        public void ReleasePointerCapture(Pointer value = null)
#endif
        {
            if (IsRunningInJavaScript())
            {
                if (Pointer.INTERNAL_captured != null)
                {
#if !BRIDGE
                    JSIL.Verbatim.Expression(@"
 document.onmousedown = null;
 document.onmouseup = null;
 document.onmouseover = null;
 document.onmouseout = null;
 document.onmousemove = null;
 document.onclick = null;
 document.oncontextmenu = null;
 document.ondblclick = null;
");
#else
                    Script.Write(@"
 document.onmousedown = null;
 document.onmouseup = null;
 document.onmouseover = null;
 document.onmouseout = null;
 document.onmousemove = null;
 document.onclick = null;
 document.oncontextmenu = null;
 document.ondblclick = null;
");

#endif

                    Pointer.INTERNAL_captured = null;
#if MIGRATION
                    OnLostMouseCapture(new MouseEventArgs());
#else
                    OnPointerCaptureLost(new PointerRoutedEventArgs());
#endif
                }
            }
            else
            {
                if (Pointer.INTERNAL_captured != null)
                {
                    //todo: remove "string.Format" on the next line when JSIL will be able to compile without it (there is currently an issue with JSIL).
                    string javaScriptCodeToExecute = string.Format(@"
 document.onmousedown = null;
 document.onmouseup = null;
 document.onmouseover = null;
 document.onmouseout = null;
 document.onmousemove = null;
 document.onclick = null;
 document.oncontextmenu = null;
 document.ondblclick = null;
");
                    INTERNAL_HtmlDomManager.ExecuteJavaScript(javaScriptCodeToExecute);
                    Pointer.INTERNAL_captured = null;
#if MIGRATION
                    OnLostMouseCapture(new MouseEventArgs());
#else
                    OnPointerCaptureLost(new PointerRoutedEventArgs());
#endif
                }
            }
        }

#if MIGRATION
        protected virtual void OnLostMouseCapture(MouseEventArgs e)
#else
        protected virtual void OnPointerCaptureLost(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            if (LostMouseCapture != null)
                LostMouseCapture(this, e);
#else
            if (PointerCaptureLost != null)
                PointerCaptureLost(this, e);
#endif
        }

#if MIGRATION
        public event MouseEventHandler LostMouseCapture;
#else
        public event PointerEventHandler PointerCaptureLost;
#endif

        #endregion

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();
            // We make sure an element that is detached cannot have the cursor captured, which causes bugs.
            // For example in a DataGrid, if we had a column with two focusable elements in its edition mode, clicking one then the other one would leave the edition mode and detach the elements
            // but the second element that was clicked would still have captured the pointer events, preventing the user to click on anything until the capture is released (if it does ever happen).
            if (Pointer.INTERNAL_captured == this)
            {
#if MIGRATION
                ReleaseMouseCapture();
#else
                ReleasePointerCapture();
#endif
            }
        }


        /// <summary>
        /// Returns a transform object that can be used to transform coordinates from
        /// the UIElement to the specified object.
        /// </summary>
        /// <param name="visual">
        /// The object to compare to the current object for purposes of obtaining the
        /// transform.
        /// </param>
        /// <returns>
        /// The transform information as an object. Call methods on this object to get
        /// a practical transform.
        /// </returns>
        public GeneralTransform TransformToVisual(UIElement visual)
        {
            dynamic outerDivOfThisControl = this.INTERNAL_OuterDomElement;

            // If no "visual" was specified, we use the Window root instead.
            // Note: This is useful for example when calculating the position of popups, which are defined in absolute coordinates, at the same level as the Window root.
            dynamic outerDivOfReferenceVisual =
                (visual != null) ? visual.INTERNAL_OuterDomElement : this.INTERNAL_ParentWindow.INTERNAL_OuterDomElement;

            double offsetLeft, offsetTop;
            if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
            {
                // ------- IN-BROWSER -------
                dynamic rectOfThisControl = outerDivOfThisControl.getBoundingClientRect();
                dynamic rectOfReferenceVisual = outerDivOfReferenceVisual.getBoundingClientRect();

                offsetLeft = rectOfThisControl.left - rectOfReferenceVisual.left;
                offsetTop = rectOfThisControl.top - rectOfReferenceVisual.top;
            }
            //#if !BRIDGE
            else
            {
                // ------- SIMULATOR -------

                // Hack to improve the Simulator performance by making only one interop call rather than two:
                string concatenated = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("($0.getBoundingClientRect().left - $1.getBoundingClientRect().left) + '|' + ($0.getBoundingClientRect().top - $1.getBoundingClientRect().top)",
                                                                        outerDivOfThisControl, outerDivOfReferenceVisual));
                int sepIndex = concatenated.IndexOf('|');
                string offsetLeftAsString = concatenated.Substring(0, sepIndex);
                string offsetTopAsString = concatenated.Substring(sepIndex + 1);
                offsetLeft = Convert.ToDouble(offsetLeftAsString);
                offsetTop = Convert.ToDouble(offsetTopAsString);
            }
            //#endif

            return new TranslateTransform() { X = offsetLeft, Y = offsetTop };
        }

        //internal virtual void INTERNAL_Render()
        //{
        //}
#if REWORKLOADED
        internal virtual void INTERNAL_OnVisualParentChanged()
        {

        }

        internal void AddVisualChild(UIElement child)
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);

            if (object.ReferenceEquals(INTERNAL_VisualTreeOperation.Current.Root, this))
            {
                INTERNAL_VisualTreeOperation.Current.Complete();
            }
        }
#endif
    }
}