// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using OpenSilver.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Resource = OpenSilver.Controls.Input.Toolkit.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that enables single value selection from a 
    /// domain of values through a Spinner and TextBox.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = StateValid, GroupName = GroupDomain)]
    [TemplateVisualState(Name = StateInvalid, GroupName = GroupDomain)]

    [TemplateVisualState(Name = VisualStates.StateEdit, GroupName = VisualStates.GroupInteractionMode)]
    [TemplateVisualState(Name = VisualStates.StateDisplay, GroupName = VisualStates.GroupInteractionMode)]

    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]

    [TemplatePart(Name = ElementTextName, Type = typeof(TextBox))]
    [TemplatePart(Name = ElementSpinnerName, Type = typeof(Spinner))]
    [StyleTypedProperty(Property = SpinnerStyleName, StyleTargetType = typeof(ButtonSpinner))]
    [ContentProperty("Items")]
    public class DomainUpDown : UpDownBase<object>, IUpdateVisualState
    {
        #region Visualstate definitions
        /// <summary>
        /// Domain state group.
        /// </summary>
        public const string GroupDomain = "DomainStates";

        /// <summary>
        /// InvalidDomain of the Domain state group.
        /// </summary>
        public const string StateInvalid = "InvalidDomain";

        /// <summary>
        /// Valid of the Domain state group.
        /// </summary>
        public const string StateValid = "ValidDomain";
        #endregion Visualstate definitions

        #region Items
        /// <summary>
        /// Gets a collection of items which represent the domain 
        /// in the DomainUpDown control.
        /// </summary>
        /// <remarks>When ItemsSource is set, will return a copy of the items in ItemsSource.
        /// When ItemsSource property is not set (set to null) this collection 
        /// will return an empty writeable colection.</remarks>
        /// <value>The items.</value>
        public ObservableObjectCollection Items
        {
            get
            {
                if (ItemsSource == null)
                {
                    // return our observable collection.
                    return _items;
                }
                // wrap the items of the itemssource in a helper collection that we set to readonly.
                return new ObservableObjectCollection(ItemsSource)
                           {
                               IsReadOnly = true
                           };
            }
        }

        /// <summary>
        /// Backing store for the Items collection, when ItemsSource is null.
        /// </summary>
        private readonly ObservableObjectCollection _items;
        #endregion       

        /// <summary>
        /// The value as set during initialization. Since no items have been added
        /// at that point, the value will be reset. However, during 
        /// initialization, the value will be cached in this field and used
        /// when items are actually added. 
        /// </summary>
        /// <remarks>After this index has been used, it will be set to new object().</remarks>
        private object _valueDuringInitialization;

        /// <summary>
        /// Indicates whether the control should not move to EditMode when
        /// it is gains Focus.
        /// </summary>
        private bool _isNotAllowedToEditByFocus;

        /// <summary>
        /// WeakEventListener used to handle INotifyCollectionChanged events.
        /// </summary>
        private WeakEventListener<DomainUpDown, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> _weakEventListener;
        
        #region IsEditing
        /// <summary>
        /// BackingField for IsEditing, indicating whether the control is in EditMode.
        /// </summary>
        private bool _isEditing;

        /// <summary>
        /// Gets a value indicating whether the control is in EditMode.
        /// </summary>
        /// <value><c>True</c> if currently in edit mode; otherwise, <c>False</c>.</value>
        public bool IsEditing
        {
            get { return _isEditing; }
            private set
            {
                if (value != _isEditing)
                {
                    if (!IsEditable && value)
                    {
                        // should not move to editmode
                        return;
                    }

                    _isEditing = value;
                    UpdateVisualState(true);

                    if (Text != null)
                    {
                        // control has transitioned from editmode to displaymode
                        if (!value)
                        {
                            // format the text currently in the textbox according the formatting rules.
                            // this removes the previous input.
                            Text.Text = FormatValue();
                            // When in display mode, the textbox should not handle mouseinput
                            Text.IsHitTestVisible = false;
                        }
                        else
                        {
                            if (FocusManager.GetFocusedElement() == Text)
                            {
                                // the textbox already had focus, even though we were in display mode
                                // this occurs because this control does not have a tabstop
                                // and should not capture focus when editmode is released
                                Text.Select(0, Text.Text.Length);
                            }
                            // In edit mode, the textbox should handle mouseinput
                            Text.IsHitTestVisible = true;
                        }
                    }
                }
            }
        }

        #endregion IsEditing

        #region IsInvalidInput
        /// <summary>
        /// Backing field for InvalidInput, indicating whether the last parsed input was invalid.
        /// </summary>
        private bool _isInvalidInput;

        /// <summary>
        /// Gets or sets a value indicating whether the last parsed input was invalid.
        /// </summary>
        /// <value><c>True</c> if input is currently Invalid; otherwise, <c>false</c>.</value>
        private bool IsInvalidInput
        {
            get { return _isInvalidInput; }
            set
            {
                if (value != _isInvalidInput)
                {
                    _isInvalidInput = value;
                    UpdateVisualState(true);
                }
            }
        }

        #endregion IsInvalidInput

        #region public int CurrentIndex - coercion
        /// <summary>
        /// Gets or sets the index of the current selected item.
        /// </summary>
        public int CurrentIndex
        {
            get { return (int)GetValue(CurrentIndexProperty); }
            set { SetValue(CurrentIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the CurrentIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty.Register(
                "CurrentIndex",
                typeof(int),
                typeof(DomainUpDown),
                new PropertyMetadata(-1, OnCurrentIndexPropertyChanged));

        /// <summary>
        /// CurrentIndexProperty property changed handler.
        /// </summary>
        /// <param name="d">DomainUpDown instance that changed its CurrentIndex.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnCurrentIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DomainUpDown source = (DomainUpDown)d;
            int newValue = (int)e.NewValue;
            int oldValue = (int)e.OldValue;

            // validate newValue
            if (!source.IsValidCurrentIndex(newValue))
            {
                // revert back to e.OldValue
                source._currentIndexNestLevel++;
                source.SetValue(e.Property, e.OldValue);
                source._currentIndexNestLevel--;

                if (source._currentIndexDuringInitialization == null)
                {
                    // index is set but no items have been added yet
                    // cache the value
                    source._currentIndexDuringInitialization = newValue;
                    return;
                }
                else
                {
                    // throw exception
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        Resource.DomainUpDown_CurrentIndex_InvalidValue,
                        e.NewValue);
                    throw new ArgumentOutOfRangeException("e", message);
                }
            }

            if (source._currentIndexNestLevel == 0)
            {
                // remember initial state
                source._initialCurrentIndex = oldValue;
            }

            source._currentIndexNestLevel++;

            // coerce newValue
            int coercedValue = source.CoerceSelectedIndex(newValue);
            if (newValue != coercedValue)
            {
                // always set SelectedIndexProperty to coerced value
                source.CurrentIndex = coercedValue;
            }

            source._currentIndexNestLevel--;

            if (source._currentIndexNestLevel == 0 && source.CurrentIndex != source._initialCurrentIndex)
            {
                // fire changed event only at root level and when there is indeed a change
                source.OnCurrentIndexChanged(oldValue, source.CurrentIndex);
            }
        }

        /// <summary>
        /// SelectedIndexProperty validation handler.
        /// </summary>
        /// <param name="value">New value of SelectedIndexProperty.</param>
        /// <returns>
        /// Returns true if value is valid for SelectedIndexProperty, false otherwise.
        /// </returns>
        private bool IsValidCurrentIndex(int value)
        {
            // -1 is only legal when the items collection is empty
            IEnumerable<object> actualItems = GetActualItems();
            int count = actualItems.Count();
            return (value == -1 && count == 0) || (value >= 0 && value < count);
        }

        /// <summary>
        /// CurrentIndexProperty coercion handler.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// Coerced effective value of CurrentIndexProperty from input parameter value.
        /// </returns>
        private int CoerceSelectedIndex(int index)
        {
            // no coercion needed
            if (IsValidCurrentIndex(index))
            {
                return index;
            }
            
            // if we are at an empty collection, return -1
            if (GetActualItems().Count() == 0)
            {
                return -1;
            }
            
            // select the first item in the collection.
            return 0;
        }

        /// <summary>
        /// Cached previous value of SelectedIndexProperty.
        /// </summary>
        private int _initialCurrentIndex = -1;

        /// <summary>
        /// The index as set during initialization. Since no items have been added
        /// at that point, an Exception should be thrown. However, during 
        /// initialization, the index will be cached in this field and used
        /// when items are actually added. 
        /// </summary>
        /// <remarks>After this index has been used, it will be set to -1.</remarks>
        private int? _currentIndexDuringInitialization;

        /// <summary>
        /// Nest level for selected index.
        /// </summary>
        private int _currentIndexNestLevel;
        #endregion public int CurrentIndex - coercion

        #region public bool IsCyclic
        /// <summary>
        /// Gets or sets a value indicating whether the DomainUpDown control 
        /// will cycle through values when trying to spin the first and last item. 
        /// </summary>
        public bool IsCyclic
        {
            get { return (bool)GetValue(IsCyclicProperty); }
            set { SetValue(IsCyclicProperty, value); }
        }

        /// <summary>
        /// Identifies the IsCyclic dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCyclicProperty =
            DependencyProperty.Register(
                "IsCyclic",
                typeof(bool),
                typeof(DomainUpDown),
                new PropertyMetadata(false, OnIsCyclicPropertyChanged));

        /// <summary>
        /// IsCyclicProperty property changed handler.
        /// </summary>
        /// <param name="d">DomainUpDown instance that changed its IsCyclic value.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsCyclicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DomainUpDown source = (DomainUpDown) d;
            source.SetValidSpinDirection();
        }

        #endregion public bool IsCyclic

        #region public Binding ValueMemberBinding and ValueMemberPath

        /// <summary>
        /// Gets or sets the Binding object used for object to string 
        /// conversions.
        /// </summary>
        public Binding ValueMemberBinding
        {
            get { return _valueBindingEvaluator == null ? null : _valueBindingEvaluator.ValueBinding; }
            set { _valueBindingEvaluator = new BindingSourceEvaluator<string>(value); }
        }

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the value.
        /// </summary>
        public string ValueMemberPath
        {
            get
            {
                return (ValueMemberBinding != null) ? ValueMemberBinding.Path.Path : null;
            }
            set
            {
                if (value == null)
                {
                    if (ValueMemberBinding != null)
                    {
                        ValueMemberBinding = new Binding()
                                           {
                                               Converter = ValueMemberBinding.Converter,
                                               ConverterCulture = ValueMemberBinding.ConverterCulture,
                                               ConverterParameter = ValueMemberBinding.ConverterParameter
                                           };
                    }
                }
                else
                {
                    if (ValueMemberBinding != null)
                    {
                        ValueMemberBinding = new Binding(value)
                                                 {
                                                         Converter = ValueMemberBinding.Converter,
                                                         ConverterCulture = ValueMemberBinding.ConverterCulture,
                                                         ConverterParameter = ValueMemberBinding.ConverterParameter,
                                                 };
                    }
                    else
                    {
                        ValueMemberBinding = new Binding(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the BindingEvaluator, a framework element that can
        /// provide updated string values from a single binding.
        /// </summary>
        private BindingSourceEvaluator<string> _valueBindingEvaluator;
        #endregion public Binding ValueMemberBinding and ValueMemberPath

        #region public InvalidInputActions InvalidInputAction
        /// <summary>
        /// Gets or sets a value determining the DomainUpDown behavior when a 
        /// user sets a value not included in the domain.
        /// </summary>
        public InvalidInputAction InvalidInputAction
        {
            get { return (InvalidInputAction)GetValue(InvalidInputActionProperty); }
            set { SetValue(InvalidInputActionProperty, value); }
        }

        /// <summary>
        /// Identifies the InvalidInputAction dependency property.
        /// </summary>
        public static readonly DependencyProperty InvalidInputActionProperty =
            DependencyProperty.Register(
                "InvalidInputAction",
                typeof(InvalidInputAction),
                typeof(DomainUpDown),
                new PropertyMetadata(InvalidInputAction.UseFallbackItem, OnInvalidInputActionPropertyChanged));

        /// <summary>
        /// InvalidInputAction property changed handler.
        /// </summary>
        /// <param name="d">DomainUpDown instance that changed the InvalidInputAction.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnInvalidInputActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InvalidInputAction action = (InvalidInputAction) e.NewValue;
            switch (action)
            {
                case InvalidInputAction.UseFallbackItem:
                    break;
                case InvalidInputAction.TextBoxCannotLoseFocus:
                    break;
                default:
                    {
                        // throw exception
                        string message = string.Format(
                            CultureInfo.InvariantCulture,
                            Resource.DomainUpDown_InvalidInputAction_InvalidValue,
                            action);
                        throw new ArgumentException(message, "e");
                    }
            }
        }

        #endregion public InvalidInputActions InvalidInputAction

        #region public object FallbackItem
        /// <summary>
        /// Gets or sets the item that is used when a user attempts to 
        /// set a value not included in the domain.
        /// </summary>
        /// <remarks>FallbackItem will only be used if it is contained within 
        /// the Items collection.</remarks>
        public object FallbackItem
        {
            get { return GetValue(FallbackItemProperty); }
            set { SetValue(FallbackItemProperty, value); }
        }

        /// <summary>
        /// Identifies the FallbackItem dependency property.
        /// </summary>
        public static readonly DependencyProperty FallbackItemProperty =
            DependencyProperty.Register(
                "FallbackItem",
                typeof(object),
                typeof(DomainUpDown),
                new PropertyMetadata(null));
        #endregion public object FallbackItem

        #region public IEnumerable ItemsSource
        /// <summary>
        /// Gets or sets a collection of items which represent the 
        /// domain in the DomainUpDown control.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return GetValue(ItemsSourceProperty) as IEnumerable; }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(DomainUpDown),
                new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        /// <summary>
        /// ItemsSourceProperty property changed handler.
        /// </summary>
        /// <param name="d">DomainUpDown that changed its ItemsSource.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DomainUpDown dud = (DomainUpDown)d;
            dud.OnItemsSourceChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable);
        }

        #endregion public IEnumerable ItemsSource
                
        #region public DataTemplate ItemTemplate
        /// <summary>
        /// Gets or sets the DataTemplate used to display an item from the 
        /// Domain when the item is selected.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return GetValue(ItemTemplateProperty) as DataTemplate; }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                "ItemTemplate",
                typeof(DataTemplate),
                typeof(DomainUpDown),
                new PropertyMetadata(null));

        #endregion public DataTemplate ItemTemplate

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainUpDown"/> class.
        /// </summary>
        public DomainUpDown()
        {
            DefaultStyleKey = typeof(DomainUpDown);
            Interaction = new InteractionHelper(this);

            _items = new ObservableObjectCollection();
            _items.CollectionChanged += OnItemsChanged;
        }

        /// <summary>
        /// Builds the visual tree for the DomainUpDown control when a new 
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SetValidSpinDirection();
        }

        /// <summary>
        /// Returns a DomainUpDownAutomationPeer for use by the 
        /// Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// The class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> subclass to return.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DomainUpDownAutomationPeer(this);
        }

        /// <summary>
        /// Gets the correct items collection that we are using, abstracting away ItemsSource and Items logic.
        /// </summary>
        /// <returns>Returns either Items or ItemsSource.</returns>
        private IEnumerable<object> GetActualItems()
        {
            return ItemsSource == null ? _items.OfType<object>() : ItemsSource.OfType<object>();
        }

        /// <summary>
        /// Called when the items collection is changed. This can either be Items or ItemsSource.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>If ItemsSource does not implement INotifyCollectionChanged, this method will not be called on
        /// changes in the ItemsSource.</remarks>
        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // use the _currentIndexDuringInitialization
            // or set to first item in list
            if (_currentIndexDuringInitialization != null && 
                _currentIndexDuringInitialization.Value > -1 &&
                IsValidCurrentIndex(_currentIndexDuringInitialization.Value))
            {
                Value = GetActualItems().ElementAt(_currentIndexDuringInitialization.Value);
                _currentIndexDuringInitialization = -1;
            }
            else if (_valueDuringInitialization != null && 
                GetActualItems().Contains(_valueDuringInitialization))
            {
                Value = _valueDuringInitialization;
                _valueDuringInitialization = new object();
            }
            else if (Value == null || !GetActualItems().Contains(Value))
            {
                // select new Value if Value is no longer part of the domain.
                Value = GetActualItems().FirstOrDefault();
            }

            // determine if the user can spin.
            SetValidSpinDirection();
        }

        /// <summary>
        /// Called when ItemsSource has changed.
        /// Handles setting the selected item.
        /// </summary>
        /// <param name="oldItemsSource">The old items source.</param>
        /// <param name="itemsSource">The items source.</param>
        /// <remarks>When switching ItemsSource, the selectedIndex will be used (if possible) 
        /// to select an item in the new collection.</remarks>
        private void OnItemsSourceChanged(IEnumerable oldItemsSource, IEnumerable itemsSource)
        {
            INotifyCollectionChanged oldObservableItemsSource = oldItemsSource as INotifyCollectionChanged;
            INotifyCollectionChanged newObservableItemsSource = itemsSource as INotifyCollectionChanged;

            if (oldObservableItemsSource != null)
            {
                // no longer react to changes in this collection.
                // Detach the WeakEventListener
                if (null != _weakEventListener)
                {
                    _weakEventListener.Detach();
                    _weakEventListener = null;
                }
            }

            if (itemsSource != null)
            {
                int index = GetIndexOf(itemsSource, Value);
                if (index > -1)
                {
                    // no need to select a different value, however, currentIndex needs to be set.
                    CurrentIndex = index;
                }
                else
                {
                    // use the currentindex to set the value
                    // or use the _currentIndexDuringInitialization
                    // or set to first item in list
                    IEnumerable<object> newItemsSource = itemsSource.OfType<object>();
                    if (_currentIndexDuringInitialization != null && _currentIndexDuringInitialization.Value > -1)
                    {
                        if (IsValidCurrentIndex(CurrentIndex))
                        {
                            Value = newItemsSource.ElementAt(CurrentIndex);
                        }
                        else
                        {
                            Value = IsValidCurrentIndex(_currentIndexDuringInitialization.Value) ? newItemsSource.ElementAt(_currentIndexDuringInitialization.Value) : newItemsSource.FirstOrDefault();
                        }
                        _currentIndexDuringInitialization = -1;
                    }
                    else if (newItemsSource.Contains(_valueDuringInitialization))
                    {
                        // set the value from the initialization cache.
                        Value = _valueDuringInitialization;
                        _valueDuringInitialization = new object();
                    }
                    else
                    {
                        // get value at the same index of the new source, of get the first.
                        // it is possible for the new source to have 0 items
                        Value = IsValidCurrentIndex(CurrentIndex) ? newItemsSource.ElementAtOrDefault(CurrentIndex) : newItemsSource.FirstOrDefault();
                    }
                }

                if (newObservableItemsSource != null)
                {
                    // if ItemsSource supplies CollectionChanged events, subscribe to them.
                    // Use a WeakEventListener so that the backwards reference doesn't keep this object alive
                    _weakEventListener = new(this, newObservableItemsSource)
                    {
                        OnEventAction = static (instance, source, eventArgs) => instance.OnItemsChanged(source, eventArgs),
                        OnDetachAction = static (listener, source) => source.CollectionChanged -= listener.OnEvent,
                    };
                    newObservableItemsSource.CollectionChanged += _weakEventListener.OnEvent;
                }
            }
            else
            {
                // clear the items collection.
                _items.Clear();
            }

            // determine if the user can spin.
            SetValidSpinDirection();
        }

        /// <summary>
        /// Raises the ValueChanging event when Value property is changing.
        /// </summary>
        /// <param name="e">Event args.</param>
        /// <remarks>Cancels the event when the value is not part of the domain.</remarks>
        protected override void OnValueChanging(RoutedPropertyChangingEventArgs<object> e)
        {
            // value needs to be contained in the items collection.
            if (e != null && ((e.NewValue == null && GetActualItems().Count() > 0) ||
                (e.NewValue != null && !GetActualItems().Contains(e.NewValue))))
            {
                e.Cancel = true;

                if (_valueDuringInitialization == null && e.NewValue != null)
                {
                    _valueDuringInitialization = e.NewValue;
                }
            }
            else
            {
                base.OnValueChanging(e);
            }
        }

        /// <summary>
        /// Raises the ValueChanged event when Value property has changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        /// <remarks>Will set CurrentIndex.</remarks>
        protected override void OnValueChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnValueChanged(e);

            // set the selected index
            CurrentIndex = GetIndexOf(GetActualItems(), Value);

            IsEditing = false;

            DomainUpDownAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as DomainUpDownAutomationPeer;
            if (peer != null)
            {
                // automation peer cannot handle nulls.
                peer.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, e.OldValue ?? String.Empty, e.NewValue ?? String.Empty);
            }
        }

        /// <summary>
        /// Processes changes to the CurrentIndex property.
        /// </summary>
        /// <param name="oldValue">The old value of CurrentIndex.</param>
        /// <param name="newValue">The new value of CurrentIndex.</param>
        protected virtual void OnCurrentIndexChanged(int oldValue, int newValue)
        {
            // the CurrentIndex is coerced, so is always valid.

            // set the selected item
            Value = GetActualItems().ElementAtOrDefault(newValue);

            IsEditing = false;

            // determine if the user can spin.
            SetValidSpinDirection();
        }

        /// <summary>
        /// Processes user input when the TextBox.TextChanged event occurs.
        /// </summary>
        /// <param name="text">User input.</param>
        protected internal override void ApplyValue(string text)
        {
            if (!IsEditable)
            {
                return;
            }

            // parsing brings us to edit mode. Expected is that we are already in editmode
            // but a derived class might call to parse directly.
            IsEditing = true;

            try
            {
                Value = ParseValue(text);
            }
            catch (Exception error)
            {
                UpDownParseErrorEventArgs args = new UpDownParseErrorEventArgs(text, error);
                OnParseError(args);

                if (!args.Handled)
                {
                    // default error handling is to discard user input and revert to the old text
                    SetTextBoxText();
                }
            }
            finally
            {
                // at any rate, parsing means we move out of edit mode
                // except if this is invalid input and we are in the textbox cannot lose focus mode
                if (!IsInvalidInput || InvalidInputAction != InvalidInputAction.TextBoxCannotLoseFocus)
                {
                    IsEditing = false;
                }
            }
        }

        /// <summary>
        /// Called by ApplyValue to parse user input as a value in the domain.
        /// </summary>
        /// <param name="text">User input.</param>
        /// <returns>Value parsed from user input.</returns>
        /// <remarks>An empty string will return the currently selected value.</remarks>
        protected override object ParseValue(string text)
        {
            object parsedItem;

            if (!String.IsNullOrEmpty(text))
            {
                // default representation of a domain object
                Func<object, string> getStringRepresentation = item => item.ToString();

                // possible use ValueMemberBinding
                if (_valueBindingEvaluator != null)
                {
                    getStringRepresentation =
                        item =>
                            {
                                return _valueBindingEvaluator.GetDynamicValue(item) ?? String.Empty;
                                // since text is not null or empty, comparison based on String.Empty will (and should) fail.
                            };
                }

                // parsing value is through user input.
                // we will not select a new value based on wrong user input.
                parsedItem = GetActualItems().Where(item => getStringRepresentation(item) == text).FirstOrDefault();

                if (parsedItem == null)
                {
                    // no item was found that matches the text
                    if (InvalidInputAction == InvalidInputAction.UseFallbackItem)
                    {
                        // we will do a fallback: either not select a new item, or use the fallbackitem
                        IsInvalidInput = false;

                        if (FallbackItem != null && GetActualItems().Contains(FallbackItem))
                        {
                            parsedItem = FallbackItem;
                        }
                        else
                        {
                            // end-user may handle this parse error. If not, no new value was set.

                            // throw exception
                            string message = string.Format(
                                CultureInfo.InvariantCulture,
                                Resource.UpDown_ParseException,
                                text);
                            throw new ArgumentException(message, "text");
                        }
                    }
                    else if (InvalidInputAction == InvalidInputAction.TextBoxCannotLoseFocus)
                    {
                        // wrong value, so no new item selected.
                        IsInvalidInput = true;
                        // return current value
                        parsedItem = Value;
                    }
                }
                else
                {
                    // we found a correct item, so input is not invalid
                    IsInvalidInput = false;
                }
            }
            else
            {
                // no text was there, so just return our old value
                IsInvalidInput = false;
                parsedItem = Value;
            }

            return parsedItem;
        }

        /// <summary>
        /// Renders the value property into the textbox text.
        /// </summary>
        /// <returns>Formatted Value.</returns>
        protected internal override string FormatValue()
        {
            if (Value == null)
            {
                return String.Empty;
            }

            if (_valueBindingEvaluator != null)
            {
                try
                {
                    return _valueBindingEvaluator.GetDynamicValue(Value);
                }
                catch
                {
                }
            }
            
            return Value.ToString();
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase. 
        /// </summary>
        /// <remarks>If the IsCyclic property is set to true the DomainUpDown 
        /// control will not increment when the selected item is the last item.</remarks>
        protected override void OnIncrement()
        {
            if (CurrentIndex > 0)
            {
                CurrentIndex -= 1;
            }
            else if (IsCyclic)
            {
                CurrentIndex = GetActualItems().Count() - 1;
            }

            IsInvalidInput = false;

            _isNotAllowedToEditByFocus = true;
            // grab focus
            Focus();

            //// focus event will move to editmode, by setting the
            //// _isNotAllowedToEditByFocus boolean, it will stay in display mode.
            //// after the event has occured, we wish to allow normal behavior.
            Dispatcher.BeginInvoke(() => _isNotAllowedToEditByFocus = false);
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Decrease. 
        /// </summary>
        /// <remarks>If the IsCyclic property is set to true the DomainUpDown 
        /// control will not decrement when the selected item is the first item. </remarks>
        protected override void OnDecrement()
        {
            if (IsValidCurrentIndex(CurrentIndex + 1))
            {
                CurrentIndex += 1;
            }
            else if (IsCyclic)
            {
                CurrentIndex = 0;
            }

            IsInvalidInput = false;

            _isNotAllowedToEditByFocus = true;
            // grab focus
            Focus();

            //// focus event will move to editmode, by setting the
            //// _isNotAllowedToEditByFocus boolean, it will stay in display mode.
            //// after the event has occured, we wish to allow normal behavior.
            Dispatcher.BeginInvoke(() => _isNotAllowedToEditByFocus = false);
        }

        /// <summary>
        /// Sets the valid spin direction based on current index, minimum and maximum.
        /// </summary>
        private void SetValidSpinDirection()
        {
            int count = GetActualItems().Count();

            // decrease and increase within domainUpDown work differently from what is expected.

            ValidSpinDirections validDirections = ValidSpinDirections.None;
            if (IsCyclic || CurrentIndex < count - 1)
            {
                validDirections = validDirections | ValidSpinDirections.Decrease;
            }
            if (IsCyclic || CurrentIndex > 0)
            {
                validDirections = validDirections | ValidSpinDirections.Increase;
            }

            if (Spinner != null)
            {
                Spinner.ValidSpinDirection = validDirections;
            }
        }

        /// <summary>
        /// Provides handling for the KeyDown event.
        /// </summary>
        /// <param name="e">Key event args.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e != null)
            {
                // React to either space or enter key when in display mode
                if ((e.Key == Key.Enter || e.Key == Key.Space) && !IsEditing && IsEditable)
                {
                    IsEditing = true;
                    e.Handled = true;
                    return;
                }
            }

            base.OnKeyDown(e);

            if (e != null)
            {
                if (!e.Handled)
                {
                    if (e.Key == Key.Escape)
                    {
                        IsInvalidInput = false;
                        IsEditing = false;

                        e.Handled = true;
                    }
                    else if (!IsEditing && IsEditable)
                    {
                        // the rest of the keys move us to edit mode.
                        IsEditing = true;
                    }
                }
            }
        }

        /// <summary>
        /// Provides handling for the GotFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowGotFocus(e))
            {
                TryEnterEditMode();

                Interaction.OnGotFocusBase();
                base.OnGotFocus(e);
            }
        }

        /// <summary>
        /// Provides handling for the LostFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowLostFocus(e))
            {
                    if (!IsInvalidInput)
                    {
                        IsEditing = false;
                    }
                    else
                    {
                        if (InvalidInputAction == InvalidInputAction.TextBoxCannotLoseFocus &&
                            FocusManager.GetFocusedElement() != Text)
                        {
                            // the control lost focus while we have invalid input, which is not accepted
                            Dispatcher.BeginInvoke(() => Text.Focus());
                        }
                    }

                Interaction.OnLostFocusBase();
                base.OnLostFocus(e);
            }
        }

        /// <summary>
        /// Provides handling for the MouseEnter event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (Interaction.AllowMouseEnter(e))
            {
                Interaction.OnMouseEnterBase();
                base.OnMouseEnter(e);
            }
        }

        /// <summary>
        /// Provides handling for the MouseLeave event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (Interaction.AllowMouseLeave(e))
            {
                Interaction.OnMouseLeaveBase();
                base.OnMouseLeave(e);
            }
        }

        /// <summary>
        /// Provides handling for the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Interaction.AllowMouseLeftButtonDown(e))
            {
                Interaction.OnMouseLeftButtonDownBase();
                base.OnMouseLeftButtonDown(e);
            }
        }

        /// <summary>
        /// Called before the MouseLeftButtonUp event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (Interaction.AllowMouseLeftButtonUp(e))
            {
                Interaction.OnMouseLeftButtonUpBase();
                base.OnMouseLeftButtonUp(e);

                // ignore mousebutton up when we are currently editing.
                if (!IsEditing)
                {
                    // capture focus
                    Focus();

                    TryEnterEditMode();
                }
            }
        }

        /// <summary>
        /// Tries the enter edit mode.
        /// </summary>
        private void TryEnterEditMode()
        {
            if (!_isNotAllowedToEditByFocus && IsEditable)
            {
                IsEditing = true;
            }
        }

        /// <summary>
        /// Selects all text.
        /// </summary>
        protected override void SelectAllText()
        {
            // DomainUpDown will handle selecting text itself.
        }

        /// <summary>
        /// Update current visual state.
        /// </summary>
        /// <param name="useTransitions">True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.</param>
        internal override void UpdateVisualState(bool useTransitions)
        {
            // edit or display state
            VisualStateManager.GoToState(this, IsEditing ? VisualStates.StateEdit : VisualStates.StateDisplay, useTransitions);

            // valid or invalid state
            VisualStateManager.GoToState(this, IsInvalidInput ? StateInvalid : StateValid, useTransitions);

            // handle common states
            base.UpdateVisualState(useTransitions);
        }

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            UpdateVisualState(useTransitions);
        }

        /// <summary>
        /// Gets the index of a sequence.
        /// </summary>
        /// <param name="sequence">The sequence that contains the item of interest.</param>
        /// <param name="item">The item that contained within the sequence.</param>
        /// <returns>The index of the item in the sequence. -1 if the item was not found.</returns>
        private static int GetIndexOf(IEnumerable sequence, object item)
        {
            int index = 0;
            IEnumerator numerator = sequence.GetEnumerator();
            while (numerator.MoveNext())
            {
                if (numerator.Current.Equals(item))
                {
                    return index;
                }
                index++;
            }

            return -1;
        }
    }
}