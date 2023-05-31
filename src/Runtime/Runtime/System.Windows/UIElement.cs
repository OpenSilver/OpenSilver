
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
using CSHTML5.Internal;
using OpenSilver.Internal;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Effects;
using System.Diagnostics;

#if MIGRATION
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
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
        static UIElement()
        {
#if MIGRATION
            MouseMoveEvent = new RoutedEvent(nameof(MouseMove), RoutingStrategy.Bubble, typeof(MouseEventHandler), typeof(UIElement));
            MouseLeftButtonDownEvent = new RoutedEvent(nameof(MouseLeftButtonDown), RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(UIElement));
            MouseRightButtonDownEvent = new RoutedEvent(nameof(MouseRightButtonDown), RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(UIElement));
            MouseWheelEvent = new RoutedEvent(nameof(MouseWheel), RoutingStrategy.Bubble, typeof(MouseWheelEventHandler), typeof(UIElement));
            MouseLeftButtonUpEvent = new RoutedEvent(nameof(MouseLeftButtonUp), RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(UIElement));
            MouseEnterEvent = new RoutedEvent(nameof(MouseEnter), RoutingStrategy.Direct, typeof(MouseEventHandler), typeof(UIElement));
            MouseLeaveEvent = new RoutedEvent(nameof(MouseLeave), RoutingStrategy.Direct, typeof(MouseEventHandler), typeof(UIElement));
            TextInputEvent = new RoutedEvent(nameof(TextInput), RoutingStrategy.Bubble, typeof(TextCompositionEventHandler), typeof(UIElement));
            TextInputStartEvent = new RoutedEvent(nameof(TextInputStart), RoutingStrategy.Bubble, typeof(TextCompositionEventHandler), typeof(UIElement));
            TextInputUpdateEvent = new RoutedEvent(nameof(TextInputUpdate), RoutingStrategy.Bubble, typeof(TextCompositionEventHandler), typeof(UIElement));
            TappedEvent = new RoutedEvent(nameof(Tapped), RoutingStrategy.Bubble, typeof(TappedEventHandler), typeof(UIElement));
            MouseRightButtonUpEvent = new RoutedEvent(nameof(MouseRightButtonUp), RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(UIElement));
            KeyDownEvent = new RoutedEvent(nameof(KeyDown), RoutingStrategy.Bubble, typeof(KeyEventHandler), typeof(UIElement));
            KeyUpEvent = new RoutedEvent(nameof(KeyUp), RoutingStrategy.Bubble, typeof(KeyEventHandler), typeof(UIElement));
            GotFocusEvent = new RoutedEvent(nameof(GotFocus), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIElement));
            LostFocusEvent = new RoutedEvent(nameof(LostFocus), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIElement));
#else
            PointerMovedEvent = new RoutedEvent(nameof(PointerMoved), RoutingStrategy.Bubble, typeof(PointerEventHandler), typeof(UIElement));
            PointerPressedEvent = new RoutedEvent(nameof(PointerPressed), RoutingStrategy.Bubble, typeof(PointerEventHandler), typeof(UIElement));
            PointerWheelChangedEvent = new RoutedEvent(nameof(PointerWheelChanged), RoutingStrategy.Bubble, typeof(PointerEventHandler), typeof(UIElement));
            PointerReleasedEvent = new RoutedEvent(nameof(PointerReleased), RoutingStrategy.Bubble, typeof(PointerEventHandler), typeof(UIElement));
            PointerEnteredEvent = new RoutedEvent(nameof(PointerEntered), RoutingStrategy.Direct, typeof(PointerEventHandler), typeof(UIElement));
            PointerExitedEvent = new RoutedEvent(nameof(PointerExited), RoutingStrategy.Direct, typeof(PointerEventHandler), typeof(UIElement));
            TextInputEvent = new RoutedEvent(nameof(TextInput), RoutingStrategy.Bubble, typeof(TextCompositionEventHandler), typeof(UIElement));
            TextInputStartEvent = new RoutedEvent(nameof(TextInputStart), RoutingStrategy.Bubble, typeof(TextCompositionEventHandler), typeof(UIElement));
            TextInputUpdateEvent = new RoutedEvent(nameof(TextInputUpdate), RoutingStrategy.Bubble, typeof(TextCompositionEventHandler), typeof(UIElement));
            TappedEvent = new RoutedEvent(nameof(Tapped), RoutingStrategy.Bubble, typeof(TappedEventHandler), typeof(UIElement));
            RightTappedEvent = new RoutedEvent(nameof(RightTapped), RoutingStrategy.Bubble, typeof(RightTappedEventHandler), typeof(UIElement));
            KeyDownEvent = new RoutedEvent(nameof(KeyDown), RoutingStrategy.Bubble, typeof(KeyEventHandler), typeof(UIElement));
            KeyUpEvent = new RoutedEvent(nameof(KeyUp), RoutingStrategy.Bubble, typeof(KeyEventHandler), typeof(UIElement));
            GotFocusEvent = new RoutedEvent(nameof(GotFocus), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIElement));
            LostFocusEvent = new RoutedEvent(nameof(LostFocus), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIElement));
