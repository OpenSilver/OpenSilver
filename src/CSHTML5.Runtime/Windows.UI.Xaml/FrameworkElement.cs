

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
using System.Windows.Input;
using System.Windows.Markup;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Provides a framework of common APIs for objects that participate in UI and
    /// programmatic layout. FrameworkElement also defines APIs related to data binding,
    /// object tree, and object lifetime feature areas.
    /// </summary>
    public abstract partial class FrameworkElement : UIElement
    {
        //--------------------------------------
        // Note: this is a "partial" class. For anything related to Size and Alignment, please refer to the other file ("FrameworkElement_HandlingSizeAndAlignment.cs").
        //--------------------------------------

        internal bool INTERNAL_IsImplicitStyle = false; //note: this is used to be able to tell whether the style applied on the FrameworkElement is an ImplicitStyle, which means that it must be removed from the element when it is detached from the visual tree.
        internal bool INTERNAL_ForceEnableAllPointerEvents = false; //Note: this is used by the PopupRoot to ensure that the PopupRoot has no pointer events while the container has. //todo: replace with another technique to achieve the same result.
        internal Style INTERNAL_defaultStyle;
        private ResourceDictionary _resources;

        /// <summary>
        /// Derived classes can set this flag in their constructor to prevent the "Style" property from being applied.
        /// </summary>
        protected bool INTERNAL_DoNotApplyStyle = false;

        /// <summary>
        /// Provides base class initialization behavior for FrameworkElement-derived
        /// classes.
        /// </summary>
        public FrameworkElement()
        {
        }

#if REVAMPPOINTEREVENTS
        internal virtual bool INTERNAL_ManageFrameworkElementPointerEventsAvailability()
        {
            return false;
        }

        internal sealed override bool INTERNAL_ManagePointerEventsAvailability()
        {
            return INTERNAL_ManageFrameworkElementPointerEventsAvailability()
                && Visibility == Visibility.Visible
                && IsEnabled  //todo: at the moment, the "IsEnabled" property is implemented with the CSS property "PointerEvents=none" (just like "IsHitTestVisible"). However, this is not good because "PointerEvents=none" makes the element transparent to click, meaning that the user's click will go to the element that is under it. Instead, the click event should be "absorbed" and lost (or bubbled up? but not go behind).
                && IsHitTestVisible;
        }
#endif

        #region Resources

        /// <summary>
        /// Gets the locally defined resource dictionary. In XAML, you can establish
        /// resource items as child object elements of a frameworkElement.Resources property
        /// element, through XAML implicit collection syntax.
        /// </summary>
        public ResourceDictionary Resources
        {
            get
            {
                if (_resources == null)
                    _resources = new ResourceDictionary();
                return _resources;
            }
            set
            {
                _resources = value;
            }
        }
        /// <summary>
        /// Returns a list of the parents' resourceDictionaries that contain at least one implicit Style.
        /// It excludes this FrameworkElement's own Resources so that we don't have to get the list from the parent.
        /// </summary>
        internal List<ResourceDictionary> INTERNAL_InheritedImplicitStyles = null; //this is set in the INTERNAL_VisualTreeManager.AttachVisualChild_Private(UIElement child, UIElement parent) method

        #endregion

        /// <summary>
        /// Gets the parent object of this FrameworkElement in the object tree.
        /// </summary>
        public DependencyObject Parent
        {
            get
            {
                return this.INTERNAL_VisualParent;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this element is in the Visual Tree, that is, if it has been loaded for presentation.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }

        /// <summary>
        /// Provides a base implementation for creating the dom elements designed to represent an instance of a FrameworkElement and defines the place where its child(ren) will be added.
        /// </summary>
        /// <param name="parentRef">The parent of the FrameworkElement</param>
        /// <param name="domElementWhereToPlaceChildren">The dom element where the FrameworkElement's children will be added.</param>
        /// <returns>The "root" dom element of the FrameworkElement.</returns>
        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            //------------------
            // It is important to create at least 2 divs so that horizontal and vertical alignments work properly (cf. "ApplyHorizontalAlignment" and "ApplyVerticalAlignment" methods)
            //------------------

            object div1;
            dynamic div1style = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out div1);
            object div2;
            dynamic div2style = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", div1, this, out div2);
            div2style.width = "100%";
            div2style.height = "100%";
            if (INTERNAL_ForceEnableAllPointerEvents)
                div2style.pointerEvents = "all";
            domElementWhereToPlaceChildren = div2;
            return div1;
        }

        //BRIDGETODO
        // Bridge bug : when we use virtual, override & out/base in a function, bridge doesn't compile
        // so delete this class when the bug is resolved
        public object CreateDomElement_WorkaroundBridgeInheritanceBug(object parentRef, out object domElementWhereToPlaceChildren)
        {
            //------------------
            // It is important to create at least 2 divs so that horizontal and vertical alignments work properly (cf. "ApplyHorizontalAlignment" and "ApplyVerticalAlignment" methods)
            //------------------

            object div1;
            dynamic div1style = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out div1);
            object div2;
            dynamic div2style = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", div1, this, out div2);
            div2style.width = "100%";
            div2style.height = "100%";
            if (INTERNAL_ForceEnableAllPointerEvents)
                div2style.pointerEvents = "all";
            domElementWhereToPlaceChildren = div2;
            return div1;
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
        /// Invoked whenever application code or internal processes (such as a rebuilding
        /// layout pass) call ApplyTemplate. In simplest terms, this means the method
        /// is called just before a UI element displays in your app. Override this method
        /// to influence the default post-template logic of a class.
        /// </summary>
