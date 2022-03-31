
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup;
using CSHTML5.Internal;
using OpenSilver.Internal;
using System.ComponentModel;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
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
        #region Inheritance Context

        internal static FrameworkElement FindMentor(DependencyObject d)
        {
            // Find the nearest FE InheritanceContext
            while (d != null)
            {
                FrameworkElement fe = d as FrameworkElement;

                if (fe != null)
                {
                    return fe;
                }
                else
                {
                    d = d.InheritanceContext;
                }
            }

            return null;
        }

        internal override bool ShouldProvideInheritanceContext(DependencyObject target, DependencyProperty property)
        {
            return base.ShouldProvideInheritanceContext(target, property) || property == ResourceDictionary.ResourceKeyProperty;
        }

        internal FrameworkElement InheritedParent { get; private set; }

        internal override void OnInheritanceContextChangedCore(EventArgs args)
        {
            FrameworkElement oldMentor = InheritedParent;
            FrameworkElement newMentor = FindMentor(InheritanceContext);

            if (oldMentor != newMentor)
            {
                InheritedParent = newMentor;

                if (oldMentor != null)
                {
                    DisconnectMentor(oldMentor);
                }

                if (newMentor != null)
                {
                    ConnectMentor(newMentor);
                }
            }
        }

        private void ConnectMentor(FrameworkElement mentor)
        {
            mentor.InheritedPropertyChanged += new InheritedPropertyChangedEventHandler(OnMentorInheritedPropertyChanged);
            
            InvalidateInheritedProperties(this, mentor);
        }

        private void DisconnectMentor(FrameworkElement mentor)
        {
            mentor.InheritedPropertyChanged -= new InheritedPropertyChangedEventHandler(OnMentorInheritedPropertyChanged);

            InvalidateInheritedProperties(this, mentor);
        }

        // handle the InheritedPropertyChanged event from the mentor
        private void OnMentorInheritedPropertyChanged(object sender, InheritedPropertyChangedEventArgs e)
        {
            TreeWalkHelper.InvalidateOnInheritablePropertyChange(this, e.Info, false);
        }

        internal event InheritedPropertyChangedEventHandler InheritedPropertyChanged;

        internal static void OnInheritedPropertyChanged(FrameworkElement fe, InheritablePropertyChangeInfo info)
        {
            var handler = fe.InheritedPropertyChanged;
            if (handler != null)
            {
                handler(fe, new InheritedPropertyChangedEventArgs(ref info));
            }
        }

        #endregion Inheritance Context

        #region Visual Children

        internal override void OnVisualParentChanged(DependencyObject oldParent)
        {
            DependencyObject newParent = VisualTreeHelper.GetParent(this);

            // Do it only if you do not have a logical parent
            if (this.Parent == null)
            {
                // Invalidate relevant properties for this subtree
                this.OnParentChangedInternal(newParent ?? oldParent);
            }

            base.OnVisualParentChanged(oldParent);
        }

        /// <summary>
        /// Gets the number of Visual children of this FrameworkElement.
        /// </summary>
        /// <remarks>
        /// Derived classes override this property getter to provide the children count
        /// of their custom children collection.
        /// </remarks>
        internal override int VisualChildrenCount
        {
            get
            {
                return (_templateChild == null) ? 0 : 1;
            }
        }

        /// <summary>
        /// Gets the Visual child at the specified index.
        /// </summary>
        /// <remarks>
        /// Derived classes that provide a custom children collection must override this method
        /// and return the child at the specified index.
        /// </remarks>
        internal override UIElement GetVisualChild(int index)
        {
            if (_templateChild == null || index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _templateChild;
        }

        #endregion Visual Children

        #region Logical Parent

        /// <summary>
        /// Gets the parent object of this FrameworkElement in the object tree.
        /// </summary>
        public DependencyObject Parent
        {
            get;
            private set;
        }

        internal void AddLogicalChild(object child)
        {
            if (child != null)
            {
                // It is invalid to modify the children collection that we
                // might be iterating during a property invalidation tree walk.
                if (IsLogicalChildrenIterationInProgress)
                {
                    throw new InvalidOperationException("Cannot modify the logical children for this node at this time because a tree walk is in progress.");
                }

                HasLogicalChildren = true;

                FrameworkElement fe = child as FrameworkElement;
                if (fe != null)
                {
                    fe.ChangeLogicalParent(this);
                }
            }
        }

        internal void RemoveLogicalChild(object child)
        {
            if (child != null)
            {
                // It is invalid to modify the children collection that we
                // might be iterating during a property invalidation tree walk.
                if (IsLogicalChildrenIterationInProgress)
                {
                    throw new InvalidOperationException("Cannot modify the logical children for this node at this time because a tree walk is in progress.");
                }

                if (child is FrameworkElement fe && fe.Parent == this)
                {
                    fe.ChangeLogicalParent(null);
                }

                // This could have been the last child, so check if we have any more children
                IEnumerator children = LogicalChildren;

                // if null, there are no children.
                if (children == null)
                {
                    HasLogicalChildren = false;
                }
                else
                {
                    // If we can move next, there is at least one child
                    HasLogicalChildren = children.MoveNext();
                }
            }
        }

        internal void ChangeLogicalParent(DependencyObject newParent)
        {
            // Logical Parent must first be dropped before you are attached to a newParent
            if (Parent != null && newParent != null && Parent != newParent)
            {
                throw new InvalidOperationException("Specified element is already the logical child of another element. Disconnect it first.");
            }

            // Trivial check to avoid loops
            if (newParent == this)
            {
                throw new InvalidOperationException("Element cannot be its own parent.");
            }

            Parent = newParent;

            OnParentChangedInternal(newParent);
        }

        private void OnParentChangedInternal(DependencyObject parent)
        {
            // For now we only update the value of inherited properties

            InvalidateInheritedProperties(this, parent);
        }

        internal static void InvalidateInheritedProperties(UIElement uie, DependencyObject newParent)
        {
            if (newParent == null)
            {
                uie.ResetInheritedProperties();
            }
            else
            {
                INTERNAL_PropertyStorage[] storages = newParent.INTERNAL_AllInheritedProperties.Values.ToArray();
                foreach (var storage in storages)
                {
                    uie.SetInheritedValue(storage.Property,
                                          INTERNAL_PropertyStore.GetEffectiveValue(storage.Entry),
                                          true);
                }
            }
        }

        /// <summary>
        /// Returns enumerator to logical children
        /// </summary>
        /*protected*/ internal virtual IEnumerator LogicalChildren
        {
            get { return null; }
        }

        internal bool IsLogicalChildrenIterationInProgress
        {
            get { return ReadInternalFlag(InternalFlags.IsLogicalChildrenIterationInProgress); }
            set { WriteInternalFlag(InternalFlags.IsLogicalChildrenIterationInProgress, value); }
        }

        internal bool HasLogicalChildren
        {
            get { return ReadInternalFlag(InternalFlags.HasLogicalChildren); }
            set { WriteInternalFlag(InternalFlags.HasLogicalChildren, value); }
        }

        #endregion Logical Parent

        internal DependencyObject TemplatedParent { get; set; }

        private FrameworkElement _templateChild; // Non-null if this FE has a child that was created as part of a template.

        // Note: TemplateChild is an UIElement in WPF.
        internal virtual FrameworkElement TemplateChild
        {
            get
            {
                return this._templateChild;
            }
            set
            {
                if (this._templateChild != value)
                {
                    INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(this._templateChild, this);
                    this.RemoveVisualChild(this._templateChild);
                    this._templateChild = value;
                    this.AddVisualChild(this._templateChild);
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(this._templateChild, this, 0);
                }
            }
        }

        /// <summary>
        /// Gets the element that should be used as the StateGroupRoot for VisualStateMangager.GoToState calls
        /// </summary>
        internal virtual FrameworkElement StateGroupsRoot
        {
            get
            {
                return _templateChild;
            }
        }

        //--------------------------------------
        // Note: this is a "partial" class. For anything related to Size and Alignment, please refer to the other file ("FrameworkElement_HandlingSizeAndAlignment.cs").
        //--------------------------------------

        // Note: this is used to be able to tell whether the style applied on 
        // the FrameworkElement is an ImplicitStyle, which means that it must 
        // be removed from the element when it is detached from the visual tree.
        //internal bool INTERNAL_IsImplicitStyle = false;

        // Note: this is used by the PopupRoot to ensure that the PopupRoot 
        // has no pointer events while the container has. 
        // todo: replace with another technique to achieve the same result.
        internal bool INTERNAL_ForceEnableAllPointerEvents = false;

        [Obsolete]
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
            // Initialize the _styleCache to the default value for StyleProperty.
            // If the default value is non-null then wire it to the current instance.
            PropertyMetadata metadata = StyleProperty.GetMetadata(this.GetType());
            Style defaultValue = (Style)metadata.DefaultValue;
            if (defaultValue != null)
            {
                OnStyleChanged(this, new DependencyPropertyChangedEventArgs(null, defaultValue, StyleProperty));
            }

            Application app = Application.Current;
            if (app != null && app.HasImplicitStylesInResources)
            {
                ShouldLookupImplicitStyles = true;
            }
        }