#endif

            RegisterEvents(typeof(UIElement));
        }

        internal bool IsConnectedToLiveTree { get; set; }

        internal bool IsUnloading { get; set; }

        internal bool LoadingIsPending { get; set; }

#region Visual Parent

        private DependencyObject _parent;

        /// <summary>
        /// Returns the parent of this UIElement.
        /// </summary>
        internal DependencyObject INTERNAL_VisualParent
        {
            get
            {
                return _parent;
            }
        }

#endregion Visual Parent

#region Visual Children

        /// <summary>
        /// Derived class must implement to support UIElement children. The method must return
        /// the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        /// By default a UIElement does not have any children.
        ///
        /// Remark:
        ///       Need to lock down Visual tree during the callbacks.
        ///       During this virtual call it is not valid to modify the Visual tree.
        /// </summary>
        internal virtual UIElement GetVisualChild(int index)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        /// <summary>
        /// Derived classes override this property to enable the UIElement code to enumerate
        /// the UIElement children. Derived classes need to return the number of children
        /// from this method.
        ///
        /// By default a UIElement does not have any children.
        ///
        /// Remark: During this virtual method the Visual tree must not be modified.
        /// </summary>
        internal virtual int VisualChildrenCount
        {
            get { return 0; }
        }

        /// <Summary>
        /// Flag to check if this visual has any children
        /// </Summary>
        internal bool HasVisualChildren { get; private set; }

        // Are we in the process of iterating the visual children.
        // This flag is set during a descendents walk, for property invalidation.
        internal bool IsVisualChildrenIterationInProgress { get; set; }

        /// <summary>
        /// AttachChild
        ///
        /// Derived classes must call this method to notify the UIElement layer that a new
        /// child appeard in the children collection. The UIElement layer will then call the GetVisualChild
        /// method to find out where the child was added.
        /// </summary>
        internal void AddVisualChild(UIElement child)
        {
            if (child == null)
            {
                return;
            }

            if (child._parent != null)
            {
                throw new ArgumentException("Must disconnect specified child from current parent UIElement before attaching to new parent UIElement.");
            }

            HasVisualChildren = true;

            // Set the parent pointer.

            child._parent = this;

            child.OnVisualParentChanged(null);
        }

        /// <summary>
        /// DisconnectChild
        ///
        /// Derived classes must call this method to notify the UIElement layer that a
        /// child was removed from the children collection. The UIElement layer will then call
        /// GetChildren to find out which child has been removed.
        /// </summary>
        internal void RemoveVisualChild(UIElement child)
        {
            if (child == null || child._parent == null)
            {
                return;
            }

            if (child._parent != this)
            {
                throw new ArgumentException("Specified UIElement is not a child of this UIElement.");
            }

            if (VisualChildrenCount == 0)
            {
                HasVisualChildren = false;
            }

            // Set the parent pointer to null.

            child._parent = null;

            child.OnVisualParentChanged(this);
        }

        /// <summary>
        /// OnVisualParentChanged is called when the parent of the UIElement is changed.
        /// </summary>
        /// <param name="oldParent">Old parent or null if the UIElement did not have a parent before.</param>
        internal virtual void OnVisualParentChanged(DependencyObject oldParent)
        {
            // Synchronize ForceInherit properties
            if (_parent != null)
            {
                SynchronizeForceInheritProperties(this, _parent);
            }
            else
            {
                if (oldParent != null)
                {
                    SynchronizeForceInheritProperties(this, oldParent);
                }
            }
        }