#if MIGRATION
        public virtual void OnApplyTemplate()
#else
        protected virtual void OnApplyTemplate()
#endif
        {

        }

        #region Cursor

        // Returns:
        //     The cursor to display. The default value is defined as null per this dependency
        //     property. However, the practical default at run time will come from a variety
        //     of factors.
        /// <summary>
        /// Gets or sets the cursor that displays when the mouse pointer is over this
        /// element.
        /// </summary>
        public Cursor Cursor
        {
            get { return (Cursor)GetValue(CursorProperty); }
            set { SetValue(CursorProperty, value); }
        }
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.Register("Cursor", typeof(Cursor), typeof(FrameworkElement), new PropertyMetadata() { MethodToUpdateDom = Cursor_MethodToUpdateDom });

        private static void Cursor_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var frameworkElement = (FrameworkElement)d;
            var newCursor = (Cursor)newValue;
            var outerDomElement = frameworkElement.INTERNAL_OuterDomElement;
            var styleOfOuterDomElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDomElement);
            if (newCursor == null) //if it is null, we want 0 everywhere
            {
                styleOfOuterDomElement.cursor = "inherit";
            }
            else
            {
                styleOfOuterDomElement.cursor = newCursor._cursorHtmlString;
            }
        }

        #endregion

        #region IsEnabled

        /// <summary>
        /// Gets or sets a value indicating whether the user can interact with the control.
        /// </summary>
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }
        /// <summary>
        /// Identifies the IsEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(FrameworkElement), new PropertyMetadata(true, IsEnabled_Changed) { MethodToUpdateDom = IsEnabled_MethodToUpdateDom, Inherits = true });



        private static void IsEnabled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)d;
            if (frameworkElement.IsEnabledChanged != null)
            {
                frameworkElement.IsEnabledChanged(frameworkElement, e);
            }
        }
        /// <summary>
        /// Occurs when the IsEnabled property changes.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsEnabledChanged;

        private static void IsEnabled_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            FrameworkElement element = (FrameworkElement)d;
#if REVAMPPOINTEREVENTS
            UIElement.INTERNAL_UpdateCssPointerEvents(element);
#else
            INTERNAL_UpdateCssPointerEventsPropertyBasedOnIsHitTestVisibleAndIsEnabled(element,
                isHitTestVisible: element.IsHitTestVisible,
                isEnabled: (bool)newValue);
