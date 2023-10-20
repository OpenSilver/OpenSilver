//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls.Common;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Automation.Peers;
using InternalVisualStates = System.Windows.Controls.Internal.VisualStates;
using resources = OpenSilver.Internal.Controls.Data.Input.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// Displays a description and tracks error state for a control.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = InternalVisualStates.StateNormal, GroupName = InternalVisualStates.GroupCommon)]
    [TemplateVisualState(Name = InternalVisualStates.StateDisabled, GroupName = InternalVisualStates.GroupCommon)]
    [TemplateVisualState(Name = InternalVisualStates.StateNoDescription, GroupName = InternalVisualStates.GroupDescription)]
    [TemplateVisualState(Name = InternalVisualStates.StateHasDescription, GroupName = InternalVisualStates.GroupDescription)]
    [TemplateVisualState(Name = InternalVisualStates.StateValidFocused, GroupName = InternalVisualStates.GroupValidation)]
    [TemplateVisualState(Name = InternalVisualStates.StateValidUnfocused, GroupName = InternalVisualStates.GroupValidation)]
    [TemplateVisualState(Name = InternalVisualStates.StateInvalidFocused, GroupName = InternalVisualStates.GroupValidation)]
    [TemplateVisualState(Name = InternalVisualStates.StateInvalidUnfocused, GroupName = InternalVisualStates.GroupValidation)]
    [StyleTypedProperty(Property = "ToolTipStyle", StyleTargetType = typeof(ToolTip))]
    public class DescriptionViewer : Control
    {
#region Member fields

        private bool _descriptionOverridden;
        private bool _initialized;

#endregion Member fields

#region Constructors

        /// <summary>
        /// Initializes a new instance of the DescriptionViewer class.
        /// </summary>
        public DescriptionViewer()
        {
            this.DefaultStyleKey = typeof(DescriptionViewer);
            // Set binding to self for DataContext change notifications
            this.SetBinding(DescriptionViewer.DataContextProperty, new Binding());
            this.Loaded += new RoutedEventHandler(this.DescriptionViewer_Loaded);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.DescriptionViewer_IsEnabledChanged);
            if (DesignerProperties.IsInDesignTool)
            {
                this.Description = typeof(DescriptionViewer).Name;
            }
        }

#endregion Constructors

#region Dependency Properties

#region DataContext

        /// <summary>
        /// Identifies the DataContext dependency property.
        /// </summary>
        private static new readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register(
            "DataContext",
            typeof(object),
            typeof(DescriptionViewer),
            new PropertyMetadata(OnDataContextPropertyChanged));

        private static void OnDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DescriptionViewer dv = d as DescriptionViewer;
            if (dv != null)
            {
                if (e.OldValue == null || e.NewValue == null || e.OldValue.GetType() != e.NewValue.GetType())
                {
                    // Refresh the metadata, but only if the DataContext's type has changed (or if either is null)
                    dv.LoadMetadata(false);
                }
            }
        }

#endregion DataContext

#region Description

        /// <summary>
        /// Identifies the Description dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description",
            typeof(string),
            typeof(DescriptionViewer),
            new PropertyMetadata(OnDescriptionPropertyChanged));

        /// <summary>
        /// Gets or sets the description text displayed by the viewer.
        /// </summary>
        public string Description
        {
            get
            {
                return GetValue(DescriptionProperty) as string;
            }

            set
            {
                this._descriptionOverridden = true;
                SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Handle the Description field property change event.  This will update the the VSM state.
        /// </summary>
        /// <param name="depObj">The DescriptionViewer that changed its Description value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDescriptionPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            // Dependency property changed
            DescriptionViewer dv = depObj as DescriptionViewer;
            if (dv != null)
            {
                dv.UpdateDescriptionState();
            }
        }

#endregion Description

#region GlyphTemplate

        /// <summary>
        /// Identifies the GlyphTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty GlyphTemplateProperty =
            DependencyProperty.Register(
            "GlyphTemplate",
            typeof(ControlTemplate),
            typeof(DescriptionViewer),
            null);

        /// <summary>
        /// Gets or sets the template that is used to display the description viewer glyph.
        /// </summary>
        public ControlTemplate GlyphTemplate
        {
            get { return GetValue(GlyphTemplateProperty) as ControlTemplate; }
            set { SetValue(GlyphTemplateProperty, value); }
        }

#endregion GlyphTemplate

#region ToolTipStyle

        /// <summary>
        /// Identifies the ToolTipStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolTipStyleProperty =
            DependencyProperty.Register(
            "ToolTipStyle",
            typeof(Style),
            typeof(DescriptionViewer),
            null);

        /// <summary>
        /// Gets or sets the style used to display tooltips.
        /// </summary>
        public Style ToolTipStyle
        {
            get { return GetValue(ToolTipStyleProperty) as Style; }
            set { SetValue(ToolTipStyleProperty, value); }
        }

#endregion ToolTipStyle

#region IsFocused

        /// <summary>
        /// Identifies the IsFocused dependency property
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
            "IsFocused",
            typeof(bool),
            typeof(DescriptionViewer),
            new PropertyMetadata(false, OnIsFocusedPropertyChanged));

        /// <summary>
        ///   Gets a value that indicates whether the <see cref="DescriptionViewer.Target" /> 
        ///   of the <see cref="DescriptionViewer" /> has focus. 
        /// </summary>
        protected bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            private set { this.SetValueNoCallback(IsFocusedProperty, value); }
        }

        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Label label = d as Label;
            if (label != null && !label.AreHandlersSuspended())
            {
                label.SetValueNoCallback(DescriptionViewer.IsFocusedProperty, e.OldValue);
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, resources.UnderlyingPropertyIsReadOnly, "IsFocused"));
            }
        }