#endregion Visual Children

        internal virtual Size MeasureCore()
        {
            return new Size(0, 0);
        }

        internal Window INTERNAL_ParentWindow { get; set; } // This is a reference to the window where this control is presented. It is useful for example to know where to display the popups. //todo-perfs: replace all these properties with fields?

        // This is the main DIV of the HTML representation of the control
#if CSHTML5NETSTANDARD
        internal object INTERNAL_OuterDomElement { get; set; }
#else
        internal dynamic INTERNAL_OuterDomElement { get; set; }
#endif

        internal object INTERNAL_InnerDomElement { get; set; } // This is used to add visual children to the DOM (optionally wrapped into additional code, c.f. "INTERNAL_VisualChildInformation")
        internal object INTERNAL_AdditionalOutsideDivForMargins { get; set; } // This is used to define the margins and to remove the div used for the margins when we remove this element.
        internal object INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny { get; set; } // This is non-null only if the parent has a "ChildWrapper", that is, a DIV that it creates for each of its children. If it is the case, we store the "inner div" of that child wrapper. It is useful for alignment purposes (cf. alignment methods in the FrameworkElement class).
        internal object INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth { get; set; } // This is optional. When set, it means that the "FrameworkElement.MinHeight" and "MinWidth" properties are applied on this specified DOM element rather than on the INTERNAL_OuterDomElement. An example is the "TextBox", for which applying MinHeight to the outer DOM does not make the inner DOM bigger.
        internal string INTERNAL_HtmlRepresentation { get; set; } // This can be used instead of overriding the "CreateDomElement" method to specify the appearance of the control.
        internal Dictionary<UIElement, INTERNAL_VisualChildInformation> INTERNAL_VisualChildrenInformation { get; set; } //todo-performance: verify that JavaScript output is a performant dictionary too, otherwise change structure.
        internal bool INTERNAL_RenderTransformOriginHasBeenApplied = false; // This is useful to ensure that the default RenderTransformOrigin is (0,0) like in normal XAML, instead of (0.5,0.5) like in CSS.
        //Note: the two following fields are only used in the PointerRoutedEventArgs class to determine how many clicks have been made on this UIElement in a short amount of time.
        public string XamlSourcePath; //this is used by the Simulator to tell where this control is defined. It is non-null only on root elements, that is, elements which class has "InitializeComponent" method. This member is public because it needs to be accessible via reflection.
        internal bool _isLoaded;
        internal Action INTERNAL_DeferredRenderingWhenControlBecomesVisible;
        internal Action INTERNAL_DeferredLoadingWhenControlBecomesVisible;

        public UIElement()
        {
            DesiredSize = new Size();
            PreviousFinalRect = Rect.Empty;
            RenderedVisualBounds = Rect.Empty;
            PreviousAvailableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            _previousDesiredSize = Size.Empty;
            _layoutMeasuredSize = Size.Empty;
            _layoutLastSize = Size.Empty;
            _layoutProcessing = false;
            IsMeasureValid = false;
            IsArrangeValid = false;
            IsRendered = false;
            IsFirstRendering = false;
            _visualLevel = -1;
        }
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

        /// <summary>
        /// Identifies the <see cref="UIElement.ClipToBounds"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty ClipToBoundsProperty =
            DependencyProperty.Register(
                nameof(ClipToBounds),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(false)
                {
                    MethodToUpdateDom = ClipToBounds_MethodToUpdateDom,
                });

        private static void ClipToBounds_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            //todo: doesn't work with vertical overflow if the display is table or table-cell
            UIElement uiElement = (UIElement)d;
            var outerDomElement = uiElement.INTERNAL_OuterDomElement;
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDomElement);
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