#endif
            element.ManageIsEnabled(newValue != null ? (bool)newValue : true);
        }

        internal protected virtual void ManageIsEnabled(bool isEnabled)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                var domElementToEnableOrDisable = (INTERNAL_OptionalSpecifyDomElementConcernedByIsEnabled != null ? INTERNAL_OptionalSpecifyDomElementConcernedByIsEnabled : INTERNAL_OuterDomElement);
                if (isEnabled)
                {
                    INTERNAL_HtmlDomManager.RemoveDomElementAttribute(domElementToEnableOrDisable, "disabled", forceSimulatorExecuteImmediately: true);
                }
                else
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(domElementToEnableOrDisable, "disabled", true, forceSimulatorExecuteImmediately: true);
                }
            }
        }

#endregion

#region Names handling

        /// <summary>
        /// Retrieves an object that has the specified identifier name.
        /// </summary>
        /// <param name="name">The name of the requested object.</param>
        /// <returns>
        /// The requested object. This can be null if no matching object was found in
        /// the current XAML namescope.
        /// </returns>
        public object FindName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (this is INameScope)
                return ((INameScope)this).FindName(name);
            else if (INTERNAL_VisualParent != null)
            {
                if (INTERNAL_VisualParent is FrameworkElement)
                    return ((FrameworkElement)INTERNAL_VisualParent).FindName(name);
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the identifying name of the object. When a XAML processor creates
        /// the object tree from XAML markup, run-time code can refer to the XAML-declared
        /// object by this name.
        /// </summary>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        /// <summary>
        /// Identifies the Name dependency property.
        /// </summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(FrameworkElement), new PropertyMetadata(string.Empty)
            {
                MethodToUpdateDom = OnNameChanged_MethodToUpdateDom,
            });

        private static void OnNameChanged_MethodToUpdateDom(DependencyObject d, object value)
        {
            var @this = (FrameworkElement)d;
            INTERNAL_HtmlDomManager.SetDomElementAttribute(@this.INTERNAL_OuterDomElement, "dataId", (value ?? string.Empty).ToString());
        }
#endregion

        #region DataContext

        /// <summary>
        /// Gets or sets the data context for a FrameworkElement when it participates
        /// in data binding.
        /// </summary>
        public object DataContext
        {
            get { return (object)GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }
        /// <summary>
        /// Identifies the DataContext dependency property.
        /// </summary>
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(object), typeof(FrameworkElement), new PropertyMetadata() { Inherits = true });

#endregion

#if WORKINPROGRESS
#region Triggers (not implemented)

        public TriggerCollection Triggers
        {
            get
            {
                return (TriggerCollection)this.GetValue(FrameworkElement.TriggersProperty);
            }
        }

        public static DependencyProperty TriggersProperty = DependencyProperty.Register("Triggers", typeof(TriggerCollection), typeof(FrameworkElement), new PropertyMetadata(new TriggerCollection()));

        #endregion

        //
        // Summary:
        //     Provides the behavior for the Arrange pass of Silverlight layout. Classes can
        //     override this method to define their own Arrange pass behavior.
        //
        // Parameters:
        //   finalSize:
        //     The final area within the parent that this object should use to arrange itself
        //     and its children.
        //
        // Returns:
        //     The actual size that is used after the element is arranged in layout.
        protected virtual Size ArrangeOverride(Size finalSize)
        {
            return new Size();
        }
        //
        // Summary:
        //     Provides the behavior for the Measure pass of Silverlight layout. Classes can
        //     override this method to define their own Measure pass behavior.
        //
        // Parameters:
        //   availableSize:
        //     The available size that this object can give to child objects. Infinity (System.Double.PositiveInfinity)
        //     can be specified as a value to indicate that the object will size to whatever
        //     content is available.
        //
        // Returns:
        //     The size that this object determines it needs during layout, based on its calculations
        //     of the allocated sizes for child objects; or based on other considerations, such
        //     as a fixed container size.
        protected virtual Size MeasureOverride(Size availableSize)
        {
            return new Size();
        }
