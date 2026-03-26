
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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace OpenSilver.Internal;

/// <summary>
/// Stores per-instance trigger state for a FrameworkElement.
/// </summary>
internal sealed class StyleTriggerStorage
{
    private readonly FrameworkElement _element;
    private readonly Style _style;
    private readonly bool _isThemeStyle;

    private HashSet<TriggerBase> _activeTriggers;
    private Dictionary<DependencyProperty, List<TriggerBase>> _propertyTriggerMap;
    private Dictionary<TriggerBase, List<DataTriggerBindingHelper>> _dataTriggerHelpers;
    private List<EventTriggerSourceListener> _eventTriggerSourceListeners;

    private bool _isProcessingTriggers;

    internal StyleTriggerStorage(FrameworkElement element, Style style, bool isThemeStyle)
    {
        _element = element;
        _style = style;
        _isThemeStyle = isThemeStyle;
    }

    /// <summary>
    /// Initializes trigger tracking and evaluates initial states.
    /// </summary>
    internal void Initialize()
    {
        // Include triggers from base styles in the chain
        foreach (TriggerBase trigger in _style.GetAllTriggers())
        {
            SetupTrigger(trigger);
        }

        // Now evaluate all triggers to set initial state
        EvaluateAllTriggers();
    }

    private void SetupTrigger(TriggerBase trigger)
    {
        switch (trigger)
        {
            case Trigger propertyTrigger:
                SetupPropertyTrigger(propertyTrigger);
                break;

            case MultiTrigger multiTrigger:
                SetupMultiTrigger(multiTrigger);
                break;

            case DataTrigger dataTrigger:
                SetupDataTrigger(dataTrigger);
                break;

            case MultiDataTrigger multiDataTrigger:
                SetupMultiDataTrigger(multiDataTrigger);
                break;

            case EventTrigger eventTrigger:
                SetupEventTrigger(eventTrigger);
                break;
        }
    }

    private void SetupPropertyTrigger(Trigger trigger) => RegisterTrigger(trigger.Property, trigger);

    private void SetupMultiTrigger(MultiTrigger trigger)
    {
        foreach (Condition condition in trigger.Conditions)
        {
            RegisterTrigger(condition.Property, trigger);
        }
    }

    private void SetupDataTrigger(DataTrigger trigger) => SetupDataTriggerBinding(trigger, trigger.Value, trigger.Binding);

    private void SetupMultiDataTrigger(MultiDataTrigger trigger)
    {
        foreach (Condition condition in trigger.Conditions)
        {
            SetupDataTriggerBinding(trigger, condition.Value, condition.Binding);
        }
    }

    private void SetupEventTrigger(EventTrigger trigger)
    {
        if (!trigger.HasActions)
        {
            return;
        }

        _eventTriggerSourceListeners ??= [];
        _eventTriggerSourceListeners.Add(new EventTriggerSourceListener(trigger, _element));
    }

    private void RegisterTrigger(DependencyProperty dp, TriggerBase trigger)
    {
        _propertyTriggerMap ??= [];

        if (!_propertyTriggerMap.TryGetValue(dp, out List<TriggerBase> triggers))
        {
            triggers = [];
            _propertyTriggerMap[dp] = triggers;
        }

        if (!triggers.Contains(trigger))
        {
            triggers.Add(trigger);
        }
    }