#region Clip

        public Geometry Clip
        {
            get { return (Geometry)GetValue(ClipProperty); }
            set { SetValue(ClipProperty, value); }
        }

        public static readonly DependencyProperty ClipProperty =
            DependencyProperty.Register("Clip",
                                        typeof(Geometry),
                                        typeof(UIElement),
                                        new PropertyMetadata(null)
                                        {
                                            MethodToUpdateDom = Clip_MethodToUpdateDom,
                                        });

        private static void Clip_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            if (newValue == null)
            {
                UIElement uiElement = (UIElement)d;
                var outerDomElement = uiElement.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDomElement);
                style.clip = "";
                return;
            }

            // Only RectangleGeometry is supported for now
            if (newValue is RectangleGeometry)
            {
                UIElement uiElement = (UIElement)d;
                var outerDomElement = uiElement.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDomElement);
                RectangleGeometry val = (RectangleGeometry)newValue;

                // CSS rect property has the following format - rect(<top>, <right>, <bottom>, <left>)
                double top = val.Rect.Y;
                double right = val.Rect.Width + val.Rect.X;
                double bottom = val.Rect.Height + val.Rect.Y;
                double left = val.Rect.X;

                string rect = "rect(" + top.ToInvariantString() + "px, " + right.ToInvariantString() + "px" + ", " + bottom.ToInvariantString() + "px, " + left.ToInvariantString() + "px)";
                style.clip = rect;
            }
            else
            {
                Debug.WriteLine("Only RectangleGeometry is supported for now.");
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
        /// <param name="index">The index for the position in which to add the child.</param>
        /// <returns></returns>
        public virtual object CreateDomChildWrapper(object parentRef, out object domElementWhereToPlaceChild, int index = -1)
        {
            // This method is optional (cf. documentation in "INTERNAL_VisualChildInformation" class). It should return null if not used.
            domElementWhereToPlaceChild = null;
            return null;
        }

        public virtual object GetDomElementWhereToPlaceChild(UIElement child) // Note: if overridden, it supercedes the "INTERNAL_InnerDomElement" property.
        {
            return null;
        }

        public object GetChildsWrapper(UIElement child)
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

        /// <summary>
        /// Identifies the <see cref="Effect"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EffectProperty =
            DependencyProperty.Register(
                nameof(Effect),
                typeof(Effect),
                typeof(UIElement),
                new PropertyMetadata(null, OnEffectChanged)
                {
                    MethodToUpdateDom2 = (d, oldValue, newValue) =>
                    {
                        var uie = (UIElement)d;
                        if (oldValue is Effect oldEffect)
                        {
                            oldEffect.Clean(uie);
                        }
                        ((Effect)newValue)?.Render(uie);
                    }
                });

        // todo: we may add the support for multiple effects on the same 
        // UIElement since it is possible in html (but not in wpf). If we 
        // try to, it will require some changes in the Effects already 
        // implemented and some work to make it work properly in the 
        // simulator.
        //
        /// <summary>
        /// Gets or sets the pixel shader effect to use for rendering this <see cref="UIElement"/>.
        /// </summary>
        /// <returns>
        /// The pixel shader effect to use for rendering this <see cref="UIElement"/>. The
        /// default is null (no effects).
        /// </returns>
        public Effect Effect
        {
            get => (Effect)GetValue(EffectProperty);
            set => SetValue(EffectProperty, value);
        }

        private static void OnEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            
            if (element._effectChangedListener != null)
            {
                element._effectChangedListener.Detach();
                element._effectChangedListener = null;
            }

            if (e.NewValue is Effect newEffect)
            {
                element._effectChangedListener = new(element, newEffect)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnEffectChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
                };
                newEffect.Changed += element._effectChangedListener.OnEvent;
            }
        }

        private void OnEffectChanged(object sender, EventArgs e) => ((Effect)sender).Render(this);

        private WeakEventListener<UIElement, Effect, EventArgs> _effectChangedListener;

#endregion