#endif

        #region Tag

        /// <summary>
        /// Gets or sets an arbitrary object value that can be used to store custom information
        /// about this object.
        /// </summary>
        public object Tag
        {
            get { return (object)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }
        /// <summary>
        /// Identifies the Tag dependency property.
        /// </summary>
        public static readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(object), typeof(FrameworkElement), new PropertyMetadata(null, null));

#endregion

#region Handling Styles

        protected void INTERNAL_SetDefaultStyle(Style defaultStyle)
        {
            INTERNAL_defaultStyle = defaultStyle;
        }

        /// <summary>
        /// Gets or sets an instance Style that is applied for this object during rendering.
        /// </summary>
        public Style Style
        {
            get { return (Style)GetValue(StyleProperty); }
            set
            {
                SetValue(StyleProperty, value);
                INTERNAL_IsImplicitStyle = false; //set to false so that we do not mistakenly remove a style (when detaching it from the visual tree) that might have been applied on an element that had an implicit Style.
            }
        }
        /// <summary>
        /// Identifies the Style dependency property.
        /// </summary>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register("Style", typeof(Style), typeof(FrameworkElement), new PropertyMetadata(null, Style_Changed)
            {
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never
            });
        private static void Style_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;

            if (!frameworkElement.INTERNAL_DoNotApplyStyle)
            {
                Style newStyle = (Style)e.NewValue;
                Style oldStyle = (Style)e.OldValue;

                Dictionary<DependencyProperty, Setter> oldStyleDictionary = new Dictionary<DependencyProperty, Setter>();
                if (oldStyle != null)
                {
                    oldStyleDictionary = oldStyle.GetDictionaryOfSettersFromStyle();
                }
                Dictionary<DependencyProperty, Setter> newStyleDictionary = new Dictionary<DependencyProperty, Setter>();
                if (newStyle != null)
                {
                    newStyleDictionary = newStyle.GetDictionaryOfSettersFromStyle();
                }

#if PERFSTAT
                var t0 = Performance.now();
#endif
                frameworkElement.RecursivelyUnregisterFromStyleChangedEvents(oldStyle);
#if PERFSTAT
                Performance.Counter("RecursivelyUnregisterFromStyleChangedEvents", t0);
#endif

#if PERFSTAT
                var t1 = Performance.now();
#endif
                frameworkElement.RecursivelyRegisterToStyleChangedEvents(newStyle);
#if PERFSTAT
                Performance.Counter("RecursivelyRegisterToStyleChangedEvents", t1);
#endif

                foreach (Setter oldSetter in
#if BRIDGE
                    INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(oldStyleDictionary)
#else
                    oldStyleDictionary.Values
#endif
                    )
                {
                    if (oldSetter.Property != null) // Note: it can be null for example in the XAML text editor during design time, because the "DependencyPropertyConverter" class returns "null".
                    {
                        if (!newStyleDictionary.ContainsKey(oldSetter.Property))
                        {
                            INTERNAL_PropertyStorage storage = INTERNAL_PropertyStore.GetStorageIfExists(d, oldSetter.Property);
                            if (storage != null)
                            {
                                INTERNAL_PropertyStore.ResetLocalStyleValue(storage);
                            }
                        }
                    }

                    // Reset the information that tells which Style the Setter belongs to (this is used so that when the Value of the Setter changes, the Setter can notify its parent Style):
                    oldSetter.INTERNAL_ParentStyle = null;
                }

                foreach (Setter newSetter in
#if BRIDGE
                    INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(newStyleDictionary)
#else
                    newStyleDictionary.Values
#endif
                    )
                {
                    if (newSetter.Property != null) // Note: it can be null for example in the XAML text editor during design time, because the "DependencyPropertyConverter" class returns "null".
                    {
                        if (!oldStyleDictionary.ContainsKey(newSetter.Property) || oldStyleDictionary[newSetter.Property] != newSetter.Value)
                        {
                            INTERNAL_PropertyStorage storage = INTERNAL_PropertyStore.GetStorageOrCreateNewIfNotExists(frameworkElement, newSetter.Property);
                            INTERNAL_PropertyStore.SetLocalStyleValue(storage, newSetter.Value);
                        }
                    }

                    // Tell the setter which Style it belongs to, so that when the Value of the Setter changes, it can notify the parent Style:
                    newSetter.INTERNAL_ParentStyle = newStyle;
                }
            }
        }

        private void RecursivelyRegisterToStyleChangedEvents(Style newStyle)
        {
            // We traverse the Style hierarchy with the "BasedOn" property:
            HashSet2<Style> stylesAlreadyVisited = new HashSet2<Style>(); // This is to prevent an infinite loop below.
            while (newStyle != null && !stylesAlreadyVisited.Contains(newStyle))
            {
                newStyle.SetterValueChanged -= StyleSetterValueChanged;
                newStyle.SetterValueChanged += StyleSetterValueChanged;
                stylesAlreadyVisited.Add(newStyle);
                newStyle = newStyle.BasedOn;
            }
        }

        private void RecursivelyUnregisterFromStyleChangedEvents(Style oldStyle)
        {
            // We traverse the Style hierarchy with the "BasedOn" property:
            HashSet2<Style> stylesAlreadyVisited = new HashSet2<Style>(); // This is to prevent an infinite loop below.
            while (oldStyle != null && !stylesAlreadyVisited.Contains(oldStyle))
            {
                oldStyle.SetterValueChanged -= StyleSetterValueChanged;
                stylesAlreadyVisited.Add(oldStyle);
                oldStyle = oldStyle.BasedOn;
            }
        }

        internal void StyleSetterValueChanged(object sender, RoutedEventArgs e)
        {
            Setter setter = (Setter)sender;
            if (setter.Property != null) // Note: it can be null for example in the XAML text editor during design time, because the "DependencyPropertyConverter" class returns "null".
            {
                INTERNAL_PropertyStorage storage = INTERNAL_PropertyStore.GetStorageOrCreateNewIfNotExists(this, setter.Property);
                HashSet2<Style> stylesAlreadyVisited = new HashSet2<Style>(); // Note: "stylesAlreadyVisited" is here to prevent an infinite recursion.
                INTERNAL_PropertyStore.SetLocalStyleValue(storage, Style.GetActiveValue(setter.Property, stylesAlreadyVisited));
            }
        }

