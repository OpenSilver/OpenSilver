
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

using System.Buffers;
using System.Collections.Generic;
using System.Windows.Media.Effects;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver;

namespace System.Windows
{
    /// <summary>
    /// UIElement is a base class for most of the objects that have visual appearance
    /// and can process basic input in a user interface.
    /// </summary>
    public abstract partial class UIElement : DependencyObject, IInputElement
    {
        static UIElement()
        {
            RegisterEvents();
        }

        #region Visual Children

        /// <summary>
        /// Gets the visual tree parent of the visual object.
        /// </summary>
        /// <returns>
        /// The <see cref="UIElement"/> parent.
        /// </returns>
        protected DependencyObject VisualParent => InternalVisualParent;

        /// <summary>
        /// Identical to <see cref="VisualParent"/>.
        /// </summary>
        internal DependencyObject InternalVisualParent { get; private set; }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the requested child element in the collection.
        /// </param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index 
        /// is out of range, an exception is thrown.
        /// </returns>
        protected virtual UIElement GetVisualChild(int index) => throw new ArgumentOutOfRangeException(nameof(index));

        /// <summary>
        /// Returns the child at index "index".
        /// </summary>
        internal UIElement InternalGetVisualChild(int index) => GetVisualChild(index);

        /// <summary>
        /// Gets the number of child elements for the <see cref="UIElement"/>.
        /// </summary>
        /// <returns>
        /// The number of child elements.
        /// </returns>
        protected virtual int VisualChildrenCount => 0;

        /// <summary>
        /// Returns the number of children.
        /// </summary>
        internal int InternalVisualChildrenCount => VisualChildrenCount;

        /// <summary>
        /// Defines the parent-child relationship between two visuals.
        /// </summary>
        /// <param name="child">
        /// The child visual object to add to parent visual.
        /// </param>
        protected void AddVisualChild(UIElement child)
        {
            if (child is null)
            {
                return;
            }

            if (child.InternalVisualParent is not null)
            {
                throw new ArgumentException(Strings.UIElement_HasParent);
            }

            HasVisualChildren = true;

            // Set the parent pointer.

            child.InternalVisualParent = this;

            //
            // Resume layout.
            //
            PropagateResumeLayout(this, child);

            // Fire notifications
            OnVisualChildrenChanged(child, null);
            child.OnVisualParentChanged(null);

            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
        }

        /// <summary>
        /// Helper method to provide access to <see cref="AddVisualChild(UIElement)"/> for visual 
        /// collections such as UIElementCollection or TextElementCollection.
        /// </summary>
        internal void InternalAddVisualChild(UIElement child) => AddVisualChild(child);

        /// <summary>
        /// Removes the parent-child relationship between two visuals.
        /// </summary>
        /// <param name="child">
        /// The child visual object to remove from the parent visual.
        /// </param>
        protected void RemoveVisualChild(UIElement child)
        {
            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(child, this);

            if (child is null || child.InternalVisualParent is null)
            {
                return;
            }

            if (child.InternalVisualParent != this)
            {
                throw new ArgumentException(Strings.UIElement_NotChild);
            }

            if (VisualChildrenCount == 0)
            {
                HasVisualChildren = false;
            }

            // Set the parent pointer to null.

            child.InternalVisualParent = null;

            PropagateSuspendLayout(child);

            child.OnVisualParentChanged(this);
            OnVisualChildrenChanged(null, child);
        }

        /// <summary>
        /// Helper method to provide access to <see cref="RemoveVisualChild(UIElement)"/> for visual 
        /// collections such as UIElementCollection or TextElementCollection.
        /// </summary>
        internal void InternalRemoveVisualChild(UIElement child) => RemoveVisualChild(child);

        /// <summary>
        /// Called when the parent of the visual object is changed.
        /// </summary>
        /// <param name="oldParent">
        /// A value of type <see cref="DependencyObject"/> that represents the previous
        /// parent of the <see cref="UIElement"/> object. If the <see cref="UIElement"/>
        /// object did not have a previous parent, the value of the parameter is null.
        /// </param>
        protected internal virtual void OnVisualParentChanged(DependencyObject oldParent)
        {
            // Synchronize ForceInherit properties
            if (InternalVisualParent is not null)
            {
                SynchronizeForceInheritProperties(this, InternalVisualParent);
            }
            else
            {
                if (oldParent is not null)
                {
                    SynchronizeForceInheritProperties(this, oldParent);
                }
            }
        }

        /// <summary>
        /// Called when the visual element collection of the visual object is modified.
        /// </summary>
        /// <param name="visualAdded">
        /// The <see cref="UIElement"/> that was added to the collection.
        /// </param>
        /// <param name="visualRemoved">
        /// The <see cref="UIElement"/> that was removed from the collection.
        /// </param>
        protected internal virtual void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) { }

        /// <Summary>
        /// Flag to check if this visual has any children
        /// </Summary>
        internal bool HasVisualChildren
        {
            get { return ReadVisualFlag(VisualFlags.HasChildren); }
            set { WriteVisualFlag(VisualFlags.HasChildren, value); }
        }

        // Are we in the process of iterating the visual children.
        // This flag is set during a descendents walk, for property invalidation.
        internal bool IsVisualChildrenIterationInProgress
        {
            get { return ReadVisualFlag(VisualFlags.IsVisualChildrenIterationInProgress); }
            set { WriteVisualFlag(VisualFlags.IsVisualChildrenIterationInProgress, value); }
        }

        /// <summary>
        /// AttachChild
        ///
        /// Derived classes must call this method to notify the UIElement layer that a new
        /// child appeard in the children collection. The UIElement layer will then call the GetVisualChild
        /// method to find out where the child was added.
        /// </summary>
        internal void AddVisualChild(IInternalUIElement child)
        {
            Debug.Assert(child is null || child is not UIElement);

            if (child == null)
            {
                return;
            }

            if (child.VisualParent != null)
            {
                throw new ArgumentException(Strings.UIElement_HasParent);
            }

            HasVisualChildren = true;

            // Set the parent pointer.

            child.VisualParent = this;

            child.OnVisualParentChanged(null);
        }

        /// <summary>
        /// DisconnectChild
        ///
        /// Derived classes must call this method to notify the UIElement layer that a
        /// child was removed from the children collection. The UIElement layer will then call
        /// GetChildren to find out which child has been removed.
        /// </summary>
        internal void RemoveVisualChild(IInternalUIElement child)
        {
            Debug.Assert(child is null || child is not UIElement);

            if (child == null || child.VisualParent == null)
            {
                return;
            }

            if (child.VisualParent != this)
            {
                throw new ArgumentException(Strings.UIElement_NotChild);
            }

            if (VisualChildrenCount == 0)
            {
                HasVisualChildren = false;
            }

            // Set the parent pointer to null.

            child.VisualParent = null;

            child.OnVisualParentChanged(this);
        }

        #endregion Visual Children

        private Window _window;

        internal Window ParentWindow
        {
            get => _window;
            set
            {
                if (_window == value)
                {
                    return;
                }

                _window = value;
                OnParentWindowChanged(value);
            }
        }