#region RenderTransform and RenderTransformOrigin

        /// <summary>
        /// Gets or sets transform information that affects the rendering position of
        /// a UIElement.
        /// </summary>
        public Transform RenderTransform
        {
            get { return (Transform)GetValue(RenderTransformProperty) ?? new MatrixTransform(); }
            set { SetValue(RenderTransformProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="UIElement.RenderTransform"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty RenderTransformProperty =
            DependencyProperty.Register(
                nameof(RenderTransform),
                typeof(Transform),
                typeof(UIElement),
                new PropertyMetadata(null, RenderTransform_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void RenderTransform_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
                    var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(uiElement);

                    try
                    {
                        domStyle.transform = "";
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

        /// <summary>
        /// Identifies the <see cref="UIElement.RenderTransformOrigin"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderTransformOriginProperty =
            DependencyProperty.Register(
                nameof(RenderTransformOrigin),
                typeof(Point),
                typeof(UIElement),
                new PropertyMetadata(new Point(0d, 0d), RenderTransformOrigin_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void RenderTransformOrigin_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
            {
                ApplyRenderTransformOrigin(uiElement, (Point)e.NewValue);
            }
        }

        private static void ApplyRenderTransformOrigin(UIElement uiElement, Point newValue)
        {
            var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(uiElement);
            string transformOriginValue = $"{(newValue.X * 100).ToString(CultureInfo.InvariantCulture)}% {(newValue.Y * 100).ToString(CultureInfo.InvariantCulture)}%";

            try
            {
                domStyle.transformOrigin = transformOriginValue;
            }
            catch
            {
            }
            uiElement.INTERNAL_RenderTransformOriginHasBeenApplied = true;
        }

#endregion

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
        /// Identifies the <see cref="UIElement.UseLayoutRounding"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty UseLayoutRoundingProperty =
            DependencyProperty.Register(
                nameof(UseLayoutRounding),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(false));

        //-------------------------------------------------------------------
        // NOTE: The "UseLayoutRounding" is currently not supported, but we 
        // provide it anyway because it's a pain for end-users to remove the 
        // option in all their XAML elements and the benefit of not including 
        // it is not significant.
        //-------------------------------------------------------------------

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
        /// Identifies the <see cref="UIElement.Visibility"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register(
                nameof(Visibility),
                typeof(Visibility),
                typeof(UIElement),
                new PropertyMetadata(Visibility.Visible, Visibility_Changed));

        private string _previousValueOfDisplayCssProperty = "block";

        private static void Visibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;
            Visibility newValue = (Visibility)e.NewValue;

            // Finish loading the element if it was not loaded yet because it was Collapsed (and optimization was enabled in the Settings):
            if (uie.INTERNAL_DeferredLoadingWhenControlBecomesVisible != null
                && newValue != Visibility.Collapsed)
            {
                Action deferredLoadingWhenControlBecomesVisible = uie.INTERNAL_DeferredLoadingWhenControlBecomesVisible;
                uie.INTERNAL_DeferredLoadingWhenControlBecomesVisible = null;
                deferredLoadingWhenControlBecomesVisible();
            }

            // Make the CSS changes required to apply the visibility at the DOM level:
            INTERNAL_ApplyVisibility(uie, newValue);

            // The IsVisible property depends on this property.
            uie.UpdateIsVisible();
        }

        internal static void INTERNAL_ApplyVisibility(UIElement uiElement, Visibility newValue)
        {
            // Set the CSS to make the DOM element visible/collapsed:
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
            {
                // Get a reference to the most outer DOM element to show/hide:
                object mostOuterDomElement = uiElement.INTERNAL_AdditionalOutsideDivForMargins ?? uiElement.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(mostOuterDomElement);

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
            }
        }

        #endregion

        #region IsVisible

        /// <summary>
        /// A property indicating if this element is visible or not.
        /// </summary>
        public bool IsVisible => (bool)GetValue(IsVisibleProperty);

        /// <summary>
        /// Identifies the <see cref="IsVisible"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register(
                nameof(IsVisible),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(false, OnIsVisibleChanged, CoerceIsVisible));

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;

            if ((bool)e.NewValue)
            {
                Action deferredLoadingWhenControlBecomesVisible = uie.INTERNAL_DeferredRenderingWhenControlBecomesVisible;
                if (deferredLoadingWhenControlBecomesVisible != null)
                {
                    uie.INTERNAL_DeferredRenderingWhenControlBecomesVisible = null;
                    deferredLoadingWhenControlBecomesVisible();
                }
            }

            // Invalidate the children so that they will inherit the new value.
            uie.InvalidateForceInheritPropertyOnChildren(e.Property);

            uie.IsVisibleChanged?.Invoke(d, e);

            uie.InvalidateParentMeasure();
            uie.InvalidateParentArrange();
        }

        private static object CoerceIsVisible(DependencyObject d, object baseValue)
        {
            UIElement uie = (UIElement)d;

            // IsVisible is a read-only property.  It derives its "base" value
            // from the Visibility property.
            bool isVisible = uie.Visibility == Visibility.Visible;

            // We must be false if our parent is false, but we can be
            // either true or false if our parent is true.
            //
            // Another way of saying this is that we can only be true
            // if our parent is true, but we can always be false.
            if (isVisible)
            {
                bool constraintAllowsVisible;

                // Our parent can constrain us.
                if (VisualTreeHelper.GetParent(uie) is UIElement parent)
                {
                    constraintAllowsVisible = parent.IsVisible;
                }
                else
                {
                    constraintAllowsVisible = INTERNAL_VisualTreeManager.IsElementInVisualTree(uie);
                }

                if (!constraintAllowsVisible)
                {
                    isVisible = false;
                }
            }

            return isVisible;
        }

        public event DependencyPropertyChangedEventHandler IsVisibleChanged;

        internal void UpdateIsVisible()
        {
            CoerceValue(IsVisibleProperty);
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
        /// Identifies the <see cref="UIElement.Opacity"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(
                nameof(Opacity), 
                typeof(double), 
                typeof(UIElement), 
                new PropertyMetadata(1.0)
                {
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        // Note: We multiply by 1000 and then divide by 1000 so as to only keep 3 decimals at the most.
                        Value = (inst, value) => (Math.Floor(Convert.ToDouble(value) * 1000) / 1000).ToInvariantString(),
                        Name = new List<string> { "opacity" },
                        ApplyAlsoWhenThereIsAControlTemplate = true,
                    }
                });

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
        /// Identifies the <see cref="UIElement.IsHitTestVisible"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.Register(
                nameof(IsHitTestVisible),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(true, OnIsHitTestVisiblePropertyChanged, CoerceIsHitTestVisibleProperty)
                {
                    MethodToUpdateDom = (d, e) =>
                    {
                        SetPointerEvents((UIElement)d);
                    },
                });

        private static void OnIsHitTestVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Invalidate the children so that they will inherit the new value.
            ((UIElement)d).InvalidateForceInheritPropertyOnChildren(e.Property);
        }

        private static object CoerceIsHitTestVisibleProperty(DependencyObject d, object baseValue)
        {
            UIElement uie = (UIElement)d;

            if (!(baseValue is bool)) //todo: this is a temporary workaround - cf. comment in "CoerceIsEnabledProperty"
                return true;

            // We must be false if our parent is false, but we can be
            // either true or false if our parent is true.
            //
            // Another way of saying this is that we can only be true
            // if our parent is true, but we can always be false.
            if ((bool)baseValue)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(uie);
                if (parent == null || (bool)parent.GetValue(IsHitTestVisibleProperty))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

#endregion

#region pointer-events

        internal static bool EnablePointerEventsBase(UIElement uie)
        {
            return (bool)uie.GetValue(FrameworkElement.IsEnabledProperty) &&
                   uie.IsHitTestVisible;
        }

        /// <summary>
        /// Fetches the value that pointer-events (css) should be coerced to.
        /// </summary>
        internal virtual bool EnablePointerEventsCore
        {
            get
            {
                return false;
            }
        }

        internal bool EnablePointerEvents
        {
            get
            {
                return this.EnablePointerEventsCore &&
                       EnablePointerEventsBase(this);
            }
        }

#endregion pointer-events

        internal static void SetPointerEvents(UIElement element)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(element.INTERNAL_OuterDomElement);
                style.pointerEvents = element.EnablePointerEvents ? "auto" : "none";
            }
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
        /// Identifies the <see cref="UIElement.AllowDrop"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty AllowDropProperty =
            DependencyProperty.Register(
                nameof(AllowDrop),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(false) { Inherits = true });