    private void SetupDataTriggerBinding(TriggerBase trigger, object referenceValue, BindingBase bindingBase)
    {
        var helper = new DataTriggerBindingHelper(this, trigger, referenceValue, bindingBase, _element);

        _dataTriggerHelpers ??= [];
        if (!_dataTriggerHelpers.TryGetValue(trigger, out var helpers))
        {
            helpers = [];
            _dataTriggerHelpers[trigger] = helpers;
        }

        helpers.Add(helper);
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

        if (_propertyTriggerMap is not null && 
            _propertyTriggerMap.TryGetValue(dp, out List<TriggerBase> triggers))
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
        _isProcessingTriggers = true;
        try
        {
            foreach (TriggerBase trigger in _style.GetAllTriggers())
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
    /// Evaluates a single trigger and resolves property values as needed.
    /// </summary>
    private void EvaluateTrigger(TriggerBase trigger)
    {
        bool isActive = EvaluateTriggerCondition(trigger);
        bool wasActive = _activeTriggers?.Contains(trigger) ?? false;

        if (isActive != wasActive)
        {
            // Update active state first, so ResolveAffectedProperties sees the correct state
            if (isActive)
            {
                _activeTriggers ??= [];
                _activeTriggers.Add(trigger);
            }
            else
            {
                _activeTriggers?.Remove(trigger);
            }

            // Re-resolve the correct value for each property this trigger affects.
            // This handles both activation and deactivation correctly, respecting
            // trigger priority (last defined wins) in both directions.
            ResolveAffectedProperties(trigger);

            if (isActive)
            {
                InvokeEnterActions(trigger);
            }
            else
            {
                InvokeExitActions(trigger);
            }
        }
    }

    /// <summary>
    /// Evaluates whether a trigger's condition(s) are met.
    /// </summary>
    private bool EvaluateTriggerCondition(TriggerBase trigger)
    {
        return trigger switch
        {
            Trigger propertyTrigger => EvaluateTrigger(propertyTrigger),
            MultiTrigger multiTrigger => EvaluateMultiTrigger(multiTrigger),
            DataTrigger dataTrigger => EvaluateDataTrigger(dataTrigger),
            MultiDataTrigger multiDataTrigger => EvaluateMultiDataTrigger(multiDataTrigger),
            _ => false,
        };
    }

    private bool EvaluateTrigger(Trigger trigger)
    {
        var state = _element.GetValue(trigger.Property);
        return StyleHelper.Match(state, trigger.Value);
    }

    private bool EvaluateMultiTrigger(MultiTrigger multiTrigger)
    {
        if (multiTrigger.Conditions.Count == 0)
        {
            return false;
        }

        foreach (Condition condition in multiTrigger.Conditions)
        {
            var state = _element.GetValue(condition.Property);
            if (!StyleHelper.Match(state, condition.Value))
            {
                return false;
            }
        }

        return true;
    }

    private bool EvaluateDataTrigger(DataTrigger trigger)
    {
        if (_dataTriggerHelpers is null ||
            !_dataTriggerHelpers.TryGetValue(trigger, out var helpers) || 
            helpers.Count == 0)
        {
            return false;
        }

        return helpers[0].IsMatch();
    }

    private bool EvaluateMultiDataTrigger(MultiDataTrigger trigger)
    {
        if (_dataTriggerHelpers is null || !_dataTriggerHelpers.TryGetValue(trigger, out var helpers))
        {
            return false;
        }

        int helperIndex = 0;
        foreach (Condition condition in trigger.Conditions)
        {
            if (helperIndex >= helpers.Count)
            {
                return false;
            }

            if (!helpers[helperIndex].IsMatch())
            {
                return false;
            }
            helperIndex++;
        }

        return trigger.Conditions.Count > 0;
    }

    /// <summary>
    /// For each property affected by the given trigger, resolves the correct winning value
    /// by walking all triggers in definition order (last active match wins).
    /// </summary>
    /// <remarks>
    /// This single method handles both trigger activation and deactivation correctly.
    /// It must be called AFTER updating <see cref="_activeTriggers"/> so it sees the
    /// current state. By always resolving from scratch, it naturally respects trigger
    /// priority (last defined wins) regardless of which trigger changed.
    /// </remarks>
    private void ResolveAffectedProperties(TriggerBase trigger)
    {
        if (GetTriggerSetters(trigger) is not List<SetterBase> setters)
        {
            return;
        }

        foreach (SetterBase setterBase in setters)
        {
            var setter = (Setter)setterBase;
            DependencyProperty dp = setter.Property;

            if (TryFindWinningTriggerValue(dp, out object winningValue))
            {
                SetTriggerValue(dp, StyleHelper.ResolveSetterValue(_element, dp, winningValue));
            }
            else
            {
                SetTriggerValue(dp, DependencyProperty.UnsetValue);
            }
        }
    }

    /// <summary>
    /// Finds the value from the highest-priority active trigger that sets the given property.
    /// Triggers are walked in definition order; the last active match wins.
    /// </summary>
    private bool TryFindWinningTriggerValue(DependencyProperty dp, out object value)
    {
        value = null;
        bool found = false;

        if (_activeTriggers is null)
        {
            return false;
        }

        foreach (TriggerBase candidateTrigger in _style.GetAllTriggers())
        {
            if (!_activeTriggers.Contains(candidateTrigger))
            {
                continue;
            }

            if (GetTriggerSetters(candidateTrigger) is not List<SetterBase> setters)
            {
                continue;
            }

            foreach (SetterBase setterBase in setters)
            {
                var setter = (Setter)setterBase;
                if (setter.Property == dp)
                {
                    value = setter.Value;
                    found = true;
                    // Don't break - a later trigger in definition order takes precedence
                }
            }
        }

        return found;
    }

    private void SetTriggerValue(DependencyProperty dp, object value)
    {
        if (_isThemeStyle)
        {
            _element.SetThemeStyleTriggerValue(dp, value);
        }
        else
        {
            _element.SetStyleTriggerValue(dp, value);
        }
    }

    private static List<SetterBase> GetTriggerSetters(TriggerBase trigger)
    {
        return trigger switch
        {
            Trigger t => t.HasSetters ? t.Setters.InternalItems : null,
            DataTrigger dt => dt.HasSetters ? dt.Setters.InternalItems : null,
            MultiTrigger mt => mt.HasSetters ? mt.Setters.InternalItems : null,
            MultiDataTrigger mdt => mdt.HasSetters ? mdt.Setters.InternalItems : null,
            _ => null
        };
    }

    private void InvokeEnterActions(TriggerBase trigger)
    {
        if (!trigger.HasEnterActions)
        {
            return;
        }

        foreach (TriggerAction action in trigger.EnterActions.InternalItems)
        {
            action.Invoke(_element);
        }
    }

    private void InvokeExitActions(TriggerBase trigger)
    {
        if (!trigger.HasExitActions)
        {
            return;
        }

        foreach (TriggerAction action in trigger.ExitActions.InternalItems)
        {
            action.Invoke(_element);
        }
    }

    /// <summary>
    /// Cleans up trigger tracking when the style is removed.
    /// </summary>
    internal void Cleanup()
    {
        // Clear all properties set by active triggers
        if (_activeTriggers is not null)
        {
            foreach (TriggerBase trigger in _activeTriggers)
            {
                if (GetTriggerSetters(trigger) is not List<SetterBase> setters)
                {
                    continue;
                }

                foreach (SetterBase setterBase in setters)
                {
                    var setter = (Setter)setterBase;
                    SetTriggerValue(setter.Property, DependencyProperty.UnsetValue);
                }
            }

            _activeTriggers.Clear();
        }

        // Remove event trigger handlers
        if (_eventTriggerSourceListeners is not null)
        {
            foreach (EventTriggerSourceListener listener in _eventTriggerSourceListeners)
            {
                listener.Detach();
            }

            _eventTriggerSourceListeners.Clear();
        }

        if (_dataTriggerHelpers is not null)
        {
            foreach (var helpers in _dataTriggerHelpers.Values)
            {
                foreach (var helper in helpers)
                {
                    helper.Detach();
                }
            }

            _dataTriggerHelpers.Clear();
        }

        _propertyTriggerMap?.Clear();
    }

    private sealed class EventTriggerSourceListener
    {
        private readonly EventTrigger _trigger;
        private readonly FrameworkElement _source;
        private readonly RoutedEventHandler _handler;

        internal EventTriggerSourceListener(EventTrigger trigger, FrameworkElement source)
        {
            _trigger = trigger;
            _source = source;
            _handler = new RoutedEventHandler(Handler);

            _source.AddHandler(_trigger.RoutedEvent, _handler, false);
        }

        internal void Detach() => _source.RemoveHandler(_trigger.RoutedEvent, _handler);

        private void Handler(object sender, RoutedEventArgs e)
        {
            foreach (TriggerAction action in _trigger.Actions.InternalItems)
            {
                action.Invoke(_source);
            }
        }
    }

    private sealed class DataTriggerBindingHelper : StyleHelper.DataTriggerBindingHelperBase
    {
        private readonly TriggerBase _trigger;
        private readonly StyleTriggerStorage _storage;

        public DataTriggerBindingHelper(
            StyleTriggerStorage storage,
            TriggerBase trigger,
            object referenceValue,
            BindingBase bindingBase,
            FrameworkElement container)
            : base(referenceValue, bindingBase, container)
        {
            _trigger = trigger;
            _storage = storage;
        }

        internal override void OnValueChanged() => _storage.OnDataTriggerValueChanged(_trigger);
    }
}