#endregion IsFocused

#region IsValid

        /// <summary>
        /// Identifies the IsValid dependency property
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
            "IsValid",
            typeof(bool),
            typeof(DescriptionViewer),
            new PropertyMetadata(true, OnIsValidPropertyChanged));

        /// <summary>
        ///   Gets a value that indicates whether the <see cref="DescriptionViewer.Target" /> field data is valid. 
        /// </summary>
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            private set { this.SetValueNoCallback(IsValidProperty, value); }
        }

        private static void OnIsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DescriptionViewer dv = d as DescriptionViewer;
            if (dv != null && !dv.AreHandlersSuspended())
            {
                dv.SetValueNoCallback(DescriptionViewer.IsValidProperty, e.OldValue);
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, resources.UnderlyingPropertyIsReadOnly, "IsValid"));
            }
        }

#endregion IsValid

#region PropertyPath

        /// <summary>
        /// Identifies the PropertyPath dependency property
        /// </summary>
        public static readonly DependencyProperty PropertyPathProperty = DependencyProperty.Register(
            "PropertyPath",
            typeof(string),
            typeof(DescriptionViewer),
            new PropertyMetadata(OnPropertyPathPropertyChanged));

        /// <summary>
        ///   Gets or sets the path to the dependency property on the <see cref="FrameworkElement.DataContext" /> 
        ///   of the <see cref="Label.Target" /> control that 
        ///   this <see cref="DescriptionViewer" /> is associated with. 
        /// </summary>
        public string PropertyPath
        {
            get { return GetValue(DescriptionViewer.PropertyPathProperty) as string; }
            set { SetValue(DescriptionViewer.PropertyPathProperty, value); }
        }

        private static void OnPropertyPathPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DescriptionViewer dv = depObj as DescriptionViewer;
            if (dv != null && dv.Initialized)
            {
                dv.LoadMetadata(false);
                // Changing the PropertyPath sometimes requires an update for the validation state, since it might be stale.
                dv.ParseTargetValidState();
            }
        }

#endregion PropertyPath

#region Target

        /// <summary>
        /// Identifies the Target dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(
            "Target",
            typeof(FrameworkElement),
            typeof(DescriptionViewer),
            new PropertyMetadata(OnTargetPropertyChanged));

        /// <summary>
        ///   Gets or sets the control that this <see cref="DescriptionViewer" /> is associated with. 
        /// </summary>
        public FrameworkElement Target
        {
            get { return GetValue(TargetProperty) as FrameworkElement; }
            set { SetValue(TargetProperty, value); }
        }

        private static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DescriptionViewer dv = d as DescriptionViewer;
            if (dv != null)
            {
                bool targetFocused = e.NewValue == FocusManager.GetFocusedElement();
                if (dv.IsFocused != targetFocused)
                {
                    dv.IsFocused = targetFocused;
                }
                dv.LoadMetadata(false);

                FrameworkElement oldElement = e.OldValue as FrameworkElement;
                FrameworkElement newElement = e.NewValue as FrameworkElement;
                EventHandler<ValidationErrorEventArgs> bindingHandler = new EventHandler<ValidationErrorEventArgs>(dv.Target_BindingValidationError);
                RoutedEventHandler gotFocusHandler = new RoutedEventHandler(dv.Target_GotFocus);
                RoutedEventHandler lostFocusHandler = new RoutedEventHandler(dv.Target_LostFocus);
                if (oldElement != null)
                {
                    oldElement.BindingValidationError -= bindingHandler;
                    oldElement.GotFocus -= gotFocusHandler;
                    oldElement.LostFocus -= lostFocusHandler;
                }
                if (newElement != null)
                {
                    newElement.BindingValidationError += bindingHandler;
                    newElement.GotFocus += gotFocusHandler;
                    newElement.LostFocus += lostFocusHandler;
                }
                dv.ParseTargetValidState();
            }
        }

#endregion Target

#endregion Dependency Properties