#endregion

        internal virtual void INTERNAL_UpdateDomStructureIfNecessary()
        {
            // Used to update the DOM structure (for example, in case of a grid, we need to (re)create the rows and columns where to place the child elements).
        }

#if BRIDGE
        [Bridge.Template("true")]
#endif
        private static bool IsRunningInJavaScript() //must be static for Bridge "Template" to work properly
        {
            return false;
        }


#region CapturePointer, ReleasePointerCapture, IsPointerCaptured, and OnLostMouseCapture

        /// <summary>
        /// Sets pointer capture to a UIElement.
        /// </summary>
        /// <returns>True if the object has pointer capture; otherwise, false.</returns>
#if MIGRATION
        public bool CaptureMouse()
#else
        public bool CapturePointer(Pointer value = null)
#endif
        {
            return InputManager.Current.CaptureMouse(this);
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
#if MIGRATION
        public void ReleaseMouseCapture()
#else
        public void ReleasePointerCapture(Pointer value = null)
#endif
        {
            InputManager.Current.ReleaseMouseCapture(this);
        }

#if MIGRATION
        internal void OnLostMouseCapturedInternal(MouseEventArgs e)
#else
        internal void OnLostMouseCapturedInternal(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            OnLostMouseCapture(e);
#else
            OnPointerCaptureLost(e);
#endif
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

#region AllowScrollOnTouchMove

        /// <summary>
        /// Gets or sets whether pressing (touchscreen devices) on this UIElement then moving should allow scrolling or not. The default value is True.
        /// </summary>
        public bool AllowScrollOnTouchMove
        {
            get { return (bool)GetValue(AllowScrollOnTouchMoveProperty); }
            set { SetValue(AllowScrollOnTouchMoveProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="UIElement.AllowScrollOnTouchMove"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty AllowScrollOnTouchMoveProperty =
            DependencyProperty.Register(
                nameof(AllowScrollOnTouchMove), 
                typeof(bool), 
                typeof(UIElement), 
                new PropertyMetadata(true, AllowScrollOnTouchMove_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void AllowScrollOnTouchMove_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                // Note: "none" disables scrolling, pinching and other gestures.
                // It is supposed to not have any effect on the "TouchStart",
                // "TouchMove", and "TouchEnd" events.
                OpenSilver.Interop.ExecuteJavaScriptVoid(
                    $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(element.INTERNAL_OuterDomElement)}.style.touchAction = \"{((bool)e.NewValue ? "auto" : "none")}\"");
            }
        }

#endregion

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            // We make sure an element that is detached cannot have the cursor 
            // captured, which causes bugs.
            // For example in a DataGrid, if we had a column with two focusable 
            // elements in its edition mode, clicking one then the other one 
            // would leave the edition mode and detach the elements but the second 
            // element that was clicked would still have captured the pointer 
            // events, preventing the user to click on anything until the capture 
            // is released (if it does ever happen).
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
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                throw new ArgumentException();
            }

            var outerDivOfThisControl = INTERNAL_OuterDomElement;

            // If no "visual" was specified, we use the Window root instead.
            // Note: This is useful for example when calculating the position of popups, which
            // are defined in absolute coordinates, at the same level as the Window root.
            object outerDivOfReferenceVisual;
            if (visual != null)
            {
                if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(visual))
                {
                    throw new ArgumentException(nameof(visual));
                }

                outerDivOfReferenceVisual = visual.INTERNAL_OuterDomElement;
            }
            else
            {
                UIElement rootVisual = Application.Current?.RootVisual;
                if (rootVisual == null)
                {
                    throw new InvalidOperationException();
                }

                outerDivOfReferenceVisual = rootVisual.INTERNAL_OuterDomElement;
            }

            // Hack to improve the Simulator performance by making only one interop call rather than two:
            string sOuterDivOfControl = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(outerDivOfThisControl);
            string sOuterDivOfReferenceVisual = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(outerDivOfReferenceVisual);
            string concatenated = OpenSilver.Interop.ExecuteJavaScriptString(
                $"({sOuterDivOfControl}.getBoundingClientRect().left - {sOuterDivOfReferenceVisual}.getBoundingClientRect().left) + '|' + ({sOuterDivOfControl}.getBoundingClientRect().top - {sOuterDivOfReferenceVisual}.getBoundingClientRect().top)");
            int sepIndex = concatenated.IndexOf('|');
            string offsetLeftAsString = concatenated.Substring(0, sepIndex);
            string offsetTopAsString = concatenated.Substring(sepIndex + 1);
            double offsetLeft = Convert.ToDouble(offsetLeftAsString, CultureInfo.InvariantCulture);
            double offsetTop = Convert.ToDouble(offsetTopAsString, CultureInfo.InvariantCulture);

            return new MatrixTransform(new Matrix(1, 0, 0, 1, offsetLeft, offsetTop));
        }

        /// <summary>
        /// Use this method for better performance in the Simulator compared to 
        /// requesting the ActualWidth and ActualHeight separately.
        /// </summary>
        /// <returns>
        /// The actual size of the element.
        /// </returns>
        internal Size GetBoundingClientSize()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                return INTERNAL_HtmlDomManager.GetBoundingClientSize(INTERNAL_OuterDomElement);
            }

            return new Size();
        }

        internal bool IsDescendantOf(DependencyObject ancestor)
        {
            if (ancestor is null)
            {
                throw new ArgumentNullException(nameof(ancestor));
            }

            if (ancestor is not UIElement)
            {
                throw new ArgumentException($"ancestor must be a UIElement.");
            }

            // Walk up the parent chain of the descendant until we run out
            // of parents or we find the ancestor.
            DependencyObject current = this;

            while ((current != null) && (current != ancestor))
            {
                current = VisualTreeHelper.GetParent(current);
            }

            return current == ancestor;
        }

        //internal virtual void INTERNAL_Render()
        //{
        //}

        #region ForceInherit property support

        internal static void SynchronizeForceInheritProperties(UIElement uie, DependencyObject parent)
        {
            if (!(bool)parent.GetValue(FrameworkElement.IsEnabledProperty))
            {
                uie.CoerceValue(FrameworkElement.IsEnabledProperty);
            }

            if (!(bool)parent.GetValue(IsHitTestVisibleProperty))
            {
                uie.CoerceValue(IsHitTestVisibleProperty);
            }

            if ((bool)parent.GetValue(IsVisibleProperty))
            {
                uie.UpdateIsVisible();
            }

            // visual parent does not always match layout parent, so we cannot use it.
            uie.CoerceValue(FrameworkElement.CustomLayoutProperty);
        }

        internal void InvalidateForceInheritPropertyOnChildren(DependencyProperty property)
        {
            int cChildren = this.VisualChildrenCount;
            for (int i = 0; i < cChildren; i++)
            {
                UIElement child = this.GetVisualChild(i);
                if (child != null)
                {
                    child.CoerceValue(property);
                }
            }
        }

