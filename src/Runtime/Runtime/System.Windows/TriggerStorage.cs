
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

using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Media.Animation;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Stores per-instance trigger state for a FrameworkElement.
/// </summary>
internal sealed class TriggerStorage
{
    private readonly FrameworkElement _element;
    private readonly Style _style;
    private readonly Dictionary<TriggerBase, TriggerState> _triggerStates = new();
    private readonly Dictionary<DependencyProperty, List<TriggerBase>> _propertyTriggerMap = new();
    private readonly Dictionary<TriggerBase, List<DataTriggerBindingHelper>> _dataTriggerHelpers = new();
    private bool _isProcessingTriggers;

    internal TriggerStorage(FrameworkElement element, Style style)
    {
        _element = element;
        _style = style;
    }

    /// <summary>
    /// Initializes trigger tracking and evaluates initial states.
    /// </summary>
    internal void Initialize()
    {
        if (!_style.HasTriggers)
        {
            return;
        }

        foreach (TriggerBase trigger in _style.Triggers)
        {
            SetupTrigger(trigger);
        }

        // Now evaluate all triggers to set initial state
        EvaluateAllTriggers();
    }

    private void SetupTrigger(TriggerBase trigger)
    {
        // Initialize state to false (inactive)
        _triggerStates[trigger] = new TriggerState();

        switch (trigger)
        {
            case Trigger propertyTrigger:
                SetupPropertyTrigger(propertyTrigger);
                break;

            case DataTrigger dataTrigger:
                SetupDataTrigger(dataTrigger);
                break;

            case MultiTrigger multiTrigger:
                SetupMultiTrigger(multiTrigger);
                break;

            case MultiDataTrigger multiDataTrigger:
                SetupMultiDataTrigger(multiDataTrigger);
                break;

            case EventTrigger:
                // EventTriggers are handled differently (via routed events)
                break;
        }
    }

    private void SetupPropertyTrigger(Trigger trigger)
    {
        if (trigger.Property is null)
        {
            return;
        }

        // Register for property changes on the source element
        DependencyProperty dp = trigger.Property;
        if (!_propertyTriggerMap.TryGetValue(dp, out List<TriggerBase> triggers))
        {
            triggers = new List<TriggerBase>();
            _propertyTriggerMap[dp] = triggers;
        }
        triggers.Add(trigger);
    }

    private void SetupDataTrigger(DataTrigger trigger)
    {
        if (trigger.Binding is null)
        {
            return;
        }

        // Create a binding to monitor the data value
        SetupDataTriggerBinding(trigger, trigger.Binding);
    }

    private void SetupMultiTrigger(MultiTrigger trigger)
    {
        foreach (Condition condition in trigger.Conditions)
        {
            if (condition.Property is not null)
            {
                DependencyProperty dp = condition.Property;
                if (!_propertyTriggerMap.TryGetValue(dp, out List<TriggerBase> triggers))
                {
                    triggers = new List<TriggerBase>();
                    _propertyTriggerMap[dp] = triggers;
                }
                if (!triggers.Contains(trigger))
                {
                    triggers.Add(trigger);
                }
            }
        }
    }

    private void SetupMultiDataTrigger(MultiDataTrigger trigger)
    {
        foreach (Condition condition in trigger.Conditions)
        {
            if (condition.Binding is not null)
            {
                SetupDataTriggerBinding(trigger, condition.Binding);
            }
        }
    }

    private void SetupDataTriggerBinding(TriggerBase trigger, BindingBase bindingBase)
    {
        // Create a helper to receive the binding value
        var helper = new DataTriggerBindingHelper(this, trigger, _element);

        if (bindingBase is Binding binding)
        {
            // Clone the binding and set up the listener
            // We need to copy relevant properties from the original binding
            var listenerBinding = new Binding
            {
                Path = binding.Path,
                Mode = BindingMode.OneWay,
            };

            // Copy source-related properties if they're set
            if (binding.Source is not null)
            {
                listenerBinding.Source = binding.Source;
            }
            if (binding.RelativeSource is not null)
            {
                listenerBinding.RelativeSource = binding.RelativeSource;
            }
            if (!string.IsNullOrEmpty(binding.ElementName))
            {
                listenerBinding.ElementName = binding.ElementName;
            }

            // Create binding expression and apply it to the helper
            BindingExpression expr = (BindingExpression)listenerBinding.CreateBindingExpression(
                helper, 
                DataTriggerBindingHelper.ValueProperty, 
                null);

            helper.SetValue(DataTriggerBindingHelper.ValueProperty, expr);

            if (!_dataTriggerHelpers.TryGetValue(trigger, out var helpers))
            {
                helpers = new List<DataTriggerBindingHelper>();
                _dataTriggerHelpers[trigger] = helpers;
            }
            helpers.Add(helper);
        }
    }

