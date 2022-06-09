//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

#if MIGRATION
#if OPENSILVER
using System.Windows.Automation.Peers;
#endif
using System.Windows.Controls.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
#else
#if OPENSILVER
using Windows.UI.Xaml.Automation.Peers;
#endif
using Windows.UI.Xaml.Controls.Common;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

using resources = OpenSilver.Internal.Controls.Data.DataForm.Toolkit.Resources;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Displays data in a customizable form.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [ContentPropertyAttribute("Content")]
    [TemplatePart(Name = DATAFORM_elementHeaderElement, Type = typeof(ContentControl))]
    [TemplatePart(Name = DATAFORM_elementContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = DATAFORM_elementValidationSummary, Type = typeof(ValidationSummary))]
    [TemplatePart(Name = DATAFORM_elementFirstItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementPreviousItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementNextItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementLastItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementButtonSeparator, Type = typeof(UIElement))]
    [TemplatePart(Name = DATAFORM_elementNewItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementDeleteItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementEditButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementCommitButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementCancelButton, Type = typeof(ButtonBase))]
    [TemplateVisualState(Name = DATAFORM_stateNormal, GroupName = DATAFORM_groupCommon)]
    [TemplateVisualState(Name = DATAFORM_stateDisabled, GroupName = DATAFORM_groupCommon)]
    [TemplateVisualState(Name = DATAFORM_stateReadOnly, GroupName = DATAFORM_groupMode)]
    [TemplateVisualState(Name = DATAFORM_stateEmpty, GroupName = DATAFORM_groupMode)]
    [TemplateVisualState(Name = DATAFORM_stateEdit, GroupName = DATAFORM_groupMode)]
    [TemplateVisualState(Name = DATAFORM_stateAddNew, GroupName = DATAFORM_groupMode)]
    [TemplateVisualState(Name = DATAFORM_stateValid, GroupName = DATAFORM_groupValidation)]
    [TemplateVisualState(Name = DATAFORM_stateInvalid, GroupName = DATAFORM_groupValidation)]
    [TemplateVisualState(Name = DATAFORM_stateCommitted, GroupName = DATAFORM_groupCommitted)]
    [TemplateVisualState(Name = DATAFORM_stateUncommitted, GroupName = DATAFORM_groupCommitted)]
    [TemplateVisualState(Name = DATAFORM_stateEntity, GroupName = DATAFORM_groupScope)]
    [TemplateVisualState(Name = DATAFORM_stateCollection, GroupName = DATAFORM_groupScope)]
    [StyleTypedProperty(Property = "CancelButtonStyle", StyleTargetType = typeof(ButtonBase))]
    [StyleTypedProperty(Property = "CommitButtonStyle", StyleTargetType = typeof(ButtonBase))]
    [StyleTypedProperty(Property = "DataFieldStyle", StyleTargetType = typeof(DataField))]
    [StyleTypedProperty(Property = "ValidationSummaryStyle", StyleTargetType = typeof(ValidationSummary))]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "The DataForm is the main control that lives in the namespace.")]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The class is necessarily complicated because of what it does.")]
    public class DataForm : Control
    {
#region Constants

        private const int ColumnSpanTwoItems = 3;
        private const int ColumnsPerField = 6;
        private const int FieldElementSpacing = 6; // px
        private const int LeftXPosition = 0;
        private const int MiddleXPosition = 2;
        private const int MiddleYPosition = 1;
        private const int RowsPerField = 4;
        private const int TopYPosition = 0;

        /// <summary>
        /// The default order to use when there is no <see cref="DisplayAttribute.Order"/>
        /// value available for the property.
        /// </summary>
        /// <remarks>
        /// The value of 10000 is a standard default value, allowing
        /// some properties to be ordered at the beginning and some at the end.
        /// </remarks>
        private const int UnspecifiedOrder = 10000;

        private const string DATAFORM_elementHeaderElement = "HeaderElement";
        private const string DATAFORM_elementContentPresenter = "ContentPresenter";
        private const string DATAFORM_elementValidationSummary = "ValidationSummary";
        private const string DATAFORM_elementDataForm = "DataForm";

        private const string DATAFORM_elementFirstItemButton = "FirstItemButton";
        private const string DATAFORM_elementPreviousItemButton = "PreviousItemButton";
        private const string DATAFORM_elementNextItemButton = "NextItemButton";
        private const string DATAFORM_elementLastItemButton = "LastItemButton";

        private const string DATAFORM_elementButtonSeparator = "ButtonSeparator";

        private const string DATAFORM_elementNewItemButton = "NewItemButton";
        private const string DATAFORM_elementDeleteItemButton = "DeleteItemButton";

        private const string DATAFORM_elementEditButton = "EditButton";
        private const string DATAFORM_elementCommitButton = "CommitButton";
        private const string DATAFORM_elementCancelButton = "CancelButton";

        private const string DATAFORM_groupCommon = "CommonStates";
        private const string DATAFORM_stateNormal = "Normal";
        private const string DATAFORM_stateDisabled = "Disabled";

        private const string DATAFORM_groupMode = "ModeStates";
        private const string DATAFORM_stateReadOnly = "ReadOnly";
        private const string DATAFORM_stateEmpty = "Empty";
        private const string DATAFORM_stateEdit = "Edit";
        private const string DATAFORM_stateAddNew = "AddNew";

        private const string DATAFORM_groupValidation = "ValidationStates";
        private const string DATAFORM_stateValid = "Valid";
        private const string DATAFORM_stateInvalid = "Invalid";

        private const string DATAFORM_groupCommitted = "CommittedStates";
        private const string DATAFORM_stateCommitted = "Committed";
        private const string DATAFORM_stateUncommitted = "Uncommitted";

        private const string DATAFORM_groupScope = "ScopeStates";
        private const string DATAFORM_stateEntity = "Entity";
        private const string DATAFORM_stateCollection = "Collection";

#endregion Constants

#region Dependency Properties

        /// <summary>
        /// Identifies the AutoCommit dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoCommitProperty =
            DependencyProperty.Register(
                "AutoCommit",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnAutoCommitPropertyChanged));

        /// <summary>
        /// AutoCommit property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its AutoCommit value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnAutoCommitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            SetAllCanPropertiesAndUpdate(dataForm, false /* onlyUpdateStates */);
        }

        /// <summary>
        /// Identifies the AutoEdit dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoEditProperty =
            DependencyProperty.Register(
                "AutoEdit",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(true, OnAutoEditPropertyChanged));

        /// <summary>
        /// AutoEdit property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its AutoEdit value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnAutoEditPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm != null)
            {
                dataForm.SetMode();
                dataForm.GenerateUI();
                dataForm.UpdateButtons();
            }
        }

        /// <summary>
        /// Identifies the AutoGenerateFields dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoGenerateFieldsProperty =
            DependencyProperty.Register(
                "AutoGenerateFields",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnAutoGenerateFieldsPropertyChanged));

        /// <summary>
        /// AutoGenerateFields property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its AutoGenerateFields value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnAutoGenerateFieldsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm.ReadOnlyTemplate == null && dataForm.EditTemplate == null && dataForm.NewItemTemplate == null)
            {
                dataForm.UpdateButtonsAndStates();
                dataForm.GenerateUI();
            }
        }

        /// <summary>
        /// Identifies the CancelButtonContent dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelButtonContentProperty =
            DependencyProperty.Register(
                "CancelButtonContent",
                typeof(object),
                typeof(DataForm),
                new PropertyMetadata(OnCancelButtonContentPropertyChanged));

        /// <summary>
        /// CancelButtonContent property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CancelButtonContent value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCancelButtonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm._cancelButton != null)
            {
                dataForm._cancelButton.Content = dataForm.CancelButtonContent;
            }
        }

        /// <summary>
        /// Identifies the CancelButtonContent dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelButtonStyleProperty =
            DependencyProperty.Register(
                "CancelButtonStyle",
                typeof(Style),
                typeof(DataForm),
                null);

        /// <summary>
        /// Identifies the CommandButtonsVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandButtonsVisibilityProperty =
            DependencyProperty.Register(
                "CommandButtonsVisibility",
                typeof(Nullable<DataFormCommandButtonsVisibility>),
                typeof(DataForm),
                new PropertyMetadata(OnCommandButtonsVisibilityPropertyChanged));

        /// <summary>
        /// CommandButtonsVisibility property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CommandButtonsVisibility value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCommandButtonsVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.UpdateButtons();
        }

        /// <summary>
        /// Identifies the CommitButtonContent dependency property.
        /// </summary>
        public static readonly DependencyProperty CommitButtonContentProperty =
            DependencyProperty.Register(
                "CommitButtonContent",
                typeof(object),
                typeof(DataForm),
                new PropertyMetadata(OnCommitButtonContentPropertyChanged));

        /// <summary>
        /// CommitButtonContent property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CommitButtonContent value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCommitButtonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm._commitButton != null)
            {
                dataForm._commitButton.Content = dataForm.CommitButtonContent;
            }
        }

        /// <summary>
        /// Identifies the CommitButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CommitButtonStyleProperty =
            DependencyProperty.Register(
                "CommitButtonStyle",
                typeof(Style),
                typeof(DataForm),
                null);


        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(FrameworkElement),
                typeof(DataForm),
                new PropertyMetadata(OnContentPropertyChanged));

        /// <summary>
        /// Content property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its Content value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.RegenerateUI(null, e.OldValue);
        }

        /// <summary>
        /// Identifies the CurrentIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty.Register(
                "CurrentIndex",
                typeof(int),
                typeof(DataForm),
                new PropertyMetadata(OnCurrentIndexPropertyChanged));

        /// <summary>
        /// CurrentIndex property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CurrentItem value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCurrentIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm != null && !dataForm.AreHandlersSuspended())
            {
                // Don't let the CurrentIndex be set if there's no collection
                // or if the value is invalid.
                if (dataForm._collectionView == null)
                {
                    if ((dataForm.CurrentItem == null && dataForm.CurrentIndex != -1) ||
                        (dataForm.CurrentItem != null && dataForm.CurrentIndex != 0))
                    {
                        dataForm.SetValueNoCallback(e.Property, e.OldValue);
                    }
                }
                else
                {
                    if (dataForm.CurrentIndex < -1 || dataForm.CurrentIndex > dataForm.ItemsCount)
                    {
                        dataForm.SetValueNoCallback(e.Property, e.OldValue);
                    }
                    else
                    {
                        dataForm._collectionView.MoveCurrentToPosition(dataForm.CurrentIndex);

                        if (dataForm.CurrentIndex != dataForm._collectionView.CurrentPosition)
                        {
                            dataForm.SetValueNoCallback(e.Property, dataForm._collectionView.CurrentPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Identifies the CurrentItem dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register(
                "CurrentItem",
                typeof(object),
                typeof(DataForm),
                new PropertyMetadata(OnCurrentItemPropertyChanged));

        /// <summary>
        /// CurrentItem property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CurrentItem value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCurrentItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm != null && !dataForm.AreHandlersSuspended())
            {
                if (dataForm._lastItem != null && dataForm.ShouldValidateOnCurrencyChange)
                {
                    dataForm.ValidateItem();
                }

                if ((!dataForm.AutoCommitPreventsCurrentItemChange && dataForm.IsItemValid) &&
                    (e.NewValue == null ||
                    dataForm._collectionView == null ||
                    dataForm._collectionView.Contains(dataForm.CurrentItem)))
                {
                    dataForm.SetUpNewCurrentItem();
                    dataForm.GenerateUI(true /* clearEntityErrors */, true /* swapOldAndNew */);
                    dataForm.UpdateCurrentItem();
                    SetAllCanPropertiesAndUpdate(dataForm, false /* onlyUpdateStates */);
                    dataForm._lastItem = dataForm.CurrentItem;
                    dataForm.OnCurrentItemChanged(EventArgs.Empty);
                }
                else
                {
                    dataForm.SetValueNoCallback(e.Property, e.OldValue);
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, resources.DataForm_CannotChangeCurrency, "AutoCommit", "ItemsSource", "ICollectionView"));
                }
            }
        }

        /// <summary>
        /// Identifies the DataFieldStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty DataFieldStyleProperty =
            DependencyProperty.Register(
                "DataFieldStyle",
                typeof(Style),
                typeof(DataForm),
                null);

        /// <summary>
        /// Identifies the DescriptionViewerPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionViewerPositionProperty =
            DependencyProperty.Register(
                "DescriptionViewerPosition",
                typeof(DataFieldDescriptionViewerPosition),
                typeof(DataForm),
                null);

        /// <summary>
        /// Identifies the EditTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty EditTemplateProperty =
            DependencyProperty.Register(
                "EditTemplate",
                typeof(DataTemplate),
                typeof(DataForm),
                new PropertyMetadata(OnEditTemplatePropertyChanged));

        /// <summary>
        /// EditTemplate property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its EditTemplate value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnEditTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.RegenerateUI(DataFormMode.Edit, null);
        }

        /// <summary>
        /// Identifies the ValidationSummaryStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationSummaryStyleProperty =
            DependencyProperty.Register(
                "ValidationSummaryStyle",
                typeof(Style),
                typeof(DataForm),
                new PropertyMetadata(OnValidationSummaryStylePropertyChanged));

        /// <summary>
        /// ValidationSummaryStyle property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its ValidationSummaryStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnValidationSummaryStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;

            if (dataForm._validationSummary != null)
            {
                dataForm._validationSummary.SetStyleWithType(dataForm.ValidationSummaryStyle);
            }
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(object),
                typeof(DataForm),
                new PropertyMetadata(OnHeaderPropertyChanged));

        /// <summary>
        /// Header property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its Header value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.SetHeaderVisibility();
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(DataForm),
                new PropertyMetadata(OnHeaderTemplatePropertyChanged));

        /// <summary>
        /// HeaderTemplate property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its HeaderTemplate value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.SetHeaderVisibility();
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register(
                "HeaderVisibility",
                typeof(Visibility),
                typeof(DataForm),
                new PropertyMetadata(OnHeaderVisibilityPropertyChanged));

        /// <summary>
        /// HeaderVisibility property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its HeaderVisibility value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnHeaderVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;

            if (!dataForm.AreHandlersSuspended())
            {
                dataForm._headerVisibilityOverridden = true;
            }
        }

        /// <summary>
        /// Identifies the IsEmpty dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEmptyProperty =
            DependencyProperty.Register(
                "IsEmpty",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnIsEmptyPropertyChanged));

        /// <summary>
        /// IsEmpty property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its IsEmpty value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsEmptyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.UpdateButtonsAndStates();
        }

        /// <summary>
        /// Identifies the IsItemChanged dependency property.
        /// </summary>
        public static readonly DependencyProperty IsItemChangedProperty =
            DependencyProperty.Register(
                "IsItemChanged",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnIsItemChangedPropertyChanged));

        /// <summary>
        /// IsItemChanged property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its IsItemChanged value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsItemChangedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.UpdateButtonsAndStates();
        }

        /// <summary>
        /// Identifies the IsItemValid dependency property.
        /// </summary>
        public static readonly DependencyProperty IsItemValidProperty =
            DependencyProperty.Register(
                "IsItemValid",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(true, OnIsItemValidPropertyChanged));

        /// <summary>
        /// IsItemValid property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its IsItemValid value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsItemValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            SetAllCanPropertiesAndUpdate(dataForm, false /* onlyUpdateStates */);
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnIsReadOnlyPropertyChanged));

        /// <summary>
        /// IsReadOnly property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its IsReadOnly value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;

            if (dataForm != null && dataForm.IsEditing)
            {
                dataForm.CancelEdit();
            }

            SetAllCanPropertiesAndUpdate(dataForm, false /* onlyUpdateStates */);
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(DataForm),
                new PropertyMetadata(OnItemsSourcePropertyChanged));

        /// <summary>
        /// ItemsSource property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its ItemsSource value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm == null)
            {
                return;
            }

            if (dataForm._collectionView != null)
            {
                if (dataForm._weakEventListenerCurrentChanging != null)
                {
                    dataForm._weakEventListenerCurrentChanging.Detach();
                    dataForm._weakEventListenerCurrentChanging = null;
                }

                if (dataForm._weakEventListenerCurrentChanged != null)
                {
                    dataForm._weakEventListenerCurrentChanged.Detach();
                    dataForm._weakEventListenerCurrentChanged = null;
                }

                if (dataForm._weakEventListenerCollectionChanged != null)
                {
                    dataForm._weakEventListenerCollectionChanged.Detach();
                    dataForm._weakEventListenerCollectionChanged = null;
                }
            }

            dataForm.ForceEndEdit();
            dataForm._collectionView = null;

            IEnumerable newItemsSource = e.NewValue as IEnumerable;
            dataForm._originalItemsSource = newItemsSource;

            bool collectionViewCreated = false;
            ICollectionView collectionView = newItemsSource as ICollectionView;

            if (collectionView != null)
            {
                dataForm._collectionView = collectionView;
            }
            else if (newItemsSource != null)
            {
                dataForm._collectionView = DataForm.CreateView(newItemsSource);
                collectionViewCreated = true;
            }

            if (dataForm._collectionView != null)
            {
                dataForm.SetUpCollectionView(collectionViewCreated);
            }
            else
            {
                dataForm.CurrentItem = null;
                dataForm.UpdateCurrentItem();
            }

            dataForm.SetAllCanProperties();

            if (dataForm._buttonSeparator != null)
            {
                dataForm.SetButtonSeparatorVisibility();
            }
        }

        /// <summary>
        /// Identifies the LabelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register(
                "LabelPosition",
                typeof(DataFieldLabelPosition),
                typeof(DataForm),
                null);

        /// <summary>
        /// Identifies the Mode dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                "Mode",
                typeof(DataFormMode),
                typeof(DataForm),
                null);

        /// <summary>
        /// Identifies the NewItemTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty NewItemTemplateProperty =
            DependencyProperty.Register(
                "NewItemTemplate",
                typeof(DataTemplate),
                typeof(DataForm),
                new PropertyMetadata(OnNewItemTemplatePropertyChanged));

        /// <summary>
        /// NewItemTemplate property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its NewItemTemplate value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnNewItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.RegenerateUI(DataFormMode.AddNew, null);
        }

        /// <summary>
        /// Identifies the ReadOnlyTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ReadOnlyTemplateProperty =
            DependencyProperty.Register(
                "ReadOnlyTemplate",
                typeof(DataTemplate),
                typeof(DataForm),
                new PropertyMetadata(OnReadOnlyTemplatePropertyChanged));

        /// <summary>
        /// ReadOnlyTemplate property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its ReadOnlyTemplate value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnReadOnlyTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.RegenerateUI(DataFormMode.ReadOnly, null);
        }