#region DefaultStyleKey

        // Returns:
        //     The key that references the default style for the control. To work correctly
        //     as part of theme style lookup, this value is expected to be the System.Type
        //     of the control being styled.
        /// <summary>
        /// Gets or sets the key that references the default style for the control.
        /// </summary>
        protected object DefaultStyleKey
        {
            get { return (object)GetValue(DefaultStyleKeyProperty); }
            set { SetValue(DefaultStyleKeyProperty, value); }
        }

        /// <summary>
        /// Identifies the System.Windows.FrameworkElement.DefaultStyleKey dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty DefaultStyleKeyProperty =
            DependencyProperty.Register("DefaultStyleKey", typeof(object), typeof(FrameworkElement), new PropertyMetadata(null, DefaultStyleKey_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void DefaultStyleKey_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            object newValue = e.NewValue;
            if (newValue != null)
            {
                // we start by looking for the generic.xaml file defined in the assembly where the control is defined.
                Style newStyle = Application.Current.XamlResourcesHandler.TryFindResourceInGenericXaml(element.GetType().Assembly, newValue) as Style;
                // if we don't find it in the element assembly then we try to find it in the assembly where newValue is defined (since newValue is supposed to be a Type)
                if (newStyle == null)
                {
                    if(newValue is Type)
                    {
                        newStyle = Application.Current.XamlResourcesHandler.TryFindResourceInGenericXaml(((Type)newValue).Assembly, newValue) as Style;
                    }
                }
                //todo: if it can be somewhere else than in themes/generic.xaml, find out how and deal with that case.
                element.INTERNAL_SetDefaultStyle(newStyle);
            }
            else
            {
                element.INTERNAL_SetDefaultStyle(null);
            }
        }


#endregion

#endregion

#region Loaded/Unloaded events

        /// <summary>
        /// Occurs when a FrameworkElement has been constructed and added to the object tree.
        /// </summary>
        public event RoutedEventHandler Loaded;

        internal void INTERNAL_RaiseLoadedEvent()
        {
            if (Loaded != null)
                Loaded(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Occurs when this object is no longer connected to the main object tree.
        /// </summary>
        public event RoutedEventHandler Unloaded;

        internal void INTERNAL_RaiseUnloadedEvent()
        {
            if (Unloaded != null)
                Unloaded(this, new RoutedEventArgs());
        }

#endregion

#region BindingValidationError event

        internal bool INTERNAL_AreThereAnyBindingValidationErrorHandlers = false;

        private List<EventHandler<ValidationErrorEventArgs>> _bindingValidationErrorHandlers;

        /// <summary>
        /// Occurs when a data validation error is reported by a binding source.
        /// </summary>
        public event EventHandler<ValidationErrorEventArgs> BindingValidationError
        {
            add
            {
                if (_bindingValidationErrorHandlers == null)
                {
                    _bindingValidationErrorHandlers = new List<EventHandler<ValidationErrorEventArgs>>();
                }
                _bindingValidationErrorHandlers.Add(value);

                this.INTERNAL_AreThereAnyBindingValidationErrorHandlers = true;
            }
            remove
            {
                if (_bindingValidationErrorHandlers != null)
                {
                    _bindingValidationErrorHandlers.Remove(value);

                    if (_bindingValidationErrorHandlers.Count == 0)
                    {
                        this.INTERNAL_AreThereAnyBindingValidationErrorHandlers = false;
                    }
                }
                else
                {
                    this.INTERNAL_AreThereAnyBindingValidationErrorHandlers = false;
                }
            }
        }

        internal void INTERNAL_RaiseBindingValidationErrorEvent(ValidationErrorEventArgs eventArgs)
        {
            if (_bindingValidationErrorHandlers != null)
            {
                foreach (EventHandler<ValidationErrorEventArgs> eventHandler in _bindingValidationErrorHandlers)
                {
                    if (eventHandler != null)
                    {
                        eventHandler(this, eventArgs);
                    }
                }
            }
        }
#endregion


#if WORKINPROGRESS

        public event EventHandler LayoutUpdated;

        /// <summary>Occurs when the data context for this element changes. </summary>
        public event DependencyPropertyChangedEventHandler DataContextChanged;


        public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(FrameworkElement), new PropertyMetadata(FlowDirection.LeftToRight));

        /// <summary>Gets or sets the direction that text and other user interface elements flow within any parent element that controls their layout.</summary>
        /// <returns>The direction that text and other UI elements flow within their parent element, as a value of the enumeration. The default value is <see cref="F:System.Windows.FlowDirection.LeftToRight" />.</returns>
        public FlowDirection FlowDirection
        {
            get
            {
                return (FlowDirection)this.GetValue(FrameworkElement.FlowDirectionProperty);
            }
            set
            {
                this.SetValue(FrameworkElement.FlowDirectionProperty, (Enum)value);
            }
        }

        public static readonly DependencyProperty LanguageProperty = DependencyProperty.Register("Language", typeof(XmlLanguage), typeof(FrameworkElement), null);

        public XmlLanguage Language
        {
            get { return (XmlLanguage)this.GetValue(LanguageProperty); }
            set { this.SetValue(LanguageProperty, value); }
        }
#endif

#if REWORKLOADED
        internal override void INTERNAL_OnVisualParentChanged()
        {
            this.INTERNAL_SizeChangedWhenAttachedToVisualTree(); // Raise SizeChanged event
            this.INTERNAL_RaiseLoadedEvent(); // Raise Loaded event
        }
#endif
    }
}
