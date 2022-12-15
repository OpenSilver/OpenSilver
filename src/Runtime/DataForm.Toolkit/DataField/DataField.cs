//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Automation;
using System.Windows.Controls.Common;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls.Common;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Contains a control and generates labels and descriptions for it.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [ContentProperty("Content")]
    [TemplatePart(Name = DATAFIELD_elementContentControl, Type = typeof(ContentControl))]
    [TemplateVisualState(Name = DATAFIELD_stateReadOnly, GroupName = DATAFIELD_groupMode)]
    [TemplateVisualState(Name = DATAFIELD_stateEdit, GroupName = DATAFIELD_groupMode)]
    [TemplateVisualState(Name = DATAFIELD_stateAddNew, GroupName = DATAFIELD_groupMode)]
    [TemplateVisualState(Name = DATAFIELD_stateValid, GroupName = DATAFIELD_groupValidation)]
    [TemplateVisualState(Name = DATAFIELD_stateInvalid, GroupName = DATAFIELD_groupValidation)]
    [StyleTypedProperty(Property = "DescriptionViewerStyle", StyleTargetType = typeof(DescriptionViewer))]
    [StyleTypedProperty(Property = "LabelStyle", StyleTargetType = typeof(Label))]
    public class DataField : Control
    {
#region Constants

        private const int BottomYPosition = 3;
        private const int ColumnSpanThreeItems = 5;
        private const int ColumnSpanTwoItems = 3;
        private const int ColumnsPerField = 6;
        private const int FieldElementSpacing = 6; // px
        private const int LeftXPosition = 1;
        private const int MiddleXPosition = 3;
        private const int MiddleYPosition = 2;
        private const int RightXPosition = 5;
        private const int RowsPerField = 4;
        private const int TopYPosition = 1;
        private const int XEdgeSpacing = 5; // px
        private const int YEdgeSpacing = 3; // px

        private const string DATAFIELD_elementContentControl = "ContentControl";

        private const string DATAFIELD_groupMode = "ModeStates";
        private const string DATAFIELD_stateReadOnly = "ReadOnly";
        private const string DATAFIELD_stateEdit = "Edit";
        private const string DATAFIELD_stateAddNew = "AddNew";

        private const string DATAFIELD_groupValidation = "ValidationStates";
        private const string DATAFIELD_stateValid = "Valid";
        private const string DATAFIELD_stateInvalid = "Invalid";

#endregion Constants

#region Dependency Properties

        /// <summary>
        /// Identifies the Content property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(FrameworkElement),
                typeof(DataField),
                new PropertyMetadata(OnContentPropertyChanged));

        /// <summary>
        /// Content property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its Content value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            field.GenerateUI();
        }

        /// <summary>
        /// Identifies the DescriptionViewerPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(
                "Description",
                typeof(string),
                typeof(DataField),
                new PropertyMetadata(OnDescriptionPropertyChanged));

        /// <summary>
        /// Description property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its Description value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDescriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            if (field.DescriptionViewer != null)
            {
                field.SetDescriptionContent();
            }
        }

        /// <summary>
        /// Identifies the DescriptionViewerPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionViewerPositionProperty =
            DependencyProperty.Register(
                "DescriptionViewerPosition",
                typeof(DataFieldDescriptionViewerPosition),
                typeof(DataField),
                new PropertyMetadata(DataFieldDescriptionViewerPosition.Auto, OnDescriptionViewerPositionPropertyChanged));

        /// <summary>
        /// DescriptionViewerPosition property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its DescriptionViewerPosition value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDescriptionViewerPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            field.GenerateUI();
        }

        /// <summary>
        /// Identifies the DescriptionViewerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionViewerStyleProperty =
            DependencyProperty.Register(
                "DescriptionViewerStyle",
                typeof(Style),
                typeof(DataField),
                new PropertyMetadata(OnDescriptionViewerStylePropertyChanged));

        /// <summary>
        /// DescriptionViewerStyle property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its DescriptionViewerStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDescriptionViewerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            if (field.DescriptionViewer != null)
            {
                field.DescriptionViewer.SetStyleWithType(field.DescriptionViewerStyle);
                field.GenerateUI();
            }
        }

        /// <summary>
        /// Identifies the DescriptionViewerVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionViewerVisibilityProperty =
            DependencyProperty.Register(
                "DescriptionViewerVisibility",
                typeof(Visibility),
                typeof(DataField),
                null);

        /// <summary>
        /// Identifies the Label dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                "Label",
                typeof(object),
                typeof(DataField),
                new PropertyMetadata(OnLabelPropertyChanged));

        /// <summary>
        /// Label property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its Label value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnLabelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            if (field.InternalLabel != null)
            {
                field.SetLabelContent();
            }
        }

        /// <summary>
        /// Identifies the LabelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register(
                "LabelPosition",
                typeof(DataFieldLabelPosition),
                typeof(DataField),
                new PropertyMetadata(DataFieldLabelPosition.Auto, OnLabelPositionPropertyChanged));

        /// <summary>
        /// LabelPosition property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its LabelPosition value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnLabelPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            field.GenerateUI();
        }

        /// <summary>
        /// Identifies the LabelStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.Register(
                "LabelStyle",
                typeof(Style),
                typeof(DataField),
                new PropertyMetadata(OnLabelStylePropertyChanged));

        /// <summary>
        /// LabelStyle property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its LabelStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            if (field.InternalLabel != null)
            {
                field.InternalLabel.SetStyleWithType(field.LabelStyle);
                field.GenerateUI();
            }
        }

        /// <summary>
        /// Identifies the LabelVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelVisibilityProperty =
            DependencyProperty.Register(
                "LabelVisibility",
                typeof(Visibility),
                typeof(DataField),
                null);

        /// <summary>
        /// Identifies the IsFieldGroup attached property.
        /// </summary>
        public static readonly DependencyProperty IsFieldGroupProperty =
            DependencyProperty.RegisterAttached(
            "IsFieldGroup",
            typeof(bool),
            typeof(DataField),
            null);

        /// <summary>
        /// Gets the IsFieldGroup attached property.
        /// </summary>
        /// <param name="target">The target panel.</param>
        /// <returns>Whether the panel is a field group.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Panel is the only element type where it makes sense to declare as the field layout scope.")]
        public static bool GetIsFieldGroup(Panel target)
        {
            return (bool)target.GetValue(IsFieldGroupProperty);
        }

        /// <summary>
        /// Sets the IsFieldGroup attached property.
        /// </summary>
        /// <param name="target">The target panel.</param>
        /// <param name="isFieldGroup">Whether or not this panel should be a field group.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Panel is the only element type where it makes sense to declare as the field layout scope.")]
        public static void SetIsFieldGroup(Panel target, bool isFieldGroup)
        {
            target.SetValue(IsFieldGroupProperty, isFieldGroup);
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(DataField),
                new PropertyMetadata(OnIsReadOnlyPropertyChanged));

        /// <summary>
        /// IsReadOnly property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its IsReadOnly value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            if (!field.AreHandlersSuspended())
            {
                field._isReadOnlyOverridden = true;
                field.GenerateUI();
            }
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register(
                "IsRequired",
                typeof(bool),
                typeof(DataField),
                new PropertyMetadata(OnIsRequiredPropertyChanged));

        /// <summary>
        /// IsRequired property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its IsRequired value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsRequiredPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            if (!field.AreHandlersSuspended())
            {
                field._isRequiredOverridden = true;

                if (field.InternalLabel != null)
                {
                    field.InternalLabel.IsRequired = field.IsRequired;
                }
            }
        }

        /// <summary>
        /// Identifies the Mode dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                "Mode",
                typeof(DataFieldMode),
                typeof(DataField),
                new PropertyMetadata(DataFieldMode.Auto, OnModePropertyChanged));

        /// <summary>
        /// IsReadOnly property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its IsReadOnly value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            field.GenerateUI();
            field.UpdateStates();
        }

        /// <summary>
        /// Identifies the PropertyPath dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyPathProperty =
            DependencyProperty.Register(
                "PropertyPath",
                typeof(string),
                typeof(DataField),
                new PropertyMetadata(OnPropertyPathPropertyChanged));

        /// <summary>
        /// PropertyPath property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its PropertyPath value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnPropertyPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            field._boundProperty = null;
        }

        /// <summary>
        /// Identifies the FieldList attached property.
        /// </summary>
        internal static readonly DependencyProperty GroupedFieldListProperty =
            DependencyProperty.RegisterAttached(
            "GroupedFieldList",
            typeof(IList<DataField>),
            typeof(DataField),
            null);

        /// <summary>
        /// Identifies the DataFormDescriptionViewerPosition dependency property.
        /// </summary>
        private static readonly DependencyProperty DataFormDescriptionViewerPositionProperty =
            DependencyProperty.Register(
                "DataFormDescriptionViewerPosition",
                typeof(DataFieldDescriptionViewerPosition),
                typeof(DataField),
                new PropertyMetadata(DataFieldDescriptionViewerPosition.Auto, OnDataFormDescriptionViewerPositionPropertyChanged));

        /// <summary>
        /// DataFormDescriptionViewerPosition property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its DataFormDescriptionViewerPosition value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDataFormDescriptionViewerPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            if (field.DescriptionViewerPosition == DataFieldDescriptionViewerPosition.Auto)
            {
                field.GenerateUI();
            }
        }

        /// <summary>
        /// Identifies the DataFormLabelPosition dependency property.
        /// </summary>
        private static readonly DependencyProperty DataFormLabelPositionProperty =
            DependencyProperty.Register(
                "DataFormLabelPosition",
                typeof(DataFieldLabelPosition),
                typeof(DataField),
                new PropertyMetadata(DataFieldLabelPosition.Auto, OnDataFormLabelPositionPropertyChanged));

        /// <summary>
        /// DataFormLabelPosition property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its DataFormLabelPosition value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDataFormLabelPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            if (field.LabelPosition == DataFieldLabelPosition.Auto)
            {
                field.GenerateUI();
            }
        }

        /// <summary>
        /// Identifies the DataFormMode dependency property.
        /// </summary>
        private static readonly DependencyProperty DataFormModeProperty =
            DependencyProperty.Register(
                "DataFormMode",
                typeof(DataFormMode),
                typeof(DataField),
                new PropertyMetadata(OnDataFormModePropertyChanged));

        /// <summary>
        /// DataFormLabelPosition property changed handler.
        /// </summary>
        /// <param name="d">Field that changed its DataFormLabelPosition value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDataFormModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataField field = d as DataField;
            if (field.Mode == DataFieldMode.Auto)
            {
                field.GenerateUI();
                field.UpdateStates();
            }
        }