#endregion Dependency Properties

#region Fields

        /// <summary>
        /// Holds the add-new contents mapped to mode that have been seen before.
        /// </summary>
        private IDictionary<Type, FrameworkElement> _addNewContentsNew;

        /// <summary>
        /// Holds the old add-new contents mapped to mode that have been seen before.
        /// </summary>
        private IDictionary<Type, FrameworkElement> _addNewContentsOld;

        /// <summary>
        /// Private accessor to ButtonSeparator.
        /// </summary>
        private UIElement _buttonSeparator;

        /// <summary>
        /// Private accessor to CanAddItems.
        /// </summary>
        private bool _canAddItems;

        /// <summary>
        /// Private accessor to CancelButton.
        /// </summary>
        private ButtonBase _cancelButton;

        /// <summary>
        /// Private accessor to CanBeginEdit.
        /// </summary>
        private bool _canBeginEdit;

        /// <summary>
        /// Private accessor to CanCancelEdit.
        /// </summary>
        private bool _canCancelEdit;

        /// <summary>
        /// Private accessor to CanCommitEdit.
        /// </summary>
        private bool _canCommitEdit;

        /// <summary>
        /// Private accessor to CanDeleteItems.
        /// </summary>
        private bool _canDeleteItems;

        /// <summary>
        /// Private accessor to CanMoveToFirstItem.
        /// </summary>
        private bool _canMoveToFirstItem;

        /// <summary>
        /// Private accessor to CanMoveToLastItem.
        /// </summary>
        private bool _canMoveToLastItem;

        /// <summary>
        /// Private accessor to CanMoveToNextItem.
        /// </summary>
        private bool _canMoveToNextItem;

        /// <summary>
        /// Private accessor to CanMoveToPreviousItem.
        /// </summary>
        private bool _canMoveToPreviousItem;

        /// <summary>
        /// Represents the items source as an ICollectionView.
        /// </summary>
        private ICollectionView _collectionView;

        /// <summary>
        /// Private accessor to CommitButton.
        /// </summary>
        private ButtonBase _commitButton;

        /// <summary>
        /// Private accessor to the content presenter.
        /// </summary>
        private ContentPresenter _contentPresenter;

        /// <summary>
        /// Holds which template is currently being used for which type.
        /// </summary>
        private IDictionary<Type, DataFormMode?> _currentlyUsedTemplate;

        /// <summary>
        /// Private accessor to DeleteItemButton.
        /// </summary>
        private ButtonBase _deleteItemButton;

        /// <summary>
        /// Holds which properties on the current item can be edited,
        /// and what their original values were.
        /// </summary>
        private IDictionary<string, object> _editablePropertiesOriginalValues;

        /// <summary>
        /// Private accessor to EditButton.
        /// </summary>
        private ButtonBase _editButton;

        /// <summary>
        /// Holds the edit contents mapped to mode that have been seen before.
        /// </summary>
        private IDictionary<Type, FrameworkElement> _editContentsNew;

        /// <summary>
        /// Holds the old edit contents mapped to mode that have been seen before.
        /// </summary>
        private IDictionary<Type, FrameworkElement> _editContentsOld;

        /// <summary>
        /// Holds which properties on the current item have been edited
        /// and not saved locally.
        /// </summary>
        private IList<string> _editedProperties;

        /// <summary>
        /// Holds a value indicating whether or not an edit is ending.
        /// </summary>
        private bool _endingEdit;

        /// <summary>
        /// Holds a collection of editing elements that have event subscriptions.
        /// </summary>
        private List<FrameworkElement> _editingElements;

        /// <summary>
        /// Holds the entity-level validation errors.
        /// </summary>
        private ObservableCollection<ValidationSummaryItem> _entityLevelErrors;

        /// <summary>
        /// Private accessor to ValidationSummary.
        /// </summary>
        private ValidationSummary _validationSummary;

        /// <summary>
        /// Holds the field-level validation errors.
        /// </summary>
        private IList<ValidationError> _fieldLevelErrors;

        /// <summary>
        /// Holds the current list of fields.
        /// </summary>
        private IList<DataField> _fields;

        /// <summary>
        /// Holds the list of fields tied to the contents from which they came.
        /// </summary>
        private IDictionary<FrameworkElement, IList<DataField>> _fieldsDictionary;

        /// <summary>
        /// Private accessor to FirstItemButton.
        /// </summary>
        private ButtonBase _firstItemButton;

        /// <summary>
        /// Holds whether an end of an edit has been forced and should override AutoEdit.
        /// </summary>
        private bool _forcedEndEdit;

        /// <summary>
        /// Private accessor to GeneratedBindingModesByType.
        /// </summary>
        private IDictionary<Type, IDictionary<string, BindingMode>> _generatedBindingModesByType;

        /// <summary>
        /// Private accessor to GeneratedPropertiesByType.
        /// </summary>
        private IDictionary<Type, IList<string>> _generatedPropertiesByType;

        /// <summary>
        /// Holds whether HeaderVisibility has been set.
        /// </summary>
        private bool _headerVisibilityOverridden;

        /// <summary>
        /// Private accessor to IsAddingNew.
        /// </summary>
        private bool _isAddingNew;

        /// <summary>
        /// Holds a value indicating whether or not we're currently committing an edit.
        /// </summary>
        private bool _isCommitting;

        /// <summary>
        /// Private accessor to IsEditing.
        /// </summary>
        private bool _isEditing;

        /// <summary>
        /// The value of CanAddItems prior to entering edit mode.
        /// </summary>
        private bool _canAddItemsAfterEdit;

        /// <summary>
        /// The value of CanDeleteItems prior to entering edit mode.
        /// </summary>
        private bool _canDeleteItemsAfterEdit;

        /// <summary>
        /// Holds the index of the last value of CurrentPosition on the collection view.
        /// </summary>
        private int _lastCurrentPosition;

        /// <summary>
        /// The last value of CurrentItem, for use when auto-committing an edit.
        /// </summary>
        private object _lastItem;

        /// <summary>
        /// Private accessor to LastItemButton.
        /// </summary>
        private ButtonBase _lastItemButton;

        /// <summary>
        /// Holds whether or not lost focus has been fired since
        /// TextBox validation on text changed began.
        /// </summary>
        private IDictionary<TextBox, bool> _lostFocusFired;

        /// <summary>
        /// Private accessor to NewItemButton.
        /// </summary>
        private ButtonBase _newItemButton;

        /// <summary>
        /// Private accessor to NextItemButton.
        /// </summary>
        private ButtonBase _nextItemButton;

        /// <summary>
        /// Holds the next tab index for generated fields.
        /// </summary>
        private int _nextTabIndex;

        /// <summary>
        /// Represents the original items source prior to wrapping.
        /// </summary>
        private IEnumerable _originalItemsSource;

        /// <summary>
        /// Private accessor to PreviousItemButton.
        /// </summary>
        private ButtonBase _previousItemButton;

        /// <summary>
        /// Holds the read-only contents mapped to mode that have been seen before.
        /// </summary>
        private IDictionary<Type, FrameworkElement> _readOnlyContentsNew;

        /// <summary>
        /// Holds the old read-only contents mapped to mode that have been seen before.
        /// </summary>
        private IDictionary<Type, FrameworkElement> _readOnlyContentsOld;

        /// <summary>
        /// Holds a value indicating whether or not focus should be moved when content is loaded.
        /// </summary>
        private bool _shouldMoveFocus;

        /// <summary>
        /// Holds the template binding information.
        /// </summary>
        private List<DataFormBindingInfo> _templateBindingInfos;

        /// <summary>
        /// Holds the weak event listener for the INotifyCollectionChanged.CollectionChanged event.
        /// </summary>
        private WeakEventListener<DataForm, object, NotifyCollectionChangedEventArgs> _weakEventListenerCollectionChanged;

        /// <summary>
        /// Holds the weak event listener for the ICollectionView.CurrentChanged event.
        /// </summary>
        private WeakEventListener<DataForm, object, EventArgs> _weakEventListenerCurrentChanged;

        /// <summary>
        /// Holds the weak event listener for the ICollectionView.CurrentChanging event.
        /// </summary>
        private WeakEventListener<DataForm, object, CurrentChangingEventArgs> _weakEventListenerCurrentChanging;

        /// <summary>
        /// Holds the weak event listener for the INotifyPropertyChanged.PropertyChanged event.
        /// </summary>
        private WeakEventListener<DataForm, object, PropertyChangedEventArgs> _weakEventListenerPropertyChanged;

#endregion Fields

#region Constructors

        /// <summary>
        /// Constructs a new instance of DataForm.
        /// </summary>
        public DataForm()
        {
            this.DefaultStyleKey = typeof(DataForm);
            this._readOnlyContentsNew = new Dictionary<Type, FrameworkElement>();
            this._readOnlyContentsOld = new Dictionary<Type, FrameworkElement>();
            this._editContentsNew = new Dictionary<Type, FrameworkElement>();
            this._editContentsOld = new Dictionary<Type, FrameworkElement>();
            this._addNewContentsNew = new Dictionary<Type, FrameworkElement>();
            this._addNewContentsOld = new Dictionary<Type, FrameworkElement>();
            this._currentlyUsedTemplate = new Dictionary<Type, DataFormMode?>();
            this._editablePropertiesOriginalValues = new Dictionary<string, object>();
            this._editedProperties = new List<string>();
            this._editingElements = new List<FrameworkElement>();
            this._entityLevelErrors = new ObservableCollection<ValidationSummaryItem>();
            this._fieldLevelErrors = new List<ValidationError>();
            this._fieldsDictionary = new Dictionary<FrameworkElement, IList<DataField>>();
            this._lostFocusFired = new Dictionary<TextBox, bool>();
        }

#endregion Constructors

#region Events

        /// <summary>
        /// Event handler for when an item is being added.
        /// </summary>
        public event EventHandler<DataFormAddingNewItemEventArgs> AddingNewItem;

        /// <summary>
        /// Event handler for when a field is being auto-generated.
        /// </summary>
        public event EventHandler<DataFormAutoGeneratingFieldEventArgs> AutoGeneratingField;

        /// <summary>
        /// Event handler for when an edit is beginning.  Cannot be canceled when AutoEdit is true.
        /// </summary>
        public event EventHandler<CancelEventArgs> BeginningEdit;

        /// <summary>
        /// Event handler for when content is loaded.
        /// </summary>
        public event EventHandler<DataFormContentLoadEventArgs> ContentLoaded;

        /// <summary>
        /// Event handler for when content is about to be loaded.
        /// </summary>
        public event EventHandler<DataFormContentLoadEventArgs> ContentLoading;

        /// <summary>
        /// Event handler for when the current item has changed.
        /// </summary>
        public event EventHandler<EventArgs> CurrentItemChanged;

        /// <summary>
        /// Event handler for when an item is being deleted.
        /// </summary>
        public event EventHandler<CancelEventArgs> DeletingItem;

        /// <summary>
        /// Event handler for when an edit has ended.
        /// </summary>
        public event EventHandler<DataFormEditEndedEventArgs> EditEnded;

        /// <summary>
        /// Event handler for when an edit is ending.
        /// </summary>
        public event EventHandler<DataFormEditEndingEventArgs> EditEnding;

        /// <summary>
        /// Event handler for when the item is being validated.
        /// </summary>
        public event EventHandler<CancelEventArgs> ValidatingItem;

#endregion Events

#region Properties