#region Resources

        /// <summary>
        ///     Check if resource is not empty.
        ///     Call HasResources before accessing resources every time you need
        ///     to query for a resource.
        /// </summary>
        internal bool HasResources
        {
            get
            {
                ResourceDictionary resources = _resources;
                return (resources != null &&
                        ((resources.Count > 0) || (resources.MergedDictionaries.Count > 0)));
            }
        }

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
                {
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.AddOwner(this);
                    _resources = resource;
                }
                return _resources;
            }
            set
            {
                ResourceDictionary oldValue = _resources;
                _resources = value;

                if (oldValue != null)
                {
                    // This element is no longer an owner for the old RD
                    oldValue.RemoveOwner(this);
                }

                if (value != null)
                {
                    if (!value.ContainsOwner(this))
                    {
                        // This element is an owner for the new RD
                        value.AddOwner(this);
                    }
                }

                // todo: implement this.
                //// Invalidate ResourceReference properties for this subtree
                //// 
                //if (oldValue != value)
                //{
                //    TreeWalkHelper.InvalidateOnResourcesChange(this, null, new ResourcesChangeInfo(oldValue, value));
                //}

                // todo: remove the following block when 'InvalidateOnResourcesChange' is implemented
                {
                    HasStyleInvalidated = false;

                    if (HasImplicitStyleFromResources == true &&
                        (oldValue.Contains(GetType()) || Style == StyleProperty.GetMetadata(GetType()).DefaultValue))
                    {
                        UpdateStyleProperty();
                    }
                }
            }
        }
        
#endregion

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
            var div1style = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out div1);
            if (!this.IsUnderCustomLayout || INTERNAL_ForceEnableAllPointerEvents)
            {
                object div2;
                var div2style = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", div1, this, out div2);
                div2style.width = "100%";
                div2style.height = "100%";
                if (INTERNAL_ForceEnableAllPointerEvents)
                    div2style.pointerEvents = "all";
                domElementWhereToPlaceChildren = div2;

                if (this.IsCustomLayoutRoot)
                {
                    div1style.position = "relative";
                }
            }
            else
            {
                domElementWhereToPlaceChildren = div1;
            }
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
            var div1style = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out div1);
            object div2;
            var div2style = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", div1, this, out div2);
            div2style.width = "100%";
            div2style.height = "100%";
            if (INTERNAL_ForceEnableAllPointerEvents)
                div2style.pointerEvents = "all";
            domElementWhereToPlaceChildren = div2;
            return div1;
        }
#if BRIDGE
        [Bridge.Template("true")]