#endregion Dependency Properties

#region Fields

        /// <summary>
        /// Holds the bound property on either the root control in the field or
        /// via the PropertyPath property.
        /// </summary>
        private PropertyInfo _boundProperty;

        /// <summary>
        /// Private accessor to the main content control.
        /// </summary>
        private ContentControl _contentControl;

        /// <summary>
        /// Holds a value indicating whether or not to ignore a call to ApplyTemplate.
        /// </summary>
        private bool _ignoreApplyTemplate;

        /// <summary>
        /// Holds a value indicating whether or not IsReadOnly has been set.
        /// </summary>
        private bool _isReadOnlyOverridden;

        /// <summary>
        /// Holds a value indicating whether or not IsRequired has been set.
        /// </summary>
        private bool _isRequiredOverridden;

        /// <summary>
        /// Holds the layout panel.
        /// </summary>
        private Panel _layoutPanel;

        /// <summary>
        /// Holds the parent DataForm that was found.
        /// </summary>
        private DataForm _parentDataForm;

        /// <summary>
        /// Holds a value indicating whether or not the template has been applied.
        /// </summary>
        private bool _templateApplied;

        /// <summary>
        /// Holds whether or not lost focus has been fired since
        /// TextBox validation on text changed began.
        /// </summary>
        private IDictionary<TextBox, bool> _lostFocusFired;

#endregion Fields

#region Constructors

        /// <summary>
        /// Constructs a new instance of Field.
        /// </summary>
        public DataField()
        {
            this.DefaultStyleKey = typeof(DataField);
            this._lostFocusFired = new Dictionary<TextBox, bool>();
            this.Loaded += new RoutedEventHandler(this.OnDataFieldLoaded);
        }

#endregion Constructors