#region Properties

        /// <summary>
        /// Internally get or set the ValidationMetadata.  
        /// </summary>
        internal ValidationMetadata ValidationMetadata
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the control has been initialized.
        /// </summary>
        internal bool Initialized
        {
            get { return this._initialized; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// When the template is applied, this loads all the template parts
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Set default states
            this.UpdateValidationState();
            this.UpdateDescriptionState();
        }

        /// <summary>
        /// Reload the metadata from the source target or DomainContext
        /// </summary>
        public virtual void Refresh()
        {
            this._descriptionOverridden = false;
            this.LoadMetadata(true);
            this.ParseTargetValidState();
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>The AutomationPeer associated with this DescriptionViewer.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DescriptionViewerAutomationPeer(this);
        }

        /// <summary>
        /// Perform initialization code
        /// </summary>
        /// <param name="sender">The DescriptionViewer that has loaded.</param>
        /// <param name="e">The RoutedEventArgs for this event.</param>
        private void DescriptionViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this._initialized)
            {
                // Loading Metadata onload because the dependency property could have changed before load and before the target was initialized
                this.LoadMetadata(false);
                this._initialized = true;
                this.Loaded -= new RoutedEventHandler(this.DescriptionViewer_Loaded);
            }
        }

        /// <summary>
        /// IsEnabled property change handler
        /// </summary>
        /// <param name="sender">The DescriptionViewer that had its IsEnabled value changed.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private void DescriptionViewer_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateCommonState();
        }

        /// <summary>
        /// Load meta data and update the UI. 
        /// </summary>
        /// <param name="forceUpdate">If true, metadata will not be loaded from cache.</param>
        private void LoadMetadata(bool forceUpdate)
        {
            ValidationMetadata vmd = null;
            object entity = null;
            BindingExpression bindingExpression = null;
            if (!String.IsNullOrEmpty(this.PropertyPath))
            {
                entity = this.DataContext;
                // Pull metadata directly from the DataContext.  This isn't cached so it will be pulled every time.
                vmd = ValidationHelper.ParseMetadata(this.PropertyPath, entity);
            }
            else if (this.Target != null)
            {
                // Pull the metadata from the target FrameworkElement.  
                vmd = ValidationHelper.ParseMetadata(this.Target, forceUpdate, out entity, out bindingExpression);
            }
            if (this.ValidationMetadata != vmd)
            {
                this.ValidationMetadata = vmd;
                // Update to the new VMD
                if (!this._descriptionOverridden)
                {
                    string description = null;
                    if (this.ValidationMetadata != null)
                    {
                        description = this.ValidationMetadata.Description;
                    }
                    SetValue(DescriptionProperty, description);
                }
            }
        }

        /// <summary>
        /// Parse the target error state and update the IsValid property
        /// </summary>
        private void ParseTargetValidState()
        {
            if (!String.IsNullOrEmpty(this.PropertyPath))
            {
                // If PropertyPath is set, the IsValid state is not used and defaults to true, even if the PropertyPath is itself invalid.
                this.IsValid = true;
            }
            else if (this.Target != null)
            {
                this.IsValid = !Validation.GetHasError(this.Target);
            }
            else
            {
                // If no target is specified, IsValid state defaults back to true.
                this.IsValid = true;
            }
            this.UpdateValidationState();
        }

        /// <summary>
        /// Event handler for target control's BindingValidationError event.
        /// </summary>
        /// <param name="sender">The sender of the BindingValidationError event.</param>
        /// <param name="e">The ValidationErrorEventArgs for this event.</param>
        private void Target_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            this.ParseTargetValidState();
        }

        /// <summary>
        /// Event handler for the target control's GotFocus event.
        /// </summary>
        /// <param name="sender">The sender of the GotFocus event.</param>
        /// <param name="e">The RoutedEventArgs for this event.</param>
        private void Target_GotFocus(object sender, RoutedEventArgs e)
        {
            this.IsFocused = true;
            this.UpdateValidationState();
        }

        /// <summary>
        /// Event handler for the target control's LostFocus event.
        /// </summary>
        /// <param name="sender">The sender of the LostFocus event.</param>
        /// <param name="e">The RoutedEventArgs for this event.</param>
        private void Target_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsFocused = false;
            this.UpdateValidationState();
        }

#region UpdateState

        /// <summary>
        /// Update the Common VSM state
        /// </summary>
        private void UpdateCommonState()
        {
            VisualStateManager.GoToState(this, this.IsEnabled ? InternalVisualStates.StateNormal : InternalVisualStates.StateDisabled, true);
        }

        /// <summary>
        /// Update the required field VSM state based on the description property.  
        /// </summary>
        private void UpdateDescriptionState()
        {
            VisualStateManager.GoToState(this, String.IsNullOrEmpty(this.Description) ? InternalVisualStates.StateNoDescription : InternalVisualStates.StateHasDescription, true);
        }

        /// <summary>
        /// When updating the validation state, check the focus state and update the VSM accordingly
        /// </summary>
        private void UpdateValidationState()
        {
            if (this.IsValid)
            {
                if (this.IsFocused && !String.IsNullOrEmpty(this.Description))
                {
                    VisualStateManager.GoToState(this, InternalVisualStates.StateValidFocused, true);
                }
                else
                {
                    VisualStateManager.GoToState(this, InternalVisualStates.StateValidUnfocused, true);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, this.IsFocused ? InternalVisualStates.StateInvalidFocused : InternalVisualStates.StateInvalidUnfocused, true);
            }
        }

#endregion UpdateState

#endregion Methods
    }
}