#endif
        static bool IsRunningInJavaScript()
        {
            return false;
        }

        // Internal helper so the FrameworkElement could see the
        // ControlTemplate/DataTemplate set on the
        // Control/ContentPresenter/ItemsPresenter
        internal virtual FrameworkTemplate TemplateInternal
        {
            get { return null; }
        }

        // Internal helper so the FrameworkElement could see the
        // ControlTemplate/DataTemplate set on the
        // Control/ContentPresenter/ItemsPresenter
        internal virtual FrameworkTemplate TemplateCache
        {
            get { return null; }
            set { }
        }

        internal bool ApplyTemplate()
        {
            // Notify the ContentPresenter/ItemsPresenter that we are about to generate the
            // template tree and allow them to choose the right template to be applied.
            this.OnPreApplyTemplate();

            bool visualsCreated = false;
            if (this.TemplateInternal != null)
            {
                FrameworkTemplate template = this.TemplateInternal;

                // we only apply the template if no template has been
                // rendered already for this control.
                if (this.TemplateChild == null)
                {
                    visualsCreated = template.ApplyTemplateContent(this);
                }
            }

            if (visualsCreated)
            {
                // Call the OnApplyTemplate method
                this.OnApplyTemplate();
            }

            return visualsCreated;
        }

        /// <summary>
        /// This virtual is called by FE.ApplyTemplate before it does work to generate the template tree.
        /// </summary>
        /// This virtual is overridden for the following reasons
        /// 1. By ContentPresenter/ItemsPresenter to choose the template to be applied in this case.
        internal virtual void OnPreApplyTemplate()
        {

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

        // Note: the returned Size is unused for now.
        internal override sealed Size MeasureCore()
        {
            if (!this.ApplyTemplate())
            {
                if (this.TemplateChild != null)
                {
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(this.TemplateChild, this, 0);
                }
            }
            return new Size();
        }

        //
        //  This method
        //  1. Updates the template cache for the given fe
        //
        internal static void UpdateTemplateCache(
            FrameworkElement fe,
            FrameworkTemplate oldTemplate,
            FrameworkTemplate newTemplate,
            DependencyProperty templateProperty)
        {
            if (newTemplate != null)
            {
                newTemplate.Seal();
            }

            // Update the template cache
            fe.TemplateCache = newTemplate;

            fe.TemplateChild = null;
        }

        /// <summary>
        /// Attaches a binding to a FrameworkElement, using the provided binding object.
        /// </summary>
        /// <param name="dependencyProperty">The dependency property identifier of the property that is data bound.</param>
        /// <param name="binding">The binding to use for the property.</param>
        /// <returns>The BindingExpression created.</returns>
        public BindingExpression SetBinding(DependencyProperty dependencyProperty, Binding binding)
        {
            return BindingOperations.SetBinding(this, dependencyProperty, binding);
        }

        public BindingExpression GetBindingExpression(DependencyProperty dp)
        {
            return BindingOperations.GetBindingExpression(this, dp);
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

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.Cursor"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.Register(
                nameof(Cursor), 
                typeof(Cursor), 
                typeof(FrameworkElement), 
                new PropertyMetadata((object)null)
                {
                    MethodToUpdateDom = Cursor_MethodToUpdateDom,
                });

        private static void Cursor_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var fe = (FrameworkElement)d;
            var styleOfOuterDomElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(fe.INTERNAL_OuterDomElement);
            styleOfOuterDomElement.cursor = ((Cursor)newValue)?.ToHtmlString() ?? "inherit";
        }

        #endregion

        #region IsEnabled

        /// <summary>
        ///     Fetches the value that IsEnabled should be coerced to.
        /// </summary>
        /// <remarks>
        ///     This method is virtual is so that controls derived from UIElement
        ///     can combine additional requirements into the coersion logic.
        ///     <P/>
        ///     It is important for anyone overriding this property to also
        ///     call CoerceValue when any of their dependencies change.
        /// </remarks>
        internal virtual bool IsEnabledCore
        {
            get
            {
                // ButtonBase.IsEnabledCore: CanExecute
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can interact with the control.
        /// </summary>
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.IsEnabled"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                nameof(IsEnabled),
                typeof(bool),
                typeof(FrameworkElement),
                new PropertyMetadata(true, IsEnabled_Changed, CoerceIsEnabled)
                {
                    MethodToUpdateDom = IsEnabled_MethodToUpdateDom,
                });

        private static void IsEnabled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            if (fe.IsEnabledChanged != null)
            {
                fe.IsEnabledChanged(fe, e);
            }
            fe.InvalidateForceInheritPropertyOnChildren(e.Property);
        }

        private static object CoerceIsEnabled(DependencyObject d, object baseValue)
        {
            FrameworkElement fe = (FrameworkElement)d;

            if (!(baseValue is bool)) //todo: this is a temporary workaround to avoid an invalid cast exception. Fix this by investigating why sometimes baseValue is not a bool, such as a Binding (eg. Client_GD).
                return true;

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
                DependencyObject parent = fe.Parent ?? VisualTreeHelper.GetParent(fe);
                if (parent == null || (bool)parent.GetValue(IsEnabledProperty))
                {
                    return fe.IsEnabledCore;
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

        /// <summary>
        /// Occurs when the IsEnabled property changes.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsEnabledChanged;

        private static void IsEnabled_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var element = (FrameworkElement)d;
            SetPointerEvents(element);
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

        #region ForceInherit property support

        internal override void InvalidateForceInheritPropertyOnChildren(DependencyProperty property)
        {
            if (property == IsEnabledProperty)
            {
                IEnumerator enumerator = LogicalChildren;

                if (enumerator != null)
                {
                    while (enumerator.MoveNext())
                    {
                        DependencyObject child = enumerator.Current as DependencyObject;
                        if (child != null)
                        {
                            child.CoerceValue(property);
                        }
                    }
                }
            }

            base.InvalidateForceInheritPropertyOnChildren(property);
        }

        #endregion ForceInherit property support

        #region Names handling

        /// <summary>
        /// Retrieves an object that has the specified identifier name.
        /// </summary>
        /// <param name="name">
        /// The name of the requested object.
        /// </param>
        /// <returns>
        /// The requested object. This can be null if no matching object was found in the
        /// current XAML namescope.
        /// </returns>
        public object FindName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            INameScope nameScope = FindScope(this);
            if (nameScope != null)
            {
                return nameScope.FindName(name);
            }

            return null;
        }

        internal static INameScope FindScope(DependencyObject d)
        {
            while (d != null)
            {
                INameScope nameScope = NameScope.GetNameScope(d);
                if (nameScope != null)
                {
                    return nameScope;
                }

                d = (d as FrameworkElement)?.Parent;
            }

            return null;
        }

        /// <summary>
        ///     Retrieves the element in the VisualTree of thie element that corresponds to
        ///     the element with the given childName in this element's style definition
        /// </summary>
        /// <param name="childName">the Name to find the matching element for</param>
        /// <returns>The Named element.  Null if no element has this Name.</returns>
        internal DependencyObject GetTemplateChild(string childName)
        {
            return FindScope(TemplateChild)?.FindName(childName) as DependencyObject;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegisterName(string name, object scopedElement)
        {
            INameScope nameScope = FindScope(this);
            if (nameScope != null)
            {
                nameScope.RegisterName(name, scopedElement);
            }
            else
            {
                throw new InvalidOperationException(string.Format("No NameScope found to {1} the Name '{0}'.", name, "register"));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UnregisterName(string name)
        {
            INameScope nameScope = FindScope(this);
            if (nameScope != null)
            {
                nameScope.UnregisterName(name);
            }
            else
            {
                throw new InvalidOperationException(string.Format("No NameScope found to {1} the Name '{0}'.", name, "unregister"));
            }
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
        /// Identifies the <see cref="Name"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register(
                nameof(Name), 
                typeof(string), 
                typeof(FrameworkElement), 
                new PropertyMetadata(string.Empty)
                {
                    MethodToUpdateDom = OnNameChanged_MethodToUpdateDom,
                });

        private static void OnNameChanged_MethodToUpdateDom(DependencyObject d, object value)
        {
            var fe = (FrameworkElement)d;
            INTERNAL_HtmlDomManager.SetDomElementAttribute(fe.INTERNAL_OuterDomElement, "dataId", (value ?? string.Empty).ToString());
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
        /// Identifies the <see cref="FrameworkElement.DataContext"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataContextProperty = 
            DependencyProperty.Register(
                nameof(DataContext),
                typeof(object),
                typeof(FrameworkElement),
                new PropertyMetadata(null, OnDataContextPropertyChanged)
                {
                    Inherits = true
                });

        private static void OnDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FrameworkElement)d).RaiseDataContextChangedEvent(e);
        }

        private void RaiseDataContextChangedEvent(DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContextChanged != null)
            {
                this.DataContextChanged(this, e);
            }
        }

        /// <summary>Occurs when the data context for this element changes. </summary>
        public event DependencyPropertyChangedEventHandler DataContextChanged;

        #endregion

        #region Triggers

        /// <summary>
        /// Gets the collection of triggers for animations that are defined for a <see cref="FrameworkElement"/>.
        /// </summary>
        /// <returns>
        /// The collection of triggers for animations that are defined for this object.
        /// </returns>
        public TriggerCollection Triggers
        {
            get
            {
                TriggerCollection triggers = (TriggerCollection)GetValue(TriggersProperty);
                if (triggers == null)
                {
                    triggers = new TriggerCollection(this);
                    SetValue(TriggersProperty, triggers);
                }

                return triggers;
            }
        }

        /// <summary>
        /// Identifies the <see cref="Triggers"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty TriggersProperty =
            DependencyProperty.Register(
                nameof(Triggers),
                typeof(TriggerCollection),
                typeof(FrameworkElement),
                null);

        #endregion Triggers

        public event EventHandler LayoutUpdated;

        protected override void OnLayoutUpdated()
        {
            LayoutUpdated?.Invoke(this, new EventArgs());
        }

        #region Work in progress

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FlowDirectionProperty =
            DependencyProperty.Register(
                nameof(FlowDirection),
                typeof(FlowDirection),
                typeof(FrameworkElement),
                new PropertyMetadata(FlowDirection.LeftToRight));

        /// <summary>
        /// Gets or sets the direction that text and other user interface 
        /// elements flow within any parent element that controls their layout.
        /// </summary>
        /// <returns>
        /// The direction that text and other UI elements flow within their 
        /// parent element, as a value of the enumeration. The default value 
        /// is <see cref="FlowDirection.LeftToRight" />.
        /// </returns>
        [OpenSilver.NotImplemented]
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

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LanguageProperty =
            DependencyProperty.Register(
                nameof(Language),
                typeof(XmlLanguage),
                typeof(FrameworkElement),
                null);

        [OpenSilver.NotImplemented]
        public XmlLanguage Language
        {
            get { return (XmlLanguage)this.GetValue(LanguageProperty); }
            set { this.SetValue(LanguageProperty, value); }
        }

        internal override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // Skip when loading or changed on TextMeasurement Div.
            if (this.INTERNAL_OuterDomElement == null || 
                Application.Current.TextMeasurementService.IsTextMeasureDivID(
                    ((INTERNAL_HtmlDomElementReference)this.INTERNAL_OuterDomElement).UniqueIdentifier))
            {
                return;
            }

            var metadata = e.Property.GetMetadata(GetType()) as FrameworkPropertyMetadata;
            
            if (metadata != null)
            {
                if (metadata.AffectsMeasure)
                {
                    InvalidateMeasure();
                }

                if (metadata.AffectsArrange)
                {
                    InvalidateArrange();
                }

                if (metadata.AffectsRender)
                {
                    //InvalidateVisual();
                }

                if (metadata.AffectsParentMeasure)
                {
                    InvalidateParentMeasure();
                }

                if (metadata.AffectsParentArrange)
                {
                    InvalidateParentArrange();
                }
            }
        }
        internal sealed override void ArrangeCore(Rect finalRect)
        {
            bool isDefaultAlignment = HorizontalAlignment == HorizontalAlignment.Stretch && VerticalAlignment == VerticalAlignment.Stretch;
            Size finalSize = isDefaultAlignment ? finalRect.Size : new Size(
                HorizontalAlignment != HorizontalAlignment.Stretch ? Math.Min(DesiredSize.Width, finalRect.Width) : finalRect.Width,
                VerticalAlignment != VerticalAlignment.Stretch ? Math.Min(DesiredSize.Height, finalRect.Height) : finalRect.Height);

            if (!this.IsCustomLayoutRoot)
            {
                finalSize.Width = Math.Max(0, finalSize.Width - Margin.Left - Margin.Right);
                finalSize.Height = Math.Max(0, finalSize.Height - Margin.Top - Margin.Bottom);
            }

            Size MinSize = new Size(MinWidth, MinHeight);
            Size MaxSize = new Size(MaxWidth, MaxHeight);
            Size size = new Size(Width, Height);

            finalSize = size.Combine(finalSize).Bounds(MinSize, MaxSize);

            Size arrangedSize = ArrangeOverride(finalSize);
            
            var w = Math.Max(0, Math.Min(finalSize.Width, arrangedSize.Width));
            var h = Math.Max(0, Math.Min(finalSize.Height, arrangedSize.Height));
            arrangedSize = new Size(w, h);

            arrangedSize = size.Combine(arrangedSize).Bounds(MinSize, MaxSize);

            Rect containingRect = new Rect(arrangedSize);

            // Add Margin
            if (!this.IsCustomLayoutRoot)
            {
                double newLeft = containingRect.Left - Margin.Left;
                double newTop = containingRect.Top - Margin.Top;
                double newWidth = containingRect.Width + Margin.Left + Margin.Right;
                double newHeight = containingRect.Height + Margin.Top + Margin.Bottom;

                if (newWidth >= 0 && newHeight >= 0)
                    containingRect = new Rect(newLeft, newTop, newWidth, newHeight);
                else
                    throw new ArgumentException("Width or Height cannot be lower than 0");
            }

            Point alignedOffset = GetAlignmentOffset(finalRect, containingRect.Size, HorizontalAlignment, VerticalAlignment);

            Point visualOffset = new Point(alignedOffset.X - containingRect.Location.X, alignedOffset.Y - containingRect.Location.Y);

            VisualBounds = new Rect(visualOffset, arrangedSize);

            // Call SizeChanged event handlers
            if (!this.IsCustomLayoutRoot)
                this.HandleSizeChanged(new Size(VisualBounds.Width, VisualBounds.Height));

            if (isFirstRendering)
            {
                isFirstRendering = false;
                INTERNAL_HtmlDomManager.GetDomElementStyleForModification(this.INTERNAL_OuterDomElement).visibility = "visible";
            }
        }

        private static Point GetAlignmentOffset(Rect container, Size alignedRectSize, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            double alignedLeft = container.Left;
            double alignedTop = container.Top;

            if (horizontalAlignment == HorizontalAlignment.Right)
            {
                alignedLeft = container.Left + container.Width - alignedRectSize.Width;
            }

            if (horizontalAlignment == HorizontalAlignment.Center || horizontalAlignment == HorizontalAlignment.Stretch)
            {
                alignedLeft = container.Left + (container.Width - alignedRectSize.Width) / 2;
            }

            if (verticalAlignment == VerticalAlignment.Bottom)
            {
                alignedTop = container.Top + container.Height - alignedRectSize.Height;
            }

            if (verticalAlignment == VerticalAlignment.Center || verticalAlignment == VerticalAlignment.Stretch)
            {
                alignedTop = container.Top + (container.Height - alignedRectSize.Height) / 2;
            }

            return alignedLeft == 0 && alignedTop == 0 ? new Point() : new Point(alignedLeft, alignedTop);
        }

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
            IEnumerable<DependencyObject> childElements = VisualTreeHelper.GetVisualChildren(this);

            if (childElements.Count() > 0)
            {
                UIElement elementChild = ((UIElement)childElements.ElementAt(0));
                elementChild.Arrange(new Rect(finalSize));
                return finalSize;
            }

            return finalSize;
        }

        internal sealed override Size MeasureCore(Size availableSize)
        {
            Size MinSize = new Size(MinWidth, MinHeight);
            Size MaxSize = new Size(MaxWidth, MaxHeight);
            Size size = new Size(Width, Height);

            availableSize.Width = Math.Max(0, availableSize.Width - Margin.Left - Margin.Right);
            availableSize.Height = Math.Max(0, availableSize.Height - Margin.Top - Margin.Bottom);
            
            availableSize = size.Combine(availableSize).Bounds(MinSize, MaxSize);

            Size measuredSize = MeasureOverride(availableSize);
            
            var w = Math.Max(0, Math.Min(availableSize.Width, measuredSize.Width));
            var h = Math.Max(0, Math.Min(availableSize.Height, measuredSize.Height));
            measuredSize = new Size(w, h);
            measuredSize = size.Combine(measuredSize).Bounds(MinSize, MaxSize);

            measuredSize.Width = Math.Max(0, measuredSize.Width + Margin.Left + Margin.Right);
            measuredSize.Height = Math.Max(0, measuredSize.Height + Margin.Top + Margin.Bottom);

            return measuredSize;
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
            INTERNAL_HtmlDomElementReference domElementReference = (INTERNAL_HtmlDomElementReference)this.INTERNAL_OuterDomElement;
            Debug.WriteLine($"FrmeworkElement MeasureOverride ({this}) {domElementReference.UniqueIdentifier}, ({Width}, {Height})");
            return new Size();
        }