#endregion ForceInherit property support

        internal bool MeasureDirty
        {
            get { return ReadFlag(CoreFlags.MeasureDirty); }
            set { WriteFlag(CoreFlags.MeasureDirty, value); }
        }

        internal bool MeasureInProgress
        {
            get { return ReadFlag(CoreFlags.MeasureInProgress); }
            set { WriteFlag(CoreFlags.MeasureInProgress, value); }
        }

        internal bool ReadFlag(CoreFlags field)
        {
            return (_flags & field) != 0;
        }

        internal void WriteFlag(CoreFlags field, bool value)
        {
            if (value)
            {
                _flags |= field;
            }
            else
            {
                _flags &= (~field);
            }
        }

        private CoreFlags _flags;

        internal void InvalidateMeasureInternal()
        {
            if (!this.MeasureDirty &&
                !this.MeasureInProgress)
            {
                this.MeasureDirty = true;
                INTERNAL_VisualTreeManager.LayoutManager.MeasureQueue.Add(this);
            }
        }

        internal void RaiseLayoutUpdated()
        {
            OnLayoutUpdated();
        }

        protected virtual void OnLayoutUpdated()
        {
            //
        }
    }

    [Flags]
    internal enum CoreFlags : uint
    {
        None = 0x00000000,
        //SnapsToDevicePixelsCache = 0x00000001,
        //ClipToBoundsCache = 0x00000002,
        MeasureDirty = 0x00000004,
        //ArrangeDirty = 0x00000008,
        MeasureInProgress = 0x00000010,
        //ArrangeInProgress = 0x00000020,
        //NeverMeasured = 0x00000040,
        //NeverArranged = 0x00000080,
        //MeasureDuringArrange = 0x00000100,
        //IsCollapsed = 0x00000200,
        //IsKeyboardFocusWithinCache = 0x00000400,
        //IsKeyboardFocusWithinChanged = 0x00000800,
        //IsMouseOverCache = 0x00001000,
        //IsMouseOverChanged = 0x00002000,
        //IsMouseCaptureWithinCache = 0x00004000,
        //IsMouseCaptureWithinChanged = 0x00008000,
        //IsStylusOverCache = 0x00010000,
        //IsStylusOverChanged = 0x00020000,
        //IsStylusCaptureWithinCache = 0x00040000,
        //IsStylusCaptureWithinChanged = 0x00080000,
        HasAutomationPeer = 0x00100000,
        //RenderingInvalidated = 0x00200000,
        //IsVisibleCache = 0x00400000,
        //AreTransformsClean = 0x00800000,
        //IsOpacitySuppressed = 0x01000000,
        //ExistsEventHandlersStore = 0x02000000,
        //TouchesOverCache = 0x04000000,
        //TouchesOverChanged = 0x08000000,
        //TouchesCapturedWithinCache = 0x10000000,
        //TouchesCapturedWithinChanged = 0x20000000,
        //TouchLeaveCache = 0x40000000,
        //TouchEnterCache = 0x80000000,
    }
}