        internal virtual void OnParentWindowChanged(Window window) { }

        // This is the main DIV of the HTML representation of the control
        internal HtmlElementReference OuterDiv { get; set; }
        internal HashSet<UIElement> VisualChildrenInformation { get; set; }
        public string XamlSourcePath; //this is used by the Simulator to tell where this control is defined. It is non-null only on root elements, that is, elements which class has "InitializeComponent" method. This member is public because it needs to be accessible via reflection.

        public UIElement()
        {
            NeverMeasured = true;
            NeverArranged = true;

            SnapsToDevicePixelsCache = (bool)SnapsToDevicePixelsProperty.GetDefaultValue(DependencyObjectType);
            VisibilityCache = (Visibility)VisibilityProperty.GetDefaultValue(DependencyObjectType);
            ClipToBoundsCache = (bool)ClipToBoundsProperty.GetDefaultValue(DependencyObjectType);

            WriteVisualFlag(VisualFlags.IsUIElement, true);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Metadata is PropertyMetadata metadata && IsLoadedCache)
            {
                metadata.MethodToUpdateDom?.Invoke(this, e.NewValue);
                metadata.MethodToUpdateDom2?.Invoke(this, e.OldValue, e.NewValue);
            }

            base.OnPropertyChanged(e);
        }

        internal void InvokeAccessKey(AccessKeyEventArgs e) => OnAccessKey(e);

        /// <summary>
        /// Provides class handling for when an access key that is meaningful for this element is invoked.
        /// </summary>
        /// <param name="e">
        /// The event data to the access key event. The event data reports which key was invoked, and indicate 
        /// whether the <see cref="AccessKeyManager"/> object that controls the sending of these events also 
        /// sent this access key invocation to other elements.
        /// </param>
        protected virtual void OnAccessKey(AccessKeyEventArgs e) => Focus();

        /// <summary>
        /// Attempts to set focus to this element.
        /// </summary>
        /// <returns>
        /// true if keyboard focus and logical focus were set to this element; false if only logical focus was set to this element, 
        /// or if the call to this method did not force the focus to change.
        /// </returns>
        public bool Focus() =>
            KeyboardNavigation.Current.Focus(this) is UIElement uie &&
            Keyboard.Focus(uie) == uie;

        /// <summary>
        /// Attempts to move focus from this element to another element. The direction to move focus is specified 
        /// by a guidance direction, which is interpreted within the organization of the visual parent for this 
        /// element.
        /// </summary>
        /// <param name="request">
        /// A traversal request, which contains a property that indicates either a mode to traverse in existing tab 
        /// order, or a direction to move visually.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the requested traversal was performed; otherwise, <see langword="false"/>.
        /// </returns>
        public virtual bool MoveFocus(TraversalRequest request) => false;

        /// <summary>
        /// Occurs when the value of the <see cref="Focusable"/> property changes.
        /// </summary>
        public event DependencyPropertyChangedEventHandler FocusableChanged;

        /// <summary>
        /// Identifies the <see cref="Focusable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusableProperty =
            DependencyProperty.Register(
                nameof(Focusable),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnFocusableChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether the element can receive focus. This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if the element is focusable; otherwise false. The default is false.
        /// </returns>
        public bool Focusable
        {
            get => (bool)GetValue(FocusableProperty);
            set => SetValueInternal(FocusableProperty, value);
        }

        private static void OnFocusableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;

            // Raise the public changed event.
            uie.FocusableChanged?.Invoke(uie, e);
        }

        internal static readonly DependencyPropertyKey IsKeyboardFocusedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsKeyboardFocused),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsKeyboardFocusedChanged));

        /// <summary>
        /// Identifies the <see cref="IsKeyboardFocused"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsKeyboardFocusedProperty = IsKeyboardFocusedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="UIElement"/> has focus.
        /// </summary>
        /// <returns>
        /// true if the <see cref="UIElement"/> has focus; otherwise, false.
        /// </returns>
        protected internal virtual bool HasEffectiveKeyboardFocus => IsKeyboardFocused;

        /// <summary>
        /// Gets a value indicating whether this element has keyboard focus. This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if this element has keyboard focus; otherwise, false. The default is false.
        /// </returns>
        public bool IsKeyboardFocused => Keyboard.FocusedElement == this;

        private static void OnIsKeyboardFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UIElement)d).RaiseIsKeyboardFocusedChanged(e);
        }

        private void RaiseIsKeyboardFocusedChanged(DependencyPropertyChangedEventArgs args)
        {
            // Call the virtual method first.
            OnIsKeyboardFocusedChanged(args);

            // Raise the public event second.
            IsKeyboardFocusedChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="IsKeyboardFocusedChanged"/> event is raised on this element.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnIsKeyboardFocusedChanged(DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// Occurs when the value of the <see cref="IsKeyboardFocused"/> property changes on this element.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsKeyboardFocusedChanged;

        internal static readonly DependencyPropertyKey IsKeyboardFocusWithinPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsKeyboardFocusWithin),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsKeyboardFocusWithinChanged));

        /// <summary>
        /// Identifies the <see cref="IsKeyboardFocusWithin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsKeyboardFocusWithinProperty = IsKeyboardFocusWithinPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating whether keyboard focus is anywhere within the element or its visual 
        /// tree child elements. This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if keyboard focus is on the element or its child elements; otherwise, false.
        /// </returns>
        public bool IsKeyboardFocusWithin => ReadFlag(CoreFlags.IsKeyboardFocusWithinCache);

        private static void OnIsKeyboardFocusWithinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;
            uie.WriteFlag(CoreFlags.IsKeyboardFocusWithinCache, (bool)e.NewValue);
            uie.RaiseIsKeyboardFocusWithinChanged(e);
        }

        private void RaiseIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs args)
        {
            // Call the virtual method first.
            OnIsKeyboardFocusWithinChanged(args);

            // Raise the public event second.
            IsKeyboardFocusWithinChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoked just before the <see cref="IsKeyboardFocusWithinChanged"/> event is raised by this element.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// Occurs when the value of the <see cref="IsKeyboardFocusWithin"/> property changes on this element.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsKeyboardFocusWithinChanged;

        internal static readonly DependencyPropertyKey IsFocusedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsFocused),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Identifies the <see cref="IsFocused"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty = IsFocusedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that determines whether this element has logical focus. This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if this element has logical focus; otherwise, false.
        /// </returns>
        public bool IsFocused => (bool)GetValue(IsFocusedProperty);

        #region ClipToBounds

        /// <summary>
        /// Gets or sets a value indicating whether to clip the content of this element
        /// (or content coming from the child elements of this element) to fit into the
        /// size of the containing element. This is a dependency property.
        /// </summary>
        public bool ClipToBounds
        {
            get { return ClipToBoundsCache; }
            set { SetValueInternal(ClipToBoundsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ClipToBounds"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClipToBoundsProperty =
            DependencyProperty.Register(
                nameof(ClipToBounds),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnClipToBoundsChanged));

        private static void OnClipToBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;
            uie.ClipToBoundsCache = (bool)e.NewValue;

            if (!uie.NeverMeasured && !uie.NeverArranged)
            {
                uie.InvalidateArrange();
            }
        }

        #endregion

        #region Clip

        /// <summary>
        /// Identifies the <see cref="Clip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClipProperty =
            DependencyProperty.Register(
                nameof(Clip),
                typeof(Geometry),
                typeof(UIElement),
                new PropertyMetadata(null, OnClipChanged));

        /// <summary>
        /// Gets or sets the <see cref="Geometry"/> used to define the outline of 
        /// the contents of a <see cref="UIElement"/>.
        /// </summary>
        /// <returns>
        /// The geometry to be used for clipping area sizing. The default value is null.
        /// </returns>
        public Geometry Clip
        {
            get => (Geometry)GetValue(ClipProperty);
            set => SetValueInternal(ClipProperty, value);
        }

        private static void OnClipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;

            // if never measured, then nothing to do, it should be measured at some point
            if (!uie.NeverMeasured || !uie.NeverArranged)
            {
                uie.InvalidateArrange();
            }
        }

        private static readonly DependencyProperty VisualClipProperty =
            DependencyProperty.Register(
                nameof(VisualClip),
                typeof(Geometry),
                typeof(UIElement),
                new PropertyMetadata(null, OnVisualClipChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((UIElement)d).SetClipPath((Geometry)newValue),
                });

        internal Geometry VisualClip
        {
            get => (Geometry)GetValue(VisualClipProperty);
            set => SetValueInternal(VisualClipProperty, value);
        }

        private static void OnVisualClipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;

            if (uie._weakVisualClipChangedEventToken != null)
            {
                uie._weakVisualClipChangedEventToken.Dispose();
                uie._weakVisualClipChangedEventToken = null;
            }

            if (e.NewValue is Geometry newClip)
            {
                uie._weakVisualClipChangedEventToken = WeakEvent.Subscribe<UIElement, Geometry, GeometryInvalidatedEventsArgs>(
                    uie,
                    newClip,
                    static (instance, sender, args) => instance.OnVisualClipChanged(sender, args),
                    static (handler, source) => source.Invalidated -= new EventHandler<GeometryInvalidatedEventsArgs>(handler),
                    static (handler, source) => source.Invalidated += new EventHandler<GeometryInvalidatedEventsArgs>(handler));
            }
        }

        private void OnVisualClipChanged(object sender, GeometryInvalidatedEventsArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && e.AffectsMeasure)
            {
                this.SetClipPath((Geometry)sender);
            }
        }

        private WeakEventToken _weakVisualClipChangedEventToken;

        #endregion

        #region SnapsToDevicePixels

        /// <summary>
        /// Identifies the <see cref="SnapsToDevicePixels"/> dependency property.
        /// </summary>
        [NotImplemented]
        public static readonly DependencyProperty SnapsToDevicePixelsProperty =
            DependencyProperty.Register(
                nameof(SnapsToDevicePixels),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnSnapsToDevicePixelsChanged));

        /// <summary>
        /// Gets or sets a value that determines whether rendering for this element should use
        /// device-specific pixel settings during rendering. This is a dependency property.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the element should render in accordance to device pixels; 
        /// otherwise, <see langword="false"/>. The default as declared on <see cref="UIElement"/> 
        /// is <see langword="false"/>.
        /// </returns>
        [NotImplemented]
        public bool SnapsToDevicePixels
        {
            get => SnapsToDevicePixelsCache;
            set => SetValueInternal(SnapsToDevicePixelsProperty, value);
        }

        private static void OnSnapsToDevicePixelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;
            uie.SnapsToDevicePixelsCache = (bool)e.NewValue;
        }

        #endregion

        /// <summary>
        /// When overridden, creates the dom elements designed to represent an instance of an UIElement and defines the place where its child(ren) will be added.
        /// </summary>
        /// <param name="parent">The parent of the UIElement</param>
        /// <returns>The "root" dom element of the UIElement.</returns>
        protected internal abstract HtmlElementReference CreateDomElement(HtmlElementReference parent);

        #region IsEnabled

        /// <summary>
        /// Gets a value that becomes the return value of <see cref="IsEnabled"/> in derived classes.
        /// </summary>
        /// <returns>
        /// true if the element is enabled; otherwise, false.
        /// </returns>
        protected virtual bool IsEnabledCore => true;

        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                nameof(IsEnabled),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.TrueBox, OnIsEnabledChanged, CoerceIsEnabled)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((UIElement)d).ManageIsEnabled((bool)newValue),
                });

        /// <summary>
        /// Gets or sets a value indicating whether the user can interact with the control.
        /// </summary>
        /// <returns>
        /// true if the user can interact with the control; otherwise, false.
        /// </returns>
        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValueInternal(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;

            // Raise the public changed event.
            uie.IsEnabledChanged?.Invoke(uie, e);
            uie.InvalidateForceInheritPropertyOnChildren(e.Property);

            // Update pointer events
            uie.CoerceIsHitTestable();
        }

        private static object CoerceIsEnabled(DependencyObject d, object baseValue)
        {
            var uie = (UIElement)d;

            // We must be false if our parent is false, but we can be
            // either true or false if our parent is true.
            //
            // Another way of saying this is that we can only be true
            // if our parent is true, but we can always be false.
            if ((bool)baseValue)
            {
                // Our parent can constrain us.  We can be plugged into either
                // a "visual" or "content" tree.  If we are plugged into a
                // "content" tree, the visual tree is just considered a
                // visual representation, and is normally composed of raw
                // visuals, not UIElements, so we prefer the content tree.
                //
                // The content tree uses the "logical" links.  But not all
                // "logical" links lead to a content tree.
                //
                DependencyObject parent = uie.InternalVisualParent;
                if (parent == null || (bool)parent.GetValue(IsEnabledProperty))
                {
                    return BooleanBoxes.Box(uie.IsEnabledCore);
                }
                else
                {
                    return BooleanBoxes.FalseBox;
                }
            }
            else
            {
                return BooleanBoxes.FalseBox;
            }
        }

        /// <summary>
        /// Occurs when the <see cref="IsEnabled"/> property changes.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsEnabledChanged;

        protected internal virtual void ManageIsEnabled(bool isEnabled) { }

        #endregion

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
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
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
            set => SetValueInternal(EffectProperty, value);
        }

        private static void OnEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;

            if (element._weakEffectChangedEventToken != null)
            {
                element._weakEffectChangedEventToken.Dispose();
                element._weakEffectChangedEventToken = null;
            }

            if (e.NewValue is Effect newEffect)
            {
                element._weakEffectChangedEventToken = WeakEvent.Subscribe<UIElement, Effect, EventArgs>(
                    element,
                    newEffect,
                    static (instance, sender, args) => instance.OnEffectChanged(sender, args),
                    static (handler, source) => source.Changed -= new EventHandler(handler),
                    static (handler, source) => source.Changed += new EventHandler(handler));
            }
        }

        private void OnEffectChanged(object sender, EventArgs e) => ((Effect)sender).Render(this);

        private WeakEventToken _weakEffectChangedEventToken;

        #endregion


        #region RenderTransform and RenderTransformOrigin

        /// <summary>
        /// Identifies the <see cref="RenderTransform"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderTransformProperty =
            DependencyProperty.Register(
                nameof(RenderTransform),
                typeof(Transform),
                typeof(UIElement),
                new PropertyMetadata(Transform.Identity, OnRenderTransformChanged));

        /// <summary>
        /// Gets or sets transform information that affects the rendering position of a <see cref="UIElement"/>.
        /// </summary>
        /// <returns>
        /// Describes the specifics of the desired render transform. The default value is null.
        /// </returns>
        public Transform RenderTransform
        {
            get => (Transform)GetValue(RenderTransformProperty) ?? Transform.Identity;
            set => SetValueInternal(RenderTransformProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RenderTransformOrigin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderTransformOriginProperty =
            DependencyProperty.Register(
                nameof(RenderTransformOrigin),
                typeof(Point),
                typeof(UIElement),
                new PropertyMetadata(new Point(0, 0), OnRenderTransformChanged));

        /// <summary>
        /// Gets or sets the origin point of any possible render transform declared by
        /// <see cref="RenderTransform"/>, relative to the bounds of the <see cref="UIElement"/>.
        /// </summary>
        /// <returns>
        /// The origin point of the render transform. The default value is a point with value 0,0.
        /// </returns>
        public Point RenderTransformOrigin
        {
            get => (Point)GetValue(RenderTransformOriginProperty);
            set => SetValueInternal(RenderTransformOriginProperty, value);
        }

        private static void OnRenderTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;

            //if never measured, then nothing to do, it should be measured at some point
            if (!uie.NeverMeasured && !uie.NeverArranged)
            {
                uie.InvalidateArrange();
                uie.AreTransformsClean = false;
            }
        }

        private static readonly DependencyProperty VisualTransformProperty =
            DependencyProperty.Register(
                nameof(VisualTransform),
                typeof(Transform),
                typeof(UIElement),
                new PropertyMetadata(null, OnVisualTransformChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((UIElement)d).SetTransform((Transform)newValue),
                });

        internal Transform VisualTransform
        {
            get => (Transform)GetValue(VisualTransformProperty);
            set => SetValueInternal(VisualTransformProperty, value);
        }

        private static void OnVisualTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;

            if (uie._weakVisualTransformChangedEventToken != null)
            {
                uie._weakVisualTransformChangedEventToken.Dispose();
                uie._weakVisualTransformChangedEventToken = null;
            }

            if (e.NewValue is Transform newTransform)
            {
                uie._weakVisualTransformChangedEventToken = WeakEvent.Subscribe<UIElement, Transform, EventArgs>(
                    uie,
                    newTransform,
                    static (instance, sender, args) => instance.OnVisualTransformChanged(sender, args),
                    static (handler, source) => source.Changed -= new EventHandler(handler),
                    static (handler, source) => source.Changed += new EventHandler(handler));
            }
        }

        private void OnVisualTransformChanged(object sender, EventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                this.SetTransform((Transform)sender);
            }
        }

        private WeakEventToken _weakVisualTransformChangedEventToken;

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
            set { SetValueInternal(UseLayoutRoundingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="UseLayoutRounding"/> dependency property.
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

        private Visibility VisibilityCache
        {
            get
            {
                if (ReadVisualFlag(VisualFlags.VisibilityCache_Visible))
                {
                    return Visibility.Visible;
                }
                else if (ReadVisualFlag(VisualFlags.VisibilityCache_TakesSpace))
                {
                    return Visibility.Hidden;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            set
            {
                Debug.Assert(value == Visibility.Visible || value == Visibility.Hidden || value == Visibility.Collapsed);

                switch (value)
                {
                    case Visibility.Visible:
                        WriteVisualFlag(VisualFlags.VisibilityCache_Visible, true);
                        WriteVisualFlag(VisualFlags.VisibilityCache_TakesSpace, false);
                        break;

                    case Visibility.Hidden:
                        WriteVisualFlag(VisualFlags.VisibilityCache_Visible, false);
                        WriteVisualFlag(VisualFlags.VisibilityCache_TakesSpace, true);
                        break;

                    case Visibility.Collapsed:
                        WriteVisualFlag(VisualFlags.VisibilityCache_Visible, false);
                        WriteVisualFlag(VisualFlags.VisibilityCache_TakesSpace, false);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of a UIElement. A UIElement that is not visible
        /// is not rendered and does not communicate its desired size to layout.
        /// </summary>
        public Visibility Visibility
        {
            get => VisibilityCache;
            set => SetValueInternal(VisibilityProperty, VisibilityBoxes.Box(value));
        }

        /// <summary>
        /// Identifies the <see cref="Visibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register(
                nameof(Visibility),
                typeof(Visibility),
                typeof(UIElement),
                new PropertyMetadata(VisibilityBoxes.VisibleBox, OnVisibilityChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                        INTERNAL_HtmlDomManager.SetVisibility(((UIElement)d).OuterDiv, (Visibility)newValue),
                },
                ValidateVisibility);

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;
            Visibility newVisibility = (Visibility)e.NewValue;

            uie.VisibilityCache = newVisibility;
            uie.SwitchVisibilityIfNeeded(newVisibility);

            uie.UpdateIsRenderableCache();

            // The IsVisible property depends on this property.
            uie.UpdateIsVisibleCache();
        }

        private static bool ValidateVisibility(object o)
        {
            var value = (Visibility)o;
            return value == Visibility.Visible || value == Visibility.Hidden || value == Visibility.Collapsed;
        }

        private void SwitchVisibilityIfNeeded(Visibility visibility)
        {
            switch (visibility)
            {
                case Visibility.Visible:
                    EnsureVisible();
                    break;

                case Visibility.Hidden:
                    EnsureInvisible(false);
                    break;

                case Visibility.Collapsed:
                    EnsureInvisible(true);
                    break;
            }
        }

        private void EnsureVisible()
        {
            if (ReadFlag(CoreFlags.IsCollapsed))
            {
                WriteFlag(CoreFlags.IsCollapsed, false);

                // invalidate parent if needed
                SignalDesiredSizeChange();

                // make sure element has been rendered
                InvalidateVisual();
            }
        }

        private void EnsureInvisible(bool collapsed)
        {
            if (ReadFlag(CoreFlags.IsCollapsed) != collapsed)
            {
                WriteFlag(CoreFlags.IsCollapsed, collapsed);

                // invalidate parent
                SignalDesiredSizeChange();
            }
        }

        private void SignalDesiredSizeChange()
        {
            if (GetLayoutParent(this) is UIElement p)
            {
                p.OnChildDesiredSizeChanged(this);
            }
        }

        internal bool IsRenderable
        {
            get => ReadVisualFlag(VisualFlags.IsRenderable);
            private set => WriteVisualFlag(VisualFlags.IsRenderable, value);
        }

        internal void UpdateIsRenderableCache()
        {
            bool isRenderable;

            if (InternalVisualParent is UIElement parent)
            {
                isRenderable = parent.IsRenderable;
            }
            else
            {
                isRenderable = INTERNAL_VisualTreeManager.IsElementInVisualTree(this);
            }

            Rec(this, isRenderable);

            static void Rec(UIElement uie, bool isRenderable)
            {
                if (isRenderable)
                {
                    isRenderable = !uie.ReadFlag(CoreFlags.IsCollapsed);
                }

                if (uie.IsRenderable != isRenderable)
                {
                    uie.IsRenderable = isRenderable;

                    if (isRenderable)
                    {
                        uie.ResumeRendering();
                        uie.InvalidateMeasureInternal();
                    }

                    int count = uie.VisualChildrenCount;
                    for (int i = 0; i < count; i++)
                    {
                        if (uie.GetVisualChild(i) is UIElement child)
                        {
                            Rec(child, isRenderable);
                        }
                    }
                }
            }
        }

        #endregion

        #region IsVisible

        // The IsVisible property is a read-only reflection of the Visibility property.
        private static readonly ReadOnlyPropertyMetadata _isVisibleMetadata = new(BooleanBoxes.FalseBox, GetIsVisible, OnIsVisibleChanged);

        private static readonly DependencyPropertyKey IsVisiblePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsVisible),
                typeof(bool),
                typeof(UIElement),
                _isVisibleMetadata);

        /// <summary>
        /// Identifies the <see cref="IsVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty = IsVisiblePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating whether this element is visible in the user interface (UI).
        /// This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if the element is visible; otherwise, false.
        /// </returns>
        public bool IsVisible => ReadFlag(CoreFlags.IsVisibleCache);

        private static object GetIsVisible(DependencyObject d) => BooleanBoxes.Box(((UIElement)d).IsVisible);

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;

            // Invalidate the children so that they will inherit the new value.
            uie.InvalidateForceInheritPropertyOnChildren(e.Property);

            // Raise the public changed event.
            uie.IsVisibleChanged?.Invoke(uie, e);

            // Update pointer events
            uie.CoerceIsHitTestable();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="IsVisible"/> property changes on this element.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsVisibleChanged;

        internal void UpdateIsVisibleCache()
        {
            // IsVisible is a read-only property.  It derives its "base" value
            // from the Visibility property.
            bool isVisible = Visibility == Visibility.Visible;

            // We must be false if our parent is false, but we can be
            // either true or false if our parent is true.
            //
            // Another way of saying this is that we can only be true
            // if our parent is true, but we can always be false.
            if (isVisible)
            {
                bool constraintAllowsVisible;

                // Our parent can constrain us.
                if (InternalVisualParent is UIElement parent)
                {
                    constraintAllowsVisible = parent.IsVisible;
                }
                else
                {
                    constraintAllowsVisible = INTERNAL_VisualTreeManager.IsElementInVisualTree(this);
                }

                if (!constraintAllowsVisible)
                {
                    isVisible = false;
                }
            }

            if (isVisible != IsVisible)
            {
                // Our IsVisible force-inherited property has changed.  Update our
                // cache and raise a change notification.

                WriteFlag(CoreFlags.IsVisibleCache, isVisible);
                NotifyPropertyChange(
                    new DependencyPropertyChangedEventArgs(
                        BooleanBoxes.Box(!isVisible),
                        BooleanBoxes.Box(isVisible),
                        IsVisibleProperty,
                        _isVisibleMetadata));
            }
        }

        #endregion

        #region Opacity

        /// <summary>
        /// Identifies the <see cref="Opacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(
                nameof(Opacity),
                typeof(double),
                typeof(UIElement),
                new PropertyMetadata(1.0)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((UIElement)d).SetOpacity((double)newValue),
                });

        /// <summary>
        /// Gets or sets the degree of the object's opacity.
        /// </summary>
        /// <returns>
        /// A value between 0 and 1.0 that declares the opacity factor, with 1.0 meaning
        /// full opacity and 0 meaning transparent. The default value is 1.0.
        /// </returns>
        public double Opacity
        {
            get => (double)GetValue(OpacityProperty);
            set => SetValueInternal(OpacityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="OpacityMask"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpacityMaskProperty =
            DependencyProperty.Register(
                nameof(OpacityMask),
                typeof(Brush),
                typeof(UIElement),
                new PropertyMetadata(null, OnOpacityMaskChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((UIElement)d).SetMaskImage((Brush)newValue),
                });

        /// <summary>
        /// Gets or sets the brush used to alter the opacity of regions of this object.
        /// </summary>
        /// <returns>
        /// A brush that describes the opacity applied to this object. The default is null.
        /// </returns>
        public Brush OpacityMask
        {
            get => (Brush)GetValue(OpacityMaskProperty);
            set => SetValueInternal(OpacityMaskProperty, value);
        }

        private static void OnOpacityMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;

            if (uie._weakOpacityMaskChangedEventToken != null)
            {
                uie._weakOpacityMaskChangedEventToken.Dispose();
                uie._weakOpacityMaskChangedEventToken = null;
            }

            if (e.NewValue is Brush newBrush && !newBrush.IsSealed)
            {
                uie._weakOpacityMaskChangedEventToken = WeakEvent.Subscribe<UIElement, Brush, EventArgs>(
                    uie,
                    newBrush,
                    static (instance, sender, args) => instance.OnOpacityMaskChanged(sender, args),
                    static (handler, source) => source.Changed -= new EventHandler(handler),
                    static (handler, source) => source.Changed += new EventHandler(handler));
            }
        }

        private void OnOpacityMaskChanged(object sender, EventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                this.SetMaskImage(OpacityMask);
            }
        }

        private WeakEventToken _weakOpacityMaskChangedEventToken;

        #endregion

        #region IsHitTestVisible

        /// <summary>
        /// Gets or sets whether the contained area of this UIElement can return true
        /// values for hit testing.
        /// </summary>
        public bool IsHitTestVisible
        {
            get => (bool)GetValue(IsHitTestVisibleProperty);
            set => SetValueInternal(IsHitTestVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsHitTestVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.Register(
                nameof(IsHitTestVisible),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.TrueBox, OnIsHitTestVisibleChanged, CoerceIsHitTestVisible));

        private static void OnIsHitTestVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;

            // Raise the public changed event.
            uie.IsHitTestVisibleChanged?.Invoke(uie, e);

            // Invalidate the children so that they will inherit the new value.
            uie.InvalidateForceInheritPropertyOnChildren(e.Property);

            // Update pointer events
            uie.CoerceIsHitTestable();
        }

        private static object CoerceIsHitTestVisible(DependencyObject d, object baseValue)
        {
            UIElement uie = (UIElement)d;

            // We must be false if our parent is false, but we can be
            // either true or false if our parent is true.
            //
            // Another way of saying this is that we can only be true
            // if our parent is true, but we can always be false.
            if ((bool)baseValue)
            {
                DependencyObject parent = uie.InternalVisualParent;
                if (parent == null || (bool)parent.GetValue(IsHitTestVisibleProperty))
                {
                    return BooleanBoxes.TrueBox;
                }
                else
                {
                    return BooleanBoxes.FalseBox;
                }
            }
            else
            {
                return BooleanBoxes.FalseBox;
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="IsHitTestVisible"/> dependency property changes on this element.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsHitTestVisibleChanged;

        #endregion

        #region pointer-events

        /// <summary>
        /// Gets the value that pointer-events (css) should be coerced to.
        /// </summary>
        internal virtual bool EnablePointerEventsCore => false;

        internal virtual void SetPointerEvents(bool hitTestable) =>
            OuterDiv.SetCssStyleProperty(CssPropertyNames.PointerEvents, hitTestable ? "auto" : "none");

        private static readonly ReadOnlyPropertyMetadata _isHitTestableMetadata =
            new(BooleanBoxes.FalseBox, GetIsHitTestable)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((UIElement)d).SetPointerEvents((bool)newValue),
            };

        private static readonly DependencyPropertyKey IsHitTestablePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsHitTestable),
                typeof(bool),
                typeof(UIElement),
                _isHitTestableMetadata);

        private static readonly DependencyProperty IsHitTestableProperty = IsHitTestablePropertyKey.DependencyProperty;

        private bool IsHitTestable => ReadVisualFlag(VisualFlags.IsHitTestable);

        private static object GetIsHitTestable(DependencyObject d) => BooleanBoxes.Box(((UIElement)d).IsHitTestable);

        internal void CoerceIsHitTestable()
        {
            bool isHitTestable = EnablePointerEventsCore && IsEnabled && IsHitTestVisible && IsVisible;

            if (IsHitTestable != isHitTestable)
            {
                WriteVisualFlag(VisualFlags.IsHitTestable, isHitTestable);

                if (!isHitTestable && IsMouseOver)
                {
                    ClearValue(IsMouseOverPropertyKey);
                }

                NotifyPropertyChange(
                    new DependencyPropertyChangedEventArgs(
                        BooleanBoxes.Box(!isHitTestable),
                        BooleanBoxes.Box(isHitTestable),
                        IsHitTestableProperty,
                        _isHitTestableMetadata));
            }
        }

        #endregion pointer-events

        private static readonly DependencyProperty UseSystemFocusVisualsProperty =
            DependencyProperty.Register(
                nameof(UseSystemFocusVisuals),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((UIElement)d).SetOutline((bool)newValue),
                });

        internal bool UseSystemFocusVisuals
        {
            get => (bool)GetValue(UseSystemFocusVisualsProperty);
            set => SetValue(UseSystemFocusVisualsProperty, value);
        }

        #region AllowDrop

        /// <summary>
        /// Gets or sets a value that determines whether this UIElement
        /// can be a drop target for purposes of drag-and-drop operations.
        /// </summary>
        public bool AllowDrop
        {
            get { return (bool)GetValue(AllowDropProperty); }
            set { SetValueInternal(AllowDropProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AllowDrop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowDropProperty =
            DependencyProperty.Register(
                nameof(AllowDrop),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(false) { Inherits = true });

        #endregion

        #region CapturePointer, ReleasePointerCapture, IsPointerCaptured, and OnLostMouseCapture

        internal static readonly DependencyPropertyKey IsMouseCapturedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsMouseCaptured),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsMouseCapturedChanged));

        /// <summary>
        /// Identifies the <see cref="IsMouseCaptured"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseCapturedProperty = IsMouseCapturedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating whether the mouse is captured to this element.
        /// </summary>
        /// <returns>
        /// true if the element has mouse capture; otherwise, false. The default is false.
        /// </returns>
        public bool IsMouseCaptured => Mouse.Captured == this;

        private static void OnIsMouseCapturedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UIElement)d).RaiseIsMouseCapturedChanged(e);
        }

        private void RaiseIsMouseCapturedChanged(DependencyPropertyChangedEventArgs args)
        {
            // Call the virtual method first.
            OnIsMouseCapturedChanged(args);

            // Raise the public event second.
            IsMouseCapturedChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="IsMouseCapturedChanged"/> event is raised on this element. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnIsMouseCapturedChanged(DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// Occurs when the value of the <see cref="IsMouseCaptured"/> property changes on this element.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsMouseCapturedChanged;

        internal static readonly DependencyPropertyKey IsMouseCaptureWithinPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsMouseCaptureWithin),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsMouseCaptureWithinChanged));

        /// <summary>
        /// Identifies the <see cref="IsMouseCaptureWithin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseCaptureWithinProperty = IsMouseCaptureWithinPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that determines whether mouse capture is held by this element or by child elements 
        /// in its visual tree. This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if this element or a contained element has mouse capture; otherwise, false.
        /// </returns>
        public bool IsMouseCaptureWithin => ReadFlag(CoreFlags.IsMouseCaptureWithinCache);

        private static void OnIsMouseCaptureWithinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UIElement)d).RaiseIsMouseCaptureWithinChanged(e);
        }

        private void RaiseIsMouseCaptureWithinChanged(DependencyPropertyChangedEventArgs args)
        {
            // Call the virtual method first.
            OnIsMouseCaptureWithinChanged(args);

            // Raise the public event second.
            IsMouseCaptureWithinChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="IsMouseCaptureWithinChanged"/> event is raised on this element.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnIsMouseCaptureWithinChanged(DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// Occurs when the value of the <see cref="IsMouseCaptureWithinProperty"/> changes on this element.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsMouseCaptureWithinChanged;

        /// <summary>
        /// Attempts to force capture of the mouse to this element.
        /// </summary>
        /// <returns>
        /// true if the mouse is successfully captured; otherwise, false.
        /// </returns>
        public bool CaptureMouse() => Mouse.Capture(this);

        /// <summary>
        /// Releases the mouse capture, if this element held the capture.
        /// </summary>
        public void ReleaseMouseCapture()
        {
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }
        }

        internal static readonly DependencyPropertyKey IsMouseOverPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsMouseOver),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsMouseOverChanged));

        /// <summary>
        /// Identifies the <see cref="IsMouseOver"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty = IsMouseOverPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating whether the mouse pointer is located over this element (including child elements 
        /// in the visual tree). This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if mouse pointer is over the element or its child elements; otherwise, false. The default is false.
        /// </returns>
        public bool IsMouseOver => ReadFlag(CoreFlags.IsMouseOverCache);

        private static void OnIsMouseOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UIElement)d).WriteFlag(CoreFlags.IsMouseOverCache, (bool)e.NewValue);
        }

        internal static readonly DependencyPropertyKey IsMouseDirectlyOverPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsMouseDirectlyOver),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsMouseDirectlyOverChanged));

        /// <summary>
        /// Identifies the <see cref="IsMouseDirectlyOver"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseDirectlyOverProperty = IsMouseDirectlyOverPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether the position of the mouse pointer corresponds to hit 
        /// test results, which take element compositing into account. This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if the mouse pointer is over the same element result as a hit test; otherwise, false. 
        /// The default is false.
        /// </returns>
        public bool IsMouseDirectlyOver => Mouse.DirectlyOver == this;

        private static void OnIsMouseDirectlyOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UIElement)d).RaiseIsMouseDirectlyOverChanged(e);
        }

        private void RaiseIsMouseDirectlyOverChanged(DependencyPropertyChangedEventArgs args)
        {
            // Call the virtual method first.
            OnIsMouseDirectlyOverChanged(args);

            // Raise the public event second.
            IsMouseDirectlyOverChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="IsMouseDirectlyOverChanged"/> event is raised on this element.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnIsMouseDirectlyOverChanged(DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// Occurs when the value of the <see cref="IsMouseDirectlyOver"/> property changes on this element.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsMouseDirectlyOverChanged;

        #endregion

        #region AllowScrollOnTouchMove

        /// <summary>
        /// Gets or sets whether pressing (touchscreen devices) on this UIElement then moving should allow scrolling or not. The default value is True.
        /// </summary>
        public bool AllowScrollOnTouchMove
        {
            get { return (bool)GetValue(AllowScrollOnTouchMoveProperty); }
            set { SetValueInternal(AllowScrollOnTouchMoveProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AllowScrollOnTouchMove"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowScrollOnTouchMoveProperty =
            DependencyProperty.Register(
                nameof(AllowScrollOnTouchMove),
                typeof(bool),
                typeof(UIElement),
                new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        /// <summary>
        /// Determines whether the visual object is an ancestor of the descendant visual object.
        /// </summary>
        /// <param name="descendant">
        /// A value of type <see cref="DependencyObject"/>.
        /// </param>
        /// <returns>
        /// true if the visual object is an ancestor of descendant; otherwise, false.
        /// </returns>
        public bool IsAncestorOf(DependencyObject descendant)
        {
            ArgumentNullException.ThrowIfNull(descendant);

            if (descendant is not UIElement uie)
            {
                throw new ArgumentException(string.Format(Strings.UIElement_NotAnUIElement, nameof(descendant)));
            }

            return uie.IsDescendantOf(this);
        }

        /// <summary>
        /// Determines whether the visual object is a descendant of the ancestor visual object.
        /// </summary>
        /// <param name="ancestor">
        /// A value of type <see cref="DependencyObject"/>.
        /// </param>
        /// <returns>
        /// true if the visual object is a descendant of ancestor; otherwise, false.
        /// </returns>
        public bool IsDescendantOf(DependencyObject ancestor)
        {
            ArgumentNullException.ThrowIfNull(ancestor);

            if (ancestor is not UIElement)
            {
                throw new ArgumentException(string.Format(Strings.UIElement_NotAnUIElement, nameof(ancestor)));
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

        /// <summary>
        /// Returns the common ancestor of two visual objects.
        /// </summary>
        /// <param name="otherVisual">
        /// A visual object of type <see cref="DependencyObject"/>.
        /// </param>
        /// <returns>
        /// The common ancestor of the visual object and otherVisual if one exists; otherwise, null.
        /// </returns>
        public DependencyObject FindCommonVisualAncestor(DependencyObject otherVisual)
        {
            ArgumentNullException.ThrowIfNull(otherVisual);

            if (otherVisual is UIElement other)
            {
                return FindCommonVisualAncestor(other);
            }

            return null;
        }

        #region ForceInherit property support

        internal static void SynchronizeForceInheritProperties(UIElement uie, DependencyObject parent)
        {
            if (parent is UIElement parentUIE && parentUIE.IsRenderable)
            {
                uie.UpdateIsRenderableCache();
            }

            if (!(bool)parent.GetValue(IsEnabledProperty))
            {
                uie.CoerceValue(IsEnabledProperty);
            }

            if (!(bool)parent.GetValue(IsHitTestVisibleProperty))
            {
                uie.CoerceValue(IsHitTestVisibleProperty);
            }

            if ((bool)parent.GetValue(IsVisibleProperty))
            {
                uie.UpdateIsVisibleCache();
            }
        }

        internal void InvalidateForceInheritPropertyOnChildren(DependencyProperty property)
        {
            int cChildren = VisualChildrenCount;
            for (int i = 0; i < cChildren; i++)
            {
                if (GetVisualChild(i) is UIElement child)
                {
                    if (property == IsVisibleProperty)
                    {
                        child.UpdateIsVisibleCache();
                    }
                    else
                    {
                        child.CoerceValue(property);
                    }
                }
            }
        }

        #endregion ForceInherit property support

        internal bool IsLoadedCache
        {
            get => ReadVisualFlag(VisualFlags.IsLoadedCache);
            set => WriteVisualFlag(VisualFlags.IsLoadedCache, value);
        }

        internal bool IsRenderingSuspended
        {
            get => ReadVisualFlag(VisualFlags.IsRenderingSuspended);
            set => WriteVisualFlag(VisualFlags.IsRenderingSuspended, value);
        }

        internal bool IsConnectedToLiveTree
        {
            get => ReadVisualFlag(VisualFlags.IsConnectedToLiveTree);
            set => WriteVisualFlag(VisualFlags.IsConnectedToLiveTree, value);
        }

        internal bool IsUnloading
        {
            get => ReadVisualFlag(VisualFlags.IsUnloading);
            set => WriteVisualFlag(VisualFlags.IsUnloading, value);
        }

        internal bool IsVisualTreeRoot
        {
            get => ReadVisualFlag(VisualFlags.IsVisualTreeRoot);
            set => WriteVisualFlag(VisualFlags.IsVisualTreeRoot, value);
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree() => AttachVisualChildren();

        internal virtual void AttachVisualChildren()
        {
            for (int i = 0; i < VisualChildrenCount; i++)
            {
                if (GetVisualChild(i) is UIElement child)
                {
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
                }
            }
        }

        internal void RenderVisual()
        {
            if (EffectiveValuesCount > 0)
            {
                // we copy the Dictionary so that the foreach doesn't break when 
                // we modify a DependencyProperty inside the Changed of another 
                // one (which causes it to be added to the Dictionary).
                // we exclude properties where source is set to default because
                // it means they have been set at some point, and unset afterward,
                // so we should not call the PropertyChanged callback.

                Storage[] storages = ArrayPool<Storage>.Shared.Rent(EffectiveValuesCount);
                int length = 0;
                foreach (Storage storage in EffectiveValues)
                {
                    if (storage.Entry.FullValueSource <= (FullValueSource)BaseValueSourceInternal.Default)
                    {
                        continue;
                    }

                    storages[length++] = storage;
                }

                Span<Storage> span = storages.AsSpan(0, length);
                try
                {
                    foreach (Storage storage in span)
                    {
                        DependencyProperty dp = DependencyProperty.RegisteredPropertyList[storage.PropertyIndex];
                        Debug.Assert(dp is not null);

                        if (dp.GetMetadata(DependencyObjectType) is not PropertyMetadata metadata)
                        {
                            continue;
                        }

                        object value = null;
                        bool valueWasRetrieved = false;

                        //--------------------------------------------------
                        // Call "MethodToUpdateDom"
                        //--------------------------------------------------
                        if (metadata.MethodToUpdateDom is not null)
                        {
                            if (!valueWasRetrieved)
                            {
                                value = DependencyObjectStore.GetEffectiveValue(ref storage.Entry, RequestFlags.FullyResolved);
                                valueWasRetrieved = true;
                            }

                            // Call the "Method to update DOM"
                            metadata.MethodToUpdateDom(this, value);
                        }

                        if (metadata.MethodToUpdateDom2 is not null)
                        {
                            if (!valueWasRetrieved)
                            {
                                value = DependencyObjectStore.GetEffectiveValue(ref storage.Entry, RequestFlags.FullyResolved);
                                valueWasRetrieved = true;
                            }

                            // DependencyProperty.UnsetValue for the old value signify that
                            // the old value should be ignored.
                            metadata.MethodToUpdateDom2(
                                this,
                                DependencyProperty.UnsetValue,
                                value);
                        }

                        //--------------------------------------------------
                        // Call PropertyChanged
                        //--------------------------------------------------

                        if (metadata.PropertyChangedCallback is not null
#pragma warning disable CS0618 // Type or member is obsolete
                            && metadata.CallPropertyChangedWhenLoadedIntoVisualTree != WhenToCallPropertyChangedEnum.Never)
#pragma warning restore CS0618 // Type or member is obsolete
                        {
                            if (!valueWasRetrieved)
                            {
                                value = DependencyObjectStore.GetEffectiveValue(ref storage.Entry, RequestFlags.FullyResolved);
                                valueWasRetrieved = true;
                            }

                            // Raise the "PropertyChanged" event
                            metadata.PropertyChangedCallback(
                                this,
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

            if (IsHitTestable)
            {
                SetPointerEvents(true);
            }
        }

        internal void SuspendRendering() => IsRenderingSuspended = true;

        private void ResumeRendering()
        {
            if (IsRenderingSuspended)
            {
                IsRenderingSuspended = false;
                RenderVisual();
            }
        }

        internal bool ReadFlag(CoreFlags field) => (_flags & field) != 0;

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

        internal bool ReadVisualFlag(VisualFlags field) => (_visualFlags & field) != 0;

        internal void WriteVisualFlag(VisualFlags field, bool value)
        {
            if (value)
            {
                _visualFlags |= field;
            }
            else
            {
                _visualFlags &= (~field);
            }
        }

        private CoreFlags _flags;
        private VisualFlags _visualFlags;

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnLayoutUpdated()
        {
            //
        }
    }

    [Flags]
    internal enum CoreFlags : uint
    {
        None = 0x00000000,
        SnapsToDevicePixelsCache = 0x00000001,
        ClipToBoundsCache = 0x00000002,
        MeasureDirty = 0x00000004,
        ArrangeDirty = 0x00000008,
        MeasureInProgress = 0x00000010,
        ArrangeInProgress = 0x00000020,
        NeverMeasured = 0x00000040,
        NeverArranged = 0x00000080,
        MeasureDuringArrange = 0x00000100,
        IsCollapsed = 0x00000200,
        IsKeyboardFocusWithinCache = 0x00000400,
        //IsKeyboardFocusWithinChanged = 0x00000800,
        IsMouseOverCache = 0x00001000,
        //IsMouseOverChanged = 0x00002000,
        IsMouseCaptureWithinCache = 0x00004000,
        //IsMouseCaptureWithinChanged = 0x00008000,
        //IsStylusOverCache = 0x00010000,
        //IsStylusOverChanged = 0x00020000,
        //IsStylusCaptureWithinCache = 0x00040000,
        //IsStylusCaptureWithinChanged = 0x00080000,
        HasAutomationPeer = 0x00100000,
        RenderingInvalidated = 0x00200000,
        IsVisibleCache = 0x00400000,
        AreTransformsClean = 0x00800000,
        BypassLayoutPolicies = 0x01000000, //IsOpacitySuppressed = 0x01000000,
        //ExistsEventHandlersStore = 0x02000000,
        //TouchesOverCache = 0x04000000,
        //TouchesOverChanged = 0x08000000,
        //TouchesCapturedWithinCache = 0x10000000,
        //TouchesCapturedWithinChanged = 0x20000000,
        //TouchLeaveCache = 0x40000000,
        //TouchEnterCache = 0x80000000,
    }

    /// <summary>
    /// Visual flags.
    /// </summary>
    [Flags]
    internal enum VisualFlags : uint
    {
        /// <summary>
        /// No flags are set for this visual.
        /// </summary>
        None = 0x0,

        // TreeLevel counter - occupies 11 bits. 
        // NOTE: The location of these bits in this ulong should be synchronized with 
        // UIElement.TreeLevel property getter/setter.
        TreeLevelBit0 = 0x00000001,
        TreeLevelBit1 = 0x00000002,
        TreeLevelBit2 = 0x00000004,
        TreeLevelBit3 = 0x00000008,
        TreeLevelBit4 = 0x00000010,
        TreeLevelBit5 = 0x00000020,
        TreeLevelBit6 = 0x00000040,
        TreeLevelBit7 = 0x00000080,
        TreeLevelBit8 = 0x00000100,
        TreeLevelBit9 = 0x00000200,
        TreeLevelBit10 = 0x00000400,

        // Needs documentation
        IsUIElement = 0x00000800,

        // For UIElement -- It's in VisualFlags so that it can be propagated through the
        // Visual subtree without casting.
        IsLayoutSuspended = 0x00001000,

        // Are we in the process of iterating the visual children. 
        // This flag is set during a descendents walk, for property invalidation.
        IsVisualChildrenIterationInProgress = 0x00002000,

        // FindCommonAncestor is used to find the common ancestor of a Visual.
        FindCommonAncestor = 0x00004000,

        // Indicates if this Visual is the root of its visual tree. This is used for popups to ignore PopupRoot when
        // calling VisualTreeHelper.GetParent(...).
        IsVisualTreeRoot = 0x00008000,

        // These bits together make up UIElement.VisibilityCache
        VisibilityCache_Visible = 0x00010000,
        VisibilityCache_TakesSpace = 0x00020000,

        // Indicates if the visual has any children. Avoids calls to visualchildrencount while checking for presence of children.
        HasChildren = 0x00040000,

        // Indicates if rendering is suspended for this Visual. Rendering is suspended when an element is inside a collapsed
        // visual tree.
        IsRenderingSuspended = 0x00080000,

        // Indicates if this Visual can be rendered. An element can be rendered if it has been attached to the render tree and
        // is not inside a collapsed tree.
        IsRenderable = 0x00100000,

        // Indicates if this Visual is connected to the render tree.
        IsConnectedToLiveTree = 0x00200000,

        // Indicates if this Visual has been loaded into the render tree. (see FrameworkElement.IsLoaded).
        IsLoadedCache = 0x00400000,

        // Indicates if this Visual is being detached from the render tree.
        IsUnloading = 0x00800000,

        // Indicates if this Visual can be the target of pointer events.
        IsHitTestable = 0x01000000,
    }
}