#endregion Work in progress

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
        /// Identifies the <see cref="FrameworkElement.Tag"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TagProperty =
            DependencyProperty.Register(
                nameof(Tag), 
                typeof(object), 
                typeof(FrameworkElement), 
                new PropertyMetadata((object)null));

#endregion

#region Handling Styles

        [Obsolete("Use DefaultStyleKey")]
        protected void INTERNAL_SetDefaultStyle(Style defaultStyle)
        {
            INTERNAL_defaultStyle = defaultStyle;
        }

        // Style/Template state (internals maintained by Style, per-instance data in StyleDataField)
        private Style _styleCache;

        // ThemeStyle used only when a ThemeStyleKey is specified (per-instance data in ThemeStyleDataField)
        private Style _themeStyleCache;

        /// <summary>
        /// Gets or sets an instance Style that is applied for this object during rendering.
        /// </summary>
        public Style Style
        {
            get { return _styleCache; }
            set { SetValue(StyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.Style"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register(
                nameof(Style), 
                typeof(Style), 
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnStyleChanged));

        private static void OnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            fe.HasLocalStyle = fe.ReadLocalValueInternal(StyleProperty) != DependencyProperty.UnsetValue;
            UpdateStyleCache(fe, (Style)e.OldValue, (Style)e.NewValue, ref fe._styleCache);
        }

        //
        //  This method
        //  1. Updates the style cache for the given fe
        //
        private static void UpdateStyleCache(
            FrameworkElement fe,
            Style oldStyle,
            Style newStyle,
            ref Style styleCache)
        {
            Dictionary<DependencyProperty, object> newStylePropertyValues = null;
            if (newStyle != null)
            {
                // We have a new style. Make sure it's targeting the right
                // type, and then seal it.

                newStyle.CheckTargetType(fe);
                newStyle.Seal();
                newStylePropertyValues = newStyle.EffectiveValues;
            }

            styleCache = newStyle;

            if (oldStyle != null)
            {
                // Clear old style values
                // if a property is about to be set again in the new style
                // we don't unset the value directly to prevent from potientially
                // firing the DependencyPropertyChanged callback twice.                
                Dictionary<DependencyProperty, object> oldStylePropertyValues = oldStyle.EffectiveValues;
                IEnumerable<DependencyProperty> removedProperties = 
                    oldStylePropertyValues
                        .Where(kp => newStylePropertyValues == null || !newStylePropertyValues.ContainsKey(kp.Key))
                        .Select(kp => kp.Key);
                foreach (var property in removedProperties)
                {
                    fe.SetLocalStyleValue(property, DependencyProperty.UnsetValue);
                }
            }

            if (newStyle != null)
            {
                foreach (var propertyValue in newStylePropertyValues)
                {
                    object value;
                    if (propertyValue.Value is Binding binding)
                    {
                        value = new BindingExpression(binding.Clone(), propertyValue.Key);
                    }
                    else
                    {
                        value = propertyValue.Value;
                    }
                    fe.SetLocalStyleValue(propertyValue.Key, value);
                }
            }
        }

        // Indicates if the StyleProperty has been invalidated during a tree walk
        internal bool HasStyleInvalidated
        {
            get { return ReadInternalFlag(InternalFlags.HasStyleInvalidated); }
            set { WriteInternalFlag(InternalFlags.HasStyleInvalidated, value); }
        }

        // Indicates if the Style is being re-evaluated
        internal bool IsStyleUpdateInProgress
        {
            get { return ReadInternalFlag(InternalFlags.IsStyleUpdateInProgress); }
            set { WriteInternalFlag(InternalFlags.IsStyleUpdateInProgress, value); }
        }

        // Indicates if there are any implicit styles in the ancestry
        internal bool ShouldLookupImplicitStyles
        {
            get { return ReadInternalFlag(InternalFlags.ShouldLookupImplicitStyles); }
            set { WriteInternalFlag(InternalFlags.ShouldLookupImplicitStyles, value); }
        }

        // Note: this is used to be able to tell whether the style applied on 
        // the FrameworkElement is an ImplicitStyle, which means that it must 
        // be removed from the element when it is detached from the visual tree.

        // Indicates if this instance has an implicit style
        internal bool HasImplicitStyleFromResources
        {
            get { return ReadInternalFlag(InternalFlags.HasImplicitStyleFromResources); }
            set { WriteInternalFlag(InternalFlags.HasImplicitStyleFromResources, value); }
        }

        private void InvalidateStyleProperty()
        {
            // Try to find an implicit style
            object implicitStyle = FrameworkElement.FindImplicitStyleResource(this, GetType());

            // Set the flag associated with the StyleProperty
            HasImplicitStyleFromResources = implicitStyle != DependencyProperty.UnsetValue;
            SetImplicitReferenceValue(StyleProperty, implicitStyle);
        }

        internal void UpdateStyleProperty()
        {
            if (!HasStyleInvalidated)
            {
                if (IsStyleUpdateInProgress == false)
                {
                    IsStyleUpdateInProgress = true;
                    try
                    {
                        InvalidateStyleProperty();
                        HasStyleInvalidated = true;
                    }
                    finally
                    {
                        IsStyleUpdateInProgress = false;
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cyclic reference found while evaluating the Style property on element '{0}'.", this));
                }
            }
        }

        internal static object FindImplicitStyleResource(FrameworkElement fe, object resourceKey)
        {
            if (fe.ShouldLookupImplicitStyles)
            {
                object implicitStyle;
                // First, try to find an implicit style in parents' resources.
                for (FrameworkElement f = fe; f != null; f = VisualTreeHelper.GetParent(f) as FrameworkElement)
                {
                    if (f.HasResources && f.Resources.HasImplicitStyles)
                    {
                        implicitStyle = f.Resources[resourceKey];
                        if (implicitStyle != null)
                        {
                            return implicitStyle;
                        }
                    }
                }
                // Then we try to find the resource in the App's Resources
                // if we can't find it in the parents.
                Application app = Application.Current;
                if (app != null)
                {
                    implicitStyle = app.Resources[resourceKey];
                    if (implicitStyle != null)
                    {
                        return implicitStyle;
                    }
                }
            }
            return DependencyProperty.UnsetValue;
        }

#region DefaultStyleKey

        // Indicates if the ThemeStyle is being re-evaluated
        internal bool IsThemeStyleUpdateInProgress
        {
            get { return ReadInternalFlag(InternalFlags.IsThemeStyleUpdateInProgress); }
            set { WriteInternalFlag(InternalFlags.IsThemeStyleUpdateInProgress, value); }
        }

        // Indicates that the ThemeStyleProperty full fetch has been
        // performed atleast once on this node
        internal bool HasThemeStyleEverBeenFetched
        {
            get { return ReadInternalFlag(InternalFlags.HasThemeStyleEverBeenFetched); }
            set { WriteInternalFlag(InternalFlags.HasThemeStyleEverBeenFetched, value); }
        }

        // Indicates that the StyleProperty has been set locally on this element
        internal bool HasLocalStyle
        {
            get { return ReadInternalFlag(InternalFlags.HasLocalStyle); }
            set { WriteInternalFlag(InternalFlags.HasLocalStyle, value); }
        }

        /// <summary>
        /// Gets or sets the key that references the default style for the control.
        /// </summary>
        protected object DefaultStyleKey
        {
            get { return (object)GetValue(DefaultStyleKeyProperty); }
            set { SetValue(DefaultStyleKeyProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.DefaultStyleKey"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty DefaultStyleKeyProperty =
            DependencyProperty.Register(
                nameof(DefaultStyleKey), 
                typeof(object), 
                typeof(FrameworkElement), 
                new PropertyMetadata(null, OnThemeStyleKeyChanged));

        private static void OnThemeStyleKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Re-evaluate ThemeStyle because it is
            // a factor of the ThemeStyleKey property
            ((FrameworkElement)d).UpdateThemeStyleProperty();
        }

        /// <summary>
        ///     This method causes the ThemeStyleProperty to be re-evaluated
        /// </summary>
        internal void UpdateThemeStyleProperty()
        {
            if (IsThemeStyleUpdateInProgress == false)
            {
                IsThemeStyleUpdateInProgress = true;
                try
                {
                    FrameworkElement.GetThemeStyle(this);
                }
                finally
                {
                    IsThemeStyleUpdateInProgress = false;
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("Cyclic reference found while evaluating the ThemeStyle property on element '{0}'.", this));
            }
        }

        internal static Style GetThemeStyle(FrameworkElement fe)
        {
            // If this is the first time that the ThemeStyleProperty
            // is being fetched then mark it such
            fe.HasThemeStyleEverBeenFetched = true;

            // Fetch the DefaultStyleKey and the self Style for
            // the given FrameworkElement
            object themeStyleKey = fe.DefaultStyleKey;
            //Style selfStyle = fe.Style;
            Style oldThemeStyle = fe.ThemeStyle;
            Style newThemeStyle = null;

            // Don't lookup properties from the themes if user has specified OverridesDefaultStyle
            // or DefaultStyleKey = null
            if (themeStyleKey != null)
            {
                // First look for an applicable style in system resources
                object styleLookup;
                // Regular lookup based on the DefaultStyleKey. Involves locking and Hashtable lookup

                Type typeKey = themeStyleKey as Type;
                if (typeKey != null)
                {
                    styleLookup = Application.Current?.XamlResourcesHandler?.TryFindResourceInGenericXaml(typeKey.Assembly, themeStyleKey);
                }
                else
                {
                    styleLookup = null;
                }

                if (styleLookup != null)
                {
                    if (styleLookup is Style)
                    {
                        // We have found an applicable Style in system resources
                        //  let's us use that as second stop to find property values.
                        newThemeStyle = (Style)styleLookup;
                    }
                    else
                    {
                        // We found something keyed to the ThemeStyleKey, but it's not
                        //  a style.  This is a problem, throw an exception here.
                        throw new InvalidOperationException(string.Format("System resource for type '{0}' is not a Style object.", themeStyleKey));
                    }
                }

                if (newThemeStyle == null)
                {
                    // No style in system resources, try to retrieve the default
                    // style for the target type.
                    Type themeStyleTypeKey = themeStyleKey as Type;
                    if (themeStyleTypeKey != null)
                    {
                        PropertyMetadata styleMetadata = FrameworkElement.StyleProperty.GetMetadata(themeStyleTypeKey);

                        if (styleMetadata != null)
                        {
                            // Have a metadata object, get the default style (if any)
                            newThemeStyle = styleMetadata.DefaultValue as Style;
                        }
                    }
                }
            }

            // Propagate change notification
            if (oldThemeStyle != newThemeStyle)
            {
                FrameworkElement.OnThemeStyleChanged(fe, oldThemeStyle, newThemeStyle);
            }

            return newThemeStyle;
        }

        // Invoked when the ThemeStyle property is changed
        internal static void OnThemeStyleChanged(DependencyObject d, object oldValue, object newValue)
        {
            FrameworkElement fe = (FrameworkElement)d;
            UpdateThemeStyleCache(fe, (Style)oldValue, (Style)newValue, ref fe._themeStyleCache);
        }

        internal static void UpdateThemeStyleCache(
             FrameworkElement fe,
             Style oldThemeStyle,
             Style newThemeStyle,
             ref Style themeStyleCache)
        {
            Dictionary<DependencyProperty, object> newStylePropertyValues = null;
            if (newThemeStyle != null)
            {
                // We have a new style.  Make sure it's targeting the right
                // type, and then seal it.

                newThemeStyle.CheckTargetType(fe);
                newThemeStyle.Seal();
                newStylePropertyValues = newThemeStyle.EffectiveValues;
            }

            themeStyleCache = newThemeStyle;

            if (oldThemeStyle != null)
            {
                // Clear old theme style values
                // if a property is about to be set again in the new theme style
                // we don't unset the value directly to prevent from potientially
                // firing the DependencyPropertyChanged callback twice.
                Dictionary<DependencyProperty, object> oldStylePropertyValues = oldThemeStyle.EffectiveValues;
                IEnumerable<DependencyProperty> removedProperties = 
                    oldStylePropertyValues
                        .Where(kp => newStylePropertyValues == null || !newStylePropertyValues.ContainsKey(kp.Key))
                        .Select(kp => kp.Key);
                foreach (var property in removedProperties)
                {
                    fe.SetThemeStyleValue(property, DependencyProperty.UnsetValue);
                }
            }

            if (newThemeStyle != null)
            {
                foreach (var propertyValue in newStylePropertyValues)
                {
                    object value;
                    if (propertyValue.Value is Binding binding)
                    {
                        value = new BindingExpression(binding.Clone(), propertyValue.Key);
                    }
                    else
                    {
                        value = propertyValue.Value;
                    }
                    fe.SetThemeStyleValue(propertyValue.Key, value);
                }
            }
        }

        // Cache the ThemeStyle for the current instance if there is a DefaultStyleKey specified for it
        internal Style ThemeStyle
        {
            get { return _themeStyleCache; }
        }

        #endregion DefaultStyleKey

        #endregion Handling Styles

        #region Loaded/Unloaded events

        public static readonly RoutedEvent LoadedEvent = 
            new RoutedEvent(
                nameof(Loaded),
                RoutingStrategy.Direct,
                typeof(RoutedEventHandler), 
                typeof(FrameworkElement));

        /// <summary>
        /// Occurs when a FrameworkElement has been constructed and added to the object tree.
        /// </summary>
        public event RoutedEventHandler Loaded;

        internal void INTERNAL_RaiseLoadedEvent()
        {
            if (Loaded != null)
                Loaded(this, new RoutedEventArgs());

            InvalidateMeasure();
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

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();
            if (HasImplicitStyleFromResources && 
                (!HasResources || !Resources.Contains(GetType())))
            {
                HasStyleInvalidated = false;
                UpdateStyleProperty();
            }
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            // We check if the parent has implicit styles in its ancestors and inherit it if it is the case
            FrameworkElement parent = VisualTreeHelper.GetParent(this) as FrameworkElement;
            if (parent != null && parent.ShouldLookupImplicitStyles)
            {
                ShouldLookupImplicitStyles = true;
            }

            // Fetch the implicit style
            // If this element's ResourceDictionary contains the
            // implicit style, it has already been retrieved.
            if (!HasImplicitStyleFromResources || 
                (!HasResources || !Resources.Contains(GetType())))
            {
                HasStyleInvalidated = false;
                UpdateStyleProperty();
            }

            if (!HasThemeStyleEverBeenFetched)
            {
                UpdateThemeStyleProperty();
            }

            InvalidateMeasureInternal();
        }

        // Extracts the required flag and returns
        // bool to indicate if it is set or unset
        internal bool ReadInternalFlag(InternalFlags reqFlag)
        {
            return (_flags & reqFlag) != 0;
        }

        // Sets or Unsets the required flag based on
        // the bool argument
        internal void WriteInternalFlag(InternalFlags reqFlag, bool set)
        {
            if (set)
            {
                _flags |= reqFlag;
            }
            else
            {
                _flags &= (~reqFlag);
            }
        }

        private InternalFlags _flags = 0; // Stores Flags (see Flags enum)
    }

    internal enum InternalFlags : uint
    {
        //// Does the instance have ResourceReference properties
        //HasResourceReferences = 0x00000001,

        //HasNumberSubstitutionChanged = 0x00000002,

        // Is the style for this instance obtained from a
        // typed-style declared in the Resources
        HasImplicitStyleFromResources = 0x00000004,
        //InheritanceBehavior0 = 0x00000008,
        //InheritanceBehavior1 = 0x00000010,
        //InheritanceBehavior2 = 0x00000020,

        IsStyleUpdateInProgress = 0x00000040,
        IsThemeStyleUpdateInProgress = 0x00000080,
        //StoresParentTemplateValues = 0x00000100,

        // free bit = 0x00000200,
        //NeedsClipBounds = 0x00000400,

        //HasWidthEverChanged = 0x00000800,
        //HasHeightEverChanged = 0x00001000,
        // free bit = 0x00002000,
        // free bit = 0x00004000,

        // Has this instance been initialized
        //IsInitialized = 0x00008000,

        // Set on BeginInit and reset on EndInit
        //InitPending = 0x00010000,

        //IsResourceParentValid = 0x00020000,
        // free bit                     0x00040000,

        // This flag is set to true when this FrameworkElement is in the middle
        //  of an invalidation storm caused by InvalidateTree for ancestor change,
        //  so we know not to trigger another one.
        //AncestorChangeInProgress = 0x00080000,

        // This is used when we know that we're in a subtree whose visibility
        //  is collapsed.  A false here does not indicate otherwise.  A false
        //  merely indicates "we don't know".
        //InVisibilityCollapsedTree = 0x00100000,

        //HasStyleEverBeenFetched = 0x00200000,
        HasThemeStyleEverBeenFetched = 0x00400000,

        HasLocalStyle = 0x00800000,

        // This instance's Visual or logical Tree was generated by a Template
        //HasTemplateGeneratedSubTree = 0x01000000,

        // free bit   = 0x02000000,
        //Remove this entry later since it is supposed to be in InternalFlags2
        HasStyleInvalidated = 0x02000000,

        HasLogicalChildren = 0x04000000,

        // Are we in the process of iterating the logical children.
        // This flag is set during a descendents walk, for property invalidation.
        IsLogicalChildrenIterationInProgress = 0x08000000,

        //Are we creating a new root after system metrics have changed?
        //CreatingRoot = 0x10000000,

        // FlowDirection is set to RightToLeft (0 == LeftToRight, 1 == RightToLeft)
        // This is an optimization to speed reading the FlowDirection property
        //IsRightToLeft = 0x20000000,

        ShouldLookupImplicitStyles = 0x40000000,

        // This flag is set to true there are mentees listening to either the
        // InheritedPropertyChanged event or the ResourcesChanged event. Once
        // this flag is set to true it does not get reset after that.

        //PotentiallyHasMentees = 0x80000000,
    }
}