#region Public Properties

        /// <summary>
        /// Gets or sets a value that indicates whether edited items are committed when the current item is changed.
        /// </summary>
        public bool AutoCommit
        {
            get
            {
                return (bool)GetValue(AutoCommitProperty);
            }

            set
            {
                this.SetValue(AutoCommitProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the 
        /// <see cref="DataForm" /> should be permanently in edit mode. 
        /// </summary>
        public bool AutoEdit
        {
            get
            {
                return (bool)GetValue(AutoEditProperty);
            }

            set
            {
                this.SetValue(AutoEditProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether or not to automatically 
        /// generate the <see cref="DataField"/>s collection. 
        /// </summary>
        public bool AutoGenerateFields
        {
            get
            {
                return (bool)GetValue(AutoGenerateFieldsProperty);
            }

            set
            {
                this.SetValue(AutoGenerateFieldsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the Cancel button.
        /// </summary>
        public object CancelButtonContent
        {
            get
            {
                return this.GetValue(CancelButtonContentProperty);
            }

            set
            {
                this.SetValue(CancelButtonContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the Cancel button.
        /// </summary>
        public Style CancelButtonStyle
        {
            get
            {
                return this.GetValue(CancelButtonStyleProperty) as Style;
            }

            set
            {
                this.SetValue(CancelButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates which command 
        /// buttons are visible on the <see cref="DataForm" /> . 
        /// </summary>
        // 


        [TypeConverter(typeof(DataFormCommandButtonsVisibilityTypeConverter))]
        public DataFormCommandButtonsVisibility? CommandButtonsVisibility
        {
            get
            {
                return (DataFormCommandButtonsVisibility?)this.GetValue(CommandButtonsVisibilityProperty);
            }

            set
            {
                this.SetValue(CommandButtonsVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the Commit button.
        /// </summary>
        public object CommitButtonContent
        {
            get
            {
                return this.GetValue(CommitButtonContentProperty);
            }

            set
            {
                this.SetValue(CommitButtonContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the Commit button.
        /// </summary>
        public Style CommitButtonStyle
        {
            get
            {
                return this.GetValue(CommitButtonStyleProperty) as Style;
            }

            set
            {
                this.SetValue(CommitButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the direct content of the DataForm, to be used when a template is not specified.
        /// </summary>
        public FrameworkElement Content
        {
            get
            {
                return (FrameworkElement)this.GetValue(ContentProperty);
            }

            set
            {
                this.SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the index of the current item.
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return (int)this.GetValue(CurrentIndexProperty);
            }

            set
            {
                this.SetValue(CurrentIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        public object CurrentItem
        {
            get
            {
                return this.GetValue(CurrentItemProperty);
            }

            set
            {
                this.SetValue(CurrentItemProperty, value);
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
        /// Gets or sets the developer-specified style for <see cref="DataField"/>s.
        /// </summary>
        public Style DataFieldStyle
        {
            get
            {
                return this.GetValue(DataFieldStyleProperty) as Style;
            }

            set
            {
                this.SetValue(DataFieldStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.DataTemplate" /> used when editing. 
        /// </summary>
        public DataTemplate EditTemplate
        {
            get
            {
                return this.GetValue(EditTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(EditTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the ValidationSummary.
        /// </summary>
        public ValidationSummary ValidationSummary
        {
            get
            {
                return this._validationSummary;
            }
        }

        /// <summary>
        /// Gets or sets the developer-specified style for the ValidationSummary.
        /// </summary>
        public Style ValidationSummaryStyle
        {
            get
            {
                return this.GetValue(ValidationSummaryStyleProperty) as Style;
            }

            set
            {
                this.SetValue(ValidationSummaryStyleProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the header for the <see cref="DataForm" /> . 
        /// </summary>
        public object Header
        {
            get
            {
                return this.GetValue(HeaderProperty);
            }

            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.DataTemplate" /> used
        /// for the header of the <see cref="DataForm" /> . 
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return this.GetValue(HeaderTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the header is visible.
        /// </summary>
        public Visibility HeaderVisibility
        {
            get
            {
                return (Visibility)this.GetValue(HeaderVisibilityProperty);
            }

            set
            {
                this.SetValue(HeaderVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not the control is displaying an item.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (bool)this.GetValue(IsEmptyProperty);
            }

            private set
            {
                this.SetValue(IsEmptyProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not the current item has been changed.
        /// </summary>
        public bool IsItemChanged
        {
            get
            {
                return (bool)this.GetValue(IsItemChangedProperty);
            }

            private set
            {
                this.SetValue(IsItemChangedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not the current item is valid.
        /// </summary>
        public bool IsItemValid
        {
            get
            {
                return (bool)this.GetValue(IsItemValidProperty);
            }

            private set
            {
                this.SetValue(IsItemValidProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can edit the values in the control.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }

            set
            {
                this.SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a collection that is used to populate the form content of the control.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return this.GetValue(ItemsSourceProperty) as IEnumerable;
            }

            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the position of labels in relation to the field.
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
        /// Gets a value that indicates whether the control is in read only, edit, or add new mode.
        /// </summary>
        public DataFormMode Mode
        {
            get
            {
                return (DataFormMode)this.GetValue(ModeProperty);
            }

            private set
            {
                this.SetValue(ModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item template used when adding a new item.
        /// </summary>
        public DataTemplate NewItemTemplate
        {
            get
            {
                return this.GetValue(NewItemTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(NewItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item template used when not editing.
        /// </summary>
        public DataTemplate ReadOnlyTemplate
        {
            get
            {
                return this.GetValue(ReadOnlyTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(ReadOnlyTemplateProperty, value);
            }
        }

#if OPENSILVER
        /// <summary>
        /// Gets or sets a value that indicates if datafields should run the alignment logic.
        /// The default value is true.
        /// </summary>
        public bool ForceAlignment { get; set; } = true;
#endif

#endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Gets a value indicating whether or not the collection allows the
        /// addition of items.
        /// </summary>
        internal bool CanAddItems
        {
            get
            {
                return this._canAddItems;
            }

            private set
            {
                if (value != this._canAddItems)
                {
                    this._canAddItems = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not an edit can be started.
        /// </summary>
        internal bool CanBeginEdit
        {
            get
            {
                return this._canBeginEdit;
            }

            private set
            {
                if (value != this._canBeginEdit)
                {
                    this._canBeginEdit = value;
                    this.UpdateButtonsAndStates();
                    this.SetMode();
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not an edit can be canceled.
        /// </summary>
        internal bool CanCancelEdit
        {
            get
            {
                return this._canCancelEdit;
            }

            private set
            {
                if (value != this._canCancelEdit)
                {
                    this._canCancelEdit = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not an edit can be committed.
        /// </summary>
        internal bool CanCommitEdit
        {
            get
            {
                return this._canCommitEdit;
            }

            private set
            {
                if (value != this._canCommitEdit)
                {
                    this._canCommitEdit = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the collection allows the
        /// deletion of items.
        /// </summary>
        internal bool CanDeleteItems
        {
            get
            {
                return this._canDeleteItems;
            }

            private set
            {
                if (value != this._canDeleteItems)
                {
                    this._canDeleteItems = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not you can to the first item.
        /// </summary>
        internal bool CanMoveToFirstItem
        {
            get
            {
                return this._canMoveToFirstItem;
            }

            private set
            {
                if (value != this._canMoveToFirstItem)
                {
                    this._canMoveToFirstItem = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not you can to the last item.
        /// </summary>
        internal bool CanMoveToLastItem
        {
            get
            {
                return this._canMoveToLastItem;
            }

            private set
            {
                if (value != this._canMoveToLastItem)
                {
                    this._canMoveToLastItem = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not you can to the next item.
        /// </summary>
        internal bool CanMoveToNextItem
        {
            get
            {
                return this._canMoveToNextItem;
            }

            private set
            {
                if (value != this._canMoveToNextItem)
                {
                    this._canMoveToNextItem = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not you can to the previous item.
        /// </summary>
        internal bool CanMoveToPreviousItem
        {
            get
            {
                return this._canMoveToPreviousItem;
            }

            private set
            {
                if (value != this._canMoveToPreviousItem)
                {
                    this._canMoveToPreviousItem = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets the type of the current item.
        /// </summary>
        internal Type CurrentItemType
        {
            get
            {
                if (this.CurrentItem == null)
                {
                    return null;
                }
                else
                {
                    return this.CurrentItem.GetType();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the DataForm is effectively read-only,
        /// taking into account both IsReadOnly and the presence of templates.
        /// </summary>
        internal bool EffectiveIsReadOnly
        {
            get
            {
                if (this.ReadOnlyTemplate != null && this.EditTemplate == null && this.NewItemTemplate == null)
                {
                    return true;
                }
                else
                {
                    return this.IsReadOnly;
                }
            }
        }

        /// <summary>
        /// Gets the list of fields.
        /// </summary>
        internal IList<DataField> Fields
        {
            get
            {
                return this._fields;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the user is appending a new item.
        /// </summary>
        internal bool IsAddingNew
        {
            get
            {
                return this._isAddingNew;
            }

            private set
            {
                if (value != this._isAddingNew)
                {
                    this._isAddingNew = value;
                    this.IsEditing = this.IsAddingNew;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the user is editing the current item.
        /// </summary>
        internal bool IsEditing
        {
            get
            {
                return this._isEditing;
            }

            private set
            {
                if (value != this._isEditing)
                {
                    this._isEditing = value;
                    this.SetMode();
                    this.GenerateUI();
                    SetAllCanPropertiesAndUpdate(this, false /* onlyUpdateStates */);
                }
            }
        }

        /// <summary>
        /// Gets the number of items in the items source.
        /// </summary>
        internal int ItemsCount
        {
            get
            {
                // If the items source implements IList, use its Count property.
                IList listItemsSource = this._originalItemsSource as IList;

                if (listItemsSource != null)
                {
                    return listItemsSource.Count;
                }

                // If the items source is a PagedCollectionView, use its Count property.
                PagedCollectionView pagedCollectionView = this._originalItemsSource as PagedCollectionView;

                if (pagedCollectionView != null)
                {
                    return pagedCollectionView.Count;
                }

                // If the items source implements IEnumerable, enumerate the items
                // and count them.
                IEnumerable enumerableItemsSource = this._originalItemsSource;

                if (enumerableItemsSource != null)
                {
                    int count = 0;
                    IEnumerator enumerator = enumerableItemsSource.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        count++;
                    }

                    return count;
                }

                // If it's not even an IEnumerable, we've got either a singleton or nothing.
                return this.CurrentItem != null ? 1 : 0;
            }
        }

#endregion Internal Properties

#region Private Properties

        /// <summary>
        /// Gets a value indicating whether or not the value of AutoCommit should
        /// prevent the changing of the current item.
        /// </summary>
        private bool AutoCommitPreventsCurrentItemChange
        {
            get
            {
                return !this.AutoCommit && this.IsEditing && this._editedProperties.Count > 0 && !this._isCommitting;
            }
        }

        /// <summary>
        /// Gets the current item as an IChangeTracking.
        /// </summary>
        private IChangeTracking CurrentItemChangeTracking
        {
            get
            {
                return this.CurrentItem as IChangeTracking;
            }
        }

        /// <summary>
        /// Gets the current item as an INotifyPropertyChanged.
        /// </summary>
        private INotifyPropertyChanged CurrentItemNotifyPropertyChanged
        {
            get
            {
                return this.CurrentItem as INotifyPropertyChanged;
            }
        }

        /// <summary>
        /// Gets the entity collection view as an IEditableCollectionView.
        /// </summary>
        private IEditableCollectionView EditableCollectionView
        {
            get
            {
                return this._collectionView as IEditableCollectionView;
            }
        }

        /// <summary>
        /// Gets the generated binding modes associated to properties for a given type.
        /// </summary>
        private IDictionary<Type, IDictionary<string, BindingMode>> GeneratedBindingModesByType
        {
            get
            {
                if (this._generatedBindingModesByType == null)
                {
                    this._generatedBindingModesByType = new Dictionary<Type, IDictionary<string, BindingMode>>();
                }

                return this._generatedBindingModesByType;
            }
        }

        /// <summary>
        /// Gets the generated properties for a given type.
        /// </summary>
        private IDictionary<Type, IList<string>> GeneratedPropertiesByType
        {
            get
            {
                if (this._generatedPropertiesByType == null)
                {
                    this._generatedPropertiesByType = new Dictionary<Type, IList<string>>();
                }

                return this._generatedPropertiesByType;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not validation should be run if
        /// currency is about to change or if currency has changed.
        /// </summary>
        private bool ShouldValidateOnCurrencyChange
        {
            get
            {
                bool areEditsAboutToBeCommitted = this.AutoCommit && this._editedProperties.Count > 0;
                bool areValidationErrorsNeedingResolution = !this.IsItemValid;

                return this.IsEditing && (areEditsAboutToBeCommitted || areValidationErrorsNeedingResolution);
            }
        }

#endregion Private Properties

#endregion Properties

#region Methods

#region Public Methods

        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <returns>Whether or not a new item was added.</returns>
        public bool AddNewItem()
        {
            if (this.IsEditing)
            {
                if (this._canAddItemsAfterEdit && this.ValidateItem())
                {
                    this.ForceEndEdit();
                }
                else
                {
                    // Don't end the edit if we won't be able to start an insert operation immediately afterwards
                    // or if there are validation errors.
                    return false;
                }
            }

            if (!this.CanAddItems)
            {
                return false;
            }

            DataFormAddingNewItemEventArgs e = new DataFormAddingNewItemEventArgs();
            this.OnAddingNewItem(e);

            if (e.Cancel)
            {
                return false;
            }

            if (this.EditableCollectionView != null && this.EditableCollectionView.CanAddNew)
            {
                this._lastCurrentPosition = this._collectionView.CurrentPosition;
                this.EditableCollectionView.AddNew();
                this.IsAddingNew = this.EditableCollectionView.IsAddingNew;
                this.GetAllOriginalPropertyValues();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Begins the editing of the current item.
        /// </summary>
        /// <returns>Whether or not editing was begun.</returns>
        public bool BeginEdit()
        {
            return this.BeginEdit(true /* startingNewEdit */);
        }

        /// <summary>
        /// Cancels the editing of the current item.
        /// </summary>
        /// <returns>Whether or not the cancellation was successful.</returns>
        public bool CancelEdit()
        {
            if (!this.CanCancelEdit)
            {
                return false;
            }

            DataFormEditEndingEventArgs e = new DataFormEditEndingEventArgs(DataFormEditAction.Cancel);
            this.OnItemEditEnding(e);

            if (e.Cancel)
            {
                return false;
            }

            DataFormMode oldMode = this.Mode;

            if (!this.IsAddingNew)
            {
                if (this.EditableCollectionView != null)
                {
                    if (this.EditableCollectionView.CanCancelEdit)
                    {
                        this.EditableCollectionView.CancelEdit();
                    }
                }
                else
                {
                    IEditableObject currentEditableObject = this._lastItem as IEditableObject;

                    if (currentEditableObject != null)
                    {
                        currentEditableObject.CancelEdit();
                    }
                }
            }
            else
            {
                this.CancelAppend();
            }

            this._fieldLevelErrors.Clear();
            this._entityLevelErrors.Clear();

            if (this._validationSummary != null)
            {
                Debug.Assert(this._validationSummary.Errors != null, "ValidationSummary.Errors should never be null.");
                this._validationSummary.Errors.Clear();
            }

            this.IsItemValid = true;
            this._editablePropertiesOriginalValues.Clear();
            this._editedProperties.Clear();
            this.EndEdit();
            this.UpdateButtonsAndStates();
            this.OnItemEditEnded(new DataFormEditEndedEventArgs(DataFormEditAction.Cancel));

            // 


            if (this._lastItem != null)
            {
                IDictionary<Type, FrameworkElement> contentsNew = null;

                if (oldMode == DataFormMode.Edit)
                {
                    contentsNew = this._editContentsNew;
                }
                else if (oldMode == DataFormMode.AddNew)
                {
                    contentsNew = this._addNewContentsNew;
                }

                if (contentsNew != null)
                {
                    this.RemoveContentForType(this._lastItem.GetType(), contentsNew);
                }
            }

            // We need to re-generate UI, which won't happen automatically
            // if we're in auto-edit mode, so force a generation.
            if (this.AutoEdit && !this._forcedEndEdit)
            {
                this.GenerateUI();
            }

            return true;
        }

        /// <summary>
        /// Commits the edit of the current item and exits editing mode.
        /// </summary>
        /// <returns>True if the commit succeeds; false otherwise.</returns>
        public bool CommitEdit()
        {
            return this.CommitEdit(true /* exitEditingMode */);
        }

        /// <summary>
        /// Commits the edit of the current item.
        /// </summary>
        /// <param name="exitEditingMode">Whether or not editing mode should be exited after committing the edit.</param>
        /// <returns>True if the commit succeeds; false otherwise.</returns>
        public bool CommitEdit(bool exitEditingMode)
        {
            if (!this.CanCommitEdit)
            {
                return false;
            }

            DataFormEditEndingEventArgs e = new DataFormEditEndingEventArgs(DataFormEditAction.Commit);
            this.OnItemEditEnding(e);

            if (e.Cancel)
            {
                return false;
            }

            if (!this.ValidateItem())
            {
                return false;
            }

            if (!this.IsAddingNew)
            {
                try
                {
                    this._isCommitting = true;

                    if (this.EditableCollectionView != null)
                    {
                        this.EditableCollectionView.CommitEdit();
                    }
                    else
                    {
                        IEditableObject currentEditableObject = this._lastItem as IEditableObject;

                        if (currentEditableObject != null)
                        {
                            currentEditableObject.EndEdit();
                        }
                    }
                }
                finally
                {
                    this._isCommitting = false;
                }
            }
            else
            {
                this.CommitAppend();
            }

            this._editablePropertiesOriginalValues.Clear();
            this._editedProperties.Clear();

            if (exitEditingMode)
            {
                this.EndEdit();
            }

            this.UpdateButtonsAndStates();
            this.OnItemEditEnded(new DataFormEditEndedEventArgs(DataFormEditAction.Commit));

            if (!exitEditingMode)
            {
                this.BeginEdit(false /* startingNewEdit */);
            }

            return true;
        }

        /// <summary>
        /// Deletes the current item.
        /// </summary>
        /// <returns>Whether or not the item was deleted.</returns>
        public bool DeleteItem()
        {
            if (this.IsEditing)
            {
                if (this._canDeleteItemsAfterEdit)
                {
                    this.ForceEndEdit();
                }
                else
                {
                    // Don't end the edit if we won't be able to start a delete operation immediately afterwards.
                    return false;
                }
            }

            if (!this.CanDeleteItems)
            {
                return false;
            }

            CancelEventArgs e = new CancelEventArgs();
            this.OnDeletingItem(e);

            if (e.Cancel)
            {
                return false;
            }

            if (this.EditableCollectionView != null &&
                this.EditableCollectionView.CanRemove &&
                this._collectionView.CurrentPosition >= 0 &&
                this._collectionView.CurrentPosition < this.ItemsCount)
            {
                this._lastCurrentPosition = this._collectionView.CurrentPosition;
                this.EditableCollectionView.RemoveAt(this._collectionView.CurrentPosition);

                if (this._lastCurrentPosition < this.ItemsCount - 1)
                {
                    this._collectionView.MoveCurrentToPosition(this._lastCurrentPosition);
                }
                else
                {
                    this._collectionView.MoveCurrentToLast();
                }
            }

            return true;
        }

        /// <summary>
        /// Finds an object with a given name in the DataForm's content.
        /// </summary>
        /// <param name="name">The name to search by.</param>
        /// <returns>The object, if found.</returns>
        public object FindNameInContent(string name)
        {
            if (this._contentPresenter != null)
            {
                FrameworkElement contentRootElement = this._contentPresenter.Content as FrameworkElement;

                if (contentRootElement != null)
                {
                    return contentRootElement.FindName(name);
                }
            }

            return null;
        }

        /// <summary>
        /// Applies the template for this control.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "No need to split up the method; it only has a large number of if statements.")]
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (this._contentPresenter != null)
            {
                this._contentPresenter.Content = null;
            }

            this._contentPresenter = GetTemplateChild(DATAFORM_elementContentPresenter) as ContentPresenter;

            if (this._firstItemButton != null)
            {
                this._firstItemButton.Click -= new RoutedEventHandler(this.OnMoveToFirstItemButtonClick);
            }

            this._firstItemButton = GetTemplateChild(DATAFORM_elementFirstItemButton) as ButtonBase;

            if (this._firstItemButton != null)
            {
                this._firstItemButton.Click += new RoutedEventHandler(this.OnMoveToFirstItemButtonClick);
            }

            if (this._validationSummary != null)
            {
                this._validationSummary.Errors.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnValidationSummaryErrorsCollectionChanged);
            }

            this._validationSummary = GetTemplateChild(DATAFORM_elementValidationSummary) as ValidationSummary;

            if (this._validationSummary != null)
            {
                this._validationSummary.Errors.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnValidationSummaryErrorsCollectionChanged);
                this._validationSummary.SetStyleWithType(this.ValidationSummaryStyle);
                if (DesignerProperties.IsInDesignTool)
                {
                    Debug.Assert(this._validationSummary.Errors != null, "The Errors collection in the ValidationSummary should never be null.");
                    // Do not add the default design time errors when in design mode.
                    this._validationSummary.Errors.Clear();
                }
            }

            if (this._previousItemButton != null)
            {
                this._previousItemButton.Click -= new RoutedEventHandler(this.OnMoveToPreviousItemButtonClick);
            }

            this._previousItemButton = GetTemplateChild(DATAFORM_elementPreviousItemButton) as ButtonBase;

            if (this._previousItemButton != null)
            {
                this._previousItemButton.Click += new RoutedEventHandler(this.OnMoveToPreviousItemButtonClick);
            }

            if (this._nextItemButton != null)
            {
                this._nextItemButton.Click -= new RoutedEventHandler(this.OnMoveToNextItemButtonClick);
            }

            this._nextItemButton = GetTemplateChild(DATAFORM_elementNextItemButton) as ButtonBase;

            if (this._nextItemButton != null)
            {
                this._nextItemButton.Click += new RoutedEventHandler(this.OnMoveToNextItemButtonClick);
            }

            if (this._lastItemButton != null)
            {
                this._lastItemButton.Click -= new RoutedEventHandler(this.OnMoveToLastItemButtonClick);
            }

            this._lastItemButton = GetTemplateChild(DATAFORM_elementLastItemButton) as ButtonBase;

            if (this._lastItemButton != null)
            {
                this._lastItemButton.Click += new RoutedEventHandler(this.OnMoveToLastItemButtonClick);
            }

            this._buttonSeparator = GetTemplateChild(DATAFORM_elementButtonSeparator) as UIElement;

            if (this._buttonSeparator != null)
            {
                this.SetButtonSeparatorVisibility();
            }

            if (this._newItemButton != null)
            {
                this._newItemButton.Click -= new RoutedEventHandler(this.OnNewItemButtonClick);
            }

            this._newItemButton = GetTemplateChild(DATAFORM_elementNewItemButton) as ButtonBase;

            if (this._newItemButton != null)
            {
                this._newItemButton.Click += new RoutedEventHandler(this.OnNewItemButtonClick);

                this.SetNewItemButtonVisibility();
            }

            if (this._deleteItemButton != null)
            {
                this._deleteItemButton.Click -= new RoutedEventHandler(this.OnDeleteItemButtonClick);
            }

            this._deleteItemButton = GetTemplateChild(DATAFORM_elementDeleteItemButton) as ButtonBase;

            if (this._deleteItemButton != null)
            {
                this._deleteItemButton.Click += new RoutedEventHandler(this.OnDeleteItemButtonClick);

                this.SetDeleteItemButtonVisibility();
            }

            if (this._editButton != null)
            {
                this._editButton.Click -= new RoutedEventHandler(this.OnBeginEditButtonClick);
            }

            this._editButton = GetTemplateChild(DATAFORM_elementEditButton) as ButtonBase;

            if (this._editButton != null)
            {
                this._editButton.Click += new RoutedEventHandler(this.OnBeginEditButtonClick);

                this.SetEditButtonVisibility();
            }

            if (this._commitButton != null)
            {
                this._commitButton.Click -= new RoutedEventHandler(this.OnCommitEditButtonClick);
            }

            this._commitButton = GetTemplateChild(DATAFORM_elementCommitButton) as ButtonBase;

            if (this._commitButton != null)
            {
                this._commitButton.Click += new RoutedEventHandler(this.OnCommitEditButtonClick);

                if (this.CommitButtonContent != null)
                {
                    this._commitButton.Content = this.CommitButtonContent;
                }

                if (this.CommitButtonStyle != null)
                {
                    this._commitButton.SetBinding(ButtonBase.StyleProperty, new Binding("CommitButtonStyle") { Source = this });
                }

                this.SetCommitButtonVisibility();
            }

            if (this._cancelButton != null)
            {
                this._cancelButton.Click -= new RoutedEventHandler(this.OnCancelEditButtonClick);
            }

            this._cancelButton = GetTemplateChild(DATAFORM_elementCancelButton) as ButtonBase;

            if (this._cancelButton != null)
            {
                this._cancelButton.Click += new RoutedEventHandler(this.OnCancelEditButtonClick);

                if (this.CancelButtonContent != null)
                {
                    this._cancelButton.Content = this.CancelButtonContent;
                }

                if (this.CancelButtonStyle != null)
                {
                    this._cancelButton.SetBinding(ButtonBase.StyleProperty, new Binding("CancelButtonStyle") { Source = this });
                }

                this.SetCancelButtonVisibility();
            }

            this.UpdateCurrentItem();
            this.GenerateUI();
        }

        /// <summary>
        /// Validates the current item.
        /// </summary>
        /// <returns>Whether or not the current item is valid.</returns>
        public bool ValidateItem()
        {
            return this.ValidateItem(true /* validateAllProperties */);
        }

#endregion Public Methods

#region Internal Methods

        /// <summary>
        /// Cancels the appending of a new item.
        /// </summary>
        internal void CancelAppend()
        {
            if (this.EditableCollectionView != null)
            {
                this.IsAddingNew = false;
                this.EditableCollectionView.CancelNew();
                this._collectionView.MoveCurrentToPosition(this._lastCurrentPosition);
                SetAllCanPropertiesAndUpdate(this, false /* onlyUpdateStates */);
            }
        }

        /// <summary>
        /// Commits the appending of a new item.
        /// </summary>
        internal void CommitAppend()
        {
            if (this.EditableCollectionView != null)
            {
                this.IsAddingNew = false;

                try
                {
                    this._isCommitting = true;
                    this.EditableCollectionView.CommitNew();
                }
                finally
                {
                    this._isCommitting = false;
                }

                SetAllCanPropertiesAndUpdate(this, false /* onlyUpdateStates */);
            }
        }

        /// <summary>
        /// Forces the ending of the editing of the current item.
        /// </summary>
        internal void ForceEndEdit()
        {
            if (this.IsEditing)
            {
                this._forcedEndEdit = true;

                try
                {
                    if (!this.AutoCommitPreventsCurrentItemChange)
                    {
                        if (!this.CommitEdit(true /* exitEditingMode */))
                        {
                            this.CancelEdit();
                        }
                    }
                    else
                    {
                        this.CancelEdit();
                    }
                }
                finally
                {
                    this._forcedEndEdit = false;
                }
            }
        }

        /// <summary>
        /// Generates the UI for this control and clears all errors.
        /// </summary>
        internal void GenerateUI()
        {
            this.GenerateUI(true /* clearEntityErrors */, false /* swapOldAndNew */);
            this._templateBindingInfos = null;
        }

        /// <summary>
        /// Generates the UI for this control, clears field errors, and optionally clears entity errors.
        /// </summary>
        /// <param name="clearEntityErrors">Whether or not to clear entity errors.</param>
        internal void GenerateUI(bool clearEntityErrors)
        {
            this.GenerateUI(clearEntityErrors, false /* swapOldAndNew */);
        }

        /// <summary>
        /// Generates the UI for this control, clears field errors, and optionally clears entity errors.
        /// Also swaps the old and new content if the same mode/type pair has been detected.
        /// </summary>
        /// <param name="clearEntityErrors">Whether or not to clear entity errors.</param>
        /// <param name="swapOldAndNew">Whether to swap the old and new content.</param>
        internal void GenerateUI(bool clearEntityErrors, bool swapOldAndNew)
        {
            if (this._contentPresenter == null)
            {
                return;
            }

            this._fieldLevelErrors.Clear();

            if (clearEntityErrors)
            {
                this.ClearEntityErrors();
            }

            this.SetIsItemValid();
            this.UpdateButtonsAndStates();

            FrameworkElement contentRootElement = this._contentPresenter.Content as FrameworkElement;

            if (contentRootElement != null)
            {
                contentRootElement.BindingValidationError -= new EventHandler<ValidationErrorEventArgs>(this.OnContentRootElementBindingValidationError);
                contentRootElement.GotFocus -= new RoutedEventHandler(this.OnContentRootElementGotFocus);
                contentRootElement.LostFocus -= new RoutedEventHandler(this.OnContentRootElementLostFocus);
            }

            // If we have no current item, we have no current item type, so don't generate anything.
            // Alternatively, if no templates are set and AutoGenerateFields is false, then we have
            // nothing to generate, so don't attempt to generate anything there either.
            if (this.CurrentItem == null ||
                (this.ReadOnlyTemplate == null &&
                this.EditTemplate == null &&
                this.NewItemTemplate == null &&
                this.Content == null &&
                !this.AutoGenerateFields))
            {
                this._contentPresenter.Content = null;
                this.UpdateCurrentItem();
                return;
            }

            bool uiChanged;
            contentRootElement = this.GetOrRecycleContent(swapOldAndNew, out uiChanged);
            contentRootElement.BindingValidationError += new EventHandler<ValidationErrorEventArgs>(this.OnContentRootElementBindingValidationError);
            contentRootElement.GotFocus += new RoutedEventHandler(this.OnContentRootElementGotFocus);
            contentRootElement.LostFocus += new RoutedEventHandler(this.OnContentRootElementLostFocus);

            if (uiChanged)
            {
                if (!this._fieldsDictionary.ContainsKey(contentRootElement))
                {
                    this._fieldsDictionary.Add(contentRootElement, new List<DataField>());
                }

                this._fields = this._fieldsDictionary[contentRootElement];

                if (contentRootElement != null)
                {
                    contentRootElement.Loaded += new RoutedEventHandler(this.OnContentRootElementLoaded);
#if OPENSILVER
                    // Note: we need to set the DataContext to the right value before adding 'contentRootElement'
                    // to the ContentPresenter or the DataContext will be inherited from the DataForm and then
                    // propagated to the generated fields.
                    contentRootElement.DataContext = this.CurrentItem;
#endif
                }

                this._contentPresenter.Content = contentRootElement;
                this.UpdateCurrentItem();
                this.OnContentLoading(new DataFormContentLoadEventArgs(contentRootElement, this.Mode));
            }
            else
            {
                if (this._contentPresenter.Content == this.Content && this.Content != null)
                {
                    this.PrepareContent(this.Content);
                }
                this._fields = this._fieldsDictionary[contentRootElement];
            }
        }

        /// <summary>
        /// Gets the next tab index for generated controls.
        /// </summary>
        /// <returns>The next tab index.</returns>
        internal int GetNextTabIndex()
        {
            this._nextTabIndex++;
            return this._nextTabIndex;
        }

        /// <summary>
        /// Goes to the first item.
        /// </summary>
        internal void MoveToFirstItem()
        {
            if (this._collectionView != null && this.CanMoveToFirstItem)
            {
                this._collectionView.MoveCurrentToFirst();
            }
        }

        /// <summary>
        /// Goes to the last item.
        /// </summary>
        internal void MoveToLastItem()
        {
            if (this._collectionView != null && this.CanMoveToLastItem)
            {
                this._collectionView.MoveCurrentToLast();
            }
        }

        /// <summary>
        /// Goes to the next item.
        /// </summary>
        internal void MoveToNextItem()
        {
            if (this._collectionView != null && this.CanMoveToNextItem)
            {
                this._collectionView.MoveCurrentToNext();
            }
        }

        /// <summary>
        /// Goes to the previous item.
        /// </summary>
        internal void MoveToPreviousItem()
        {
            if (this._collectionView != null && this.CanMoveToPreviousItem)
            {
                this._collectionView.MoveCurrentToPrevious();
            }
        }

        /// <summary>
        /// Updates the enabled state of the buttons within the DataForm.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There is no need to make the method less complex.")]
        internal void UpdateButtons()
        {
            if (this._newItemButton != null)
            {
                this._newItemButton.IsEnabled = this.CanAddItems;
                this.SetNewItemButtonVisibility();
            }

            if (this._deleteItemButton != null)
            {
                this._deleteItemButton.IsEnabled = this.CanDeleteItems;
                this.SetDeleteItemButtonVisibility();
            }

            if (this._editButton != null)
            {
                this._editButton.IsEnabled = this.CanBeginEdit && this.Mode != DataFormMode.Edit;
                this.SetEditButtonVisibility();
            }

            if (this._cancelButton != null)
            {
                this._cancelButton.IsEnabled =
                    this.CanCancelEdit &&
                    (!this.AutoEdit ||
                    this._editedProperties.Count > 0 ||
                    this.CurrentItemNotifyPropertyChanged == null ||
                    this.IsAddingNew);
                this.SetCancelButtonVisibility();
            }

            if (this._commitButton != null)
            {
                this._commitButton.IsEnabled =
                    this.CanCommitEdit &&
                    (this._editedProperties.Count > 0 ||
                    this.CurrentItemNotifyPropertyChanged == null ||
                    this.IsAddingNew ||
                    (!this.CanCancelEdit && !this.AutoEdit));
                this.SetCommitButtonVisibility();
            }

            if (this._firstItemButton != null)
            {
                this._firstItemButton.IsEnabled = this.CanMoveToFirstItem;
                this.SetFirstItemButtonVisibility();
            }

            if (this._lastItemButton != null)
            {
                this._lastItemButton.IsEnabled = this.CanMoveToLastItem;
                this.SetLastItemButtonVisibility();
            }

            if (this._nextItemButton != null)
            {
                this._nextItemButton.IsEnabled = this.CanMoveToNextItem;
                this.SetNextItemButtonVisibility();
            }

            if (this._previousItemButton != null)
            {
                this._previousItemButton.IsEnabled = this.CanMoveToPreviousItem;
                this.SetPreviousItemButtonVisibility();
            }

            if (this._buttonSeparator != null)
            {
                this.SetButtonSeparatorVisibility();
            }

            this.SetHeaderVisibility();
        }

        /// <summary>
        /// Updates all sources.
        /// </summary>
        internal void UpdateAllSources()
        {
            if (this._contentPresenter != null)
            {
                FrameworkElement content = this._contentPresenter.Content as FrameworkElement;

                if (content != null)
                {
                    if (this._templateBindingInfos == null)
                    {
                        this._templateBindingInfos = content.GetDataFormBindingInfo(this._lastItem, true /* twoWayOnly */, true /* searchChildren */);

                        if (this._templateBindingInfos != null)
                        {
                            foreach (DataFormBindingInfo bindingInfo in this._templateBindingInfos)
                            {
                                bindingInfo.BindingExpression.UpdateSource();
                            }
                        }
                    }
                }
            }

            if (this._fields != null)
            {
                foreach (DataField field in this._fields)
                {
                    field.Validate();
                }
            }

            this.SetIsItemValid();
        }

        /// <summary>
        /// Finds any bindings on an element and updates the ones in which Mode is TwoWay
        /// to set the two Boolean properties to true.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void UpdateBindingsOnElement(FrameworkElement element)
        {
            this.UpdateBindingsOnElement(element, false /* isBeneathDataField */);
        }

        /// <summary>
        /// Finds any bindings on an element and updates the ones in which Mode is TwoWay
        /// to set the two Boolean properties to true.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="isBeneathDataField">Whether or not a DataField is a parent element.</param>
        internal void UpdateBindingsOnElement(FrameworkElement element, bool isBeneathDataField)
        {
            if (this.CurrentItem == null)
            {
                return;
            }

            DataField field = element as DataField;

            // DataFields will run themselves, so don't bother if we're looking at a DataField.
            if (field == null)
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

                            if (binding.Mode == BindingMode.TwoWay)
                            {
                                TextBox textBox = element as TextBox;

                                if (!isBeneathDataField)
                                {
                                    if (binding.Path != null && !String.IsNullOrEmpty(binding.Path.Path))
                                    {
                                        PropertyInfo propertyInfo = this.CurrentItemType.GetPropertyInfo(binding.Path.Path);

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

                                    if (textBox != null)
                                    {
                                        if (binding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
                                        {
                                            this._lostFocusFired[textBox] = true;

                                            textBox.LostFocus -= new RoutedEventHandler(this.OnExternalTextBoxLostFocus);
                                            textBox.LostFocus += new RoutedEventHandler(this.OnExternalTextBoxLostFocus);
                                            textBox.TextChanged -= new TextChangedEventHandler(this.OnExternalTextBoxTextChanged);
                                            textBox.TextChanged += new TextChangedEventHandler(this.OnExternalTextBoxTextChanged);
                                        }
                                        else
                                        {
                                            if (this._lostFocusFired.ContainsKey(textBox))
                                            {
                                                this._lostFocusFired.Remove(textBox);
                                            }

                                            textBox.LostFocus -= new RoutedEventHandler(this.OnExternalTextBoxLostFocus);
                                            textBox.TextChanged -= new TextChangedEventHandler(this.OnExternalTextBoxTextChanged);
                                        }
                                    }
                                }

                                if (textBox != null)
                                {
                                    if (!this._editingElements.Contains(textBox))
                                    {
                                        this._editingElements.Add(textBox);
                                    }
                                    textBox.TextChanged -= new TextChangedEventHandler(this.OnTextBoxTextChanged);
                                    textBox.TextChanged += new TextChangedEventHandler(this.OnTextBoxTextChanged);
                                }
                                else
                                {
                                    // 


                                    DatePicker datePicker = element as DatePicker;

                                    if (datePicker != null)
                                    {
                                        if (!this._editingElements.Contains(datePicker))
                                        {
                                            this._editingElements.Add(datePicker);
                                        }
                                        datePicker.CalendarOpened -= new RoutedEventHandler(this.OnDatePickerCalendarOpened);
                                        datePicker.CalendarOpened += new RoutedEventHandler(this.OnDatePickerCalendarOpened);
                                    }
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
                        this.UpdateBindingsOnElement(childElement, isBeneathDataField);
                    }
                }
            }
            else
            {
                if (field.Content != null)
                {
                    this.UpdateBindingsOnElement(field.Content, true /* isBeneathDataField */);
                }
            }
        }

        /// <summary>
        /// Updates the enabled state of the buttons and the visual states within the DataForm.
        /// </summary>
        internal void UpdateButtonsAndStates()
        {
            this.UpdateButtons();
            this.UpdateStates();
        }

        /// <summary>
        /// Updates the visual states within the DataForm.
        /// </summary>
        internal void UpdateStates()
        {
            if (!this.IsEnabled)
            {
                VisualStateManager.GoToState(this, DATAFORM_stateDisabled, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateNormal, true);
            }

            if (this.Mode == DataFormMode.AddNew)
            {
                if (!VisualStateManager.GoToState(this, DATAFORM_stateAddNew, true))
                {
                    if (!VisualStateManager.GoToState(this, DATAFORM_stateEdit, true))
                    {
                        VisualStateManager.GoToState(this, DATAFORM_stateReadOnly, true);
                    }
                }
            }
            else if (this.Mode == DataFormMode.Edit)
            {
                if (!VisualStateManager.GoToState(this, DATAFORM_stateEdit, true))
                {
                    VisualStateManager.GoToState(this, DATAFORM_stateReadOnly, true);
                }
            }
            else if (this.IsEmpty)
            {
                if (!VisualStateManager.GoToState(this, DATAFORM_stateEmpty, true))
                {
                    VisualStateManager.GoToState(this, DATAFORM_stateReadOnly, true);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateReadOnly, true);
            }

            if (this.IsItemValid)
            {
                VisualStateManager.GoToState(this, DATAFORM_stateValid, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateInvalid, true);
            }

            if (this.IsItemChanged)
            {
                VisualStateManager.GoToState(this, DATAFORM_stateUncommitted, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateCommitted, true);
            }

            if (this.ItemsSource == null)
            {
                VisualStateManager.GoToState(this, DATAFORM_stateEntity, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateCollection, true);
            }
        }

#endregion Internal Methods

#region Protected Methods

        /// <summary>
        /// Raises the AddingItem event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnAddingNewItem(DataFormAddingNewItemEventArgs e)
        {
            EventHandler<DataFormAddingNewItemEventArgs> handler = this.AddingNewItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the AutoGeneratingField event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnAutoGeneratingField(DataFormAutoGeneratingFieldEventArgs e)
        {
            EventHandler<DataFormAutoGeneratingFieldEventArgs> handler = this.AutoGeneratingField;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the BeginningEdit event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnBeginningEdit(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.BeginningEdit;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the ContentLoaded event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnContentLoaded(DataFormContentLoadEventArgs e)
        {
            EventHandler<DataFormContentLoadEventArgs> handler = this.ContentLoaded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the ContentLoading event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnContentLoading(DataFormContentLoadEventArgs e)
        {
            EventHandler<DataFormContentLoadEventArgs> handler = this.ContentLoading;
            if (handler != null)
            {
                handler(this, e);
            }
        }

#if OPENSILVER
        /// <summary>
        /// Returns an automation peer for this DataForm.
        /// </summary>
        /// <returns>The automation peer.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataFormAutomationPeer(this);
        }
#endif

        /// <summary>
        /// Raises the CurrentItemChanged event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnCurrentItemChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.CurrentItemChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the DeletingItem event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnDeletingItem(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.DeletingItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the ItemEditEnded event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnItemEditEnded(DataFormEditEndedEventArgs e)
        {
            EventHandler<DataFormEditEndedEventArgs> handler = this.EditEnded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the ItemEditEnding event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnItemEditEnding(DataFormEditEndingEventArgs e)
        {
            EventHandler<DataFormEditEndingEventArgs> handler = this.EditEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the ValidatingItem event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnValidatingItem(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.ValidatingItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

#endregion Protected Methods

#region Private Static Methods

        /// <summary>
        /// Creates a collection view around the DataForm's source. ICollectionViewFactory is
        /// used if the source implements it. Otherwise a PagedCollectionView is returned.
        /// </summary>
        /// <param name="source">Enumerable source for which to create a view</param>
        /// <returns>ICollectionView view over the provided source</returns>
        private static ICollectionView CreateView(IEnumerable source)
        {
            Debug.Assert(source != null, "source unexpectedly null");
            Debug.Assert(!(source is ICollectionView), "source is an ICollectionView");

            ICollectionView collectionView = null;

            ICollectionViewFactory collectionViewFactory = source as ICollectionViewFactory;
            if (collectionViewFactory != null)
            {
                // If the source is a collection view factory, give it a chance to produce a custom collection view.
                collectionView = collectionViewFactory.CreateView();
                // Intentionally not catching potential exception thrown by ICollectionViewFactory.CreateView().
            }
            if (collectionView == null)
            {
                // If we still do not have a collection view, default to a PagedCollectionView.
                collectionView = new PagedCollectionView(source);
            }
            return collectionView;
        }

        /// <summary>
        /// Returns a DependencyProperty for a type corresponding to the property to which
        /// a binding should be applied.
        /// </summary>
        /// <param name="type">The type for which to generate the DependencyProperty.</param>
        /// <returns>The DependencyProperty.</returns>
        private static DependencyProperty GetBindingPropertyFromType(Type type)
        {
            Debug.Assert(type != null, "The type must not be null.");

            if (type == typeof(bool) || type == typeof(bool?))
            {
                return CheckBox.IsCheckedProperty;
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return DatePicker.SelectedDateProperty;
            }
            else if (type.IsEnum)
            {
                return ComboBox.SelectedItemProperty;
            }
            else
            {
                return TextBox.TextProperty;
            }
        }

        /// <summary>
        /// Returns a control for a type.
        /// </summary>
        /// <param name="type">The type for which to generate the control.</param>
        /// <returns>The control.</returns>
        private static Control GetControlFromType(Type type)
        {
            Debug.Assert(type != null, "The type must not be null.");

            if (type == typeof(bool))
            {
                return new CheckBox();
            }
            else if (type == typeof(bool?))
            {
                CheckBox checkBox = new CheckBox();
                checkBox.IsThreeState = true;
                return checkBox;
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DatePicker();
            }
            else if (type.IsEnum)
            {
                ComboBox comboBox = new ComboBox();

                FieldInfo[] valueFieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                List<string> valueList = new List<string>();

                foreach (FieldInfo valueFieldInfo in valueFieldInfos)
                {
                    Enum value = valueFieldInfo.GetValue(null) as Enum;

                    if (value != null)
                    {
                        valueList.Add(value.ToString());
                    }
                }

                comboBox.ItemsSource = valueList;
                return comboBox;
            }
            else
            {
                return new TextBox();
            }
        }

        /// <summary>
        /// Gets the first focusable control (with respect to TabIndex).
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The first focusable control.</returns>
        private static Control GetFirstFocusableControl(DependencyObject dependencyObject)
        {
            int lowestTabIndex = int.MaxValue;
            Control controlWithLowestTabIndex = null;

            if (dependencyObject == null)
            {
                return controlWithLowestTabIndex;
            }

            Stack<DependencyObject> dependencyObjectStack = new Stack<DependencyObject>();
            dependencyObjectStack.Push(dependencyObject);

            while (dependencyObjectStack.Count > 0 && lowestTabIndex > 0)
            {
                DependencyObject currentDependencyObject = dependencyObjectStack.Pop();
                Control control = currentDependencyObject as Control;

                if (control != null && control.TabIndex < lowestTabIndex && IsKeyboardFocusable(control))
                {
                    lowestTabIndex = control.TabIndex;
                    controlWithLowestTabIndex = control;
                }
                else
                {
                    int numChildren = VisualTreeHelper.GetChildrenCount(currentDependencyObject);

                    for (int i = 0; i < numChildren; i++)
                    {
                        DependencyObject childDependencyObject = VisualTreeHelper.GetChild(currentDependencyObject, i);
                        dependencyObjectStack.Push(childDependencyObject);
                    }
                }
            }

            return controlWithLowestTabIndex;
        }

        /// <summary>
        /// Returns whether or not a framework element is keyboard focusable.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Whether or not the element is keyboard focusable.</returns>
        private static bool IsKeyboardFocusable(FrameworkElement element)
        {
            if (element == null)
            {
                return false;
            }

#if OPENSILVER
            FrameworkElementAutomationPeer elementPeer = FrameworkElementAutomationPeer.CreatePeerForElement(element) as FrameworkElementAutomationPeer;

            if (elementPeer != null)
            {
                return elementPeer.IsKeyboardFocusable();
            }
#endif

            return false;
        }

        /// <summary>
        /// Sets focus to the first focusable control (with respect to TabIndex) within the hierarchy of a given dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        private static void SetFocusToFirstFocusableControl(DependencyObject dependencyObject)
        {
            Control controlWithLowestTabIndex = GetFirstFocusableControl(dependencyObject);

            if (controlWithLowestTabIndex != null && !(controlWithLowestTabIndex is DataField))
            {
                controlWithLowestTabIndex.Focus();
            }
        }

        /// <summary>
        /// Sorts the generated field paths in GenerateFields() according to the order specified
        /// in the DisplayAttributes.
        /// </summary>
        /// <param name="generatedFieldPaths">The generated field paths.</param>
        /// <param name="orders">The orders.</param>
        /// <returns>The sorted list of generated field paths.</returns>
        private static IList<string> SortGeneratedFieldPaths(IList<string> generatedFieldPaths, IDictionary<string, int> orders)
        {
            IList<PathOrderPair> pathOrderPairList = new List<PathOrderPair>();

            foreach (string path in generatedFieldPaths)
            {
                PathOrderPair pathOrderPair = new PathOrderPair();
                pathOrderPair.Path = path;
                pathOrderPair.Order = orders[path];
                pathOrderPairList.Add(pathOrderPair);
            }

            pathOrderPairList = new List<PathOrderPair>(pathOrderPairList.OrderBy<PathOrderPair, int>(PathOrderPair.GetOrder));

            IList<string> sortedGeneratedFieldPaths = new List<string>();

            foreach (PathOrderPair pathOrderPair in pathOrderPairList)
            {
                sortedGeneratedFieldPaths.Add(pathOrderPair.Path);
            }

            return sortedGeneratedFieldPaths;
        }

        /// <summary>
        /// Calls SetAllCanProperties() and either UpdateStates() or UpdateButtonsAndStates()
        /// on the given DataForm.
        /// </summary>
        /// <param name="dataForm">The DataForm.</param>
        /// <param name="onlyUpdateStates">Whether to call UpdateStates() or UpdateButtonsAndStates()</param>
        private static void SetAllCanPropertiesAndUpdate(DataForm dataForm, bool onlyUpdateStates)
        {
            if (dataForm != null)
            {
                dataForm.SetAllCanProperties();

                if (onlyUpdateStates)
                {
                    dataForm.UpdateStates();
                }
                else
                {
                    dataForm.UpdateButtonsAndStates();
                }
            }
        }

#endregion Private Static Methods

#region Private Methods

        /// <summary>
        /// Begins the editing of the current item.
        /// </summary>
        /// <param name="startingNewEdit">Whether or not this is starting a new edit or continuing an existing edit.</param>
        /// <returns>Whether or not editing was begun.</returns>
        private bool BeginEdit(bool startingNewEdit)
        {
            if (!this.CanBeginEdit && startingNewEdit)
            {
                return false;
            }

            CancelEventArgs e = new CancelEventArgs();
            this.OnBeginningEdit(e);

            if (e.Cancel)
            {
                if (!this.AutoEdit)
                {
                    return false;
                }
                else
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, resources.DataForm_CannotCancelBeginEditWhenInAutoEdit, "BeginEdit", "AutoEdit"));
                }
            }

            if (this.EditableCollectionView != null)
            {
                if (startingNewEdit)
                {
                    this._canAddItemsAfterEdit = this.CanAddItems;
                    this._canDeleteItemsAfterEdit = this.CanDeleteItems;
                }

                // Don't call EditableCollectionView.EditItem() if the current item is already being edited.
                if (!this.EditableCollectionView.IsEditingItem || this.EditableCollectionView.CurrentEditItem != this.CurrentItem)
                {
                    this.EditableCollectionView.EditItem(this.CurrentItem);
                }

                this.IsEditing = this.EditableCollectionView.IsEditingItem;
            }
            else
            {
                IEditableObject ieo = this.CurrentItem as IEditableObject;

                if (ieo != null)
                {
                    ieo.BeginEdit();
                }

                this.IsEditing = true;
            }

            if (this.IsEditing)
            {
                this.GetAllOriginalPropertyValues();
                this.UpdateButtonsAndStates();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if a given value matches a property's original value,
        /// and updates the ability to commit the edit if not.
        /// </summary>
        /// <param name="propertyName">The name of the property to check against.</param>
        /// <param name="value">The value to check against.</param>
        /// <param name="doConversion">Whether or not to convert the value to the original type.</param>
        /// <param name="originalType">The original type.</param>
        /// <param name="valueConverter">The IValueConverter to use for conversion.</param>
        /// <param name="converterParameter">The converter parameter.</param>
        /// <param name="converterCulture">The converter culture.</param>
        private void CheckIfPropertyEditedAndUpdate(
            string propertyName,
            object value,
            bool doConversion,
            Type originalType,
            IValueConverter valueConverter,
            object converterParameter,
            CultureInfo converterCulture)
        {
            object originalValue;

            if (!this._editablePropertiesOriginalValues.TryGetValue(propertyName, out originalValue))
            {
                return;
            }

            if (doConversion && value != null)
            {
                // Attempt to convert the value provided to the type of the original type.
                // If the conversion fails, just keep it as is.
                try
                {
                    if (valueConverter != null)
                    {
#if MIGRATION
                        value = valueConverter.ConvertBack(value, originalType, converterParameter, converterCulture);
#else
                        value = valueConverter.ConvertBack(value, originalType, converterParameter, converterCulture.Name);
#endif
                    }
                    else
                    {
                        value = Convert.ChangeType(value, originalType, converterCulture);
                    }
                }
                catch (InvalidCastException)
                {
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }

            bool valueEqualsOriginal =
                (value == null && originalValue == null) ||
                (value != null && originalValue != null && value.Equals(originalValue));

            if (valueEqualsOriginal &&
                this._editedProperties.Contains(propertyName))
            {
                this._editedProperties.Remove(propertyName);
                SetAllCanPropertiesAndUpdate(this, false /* onlyUpdateStates */);
            }
            else if (!valueEqualsOriginal &&
                !this._editedProperties.Contains(propertyName))
            {
                this._editedProperties.Add(propertyName);
                SetAllCanPropertiesAndUpdate(this, false /* onlyUpdateStates */);
            }
        }

        /// <summary>
        /// Clears all EntityErrors from this DataForm before ValidateForm is performed.
        /// </summary>
        private void ClearEntityErrors()
        {
            if (this.ValidationSummary != null)
            {
                Debug.Assert(this.ValidationSummary.Errors != null, "Unexpected null ValidationSummary.Errors collection");
                Debug.Assert(this._entityLevelErrors != null, "Unexpected null value for _entityLevelErrors");

                foreach (ValidationSummaryItem esi in this._entityLevelErrors)
                {
                    if (this.ValidationSummary.Errors.Contains(esi))
                    {
                        this.ValidationSummary.Errors.Remove(esi);
                    }
                }
            }

            this._entityLevelErrors.Clear();
        }

        /// <summary>
        /// Detaches element events that were used during editing.
        /// </summary>
        private void DetachEditingEvents()
        {
            foreach (FrameworkElement editingElement in this._editingElements)
            {
                TextBox textBox = editingElement as TextBox;
                if (textBox != null)
                {
                    textBox.TextChanged -= new TextChangedEventHandler(this.OnTextBoxTextChanged);
                    textBox.TextChanged -= new TextChangedEventHandler(this.OnExternalTextBoxTextChanged);
                    textBox.LostFocus -= new RoutedEventHandler(this.OnExternalTextBoxLostFocus);
                }
                else
                {
                    DatePicker datePicker = editingElement as DatePicker;
                    if (datePicker != null)
                    {
                        datePicker.CalendarOpened -= new RoutedEventHandler(this.OnDatePickerCalendarOpened);
                    }
                }
            }
            this._editingElements.Clear();
        }

        /// <summary>
        /// Moves out of edit mode.
        /// </summary>
        private void EndEdit()
        {
            try
            {
                this._endingEdit = true;
                this._canAddItemsAfterEdit = false;
                this._canDeleteItemsAfterEdit = false;
                this.IsEditing = false;
            }
            finally
            {
                this._endingEdit = false;
            }
        }

        /// <summary>
        /// Returns whether or not a new error should be added to the list of errors.
        /// </summary>
        /// <param name="newError">The new error.</param>
        /// <returns>Whether or not the new error should be added to the list of errors.</returns>
        private bool EntityErrorShouldBeAdded(ValidationSummaryItem newError)
        {
            if (this.ValidationSummary != null)
            {
                foreach (ValidationSummaryItem item in this.ValidationSummary.Errors)
                {
                    if (item.ItemType == ValidationSummaryItemType.PropertyError &&
                        item.Sources != null)
                    {
                        foreach (ValidationSummaryItemSource source in item.Sources)
                        {
                            foreach (ValidationSummaryItemSource newErrorSource in newError.Sources)
                            {
                                if (newErrorSource.Control == source.Control)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Generates the DataFormFields from the properties that should participate in auto-generation.
        /// </summary>
        /// <returns>The DataFormFields in a containing element.</returns>
        private FrameworkElement GenerateFields()
        {
            if (!this.AutoGenerateFields || this.CurrentItemType == null || this._contentPresenter == null)
            {
                return null;
            }

            StackPanel stackPanel = new StackPanel();
            this.ResetNextTabIndex();

            // If it's a primitive type, generate a binding to the current item itself
            // instead of a binding to its properties.
            if (TypeHelper.TypeIsPrimitive(this.CurrentItemType))
            {
                this.GenerateField(this.CurrentItemType, string.Empty, BindingMode.TwoWay, stackPanel);
                return stackPanel;
            }

            IList<string> generatedFieldPaths;
            IDictionary<string, BindingMode> generatedBindingModes;

            if (this.GeneratedPropertiesByType.TryGetValue(this.CurrentItemType, out generatedFieldPaths) == false)
            {
                PropertyInfo[] propertyInfos = this.CurrentItemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfos == null)
                {
                    return null;
                }

                IList<string> generatedFieldPathsNoGroup = new List<string>();
                IDictionary<string, BindingMode> bindingModes = new Dictionary<string, BindingMode>();
                IDictionary<string, int> orders = new Dictionary<string, int>();

                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (propertyInfo.GetIndexParameters().Length > 0)
                    {
                        // Don't generate anything for indexed properties.
                        continue;
                    }

                    DisplayAttribute displayAttribute =
                        propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true /* inherit */)
                        .Cast<DisplayAttribute>()
                        .FirstOrDefault();

                    bool propertyGenerated = false;

                    if (displayAttribute == null ||
                        !displayAttribute.GetAutoGenerateField().HasValue ||
                        displayAttribute.GetAutoGenerateField().Value == true)
                    {
                        if (propertyInfo.GetSetMethod() != null && propertyInfo.PropertyType.GetNonNullableType().IsEditable())
                        {
                            bindingModes.Add(propertyInfo.Name, BindingMode.TwoWay);
                        }
                        else
                        {
                            bindingModes.Add(propertyInfo.Name, BindingMode.OneWay);
                        }

                        propertyGenerated = true;
                    }

                    if (propertyGenerated)
                    {
                        if (displayAttribute != null)
                        {
                            generatedFieldPathsNoGroup.Add(propertyInfo.Name);
                            orders.Add(propertyInfo.Name, displayAttribute.GetOrder() ?? UnspecifiedOrder);
                        }
                        else
                        {
                            generatedFieldPathsNoGroup.Add(propertyInfo.Name);
                            orders.Add(propertyInfo.Name, UnspecifiedOrder);
                        }
                    }
                }

                generatedFieldPathsNoGroup = SortGeneratedFieldPaths(generatedFieldPathsNoGroup, orders);
                generatedFieldPaths = generatedFieldPathsNoGroup;
                generatedBindingModes = bindingModes;

                this.GeneratedPropertiesByType.Add(this.CurrentItemType, generatedFieldPathsNoGroup);
                this.GeneratedBindingModesByType.Add(this.CurrentItemType, bindingModes);
            }
            else
            {
                generatedBindingModes = this.GeneratedBindingModesByType[this.CurrentItemType];
            }

            foreach (string generatedFieldPath in generatedFieldPaths)
            {
                PropertyInfo propertyInfo = this.CurrentItemType.GetProperty(generatedFieldPath);
                Debug.Assert(propertyInfo != null, "Property info should never be null.");
                this.GenerateField(propertyInfo.PropertyType, propertyInfo.Name, generatedBindingModes[generatedFieldPath], stackPanel);
            }

            return stackPanel;
        }

        /// <summary>
        /// Generates a field based on a property type and a property name.
        /// </summary>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="bindingMode">The binding mode.</param>
        /// <param name="panel">The panel to insert the field into.</param>
        /// <returns>A value indicating whether or not the field was generated.</returns>
        private bool GenerateField(Type propertyType, string propertyName, BindingMode bindingMode, Panel panel)
        {
            // Create a new DataField for the property.
            DataField newField = new DataField();
            Control control = GetControlFromType(propertyType);
            DependencyProperty dependencyProperty = GetBindingPropertyFromType(propertyType);

            control.TabIndex = this.GetNextTabIndex();
            newField.Content = control;

            if (bindingMode == BindingMode.TwoWay && !string.IsNullOrEmpty(propertyName))
            {
                control.SetBinding(
                    dependencyProperty,
                    new Binding(propertyName)
                    {
                        Mode = BindingMode.TwoWay,
                        ValidatesOnExceptions = true,
                        NotifyOnValidationError = true,
                        Converter = propertyType.IsEnum ? new DataFormToStringConverter() : null
                    });
            }
            else
            {
                control.SetBinding(
                    dependencyProperty,
                    new Binding(propertyName)
                    {
                        Mode = BindingMode.OneWay,
                        Converter = propertyType.IsEnum ? new DataFormToStringConverter() : null
                    });

                newField.IsReadOnly = true;
            }

            // If there's no path, coerce the field to read-only mode and give it
            // a label of "value" (since otherwise it would be blank).
            if (string.IsNullOrEmpty(propertyName))
            {
                newField.IsReadOnly = true;
                newField.Label = propertyType.Name;
            }

            // Raise the AutoGeneratingField event in case the user wants to cancel or replace the
            // field being generated.
            DataFormAutoGeneratingFieldEventArgs e = new DataFormAutoGeneratingFieldEventArgs(propertyName, propertyType, newField);
            this.OnAutoGeneratingField(e);

            if (e.Cancel)
            {
                return false;
            }
            else
            {
                panel.Children.Add(e.Field);
                return true;
            }
        }

        /// <summary>
        /// Gets all of the original property values for the properties on the current item.
        /// </summary>
        private void GetAllOriginalPropertyValues()
        {
            this._editablePropertiesOriginalValues.Clear();

            // Get all of the un-indexed properties on the current item with a setter -
            // these are the properties that could be edited.
            IEnumerable<PropertyInfo> propertyInfos =
                this.CurrentItem.GetType().GetProperties()
                .Where(propertyInfo => propertyInfo.GetSetMethod() != null && propertyInfo.GetIndexParameters().Length == 0);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                this._editablePropertiesOriginalValues.Add(propertyInfo.Name, propertyInfo.GetValue(this.CurrentItem, null));
            }
        }

        /// <summary>
        /// Gets new content from the current mode.
        /// </summary>
        /// <returns>The new content.</returns>
        private FrameworkElement GetContentFromMode()
        {
            if (this.Mode == DataFormMode.ReadOnly)
            {
                FrameworkElement templateContent = null;

                if (this.ReadOnlyTemplate != null && (templateContent = this.GetReadOnlyTemplateContent()) != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = DataFormMode.ReadOnly;
                    return templateContent;
                }
                else if (this.EditTemplate != null && (templateContent = this.GetEditTemplateContent()) != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = DataFormMode.Edit;
                    return templateContent;
                }
                else if (this.NewItemTemplate != null && (templateContent = this.GetNewItemTemplateContent()) != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = DataFormMode.AddNew;
                    return templateContent;
                }
                else if (this.Content != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = null;
                    return this.Content;
                }
                else if (this.AutoGenerateFields)
                {
                    return this.GenerateFields();
                }
            }
            else if (this.Mode == DataFormMode.Edit)
            {
                FrameworkElement templateContent = null;

                if (this.EditTemplate != null && (templateContent = this.GetEditTemplateContent()) != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = DataFormMode.Edit;
                    return templateContent;
                }
                else if (this.NewItemTemplate != null && (templateContent = this.GetNewItemTemplateContent()) != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = DataFormMode.AddNew;
                    return templateContent;
                }
                else if (this.Content != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = null;
                    return this.Content;
                }
                else if (this.AutoGenerateFields)
                {
                    return this.GenerateFields();
                }
            }
            else
            {
                FrameworkElement templateContent = null;

                if (this.NewItemTemplate != null && (templateContent = this.GetNewItemTemplateContent()) != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = DataFormMode.AddNew;
                    return templateContent;
                }
                else if (this.EditTemplate != null && (templateContent = this.GetEditTemplateContent()) != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = DataFormMode.Edit;
                    return templateContent;
                }
                else if (this.Content != null)
                {
                    this._currentlyUsedTemplate[this.CurrentItemType] = null;
                    return this.Content;
                }
                else if (this.AutoGenerateFields)
                {
                    return this.GenerateFields();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the generated content from the edit template.
        /// </summary>
        /// <returns>The edit template content.</returns>
        private FrameworkElement GetEditTemplateContent()
        {
            if (this.EditTemplate != null)
            {
                return this.EditTemplate.LoadContent() as FrameworkElement;
            }

            return null;
        }

        /// <summary>
        /// Gets the generated content from the new item template.
        /// </summary>
        /// <returns>The new item template content.</returns>
        private FrameworkElement GetNewItemTemplateContent()
        {
            if (this.NewItemTemplate != null)
            {
                return this.NewItemTemplate.LoadContent() as FrameworkElement;
            }

            return null;
        }

        /// <summary>
        /// Either gets new content or recycles old content based on the current mode.
        /// </summary>
        /// <param name="swapOldAndNew">Whether or not we should swap old and new content when the mode/type pair is unchanged.</param>
        /// <param name="uiChanged">Whether or not the UI changed from its current content.</param>
        /// <returns>The content.</returns>
        private FrameworkElement GetOrRecycleContent(bool swapOldAndNew, out bool uiChanged)
        {
            IDictionary<Type, FrameworkElement> newContents = null;
            IDictionary<Type, FrameworkElement> oldContents = null;

            if (this.Mode == DataFormMode.ReadOnly)
            {
                newContents = this._readOnlyContentsNew;
                oldContents = this._readOnlyContentsOld;
            }
            else if (this.Mode == DataFormMode.Edit)
            {
                newContents = this._editContentsNew;
                oldContents = this._editContentsOld;
            }
            else
            {
                newContents = this._addNewContentsNew;
                oldContents = this._addNewContentsOld;
            }

            FrameworkElement contentRootElement = null;
            uiChanged = true;

            if (!newContents.ContainsKey(this.CurrentItemType))
            {
                contentRootElement = this.GetContentFromMode();
                if (contentRootElement == this.Content)
                {
                    uiChanged = (contentRootElement != this._contentPresenter.Content);
                }
                else
                {
                    newContents.Add(this.CurrentItemType, contentRootElement);
                }
            }
            else
            {
                contentRootElement = newContents[this.CurrentItemType];

                if (newContents[this.CurrentItemType] == this._contentPresenter.Content)
                {
                    if (swapOldAndNew)
                    {
                        FrameworkElement content;

                        if (oldContents.TryGetValue(this.CurrentItemType, out content) == false)
                        {
                            content = this.GetContentFromMode();
                        }

                        oldContents[this.CurrentItemType] = contentRootElement;
                        contentRootElement = content;
                        newContents[this.CurrentItemType] = contentRootElement;
                    }
                    else
                    {
                        uiChanged = false;
                    }
                }
            }

            return contentRootElement;
        }

        /// <summary>
        /// Gets the generated content from the read-only template.
        /// </summary>
        /// <returns>The read-only template content.</returns>
        private FrameworkElement GetReadOnlyTemplateContent()
        {
            if (this.ReadOnlyTemplate != null)
            {
                return this.ReadOnlyTemplate.LoadContent() as FrameworkElement;
            }

            return null;
        }

        /// <summary>
        /// Returns whether or not the specified type of command button is visible.
        /// </summary>
        /// <param name="commandButtonVisibility">The type of command button.</param>
        /// <returns>Whether or not the type of command button is visible.</returns>
        private bool IsCommandButtonVisible(DataFormCommandButtonsVisibility commandButtonVisibility)
        {
            if (this.CommandButtonsVisibility.HasValue)
            {
                return (this.CommandButtonsVisibility.Value & commandButtonVisibility) == commandButtonVisibility;
            }

            return false;
        }

        /// <summary>
        /// Handles the situation where the collection changed on the ICollectionView.
        /// </summary>
        /// <param name="sender">The ICollectionView.</param>
        /// <param name="e">The event args.</param>
        private void OnCollectionViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateCurrentItem();
            this.SetAllCanProperties();
        }

        /// <summary>
        /// Handles the situation where the current item changed on the ICollectionView.
        /// </summary>
        /// <param name="sender">The ICollectionView.</param>
        /// <param name="e">The event args.</param>
        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            this.UpdateCurrentItem();
            this.SetAllCanProperties();
        }

        /// <summary>
        /// Handles the situation where the current item is changing on the ICollectionView.
        /// </summary>
        /// <param name="sender">The ICollectionView.</param>
        /// <param name="e">The event args.</param>
        private void OnCollectionViewCurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (this.ShouldValidateOnCurrencyChange)
            {
                this.ValidateItem();
            }

            if (e.IsCancelable && (this.AutoCommitPreventsCurrentItemChange || !this.IsItemValid))
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles the situation where a property on the current item changes.
        /// </summary>
        /// <param name="sender">The current item.</param>
        /// <param name="e">The event args.</param>
        private void OnCurrentItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.CurrentItemChangeTracking != null && e.PropertyName == "IsChanged")
            {
                this.IsItemChanged = this.CurrentItemChangeTracking.IsChanged;
            }
            else if (this._editablePropertiesOriginalValues.ContainsKey(e.PropertyName))
            {
                PropertyInfo propertyInfo = this.CurrentItem.GetType().GetProperty(e.PropertyName);
                object value = propertyInfo.GetValue(this.CurrentItem, null);
                this.CheckIfPropertyEditedAndUpdate(
                    e.PropertyName,
                    value,
                    false /* doConversion */,
                    propertyInfo.PropertyType,
                    null,
                    null,
                    CultureInfo.CurrentCulture);

                bool needToValidateItem = false;

                foreach (ValidationSummaryItem item in this._entityLevelErrors)
                {
                    if (item.Sources.Count == 0)
                    {
                        needToValidateItem = true;
                        break;
                    }
                    else
                    {
                        foreach (ValidationSummaryItemSource source in item.Sources)
                        {
                            if (source.PropertyName != null &&
                                source.PropertyName.Equals(e.PropertyName))
                            {
                                needToValidateItem = true;
                                break;
                            }
                        }
                    }
                }

                if (needToValidateItem)
                {
                    this.ValidateItem(false /* validateAllProperties */);
                }
            }
        }

        /// <summary>
        /// Prepares the given content.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private void PrepareContent(FrameworkElement rootElement)
        {
            if (this._shouldMoveFocus)
            {
                this._shouldMoveFocus = false;
                SetFocusToFirstFocusableControl(rootElement);
            }

            this._lostFocusFired.Clear();
            this.DetachEditingEvents();

            if (this.Mode != DataFormMode.ReadOnly)
            {
                this.UpdateBindingsOnElement(rootElement);
            }
        }
        
        /// <summary>
        /// Re-generates the UI if a template or direct content change requires it.
        /// </summary>
        /// <param name="changedTemplateMode">The mode for which template was changed, or null if the content was changed.</param>
        /// <param name="oldContent">The old Content value, or null in the case of a template change.</param>
        private void RegenerateUI(DataFormMode? changedTemplateMode, object oldContent)
        {
            if (this.CurrentItemType != null &&
                (this._currentlyUsedTemplate.Count == 0 || (this._currentlyUsedTemplate.ContainsKey(this.CurrentItemType) && this.ShouldRegenerateUI(changedTemplateMode))))
            {
                if (changedTemplateMode == null && this._contentPresenter.Content == oldContent && oldContent != null)
                {
                    this.RemoveContent(oldContent as FrameworkElement);
                }
                else if (this.Mode == DataFormMode.ReadOnly)
                {
                    this.RemoveContentForType(this.CurrentItemType, this._readOnlyContentsNew);
                    this.RemoveContentForType(this.CurrentItemType, this._readOnlyContentsOld);
                }
                else if (this.Mode == DataFormMode.Edit)
                {
                    this.RemoveContentForType(this.CurrentItemType, this._editContentsNew);
                    this.RemoveContentForType(this.CurrentItemType, this._editContentsOld);
                }
                else
                {
                    this.RemoveContentForType(this.CurrentItemType, this._addNewContentsNew);
                    this.RemoveContentForType(this.CurrentItemType, this._addNewContentsOld);
                }

                this.GenerateUI(false /* clearEntityErrors */);
            }

            SetAllCanPropertiesAndUpdate(this, true /* onlyUpdateStates */);
        }

        /// <summary>
        /// Removes the content attached to a given type from the fields dictionary
        /// and from the given dictionary of contents.
        /// </summary>
        /// <param name="type">The type to use as a key.</param>
        /// <param name="contents">The contents dictionary.</param>
        private void RemoveContentForType(Type type, IDictionary<Type, FrameworkElement> contents)
        {
            if (type == null || contents == null)
            {
                return;
            }

            FrameworkElement content;

            if (contents.TryGetValue(type, out content))
            {
                this.RemoveContent(content);
                contents.Remove(type);
            }
        }

        /// <summary>
        /// Removes the specified content from the fields dictionary.
        /// </summary>
        /// <param name="content">The content to remove.</param>
        private void RemoveContent(FrameworkElement content)
        {
            IList<DataField> fields;
            if (this._fieldsDictionary.TryGetValue(content, out fields))
            {
                for (int i = fields.Count - 1; i >= 0; i--)
                {
                    fields[i].DetachFieldFromLayoutPanel();
                    fields[i].RemoveBindingsFromParentDataForm();

                    if (this.Fields.Contains(fields[i]))
                    {
                        this.Fields.Remove(fields[i]);
                    }
                }

                this._fieldsDictionary[content].Clear();
                this._fieldsDictionary.Remove(content);
            }
        }

        /// <summary>
        /// Resets the next tab index.
        /// </summary>
        private void ResetNextTabIndex()
        {
            this._nextTabIndex = -1;
        }

        /// <summary>
        /// Sets all of the properties of the form "CanXXXX".
        /// </summary>
        private void SetAllCanProperties()
        {
            this.SetCanAddItems();
            this.SetCanBeginEdit();
            this.SetCanCancelEdit();
            this.SetCanDeleteItems();
            this.SetCanCommitEdit();
            this.SetCanMoveToFirstItem();
            this.SetCanMoveToLastItem();
            this.SetCanMoveToNextItem();
            this.SetCanMoveToPreviousItem();
        }

        /// <summary>
        /// Sets the visibility of the button separator.
        /// </summary>
        private void SetButtonSeparatorVisibility()
        {
            Debug.Assert(this._buttonSeparator != null, "The button separator should never be null when this method is called.");

            // The button separator is visible if at least one of the buttons on both sides of it is visible.
            if (((this._firstItemButton != null && this._firstItemButton.Visibility == Visibility.Visible) ||
                (this._previousItemButton != null && this._previousItemButton.Visibility == Visibility.Visible) ||
                (this._nextItemButton != null && this._nextItemButton.Visibility == Visibility.Visible) ||
                (this._lastItemButton != null && this._lastItemButton.Visibility == Visibility.Visible)) &&
                ((this._editButton != null && this._editButton.Visibility == Visibility.Visible) ||
                (this._newItemButton != null && this._newItemButton.Visibility == Visibility.Visible) ||
                (this._deleteItemButton != null && this._deleteItemButton.Visibility == Visibility.Visible)))
            {
                this._buttonSeparator.Visibility = Visibility.Visible;
            }
            else
            {
                this._buttonSeparator.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets whether or not the user can add a new item.
        /// </summary>
        private void SetCanAddItems()
        {
            this.CanAddItems =
                this.EditableCollectionView != null &&
                (this.EditableCollectionView.CanAddNew || (this.IsEditing && this._canAddItemsAfterEdit)) &&
                !this.EffectiveIsReadOnly &&
                !this.AutoCommitPreventsCurrentItemChange &&
                !this.IsAddingNew;
        }

        /// <summary>
        /// Sets whether or not the user can begin an edit.
        /// </summary>
        private void SetCanBeginEdit()
        {
            this.CanBeginEdit =
                !this.IsEditing &&
                !this.EffectiveIsReadOnly &&
                this.CurrentItem != null;
        }

        /// <summary>
        /// Sets whether or not the user can cancel an edit.
        /// </summary>
        private void SetCanCancelEdit()
        {
            this.CanCancelEdit =
                (((this.EditableCollectionView != null && this.EditableCollectionView.CanCancelEdit) ||
                (this._lastItem is IEditableObject)) &&
                this.IsEditing) ||
                this.IsAddingNew;
        }

        /// <summary>
        /// Sets the visibility of the cancel button.
        /// </summary>
        private void SetCancelButtonVisibility()
        {
            Debug.Assert(this._cancelButton != null, "The cancel button should never be null when this method is called.");

            if (this.Mode != DataFormMode.ReadOnly &&
                ((!this.CommandButtonsVisibility.HasValue && (this.CanCancelEdit || this._lastItem is IEditableObject)) ||
                this.IsCommandButtonVisible(DataFormCommandButtonsVisibility.Cancel)))
            {
                this._cancelButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._cancelButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets whether or not the user can delete an item.
        /// </summary>
        private void SetCanDeleteItems()
        {
            this.CanDeleteItems =
                this._collectionView != null &&
                this.EditableCollectionView != null &&
                !this._collectionView.IsCurrentBeforeFirst &&
                !this._collectionView.IsCurrentAfterLast &&
                (this.EditableCollectionView.CanRemove || (this.IsEditing && this._canDeleteItemsAfterEdit)) &&
                !this.AutoCommitPreventsCurrentItemChange &&
                !this.IsAddingNew;
        }

        /// <summary>
        /// Sets whether or not the user can commit an edit.
        /// </summary>
        private void SetCanCommitEdit()
        {
            this.CanCommitEdit =
                this.IsEditing;
        }

        /// <summary>
        /// Sets whether or not the user can move to the first item.
        /// </summary>
        private void SetCanMoveToFirstItem()
        {
            this.CanMoveToFirstItem =
                this._collectionView != null &&
                this._collectionView.CurrentPosition > 0 &&
                !this.AutoCommitPreventsCurrentItemChange &&
                !this.IsAddingNew;
        }

        /// <summary>
        /// Sets whether or not the user can move to the last item.
        /// </summary>
        private void SetCanMoveToLastItem()
        {
            this.CanMoveToLastItem =
                this._collectionView != null &&
                this._collectionView.CurrentPosition < this.ItemsCount - 1 &&
                !this.AutoCommitPreventsCurrentItemChange &&
                !this.IsAddingNew;
        }

        /// <summary>
        /// Sets whether or not the user can move to the next item.
        /// </summary>
        private void SetCanMoveToNextItem()
        {
            this.CanMoveToNextItem =
                this._collectionView != null &&
                this._collectionView.CurrentPosition < this.ItemsCount - 1 &&
                !this.AutoCommitPreventsCurrentItemChange &&
                !this.IsAddingNew;
        }

        /// <summary>
        /// Sets whether or not the user can move to the previous item.
        /// </summary>
        private void SetCanMoveToPreviousItem()
        {
            this.CanMoveToPreviousItem =
                this._collectionView != null &&
                this._collectionView.CurrentPosition > 0 &&
                !this.AutoCommitPreventsCurrentItemChange &&
                !this.IsAddingNew;
        }

        /// <summary>
        /// Sets the visibility of the commit button.
        /// </summary>
        private void SetCommitButtonVisibility()
        {
            Debug.Assert(this._commitButton != null, "The commit button should never be null when this method is called.");
            bool cannotCommitWithoutCommitButton = !this.AutoCommit || this.IsAddingNew;

            if (this.Mode != DataFormMode.ReadOnly &&
                ((!this.CommandButtonsVisibility.HasValue && cannotCommitWithoutCommitButton) ||
                this.IsCommandButtonVisible(DataFormCommandButtonsVisibility.Commit)))
            {
                this._commitButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._commitButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets the visibility of the delete item button.
        /// </summary>
        private void SetDeleteItemButtonVisibility()
        {
            Debug.Assert(this._deleteItemButton != null, "The delete item button should never be null when this method is called.");

            if ((!this.CommandButtonsVisibility.HasValue && this.ItemsSource != null) ||
                this.IsCommandButtonVisible(DataFormCommandButtonsVisibility.Delete))
            {
                this._deleteItemButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._deleteItemButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets the visibility of the edit button.
        /// </summary>
        private void SetEditButtonVisibility()
        {
            Debug.Assert(this._editButton != null, "The edit button should never be null when this method is called.");

            if ((!this.CommandButtonsVisibility.HasValue && !this.AutoEdit) ||
                this.IsCommandButtonVisible(DataFormCommandButtonsVisibility.Edit))
            {
                this._editButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._editButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets the visibility of the first item button.
        /// </summary>
        private void SetFirstItemButtonVisibility()
        {
            Debug.Assert(this._firstItemButton != null, "The first item button should never be null when this method is called.");

            if ((!this.CommandButtonsVisibility.HasValue && this.ItemsSource != null) ||
                this.IsCommandButtonVisible(DataFormCommandButtonsVisibility.Navigation))
            {
                this._firstItemButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._firstItemButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets whether or not the item is valid and updates buttons and states accordingly.
        /// </summary>
        private void SetIsItemValid()
        {
            this.IsItemValid =
                this._fieldLevelErrors.Count == 0 &&
                this._entityLevelErrors.Count == 0 &&
                (this._validationSummary == null || this._validationSummary.Errors.Count == 0);

            this.SetAllCanProperties();
            this.UpdateButtonsAndStates();
        }

        /// <summary>
        /// Sets the visibility of the header.
        /// </summary>
        private void SetHeaderVisibility()
        {
            if (!this._headerVisibilityOverridden)
            {
                if ((this._firstItemButton != null && this._firstItemButton.Visibility == Visibility.Visible) ||
                    (this._previousItemButton != null && this._previousItemButton.Visibility == Visibility.Visible) ||
                    (this._nextItemButton != null && this._nextItemButton.Visibility == Visibility.Visible) ||
                    (this._lastItemButton != null && this._lastItemButton.Visibility == Visibility.Visible) ||
                    (this._editButton != null && this._editButton.Visibility == Visibility.Visible) ||
                    (this._newItemButton != null && this._newItemButton.Visibility == Visibility.Visible) ||
                    (this._deleteItemButton != null && this._deleteItemButton.Visibility == Visibility.Visible) ||
                    this.Header != null ||
                    this.HeaderTemplate != null)
                {
                    this.SetValueNoCallback(DataForm.HeaderVisibilityProperty, Visibility.Visible);
                }
                else
                {
                    this.SetValueNoCallback(DataForm.HeaderVisibilityProperty, Visibility.Collapsed);
                }
            }
        }

        /// <summary>
        /// Sets the visibility of the last item button.
        /// </summary>
        private void SetLastItemButtonVisibility()
        {
            Debug.Assert(this._lastItemButton != null, "The last item button should never be null when this method is called.");

            if ((!this.CommandButtonsVisibility.HasValue && this.ItemsSource != null) ||
                this.IsCommandButtonVisible(DataFormCommandButtonsVisibility.Navigation))
            {
                this._lastItemButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._lastItemButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets the mode of the DataForm.
        /// </summary>
        private void SetMode()
        {
            if (this.IsAddingNew)
            {
                this.Mode = DataFormMode.AddNew;
            }
            else if (this.IsEditing || (this.AutoEdit && (this.CanBeginEdit || this._endingEdit)))
            {
                this.Mode = DataFormMode.Edit;
            }
            else
            {
                this.Mode = DataFormMode.ReadOnly;
            }
        }

        /// <summary>
        /// Sets the visibility of the new item button.
        /// </summary>
        private void SetNewItemButtonVisibility()
        {
            Debug.Assert(this._newItemButton != null, "The new item button should never be null when this method is called.");

            if ((!this.CommandButtonsVisibility.HasValue && this.ItemsSource != null) ||
                this.IsCommandButtonVisible(DataFormCommandButtonsVisibility.Add))
            {
                this._newItemButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._newItemButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets the visibility of the next item button.
        /// </summary>
        private void SetNextItemButtonVisibility()
        {
            Debug.Assert(this._nextItemButton != null, "The next item button should never be null when this method is called.");

            if ((!this.CommandButtonsVisibility.HasValue && this.ItemsSource != null) ||
                this.IsCommandButtonVisible(DataFormCommandButtonsVisibility.Navigation))
            {
                this._nextItemButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._nextItemButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets the visibility of the previous item button.
        /// </summary>
        private void SetPreviousItemButtonVisibility()
        {
            Debug.Assert(this._previousItemButton != null, "The previous item button should never be null when this method is called.");

            if ((!this.CommandButtonsVisibility.HasValue && this.ItemsSource != null) ||
                this.IsCommandButtonVisible(DataFormCommandButtonsVisibility.Navigation))
            {
                this._previousItemButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._previousItemButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets up the collection view.
        /// </summary>
        /// <param name="collectionViewCreated">
        /// True if the collection view was created by the DataForm or its source's 
        /// implementation of ICollectionViewFactory.CreateView().
        /// </param>
        private void SetUpCollectionView(bool collectionViewCreated)
        {
            Debug.Assert(this._collectionView != null, "SetUpCollectionView should never be called when there is no collection view.");

            this._weakEventListenerCurrentChanging = new WeakEventListener<DataForm, object, CurrentChangingEventArgs>(this);
            this._weakEventListenerCurrentChanging.OnEventAction = (instance, source, eventArgs) => instance.OnCollectionViewCurrentChanging(source, eventArgs);
            this._weakEventListenerCurrentChanging.OnDetachAction = (weakEventListener) => this._collectionView.CurrentChanging -= weakEventListener.OnEvent;
            this._collectionView.CurrentChanging += this._weakEventListenerCurrentChanging.OnEvent;

            this._weakEventListenerCurrentChanged = new WeakEventListener<DataForm, object, EventArgs>(this);
            this._weakEventListenerCurrentChanged.OnEventAction = (instance, source, eventArgs) => instance.OnCollectionViewCurrentChanged(source, eventArgs);
            this._weakEventListenerCurrentChanged.OnDetachAction = (weakEventListener) => this._collectionView.CurrentChanged -= weakEventListener.OnEvent;
            this._collectionView.CurrentChanged += this._weakEventListenerCurrentChanged.OnEvent;

            this._weakEventListenerCollectionChanged = new WeakEventListener<DataForm, object, NotifyCollectionChangedEventArgs>(this);
            this._weakEventListenerCollectionChanged.OnEventAction = (instance, source, eventArgs) => instance.OnCollectionViewCollectionChanged(source, eventArgs);
            this._weakEventListenerCollectionChanged.OnDetachAction = (weakEventListener) => this._collectionView.CollectionChanged -= weakEventListener.OnEvent;
            this._collectionView.CollectionChanged += this._weakEventListenerCollectionChanged.OnEvent;

            if (collectionViewCreated || this._collectionView.IsCurrentBeforeFirst)
            {
                if (this._collectionView.CurrentPosition == 0 || !this._collectionView.MoveCurrentToFirst())
                {
                    // This move can fail in the case of an empty set, which will then require that
                    // the below happen manually.
                    this.UpdateCurrentItem();
                }
            }
            else
            {
                // The above will not cause a CurrentChanged event to be fired when the position
                // is initialized to the first item, so change DataForm.CurrentItem manually
                // in that case.
                this.UpdateCurrentItem();
            }
        }

        /// <summary>
        /// Sets up the new current item.
        /// </summary>
        private void SetUpNewCurrentItem()
        {
            if (this._collectionView != null)
            {
                if (this.CurrentItem != null || this._collectionView.Contains(this.CurrentItem))
                {
                    this._collectionView.MoveCurrentTo(this.CurrentItem);
                }
                else
                {
                    this._collectionView.MoveCurrentToPosition(-1);
                }
            }

            if (this.CurrentItemNotifyPropertyChanged != null && this._weakEventListenerPropertyChanged != null)
            {
                this._weakEventListenerPropertyChanged.Detach();
                this._weakEventListenerPropertyChanged = null;
            }

            this.ForceEndEdit();

            if (this.CurrentItemNotifyPropertyChanged != null)
            {
                this._weakEventListenerPropertyChanged = new WeakEventListener<DataForm, object, PropertyChangedEventArgs>(this);
                this._weakEventListenerPropertyChanged.OnEventAction = (instance, source, eventArgs) => instance.OnCurrentItemPropertyChanged(source, eventArgs);
                this._weakEventListenerPropertyChanged.OnDetachAction = (weakEventListener) => this.CurrentItemNotifyPropertyChanged.PropertyChanged -= weakEventListener.OnEvent;
                this.CurrentItemNotifyPropertyChanged.PropertyChanged += this._weakEventListenerPropertyChanged.OnEvent;
            }
        }

        /// <summary>
        /// Returns a value indicating whether or not the UI should be re-generated based on
        /// a changed template.
        /// </summary>
        /// <param name="changedTemplateMode">The mode for which template was changed, or null if the content was changed.</param>
        /// <returns>A value indicating whether or not the UI should be re-generated.</returns>
        private bool ShouldRegenerateUI(DataFormMode? changedTemplateMode)
        {
            Debug.Assert(this.CurrentItemType != null, "ShouldUpdateTemplate should never be called if the current item type is null.");

            // If the changed template was for the same mode as the current mode,
            // we know we should update the template.
            if (this.Mode == changedTemplateMode)
            {
                return true;
            }

            DataFormMode? currentlyUsedTemplateMode = this._currentlyUsedTemplate[this.CurrentItemType];
            if (currentlyUsedTemplateMode == null)
            {
                // We're currently using direct content, so we will re-generate the UI no matter what
                // template has changed (since every template takes precedence over the direct content).
                return true;
            }

            if (changedTemplateMode == null)
            {
                // We're using a template and the direct content has changed, so there's no need to update.
                return false;
            }

            // If the changed template was not for the same mode as the current mode,
            // then we should update the template if the mode for the currently used template
            // is after the mode for the changed template in the fallback list.
            if (this.Mode == DataFormMode.ReadOnly)
            {
                // If we're in read-only mode, the fallback mechanism is edit -> add-new.
                return
                    (currentlyUsedTemplateMode == DataFormMode.Edit && changedTemplateMode == DataFormMode.Edit) ||
                    currentlyUsedTemplateMode == DataFormMode.AddNew;
            }
            else if (this.Mode == DataFormMode.Edit)
            {
                // If we're in edit mode, the fallback mechanism is add-new -> read-only.
                return
                    (currentlyUsedTemplateMode == DataFormMode.AddNew && changedTemplateMode == DataFormMode.AddNew) ||
                    currentlyUsedTemplateMode == DataFormMode.ReadOnly;
            }
            else if (this.Mode == DataFormMode.AddNew)
            {
                // If we're in add-new mode, the fallback mechanism is edit -> read-only.
                return
                    (currentlyUsedTemplateMode == DataFormMode.Edit && changedTemplateMode == DataFormMode.Edit) ||
                    currentlyUsedTemplateMode == DataFormMode.ReadOnly;
            }

            return false;
        }

        /// <summary>
        /// Updates the current item.
        /// </summary>
        private void UpdateCurrentItem()
        {
            if (this._collectionView != null)
            {
                this.CurrentItem = this._collectionView.CurrentItem;
                this.CurrentIndex = this._collectionView.CurrentPosition;
            }
            else
            {
                this.CurrentIndex = this.CurrentItem != null ? 0 : -1;
            }

            this.IsEmpty = this.CurrentItem == null;

            if (this.CurrentItemChangeTracking != null)
            {
                this.IsItemChanged = this.CurrentItemChangeTracking.IsChanged;
            }

            if (this._contentPresenter != null && this._contentPresenter.Content != null)
            {
                FrameworkElement contentPresenterContent = this._contentPresenter.Content as FrameworkElement;

                if (contentPresenterContent != null)
                {
                    contentPresenterContent.DataContext = this.CurrentItem;
                    if (this._validationSummary != null)
                    {
                        this._validationSummary.Target = contentPresenterContent;
                    }
                }
            }
            else
            {
                if (this._validationSummary != null)
                {
                    this._validationSummary.Target = null;
                }
            }

            this.UpdateButtonsAndStates();
        }

        /// <summary>
        /// Validates the current item.
        /// </summary>
        /// <param name="validateAllProperties">Whether or not to validate all properties.</param>
        /// <returns>Whether or not the current item is valid.</returns>
        private bool ValidateItem(bool validateAllProperties)
        {
            CancelEventArgs e = new CancelEventArgs();
            this.OnValidatingItem(e);

            if (e.Cancel)
            {
                return this.IsItemValid;
            }

            // Clear entity errors both from the ErrorSummary and the entity errors list.
            this.ClearEntityErrors();

            if (validateAllProperties)
            {
                // Check all input controls and validate them.
                this.UpdateAllSources();
            }

            if (this._lastItem != null)
            {
                // Check the rest of the parameters on the entity object.
                ValidationContext context = new ValidationContext(this._lastItem, null, null);
                List<ValidationResult> validationResults = new List<ValidationResult>();
                bool valid = Validator.TryValidateObject(this._lastItem, context, validationResults, validateAllProperties);

                if (!valid)
                {
                    Debug.Assert(validationResults.Count > 0, "Entity is not valid, but there are no errors.");
                    foreach (ValidationResult result in validationResults)
                    {
                        string errorMessage = result.ErrorMessage;
                        ValidationSummaryItem newError = new ValidationSummaryItem(errorMessage, null, ValidationSummaryItemType.ObjectError, null, null);

                        // Indicate that this DataForm is the owner of the error.  When clearing errors, only errors from this DataForm should be cleared.
                        newError.Context = this;
                        foreach (string property in result.MemberNames)
                        {
                            // Find a control matching this property name.
                            Control c = null;
                            if (this._templateBindingInfos != null)
                            {
                                foreach (DataFormBindingInfo bindingInfo in this._templateBindingInfos)
                                {
                                    if (bindingInfo.BindingExpression != null && bindingInfo.BindingExpression.ParentBinding != null &&
                                       bindingInfo.BindingExpression.ParentBinding.Path.Path == property)
                                    {
                                        c = bindingInfo.Element as Control;
                                        break;
                                    }
                                }
                            }
                            ValidationSummaryItemSource errorSource = new ValidationSummaryItemSource(property, c);
                            newError.Sources.Add(errorSource);
                        }

                        // Only add the errors that weren't picked up from the field-level validation.
                        if (this.EntityErrorShouldBeAdded(newError))
                        {
                            this._entityLevelErrors.Add(newError);

                            if (this.ValidationSummary != null)
                            {
                                Debug.Assert(this.ValidationSummary.Errors != null, "ValidationSummary.Errors should never be null.");

                                if (!this.ValidationSummary.Errors.Contains(newError))
                                {
                                    this.ValidationSummary.Errors.Add(newError);
                                }
                            }
                        }
                    }

                    this.SetIsItemValid();
                    return false;
                }

                validationResults.Clear();
            }

            this.SetIsItemValid();
            return this.IsItemValid;
        }

        /// <summary>
        /// Handles the click of the "new item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnNewItemButtonClick(object sender, RoutedEventArgs e)
        {
            this._shouldMoveFocus = true;
            this.AddNewItem();
        }

        /// <summary>
        /// Handles the click of the "begin edit" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnBeginEditButtonClick(object sender, RoutedEventArgs e)
        {
            this._shouldMoveFocus = true;
            this.BeginEdit();
        }

        /// <summary>
        /// Handles the click of the "cancel edit" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnCancelEditButtonClick(object sender, RoutedEventArgs e)
        {
            this.CancelEdit();
        }

        /// <summary>
        /// Handles the click of the "delete item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnDeleteItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.DeleteItem();
        }

        /// <summary>
        /// Handles the click of the "commit edit" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnCommitEditButtonClick(object sender, RoutedEventArgs e)
        {
            this.CommitEdit();
        }

        /// <summary>
        /// Handles the click of the "move to first item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnMoveToFirstItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToFirstItem();
        }

        /// <summary>
        /// Handles the click of the "move to last item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnMoveToLastItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToLastItem();
        }

        /// <summary>
        /// Handles the click of the "move to previous item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnMoveToPreviousItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToPreviousItem();
        }

        /// <summary>
        /// Handles the click of the "move to next item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnMoveToNextItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToNextItem();
        }

        /// <summary>
        /// Handles the case where the root element in the fields presenter has got focus.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnContentRootElementGotFocus(object sender, RoutedEventArgs e)
        {
            if (this.AutoEdit && !this.IsEditing)
            {
                this.BeginEdit();
            }
        }

        /// <summary>
        /// Handles the case where the root element in the fields presenter has loaded.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnContentRootElementLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement contentRootElement = sender as FrameworkElement;
            contentRootElement.Loaded -= new RoutedEventHandler(this.OnContentRootElementLoaded);
            this.PrepareContent(contentRootElement);
            this.OnContentLoaded(new DataFormContentLoadEventArgs(this._contentPresenter.Content as FrameworkElement, this.Mode));
        }

        /// <summary>
        /// Handles the case where the root element in the fields presenter has lost focus.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnContentRootElementLostFocus(object sender, RoutedEventArgs e)
        {
            // Only cancel the edit if the newly focused element is not also in the content.
            FrameworkElement content = sender as FrameworkElement;

            if (this.AutoEdit &&
                this.IsEditing &&
                !this.IsAddingNew &&
                this._editedProperties.Count == 0 &&
                content != null &&
                !content.ContainsFocusedElement())
            {
                this.CancelEdit();
            }
        }

        /// <summary>
        /// Handles the case where a field has had a binding validation error occur on it.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnContentRootElementBindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            switch (e.Action)
            {
                case ValidationErrorEventAction.Added:
                    if (!this._fieldLevelErrors.Contains(e.Error))
                    {
                        this._fieldLevelErrors.Add(e.Error);
                    }
                    break;

                case ValidationErrorEventAction.Removed:
                    if (this._fieldLevelErrors.Contains(e.Error))
                    {
                        this._fieldLevelErrors.Remove(e.Error);
                    }
                    break;
            }

            this.SetIsItemValid();
        }

        /// <summary>
        /// Handles the case where a date picker's calendar was opened.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnDatePickerCalendarOpened(object sender, RoutedEventArgs e)
        {
            if (this.AutoEdit && !this.IsEditing)
            {
                this.BeginEdit();
            }
        }

        /// <summary>
        /// Handles the case where a text box outside a DataField has lost focus.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnExternalTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Debug.Assert(this._lostFocusFired.ContainsKey(textBox), "LostFocus should never be handled on a text box without _lostFocusFired getting the text box as a key.");
            this._lostFocusFired[textBox] = true;
        }

        /// <summary>
        /// Handles the case where a text box outside a DataField has is text changed.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnExternalTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && (ValidationUtil.ElementHasErrors(textBox) || !this._lostFocusFired[textBox]))
            {
                this._lostFocusFired[textBox] = false;
                ValidationUtil.UpdateSourceOnElementBindings(textBox);
            }
        }

        /// <summary>
        /// Handles the case where a text box's text changed.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && this.CurrentItemType != null)
            {
                BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                string propertyPath = null;

                if (bindingExpression != null &&
                    bindingExpression.ParentBinding != null &&
                    bindingExpression.ParentBinding.Path != null &&
                    !string.IsNullOrEmpty(bindingExpression.ParentBinding.Path.Path))
                {
                    propertyPath = bindingExpression.ParentBinding.Path.Path;
                }

                if (propertyPath != null)
                {
                    PropertyInfo propertyInfo = this.CurrentItemType.GetPropertyInfo(propertyPath);

                    if (propertyInfo != null)
                    {
                        this.CheckIfPropertyEditedAndUpdate(
                            propertyInfo.Name,
                            textBox.Text,
                            true /* doConversion */,
                            propertyInfo != null ? propertyInfo.PropertyType : null,
                            bindingExpression.ParentBinding.Converter,
                            bindingExpression.ParentBinding.ConverterParameter,
#if MIGRATION
                            bindingExpression.ParentBinding.ConverterCulture ?? (textBox.Language != null ? new CultureInfo(textBox.Language.IetfLanguageTag) : CultureInfo.CurrentCulture));
#else
                            bindingExpression.ParentBinding.ConverterLanguage != null ? new CultureInfo(bindingExpression.ParentBinding.ConverterLanguage) :
                                (textBox.Language != null ? new CultureInfo(textBox.Language.IetfLanguageTag) : CultureInfo.CurrentCulture));
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Handles the case where the validation summary's error collection changed.
        /// </summary>
        /// <param name="sender">The validation summary.</param>
        /// <param name="e">The event args.</param>
        private void OnValidationSummaryErrorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SetIsItemValid();
        }

#endregion Private Methods

#endregion Methods
    }
}