    /// <summary>
    /// Called when a dependency property value changes on the element.
    /// </summary>
    internal void OnPropertyChanged(DependencyProperty dp)
    {
        if (_isProcessingTriggers)
        {
            return;
        }

        if (_propertyTriggerMap.TryGetValue(dp, out List<TriggerBase> triggers))
        {
            foreach (TriggerBase trigger in triggers)
            {
                EvaluateTrigger(trigger);
            }
        }
    }

    /// <summary>
    /// Called when a data trigger binding value changes.
    /// </summary>
    internal void OnDataTriggerValueChanged(TriggerBase trigger)
    {
        if (_isProcessingTriggers)
        {
            return;
        }

        EvaluateTrigger(trigger);
    }

    /// <summary>
    /// Evaluates all triggers and applies their setters as needed.
    /// </summary>
    private void EvaluateAllTriggers()
    {
        if (!_style.HasTriggers)
        {
            return;
        }

        _isProcessingTriggers = true;
        try
        {
            foreach (TriggerBase trigger in _style.Triggers)
            {
                EvaluateTrigger(trigger);
            }
        }
        finally
        {
            _isProcessingTriggers = false;
        }
    }

    /// <summary>
    /// Evaluates a single trigger and applies/unapplies its setters.
    /// </summary>
    private void EvaluateTrigger(TriggerBase trigger)
    {
        if (!_triggerStates.TryGetValue(trigger, out TriggerState state))
        {
            return;
        }

        bool isActive = EvaluateTriggerCondition(trigger);
        bool wasActive = state.IsActive;

        if (isActive != wasActive)
        {
            state.IsActive = isActive;

            if (isActive)
            {
                // Apply trigger setters
                ApplyTriggerSetters(trigger);
                InvokeEnterActions(trigger);
            }
            else
            {
                // Remove trigger setters
                UnapplyTriggerSetters(trigger);
                InvokeExitActions(trigger);
            }
        }
    }

    /// <summary>
    /// Evaluates whether a trigger's condition(s) are met.
    /// </summary>
    private bool EvaluateTriggerCondition(TriggerBase trigger)
    {
        switch (trigger)
        {
            case Trigger propertyTrigger:
                return propertyTrigger.Evaluate(_element);

            case MultiTrigger multiTrigger:
                return multiTrigger.Evaluate(_element);

            case DataTrigger dataTrigger:
                return EvaluateDataTrigger(dataTrigger);

            case MultiDataTrigger multiDataTrigger:
                return EvaluateMultiDataTrigger(multiDataTrigger);

            default:
                return false;
        }
    }

    private bool EvaluateDataTrigger(DataTrigger trigger)
    {
        if (!_dataTriggerHelpers.TryGetValue(trigger, out var helpers) || helpers.Count == 0)
        {
            return false;
        }

        object currentValue = helpers[0].Value;
        return Trigger.Match(currentValue, trigger.Value);
    }

    private bool EvaluateMultiDataTrigger(MultiDataTrigger trigger)
    {
        if (!_dataTriggerHelpers.TryGetValue(trigger, out var helpers))
        {
            return false;
        }

        int helperIndex = 0;
        foreach (Condition condition in trigger.Conditions)
        {
            if (condition.Binding is null || helperIndex >= helpers.Count)
            {
                return false;
            }

            object currentValue = helpers[helperIndex].Value;
            if (!Trigger.Match(currentValue, condition.Value))
            {
                return false;
            }
            helperIndex++;
        }

        return trigger.Conditions.Count > 0;
    }