#region Properties

#region Public Properties

        /// <summary>
        /// Gets or sets the content of the field.
        /// </summary> 
        public FrameworkElement Content
        {
            get
            {
                return this.GetValue(ContentProperty) as FrameworkElement;
            }

            set
            {
                this.SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text displayed in the description viewer.
        /// </summary>
        public string Description
        {
            get
            {
                return this.GetValue(DescriptionProperty) as string;
            }

            set
            {
                this.SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the position of descriptions in relation to the field.
        /// </summary>
        public DataFieldDescriptionViewerPosition DescriptionViewerPosition
        {
            get
            {
                return (DataFieldDescriptionViewerPosition)this.GetValue(DescriptionViewerPositionProperty);
            }

            set
            {
                this.SetValue(DescriptionViewerPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the developer-specified style for descriptions.
        /// </summary>
        public Style DescriptionViewerStyle
        {
            get
            {
                return this.GetValue(DescriptionViewerStyleProperty) as Style;
            }

            set
            {
                this.SetValue(DescriptionViewerStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the description viewer is visible.
        /// </summary>
        public Visibility DescriptionViewerVisibility
        {
            get
            {
                return (Visibility)this.GetValue(DescriptionViewerVisibilityProperty);
            }

            set
            {
                this.SetValue(DescriptionViewerVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the label displayed for this control.
        /// </summary>
        public object Label
        {
            get
            {
                return this.GetValue(LabelProperty);
            }

            set
            {
                this.SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the position of the label in relation to the field.
        /// </summary>
        public DataFieldLabelPosition LabelPosition
        {
            get
            {
                return (DataFieldLabelPosition)this.GetValue(LabelPositionProperty);
            }

            set
            {
                this.SetValue(LabelPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the developer-specified style for labels.
        /// </summary>
        public Style LabelStyle
        {
            get
            {
                return this.GetValue(LabelStyleProperty) as Style;
            }

            set
            {
                this.SetValue(LabelStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the label is visible.
        /// </summary>
        public Visibility LabelVisibility
        {
            get
            {
                return (Visibility)this.GetValue(LabelVisibilityProperty);
            }

            set
            {
                this.SetValue(LabelVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can edit the values in the control.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (bool)this.GetValue(IsReadOnlyProperty);
            }

            set
            {
                this.SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field is required.
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return (bool)this.GetValue(IsRequiredProperty);
            }

            set
            {
                this.SetValue(IsRequiredProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the input control contains valid data.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (this.Content != null)
                {
                    return !ValidationUtil.ElementHasErrors(this.Content);
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the control is in read only, edit, or add new mode.
        /// </summary>
        public DataFieldMode Mode
        {
            get
            {
                return (DataFieldMode)this.GetValue(ModeProperty);
            }

            set
            {
                this.SetValue(ModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the path to the property that the control is bound to.
        /// </summary>
        public string PropertyPath
        {
            get
            {
                return this.GetValue(PropertyPathProperty) as string;
            }

            set
            {
                this.SetValue(PropertyPathProperty, value);
            }
        }

#endregion Public Properties

#region Internal Properties

        /// <summary>
        /// Gets or sets the column definition for the description when
        /// DescriptionViewerPosition = BesideContent.
        /// </summary>
        internal ColumnDefinition DescriptionColumnBesideContent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the column definition for the description when
        /// DescriptionViewerPosition = BesideLabel.
        /// </summary>
        internal ColumnDefinition DescriptionColumnBesideLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the column definition for the separator column
        /// by the DescriptionViewer when DescriptionViewerPosition = BesideLabel.
        /// </summary>
        internal ColumnDefinition DescriptionColumnBesideLabelSeparator
        {
            get;
            set;
        }

        /// <summary>
        /// Description generated from GenerateDescription().
        /// </summary>
        internal DescriptionViewer DescriptionViewer
        {
            get;
            set;
        }

        /// <summary>
        /// Label generated from GenerateLabel().
        /// </summary>
        internal Label InternalLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the column definition for the label.
        /// </summary>
        internal ColumnDefinition LabelColumn
        {
            get;
            set;
        }

#endregion Internal Properties

#region Private Properties

        /// <summary>
        /// Gets the property info for the bound property.
        /// </summary>
        private PropertyInfo BoundProperty
        {
            get
            {
                if (this._boundProperty == null && this.DataContext != null)
                {
                    if (!string.IsNullOrEmpty(this.PropertyPath))
                    {
                        this._boundProperty = this.DataContext.GetType().GetProperty(this.PropertyPath);
                    }
                    else if (this.Content != null)
                    {
                        List<DataFormBindingInfo> bindingInfos = this.Content.GetDataFormBindingInfo(this.DataContext, false /* twoWayOnly */, false /* searchChildren */);

                        if (bindingInfos != null)
                        {
                            DataFormBindingInfo bindingInfo = bindingInfos.FirstOrDefault();

                            if (bindingInfo != null &&
                                bindingInfo.BindingExpression != null &&
                                bindingInfo.BindingExpression.ParentBinding != null &&
                                bindingInfo.BindingExpression.ParentBinding.Path != null)
                            {
                                object dataItem = bindingInfo.BindingExpression.DataItem ?? this.DataContext;
                                this._boundProperty = dataItem.GetType().GetProperty(bindingInfo.BindingExpression.ParentBinding.Path.Path);
                            }
                        }
                    }
                }

                return this._boundProperty;
            }
        }

        /// <summary>
        /// Gets or sets the desired position of the description viewer on the DataForm.
        /// </summary>
        private DataFieldDescriptionViewerPosition DataFormDescriptionViewerPosition
        {
            get
            {
                return (DataFieldDescriptionViewerPosition)this.GetValue(DataFormDescriptionViewerPositionProperty);
            }

            set
            {
                this.SetValue(DataFormDescriptionViewerPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the desired position of the label on the DataForm.
        /// </summary>
        private DataFieldLabelPosition DataFormLabelPosition
        {
            get
            {
                return (DataFieldLabelPosition)this.GetValue(DataFormLabelPositionProperty);
            }

            set
            {
                this.SetValue(DataFormLabelPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mode on the DataForm.
        /// </summary>
        private DataFormMode DataFormMode
        {
            get
            {
                return (DataFormMode)this.GetValue(DataFormModeProperty);
            }

            set
            {
                this.SetValue(DataFormModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the effective position of the description viewer.
        /// </summary>
        private DataFieldDescriptionViewerPosition EffectiveDescriptionViewerPosition
        {
            get
            {
                if (this.DescriptionViewerPosition == DataFieldDescriptionViewerPosition.Auto)
                {
                    return this.DataFormDescriptionViewerPosition;
                }

                return this.DescriptionViewerPosition;
            }
        }

        /// <summary>
        /// Gets or sets the effective position of the label.
        /// </summary>
        private DataFieldLabelPosition EffectiveLabelPosition
        {
            get
            {
                if (this.LabelPosition == DataFieldLabelPosition.Auto)
                {
                    return this.DataFormLabelPosition;
                }

                return this.LabelPosition;
            }
        }

        /// <summary>
        /// Gets or sets the effective mode of the DataField.
        /// </summary>
        private DataFieldMode EffectiveMode
        {
            get
            {
                if (this.Mode == DataFieldMode.Auto)
                {
                    if (this._parentDataForm != null)
                    {
                        if (this.DataFormMode == DataFormMode.ReadOnly)
                        {
                            return DataFieldMode.ReadOnly;
                        }
                        else if (this.DataFormMode == DataFormMode.Edit)
                        {
                            return DataFieldMode.Edit;
                        }
                        else
                        {
                            return DataFieldMode.AddNew;
                        }
                    }
                    else
                    {
                        return DataFieldMode.Edit;
                    }
                }

                return this.Mode;
            }
        }

#endregion Private Properties

#endregion Properties

#region Methods

#region Public Methods

        /// <summary>
        /// Applies the template for this field.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (this._ignoreApplyTemplate)
            {
                return;
            }

            DataForm parentDataForm = this.GetParentDataForm();

            if (parentDataForm != this._parentDataForm)
            {
                this.RemoveBindingsFromParentDataForm();
                this.SetBindingsFromParentDataForm(parentDataForm);

                // We need to reapply the template, since the template could have changed when the
                // style in the DataForm got picked up.
                try
                {
                    // This will call OnApplyTemplate again, so ignore it this time.
                    this._ignoreApplyTemplate = true;
                    this.ApplyTemplate();
                }
                finally
                {
                    this._ignoreApplyTemplate = false;
                }
            }

            Panel layoutPanel = this.GetLayoutPanel();

            if (layoutPanel != this._layoutPanel)
            {
                this.DetachFieldFromLayoutPanel();
                this._layoutPanel = layoutPanel;
                this.AttachFieldToLayoutPanel();
            }

            this._contentControl = this.GetTemplateChild("ContentControl") as ContentControl;
            this._templateApplied = true;
            this.GenerateUI();
        }

        /// <summary>
        /// Validates this field.
        /// </summary>
        /// <returns>Whether or not the field is valid.</returns>
        public bool Validate()
        {
            if (this.Content != null)
            {
                ValidationUtil.UpdateSourceOnElementBindings(this.Content);
            }

            return this.IsValid;
        }

#endregion Public Methods

#region Internal Methods

        /// <summary>
        /// Detaches the field from its parent layout panel.
        /// </summary>
        internal void DetachFieldFromLayoutPanel()
        {
            if (this._layoutPanel == null)
            {
                return;
            }

            IList<DataField> fieldList = this._layoutPanel.GetValue(DataField.GroupedFieldListProperty) as IList<DataField>;

            if (fieldList != null && fieldList.Contains(this))
            {
                fieldList.Remove(this);

                if (fieldList.Count == 0)
                {
                    this._layoutPanel.SetValue(DataField.GroupedFieldListProperty, null);
                }
            }
        }

        /// <summary>
        /// Removes all references of this DataField from its parent DataForm, if one exists.
        /// </summary>
        internal void RemoveBindingsFromParentDataForm()
        {
            if (this._parentDataForm != null)
            {
                // Clear the value of the properties to ensure the binding is broken.
                this.ClearValue(DataField.DataFormModeProperty);
                this.ClearValue(DataField.DataFormLabelPositionProperty);
                this.ClearValue(DataField.DataFormDescriptionViewerPositionProperty);

                // Only reset the style if it's getting it from the DataForm.
                if (this._parentDataForm.DataFieldStyle == this.Style)
                {
                    this.ClearValue(DataField.StyleProperty);
                }

                this._parentDataForm = null;
            }
        }

#endregion Internal Methods

#region Private Methods

        /// <summary>
        /// Aligns the grouped description columns.
        /// </summary>
        private void AlignDescriptionColumns()
        {
#if OPENSILVER
            if (!_parentDataForm.ForceAlignment)
                return;
#endif
            IList<DataField> fieldList = this.GetAlignmentGroup();

            if (fieldList == null)
            {
                return;
            }

            double maxWidthBesideContent = 0;
            double maxWidthBesideLabel = 0;

            foreach (DataField field in fieldList)
            {
                if (field.DescriptionColumnBesideContent.ActualWidth > maxWidthBesideContent && !double.IsInfinity(field.DescriptionColumnBesideContent.ActualWidth))
                {
                    maxWidthBesideContent = field.DescriptionColumnBesideContent.ActualWidth;
                }

                if (field.DescriptionColumnBesideLabel.ActualWidth > maxWidthBesideLabel && !double.IsInfinity(field.DescriptionColumnBesideLabel.ActualWidth))
                {
                    maxWidthBesideLabel = field.DescriptionColumnBesideLabel.ActualWidth;
                }
            }

            foreach (DataField field in fieldList)
            {
                if (maxWidthBesideContent > 0 && !double.IsInfinity(field.DescriptionColumnBesideContent.ActualWidth))
                {
                    field.DescriptionColumnBesideContent.MinWidth = maxWidthBesideContent;
                }

                if (maxWidthBesideLabel > 0 && !double.IsInfinity(field.DescriptionColumnBesideLabel.ActualWidth))
                {
                    field.DescriptionColumnBesideLabelSeparator.Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel);
                    field.DescriptionColumnBesideLabel.MinWidth = maxWidthBesideLabel;
                }
            }
        }

        /// <summary>
        /// Aligns the grouped label columns.
        /// </summary>
        private void AlignLabelColumns()
        {
#if OPENSILVER
            if (!_parentDataForm.ForceAlignment)
                return;
#endif
            IList<DataField> fieldList = this.GetAlignmentGroup();

            if (fieldList == null)
            {
                return;
            }

            double maxWidth = 0;

            foreach (DataField field in fieldList)
            {
                if (field.LabelColumn.ActualWidth > maxWidth && !double.IsInfinity(field.LabelColumn.ActualWidth))
                {
                    maxWidth = field.LabelColumn.ActualWidth;
                }
            }

            foreach (DataField field in fieldList)
            {
                if (maxWidth > 0 && !double.IsInfinity(field.LabelColumn.ActualWidth))
                {
                    field.LabelColumn.Width = new GridLength(maxWidth);
                }
            }
        }

        /// <summary>
        /// Attaches the field to its parent layout panel.
        /// </summary>
        private void AttachFieldToLayoutPanel()
        {
            if (this._layoutPanel == null)
            {
                return;
            }

            IList<DataField> fieldList = this._layoutPanel.GetValue(DataField.GroupedFieldListProperty) as IList<DataField>;

            if (fieldList == null)
            {
                fieldList = new List<DataField>();
                this._layoutPanel.SetValue(DataField.GroupedFieldListProperty, fieldList);
            }

            if (!fieldList.Contains(this))
            {
                fieldList.Add(this);
            }
        }

        /// <summary>
        /// Generates the UI for this field.
        /// </summary>
        private void GenerateUI()
        {
            if (this._contentControl == null)
            {
                return;
            }

            this._boundProperty = null;

            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            this.LabelColumn = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) };
            grid.ColumnDefinitions.Add(this.LabelColumn);
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
            // Content column:
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
            this.DescriptionColumnBesideContent = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) };
            grid.ColumnDefinitions.Add(this.DescriptionColumnBesideContent);

            Grid labelGrid = new Grid();
            this.InternalLabel = new Label();
            this.InternalLabel.SetBinding(Controls.Label.PropertyPathProperty, new Binding("PropertyPath") { Source = this });
            this.InternalLabel.SetBinding(Controls.Label.VisibilityProperty, new Binding("LabelVisibility") { Source = this });
            this.InternalLabel.SetBinding(Controls.Label.ForegroundProperty, new Binding("Foreground") { Source = this });
            this.SetLabelContent();

            this.SetIsReadOnlyIfNotOverridden();
            this.SetIsRequiredIfNotOverridden();

            labelGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            labelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            this.DescriptionColumnBesideLabelSeparator = new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Pixel) };
            labelGrid.ColumnDefinitions.Add(this.DescriptionColumnBesideLabelSeparator);
            this.DescriptionColumnBesideLabel = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) };
            labelGrid.ColumnDefinitions.Add(this.DescriptionColumnBesideLabel);

            if (this._isRequiredOverridden)
            {
                this.InternalLabel.IsRequired = this.IsRequired;
            }
            else if (this.EffectiveMode == DataFieldMode.ReadOnly)
            {
                this.InternalLabel.IsRequired = false;
            }

            Panel oldPanel = VisualTreeHelper.GetParent(this.InternalLabel) as Panel;

            if (oldPanel != null)
            {
                oldPanel.Children.Remove(this.InternalLabel);
            }

            labelGrid.Children.Add(this.InternalLabel);

            if (this.EffectiveLabelPosition == DataFieldLabelPosition.Top)
            {
                Grid.SetRow(labelGrid, 0);
                Grid.SetColumn(labelGrid, 2);
            }
            else
            {
                Grid.SetRow(labelGrid, 1);
                Grid.SetColumn(labelGrid, 0);
            }

            // Make the label grid have the same horizontal alignment as the label.
            labelGrid.SetBinding(FrameworkElement.HorizontalAlignmentProperty, new Binding("HorizontalAlignment") { Source = this.InternalLabel });
            grid.Children.Add(labelGrid);

#if MIGRATION
            this.InternalLabel.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnLabelMouseLeftButtonDown);
#else
            this.InternalLabel.PointerPressed += new PointerEventHandler(this.OnLabelMouseLeftButtonDown);
#endif
            this.InternalLabel.SizeChanged += new SizeChangedEventHandler(this.OnLabelSizeChanged);

            if (this.Content != null)
            {
                if (this.IsReadOnly)
                {
                    this.SetContentReadOnlyState(true /* isReadOnly */);
                }
                else
                {
                    this.SetContentReadOnlyState(false /* isReadOnly */);
                }

                this.Content.Loaded -= new RoutedEventHandler(this.OnContentLoaded);
                this.Content.Loaded += new RoutedEventHandler(this.OnContentLoaded);

                oldPanel = VisualTreeHelper.GetParent(this.Content) as Panel;

                if (oldPanel != null)
                {
                    oldPanel.Children.Remove(this.Content);
                }

                grid.Children.Add(this.Content);

                Grid.SetRow(this.Content, 1);
                Grid.SetColumn(this.Content, 2);
            }

            this.DescriptionViewer = new DescriptionViewer();
            this.DescriptionViewer.SetBinding(DescriptionViewer.PropertyPathProperty, new Binding("PropertyPath") { Source = this });
            this.DescriptionViewer.SetBinding(DescriptionViewer.VisibilityProperty, new Binding("DescriptionViewerVisibility") { Source = this });
            this.SetDescriptionContent();

            oldPanel = VisualTreeHelper.GetParent(this.DescriptionViewer) as Panel;

            if (oldPanel != null)
            {
                oldPanel.Children.Remove(this.DescriptionViewer);
            }

            if (this.EffectiveDescriptionViewerPosition == DataFieldDescriptionViewerPosition.BesideLabel)
            {
                this.DescriptionColumnBesideLabelSeparator.Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel);
                Grid.SetColumn(this.DescriptionViewer, 2);
                labelGrid.Children.Add(this.DescriptionViewer);
            }
            else
            {
                Grid.SetRow(this.DescriptionViewer, 1);
                Grid.SetColumn(this.DescriptionViewer, 4);
                grid.Children.Add(this.DescriptionViewer);
            }

            this.DescriptionViewer.SizeChanged += new SizeChangedEventHandler(this.OnDescriptionSizeChanged);

            // Don't display the description when in read-only mode.
            if (this.IsReadOnly)
            {
                this.DescriptionViewer.Opacity = 0;
                this.DescriptionViewer.IsHitTestVisible = false;
            }

            this.RemoveLabelColumnAlignment();
            this.RemoveDescriptionColumnAlignment();
            this._contentControl.Content = grid;
        }

        /// <summary>
        /// Retrieves the list of fields associated with this field's alignment group.
        /// </summary>
        /// <returns>The list of fields associated with this field's alignment group.</returns>
        private IList<DataField> GetAlignmentGroup()
        {
            if (this._layoutPanel != null)
            {
                return this._layoutPanel.GetValue(DataField.GroupedFieldListProperty) as IList<DataField>;
            }

            return null;
        }

        /// <summary>
        /// Gets the property info for the binding.
        /// </summary>
        /// <returns>The property info.</returns>
        private PropertyInfo GetPropertyInfo()
        {
            Debug.Assert(this.DataContext != null, "DataContext should never be null when GetPropertyInfo() is called.");

            PropertyInfo propertyInfo = null;

            if (!string.IsNullOrEmpty(this.PropertyPath))
            {
                propertyInfo = this.DataContext.GetType().GetPropertyInfo(this.PropertyPath);
            }

            if (propertyInfo == null && this.Content != null)
            {
                IList<DataFormBindingInfo> bindingInfos = this.Content.GetDataFormBindingInfo(this.DataContext, false /* twoWayOnly */, false /* searchChildren */);

                foreach (DataFormBindingInfo bindingInfo in bindingInfos)
                {
                    Binding binding = bindingInfo.BindingExpression.ParentBinding;

                    if (binding != null &&
                        binding.Path != null &&
                        !string.IsNullOrEmpty(binding.Path.Path))
                    {
                        propertyInfo = this.DataContext.GetType().GetPropertyInfo(binding.Path.Path) as PropertyInfo;

                        if (propertyInfo != null)
                        {
                            break;
                        }
                    }
                }
            }

            return propertyInfo;
        }

        /// <summary>
        /// Gets the panel to be used for layout.
        /// </summary>
        /// <returns>The panel to be used for layout.</returns>
        private Panel GetLayoutPanel()
        {
            DependencyObject curObject = VisualTreeHelper.GetParent(this);

            while (curObject != null)
            {
                Panel panel = curObject as Panel;

                if (panel != null)
                {
                    bool isLabelGroup = DataField.GetIsFieldGroup(panel);

                    if (isLabelGroup)
                    {
                        return panel;
                    }
                }

                curObject = VisualTreeHelper.GetParent(curObject);
            }

            return null;
        }

        /// <summary>
        /// Gets the parent DataForm for this DataField.
        /// </summary>
        /// <returns>The parent DataForm.</returns>
        private DataForm GetParentDataForm()
        {
            DataForm parentDataForm = null;
            DependencyObject curObject = VisualTreeHelper.GetParent(this);

            while (curObject != null && parentDataForm == null)
            {
                parentDataForm = curObject as DataForm;

                if (parentDataForm == null)
                {
                    curObject = VisualTreeHelper.GetParent(curObject);
                }
            }
            return parentDataForm;
        }

        /// <summary>
        /// Handles the case where an element has loaded.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnContentLoaded(object sender, RoutedEventArgs e)
        {
            if (this.InternalLabel != null)
            {
                this.InternalLabel.Target = this.Content;
            }

            if (this.DescriptionViewer != null)
            {
                this.DescriptionViewer.Target = this.Content;
            }

            AutomationProperties.SetLabeledBy(this.Content, this.InternalLabel);
            AutomationProperties.SetIsRequiredForForm(this.Content, this.InternalLabel.IsRequired);
            AutomationProperties.SetHelpText(this.Content, this.DescriptionViewer.Description);

            this._lostFocusFired.Clear();
            this.UpdateBindingsOnElement(this.Content);
        }

        /// <summary>
        /// Handles the case where the DataField has loaded.
        /// </summary>
        /// <param name="sender">The DataField.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFieldLoaded(object sender, RoutedEventArgs e)
        {
            // Don't do anything if the template hasn't been applied yet.
            if (!this._templateApplied)
            {
                return;
            }

            bool shouldGenerateUI = false;
            DataForm parentDataForm = this.GetParentDataForm();

            if (parentDataForm != this._parentDataForm)
            {
                this.RemoveBindingsFromParentDataForm();
                this.SetBindingsFromParentDataForm(parentDataForm);
                shouldGenerateUI = true;
            }

            Panel layoutPanel = this.GetLayoutPanel();

            if (layoutPanel != this._layoutPanel)
            {
                this.DetachFieldFromLayoutPanel();
                this._layoutPanel = layoutPanel;
                this.AttachFieldToLayoutPanel();
                shouldGenerateUI = true;
            }

            if (shouldGenerateUI)
            {
                this.GenerateUI();
            }
        }

        /// <summary>
        /// Handles the case where the size of a description has changed.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnDescriptionSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.AlignLabelColumns();
            this.AlignDescriptionColumns();
        }

        /// <summary>
        /// Handles the case where the left mouse button went down with the cursor over a label.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
#if MIGRATION
        private void OnLabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#else
        private void OnLabelMouseLeftButtonDown(object sender, PointerRoutedEventArgs e)
#endif
        {
            e.Handled = true;

            // If we already have focus in the content of this DataField,
            // do nothing - we don't need to give focus to anything.
            if (this._contentControl == null ||
                this.Content == null ||
                this.Content.ContainsFocusedElement())
            {
                return;
            }

            this._contentControl.Focus();
        }

        /// <summary>
        /// Handles the case where the size of a label has changed.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnLabelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.AlignLabelColumns();
            this.AlignDescriptionColumns();
        }

        /// <summary>
        /// Handles the case where a text box has lost focus.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Debug.Assert(this._lostFocusFired.ContainsKey(textBox), "LostFocus should never be handled on a text box without _lostFocusFired getting the text box as a key.");
            this._lostFocusFired[textBox] = true;
        }

        /// <summary>
        /// Handles the case where a text box's text changed.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && (ValidationUtil.ElementHasErrors(textBox) || !this._lostFocusFired[textBox]))
            {
                this._lostFocusFired[textBox] = false;
                ValidationUtil.UpdateSourceOnElementBindings(textBox);
            }
        }

        /// <summary>
        /// Removes the alignment of the grouped description columns.
        /// </summary>
        private void RemoveDescriptionColumnAlignment()
        {
#if OPENSILVER
            if (!_parentDataForm.ForceAlignment)
                return;
#endif
            IList<DataField> fieldList = this.GetAlignmentGroup();

            if (fieldList == null)
            {
                return;
            }

            foreach (DataField field in fieldList)
            {
                field.DescriptionColumnBesideContent.MinWidth = 0;
                field.DescriptionColumnBesideLabel.MinWidth = 0;
                field.DescriptionColumnBesideLabelSeparator.MinWidth = 0;
            }
        }

        /// <summary>
        /// Removes the alignment of the grouped label columns.
        /// </summary>
        private void RemoveLabelColumnAlignment()
        {
#if OPENSILVER
            if (!_parentDataForm.ForceAlignment)
                return;
#endif
            IList<DataField> fieldList = this.GetAlignmentGroup();

            if (fieldList == null)
            {
                return;
            }

            foreach (DataField field in fieldList)
            {
                field.LabelColumn.MinWidth = 0;
            }
        }

        /// <summary>
        /// Sets the read-only state of the content.
        /// </summary>
        /// <param name="isReadOnly">Whether the content should be read-only.</param>
        private void SetContentReadOnlyState(bool isReadOnly)
        {
            if (this.Content == null)
            {
                return;
            }

            Stack<DependencyObject> dependencyObjectStack = new Stack<DependencyObject>();
            dependencyObjectStack.Push(this.Content);

            while (dependencyObjectStack.Count > 0)
            {
                DependencyObject curObject = dependencyObjectStack.Pop();
                PropertyInfo propertyInfo = curObject.GetType().GetProperty("IsReadOnly");

                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(curObject, isReadOnly, null);
                    continue;
                }

                propertyInfo = curObject.GetType().GetProperty("IsEnabled");

                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(curObject, !isReadOnly, null);
                    continue;
                }

                int numChildren = VisualTreeHelper.GetChildrenCount(curObject);

                for (int i = 0; i < numChildren; i++)
                {
                    dependencyObjectStack.Push(VisualTreeHelper.GetChild(curObject, i));
                }
            }
        }

        /// <summary>
        /// Sets the description content.
        /// </summary>
        private void SetDescriptionContent()
        {
            if (this.DescriptionViewer != null)
            {
                if (this.Description != null)
                {
                    this.DescriptionViewer.Description = this.Description;
                }
                else
                {
                    if (this.DataContext != null)
                    {
                        PropertyInfo propertyInfo = this.GetPropertyInfo();

                        if (propertyInfo != null)
                        {
                            foreach (object attribute in propertyInfo.GetCustomAttributes(true))
                            {
                                DisplayAttribute displayAttribute = attribute as DisplayAttribute;
                                string description = null;

                                if (displayAttribute != null && !string.IsNullOrEmpty(description = displayAttribute.GetDescription()))
                                {
                                    this.DescriptionViewer.Description = description;
                                }
                            }
                        }
                    }
                }

                if (this.DescriptionViewerStyle != null)
                {
                    this.DescriptionViewer.SetStyleWithType(this.DescriptionViewerStyle);
                }
            }
        }

        /// <summary>
        /// Sets the value of IsReadOnlyProperty if it has not been overridden by the developer.
        /// </summary>
        private void SetIsReadOnlyIfNotOverridden()
        {
            if (!this._isReadOnlyOverridden)
            {
                bool isReadOnly = false;

                if (this.EffectiveMode == DataFieldMode.ReadOnly)
                {
                    isReadOnly = true;
                }
                else if (this.BoundProperty != null)
                {
                    if (this.BoundProperty.GetSetMethod() == null)
                    {
                        isReadOnly = true;
                    }

                    EditableAttribute editableAttribute =
                        this.BoundProperty.GetCustomAttributes(typeof(EditableAttribute), true /* inherit */)
                        .Cast<EditableAttribute>()
                        .FirstOrDefault();

                    if (editableAttribute != null &&
                        ((!editableAttribute.AllowEdit && this.EffectiveMode == DataFieldMode.Edit) ||
                        (!editableAttribute.AllowInitialValue && this.EffectiveMode == DataFieldMode.AddNew)))
                    {
                        isReadOnly = true;
                    }

                    ReadOnlyAttribute readOnlyAttribute =
                        this.BoundProperty.GetCustomAttributes(typeof(ReadOnlyAttribute), true /* inherit */)
                        .Cast<ReadOnlyAttribute>()
                        .FirstOrDefault();

                    if (readOnlyAttribute != null && readOnlyAttribute.IsReadOnly)
                    {
                        isReadOnly = true;
                    }

                    ReadOnlyAttribute classReadOnlyAttribute =
                        this.DataContext.GetType().GetCustomAttributes(typeof(ReadOnlyAttribute), true /* inherit */)
                        .Cast<ReadOnlyAttribute>()
                        .FirstOrDefault();

                    if (classReadOnlyAttribute != null && classReadOnlyAttribute.IsReadOnly)
                    {
                        isReadOnly = true;
                    }
                }

                this.SetValueNoCallback(DataField.IsReadOnlyProperty, isReadOnly);
            }
        }

        /// <summary>
        /// Sets the value of IsRequiredProperty if it has not been overridden by the developer.
        /// </summary>
        private void SetIsRequiredIfNotOverridden()
        {
            if (!this._isRequiredOverridden && this.InternalLabel != null)
            {
                this.SetValueNoCallback(DataField.IsRequiredProperty, this.InternalLabel.IsRequired);
            }
        }

        /// <summary>
        /// Sets the label content.
        /// </summary>
        private void SetLabelContent()
        {
            if (this.InternalLabel != null)
            {
                if (this.Label != null)
                {
                    this.InternalLabel.Content = this.Label;
                }
                else
                {
                    if (this.DataContext != null)
                    {
                        PropertyInfo propertyInfo = this.GetPropertyInfo();

                        if (propertyInfo != null)
                        {
                            foreach (object attribute in propertyInfo.GetCustomAttributes(true))
                            {
                                DisplayAttribute displayAttribute = attribute as DisplayAttribute;

                                if (displayAttribute != null)
                                {
                                    string name = displayAttribute.GetName();

                                    if (name == null)
                                    {
                                        name = displayAttribute.GetShortName();
                                    }

                                    if (name != null)
                                    {
                                        this.InternalLabel.Content = name;
                                    }
                                }

                                RequiredAttribute requiredAttribute = attribute as RequiredAttribute;

                                if (requiredAttribute != null)
                                {
                                    this.InternalLabel.IsRequired = true;
                                }
                            }

                            if (this.InternalLabel.Content == null)
                            {
                                this.InternalLabel.Content = propertyInfo.Name;
                            }
                        }
                    }
                }

                if (this.LabelStyle != null)
                {
                    this.InternalLabel.SetStyleWithType(this.LabelStyle);
                }
                else
                {
                    if (this.EffectiveLabelPosition != DataFieldLabelPosition.Top)
                    {
                        this.InternalLabel.HorizontalAlignment = HorizontalAlignment.Right;
                    }
                    else
                    {
                        this.InternalLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    }
                }
            }
        }

        /// <summary>
        /// Sets references on this DataField to its parent DataForm.
        /// </summary>
        /// <param name="parentDataForm">The parent DataForm.</param>
        private void SetBindingsFromParentDataForm(DataForm parentDataForm)
        {
            this._parentDataForm = parentDataForm;

            if (parentDataForm != null)
            {
                this.SetBinding(DataField.DataFormModeProperty, new Binding("Mode") { Source = parentDataForm });
                this.SetBinding(DataField.DataFormLabelPositionProperty, new Binding("LabelPosition") { Source = parentDataForm });
                this.SetBinding(DataField.DataFormDescriptionViewerPositionProperty, new Binding("DescriptionViewerPosition") { Source = parentDataForm });
                this.SetBinding(DataField.StyleProperty, new Binding("DataFieldStyle") { Source = parentDataForm });
                this.SetBinding(DataField.ForegroundProperty, new Binding("Foreground") { Source = parentDataForm });
                parentDataForm.Fields.Add(this);
            }
        }

        /// <summary>
        /// Finds any bindings on an element and updates the ones in which Mode is TwoWay
        /// to set the two Boolean properties to true.
        /// </summary>
        /// <param name="element">The element.</param>
        private void UpdateBindingsOnElement(FrameworkElement element)
        {
            if (this.DataContext == null)
            {
                return;
            }

            // DataFields will run themselves, so don't bother if we're looking at a DataField.
            if (!(element is DataField))
            {
                List<DependencyProperty> dependencyProperties = ValidationUtil.GetDependencyPropertiesForElement(element);
                Debug.Assert(dependencyProperties != null, "GetDependencyPropertiesForElement() should never return null.");

                foreach (DependencyProperty dependencyProperty in dependencyProperties)
                {
                    if (dependencyProperty != null)
                    {
                        BindingExpression bindingExpression = element.GetBindingExpression(dependencyProperty);

                        if (bindingExpression != null && bindingExpression.ParentBinding != null)
                        {
                            Binding binding = ValidationUtil.CopyBinding(bindingExpression.ParentBinding);

                            if (binding.Path != null && !String.IsNullOrEmpty(binding.Path.Path) && binding.Mode == BindingMode.TwoWay)
                            {
                                PropertyInfo propertyInfo = this.DataContext.GetType().GetPropertyInfo(binding.Path.Path);

                                if (propertyInfo == null || !propertyInfo.CanWrite)
                                {
                                    // Ignore bindings to nonexistent or read-only properties.
                                    continue;
                                }

                                binding.ValidatesOnExceptions = true;
                                binding.NotifyOnValidationError = true;
                            }

                            if (binding.Converter == null)
                            {
                                binding.Converter = new DataFormValueConverter();
                            }

                            element.SetBinding(dependencyProperty, binding);

                            TextBox textBox = element as TextBox;

                            if (textBox != null)
                            {
                                if (binding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
                                {
                                    this._lostFocusFired[textBox] = true;

                                    textBox.LostFocus -= new RoutedEventHandler(this.OnTextBoxLostFocus);
                                    textBox.LostFocus += new RoutedEventHandler(this.OnTextBoxLostFocus);
                                    textBox.TextChanged -= new TextChangedEventHandler(this.OnTextBoxTextChanged);
                                    textBox.TextChanged += new TextChangedEventHandler(this.OnTextBoxTextChanged);
                                }
                                else
                                {
                                    if (this._lostFocusFired.ContainsKey(textBox))
                                    {
                                        this._lostFocusFired.Remove(textBox);
                                    }

                                    textBox.LostFocus -= new RoutedEventHandler(this.OnTextBoxLostFocus);
                                    textBox.TextChanged -= new TextChangedEventHandler(this.OnTextBoxTextChanged);
                                }
                            }
                        }
                    }
                }

                int childrenCount = VisualTreeHelper.GetChildrenCount(element);

                for (int i = 0; i < childrenCount; i++)
                {
                    FrameworkElement childElement = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                    // Stop if we've found a child DataForm.
                    if (childElement != null && childElement.GetType() != typeof(DataForm))
                    {
                        this.UpdateBindingsOnElement(childElement);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the states on the DataField.
        /// </summary>
        private void UpdateStates()
        {
            if (this.EffectiveMode == DataFieldMode.ReadOnly)
            {
                VisualStateManager.GoToState(this, DATAFIELD_stateReadOnly, true /* useTransitions */);
            }
            else if (this.EffectiveMode == DataFieldMode.Edit)
            {
                VisualStateManager.GoToState(this, DATAFIELD_stateEdit, true /* useTransitions */);
            }
            else if (this.EffectiveMode == DataFieldMode.AddNew)
            {
                VisualStateManager.GoToState(this, DATAFIELD_stateAddNew, true /* useTransitions */);
            }

            if (this.IsValid)
            {
                VisualStateManager.GoToState(this, DATAFIELD_stateValid, true /* useTransitions */);
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFIELD_stateInvalid, true /* useTransitions */);
            }
        }

#endregion Private Methods

#endregion Methods
    }
}