    /// <summary>
    /// Applies the setters of an active trigger.
    /// </summary>
    private void ApplyTriggerSetters(TriggerBase trigger)
    {
        SetterBaseCollection setters = GetTriggerSetters(trigger);
        if (setters is null)
        {
            return;
        }

        foreach (SetterBase setterBase in setters)
        {
            if (setterBase is Setter setter && setter.Property is not null)
            {
                object value = setter.ValueInternal;

                // Handle BindingBase values
                if (value is BindingBase bindingBase)
                {
                    value = bindingBase.CreateBindingExpression(_element, setter.Property, null);
                }
                // Handle DynamicResourceExtension
                else if (value is DynamicResourceExtension dynamicResource)
                {
                    value = new ResourceReferenceExpression(dynamicResource.ResourceKey ??
                        throw new InvalidOperationException(Strings.MarkupExtensionResourceKey));
                }

                _element.SetTriggerValue(setter.Property, value);
            }
        }
    }

    /// <summary>
    /// Removes the setters of an inactive trigger.
    /// </summary>
    private void UnapplyTriggerSetters(TriggerBase trigger)
    {
        SetterBaseCollection setters = GetTriggerSetters(trigger);
        if (setters is null)
        {
            return;
        }

        foreach (SetterBase setterBase in setters)
        {
            if (setterBase is Setter setter && setter.Property is not null)
            {
                _element.ClearTriggerValue(setter.Property);
            }
        }
    }

    private static SetterBaseCollection GetTriggerSetters(TriggerBase trigger)
    {
        return trigger switch
        {
            Trigger t => t.HasSetters ? t.Setters : null,
            DataTrigger dt => dt.HasSetters ? dt.Setters : null,
            MultiTrigger mt => mt.HasSetters ? mt.Setters : null,
            MultiDataTrigger mdt => mdt.HasSetters ? mdt.Setters : null,
            _ => null
        };
    }

    private void InvokeEnterActions(TriggerBase trigger)
    {
        if (!trigger.HasEnterActions)
        {
            return;
        }

        foreach (TriggerAction action in trigger.EnterActions)
        {
            if (action is BeginStoryboard beginStoryboard)
            {
                beginStoryboard.Storyboard?.Begin(_element);
            }
        }
    }

    private void InvokeExitActions(TriggerBase trigger)
    {
        if (!trigger.HasExitActions)
        {
            return;
        }

        foreach (TriggerAction action in trigger.ExitActions)
        {
            if (action is BeginStoryboard beginStoryboard)
            {
                beginStoryboard.Storyboard?.Begin(_element);
            }
        }
    }

    /// <summary>
    /// Cleans up trigger tracking when the style is removed.
    /// </summary>
    internal void Cleanup()
    {
        // Unapply all active triggers
        foreach (var kvp in _triggerStates)
        {
            if (kvp.Value.IsActive)
            {
                UnapplyTriggerSetters(kvp.Key);
            }
        }

        _triggerStates.Clear();
        _propertyTriggerMap.Clear();
        _dataTriggerHelpers.Clear();
    }

    /// <summary>
    /// Stores the state of a single trigger.
    /// </summary>
    private sealed class TriggerState
    {
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Helper class to receive data trigger binding values.
    /// This class inherits from FrameworkElement to properly participate in DataContext inheritance.
    /// </summary>
    private sealed class DataTriggerBindingHelper : FrameworkElement
    {
        private readonly TriggerStorage _storage;
        private readonly TriggerBase _trigger;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(object),
                typeof(DataTriggerBindingHelper),
                new PropertyMetadata(null, OnValueChanged));

        public DataTriggerBindingHelper(TriggerStorage storage, TriggerBase trigger, FrameworkElement element)
        {
            _storage = storage;
            _trigger = trigger;

            // Bind our DataContext to the element's DataContext so bindings work correctly
            var dcBinding = new Binding
            {
                Source = element,
                Path = new PropertyPath(nameof(DataContext)),
                Mode = BindingMode.OneWay
            };
            SetBinding(DataContextProperty, dcBinding);
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValueInternal(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var helper = (DataTriggerBindingHelper)d;
            helper._storage.OnDataTriggerValueChanged(helper._trigger);
        }
    }
}
