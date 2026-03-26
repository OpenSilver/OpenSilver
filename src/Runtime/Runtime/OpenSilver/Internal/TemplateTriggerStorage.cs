
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

using OpenSilver.Internal.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace OpenSilver.Internal;

/// <summary>
/// Stores per-instance trigger state for a FrameworkElement with a template.
/// This class handles triggers defined in ControlTemplate and DataTemplate.
/// </summary>
internal sealed class TemplateTriggerStorage
{
    private readonly FrameworkElement _templatedParent;
    private readonly TriggerCollection _triggers;
    private readonly INameScope _nameScope;

    private HashSet<TriggerBase> _activeTriggers;
    private Dictionary<FrameworkElement, Dictionary<DependencyProperty, List<TriggerBase>>> _sourceElementTriggerMap;
    private Dictionary<TriggerBase, List<DataTriggerBindingHelper>> _dataTriggerHelpers;
    private List<EventTriggerSourceListener> _eventTriggerSourceListeners;

    private bool _isProcessingTriggers;

    internal TemplateTriggerStorage(FrameworkElement templatedParent, TriggerCollection triggers)
    {
        Debug.Assert(templatedParent is not null);
        Debug.Assert(triggers is not null);
        _templatedParent = templatedParent;
        _triggers = triggers;
        _nameScope = FrameworkTemplate.GetTemplateNameScope(templatedParent);
    }

    /// <summary>
    /// Initializes trigger tracking and evaluates initial states.
    /// </summary>
    internal void Initialize()
    {
        foreach (TriggerBase trigger in _triggers.InternalItems)
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

    private void SetupPropertyTrigger(Trigger trigger)
    {
        var sourceElement = ResolveNamedElement(trigger.SourceName);
        RegisterSourceElementTrigger(sourceElement, trigger.Property, trigger);
    }

    private void SetupMultiTrigger(MultiTrigger trigger)
    {
        foreach (Condition condition in trigger.Conditions)
        {
            var sourceElement = ResolveNamedElement(condition.SourceName);
            RegisterSourceElementTrigger(sourceElement, condition.Property, trigger);
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

        var sourceElement = ResolveNamedElement(trigger.SourceName);
        _eventTriggerSourceListeners ??= [];
        _eventTriggerSourceListeners.Add(new EventTriggerSourceListener(trigger, sourceElement, this));
    }

    private void RegisterSourceElementTrigger(FrameworkElement sourceElement, DependencyProperty dp, TriggerBase trigger)
    {
        _sourceElementTriggerMap ??= [];

        if (!_sourceElementTriggerMap.TryGetValue(sourceElement, out var propMap))
        {
            propMap = [];
            _sourceElementTriggerMap[sourceElement] = propMap;
        }

        if (!propMap.TryGetValue(dp, out var triggers))
        {
            triggers = [];
            propMap[dp] = triggers;
        }

        if (!triggers.Contains(trigger))
        {
            triggers.Add(trigger);
        }
    }

    private void SetupDataTriggerBinding(TriggerBase trigger, object referenceValue, BindingBase bindingBase)
    {
        var helper = new DataTriggerBindingHelper(this, trigger, referenceValue, bindingBase, _templatedParent);

        _dataTriggerHelpers ??= [];
        if (!_dataTriggerHelpers.TryGetValue(trigger, out var helpers))
        {
            helpers = [];
            _dataTriggerHelpers[trigger] = helpers;
        }

        helpers.Add(helper);
    }

    /// <summary>
    /// Called when a dependency property value changes on the templated parent.
    /// </summary>
    internal void OnPropertyChanged(DependencyProperty dp) => OnTriggerSourcePropertyInvalidated(_templatedParent, dp);

    /// <summary>
    /// Called when a dependency property value changes on a source element (used for SourceName triggers).
    /// </summary>
    internal void OnTriggerSourcePropertyInvalidated(FrameworkElement sourceElement, DependencyProperty dp)
    {
        if (_isProcessingTriggers)
        {
            return;
        }

        if (_sourceElementTriggerMap is not null &&
            _sourceElementTriggerMap.TryGetValue(sourceElement, out var propMap) &&
            propMap.TryGetValue(dp, out List<TriggerBase> triggers))
        {
            foreach (TriggerBase trigger in triggers)
            {
                EvaluateTrigger(trigger);
            }
        }
    }

    internal void OnDataTriggerValueChanged(TriggerBase trigger)
    {
        if (_isProcessingTriggers)
        {
            return;
        }

        EvaluateTrigger(trigger);
    }

    private void EvaluateAllTriggers()
    {
        _isProcessingTriggers = true;
        try
        {
            foreach (TriggerBase trigger in _triggers.InternalItems)
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

    private bool EvaluateTriggerCondition(TriggerBase triggerBase)
    {
        return triggerBase switch
        {
            Trigger trigger => EvaluateTrigger(trigger),
            MultiTrigger multiTrigger => EvaluateMultiTrigger(multiTrigger),
            DataTrigger dataTrigger => EvaluateDataTrigger(dataTrigger),
            MultiDataTrigger multiDataTrigger => EvaluateMultiDataTrigger(multiDataTrigger),
            _ => false,
        };
    }

    private bool EvaluateTrigger(Trigger trigger)
    {
        var sourceElement = ResolveNamedElement(trigger.SourceName);
        var state = sourceElement.GetValue(trigger.Property);
        return StyleHelper.Match(state, trigger.Value);
    }

    private bool EvaluateMultiTrigger(MultiTrigger trigger)
    {
        foreach (Condition condition in trigger.Conditions)
        {
            var sourceElement = ResolveNamedElement(condition.SourceName);
            var state = sourceElement.GetValue(condition.Property);
            if (!StyleHelper.Match(state, condition.Value))
            {
                return false;
            }
        }

        return trigger.Conditions.Count > 0;
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
        if (_dataTriggerHelpers is null ||
            !_dataTriggerHelpers.TryGetValue(trigger, out var helpers))
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
    /// For each property/target affected by the given trigger, resolves the correct winning
    /// value by walking all triggers in definition order (last active match wins).
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

            var target = ResolveNamedElement(setter.TargetName);
            DependencyProperty dp = setter.Property;

            if (TryFindWinningTriggerValue(dp, GetTargetName(setter.TargetName), out object winningValue))
            {
                SetTriggerValue(target, dp, StyleHelper.ResolveSetterValue(target, dp, winningValue));
            }
            else
            {
                SetTriggerValue(target, dp, DependencyProperty.UnsetValue);
            }
        }
    }

    private void SetTriggerValue(FrameworkElement target, DependencyProperty dp, object value)
    {
        if (target == _templatedParent)
        {
            target.SetTemplateTriggerValue(dp, value);
        }
        else
        {
            target.SetParentTemplateTriggerValue(dp, value);
        }
    }

    /// <summary>
    /// Finds the value from the highest-priority active trigger that sets the given
    /// property on the given target. Triggers are walked in definition order; the last
    /// active match wins. Both property and target name must match.
    /// </summary>
    private bool TryFindWinningTriggerValue(DependencyProperty dp, string targetName, out object value)
    {
        value = null;
        bool found = false;

        if (_activeTriggers is null)
        {
            return false;
        }

        foreach (TriggerBase candidateTrigger in _triggers.InternalItems)
        {
            if (!_activeTriggers.Contains(candidateTrigger))
            {
                continue;
            }

            if (GetTriggerSetters(candidateTrigger) is not List<SetterBase> setters)
            {
                continue;
            }

            foreach (var setterBase in setters)
            {
                var setter = (Setter)setterBase;
                if (setter.Property == dp && GetTargetName(setter.TargetName) == targetName)
                {
                    value = setter.Value;
                    found = true;
                    // Don't break - a later trigger in definition order takes precedence
                }
            }
        }

        return found;
    }

    private FrameworkElement ResolveNamedElement(string name)
    {
        name = GetTargetName(name);

        if (name == StyleHelper.SelfName)
        {
            return _templatedParent;
        }

        if (_nameScope?.FindName(name) is not FrameworkElement element)
        {
            throw new InvalidOperationException(string.Format(Strings.NameNotFound, name));
        }

        return element;
    }

    private static string GetTargetName(string name) => name ?? StyleHelper.SelfName;

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
            action.Invoke(_templatedParent, _nameScope);
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
            action.Invoke(_templatedParent, _nameScope);
        }
    }

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

                foreach (var setterBase in setters)
                {
                    var setter = (Setter)setterBase;
                    var target = ResolveNamedElement(setter.TargetName);
                    SetTriggerValue(target, setter.Property, DependencyProperty.UnsetValue);
                }
            }

            _activeTriggers.Clear();
        }

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

        _sourceElementTriggerMap?.Clear();
    }

    private sealed class EventTriggerSourceListener
    {
        private readonly EventTrigger _trigger;
        private readonly FrameworkElement _source;
        private readonly TemplateTriggerStorage _owner;
        private readonly RoutedEventHandler _handler;

        internal EventTriggerSourceListener(EventTrigger trigger, FrameworkElement source, TemplateTriggerStorage owner)
        {
            _trigger = trigger;
            _source = source;
            _owner = owner;
            _handler = new RoutedEventHandler(Handler);

            _source.AddHandler(_trigger.RoutedEvent, _handler, false);
        }

        internal void Detach() => _source.RemoveHandler(_trigger.RoutedEvent, _handler);

        private void Handler(object sender, RoutedEventArgs e)
        {
            foreach (TriggerAction action in _trigger.Actions.InternalItems)
            {
                action.Invoke(_owner._templatedParent, _owner._nameScope);
            }
        }
    }

    private sealed class DataTriggerBindingHelper : StyleHelper.DataTriggerBindingHelperBase
    {
        private readonly TriggerBase _trigger;
        private readonly TemplateTriggerStorage _storage;

        public DataTriggerBindingHelper(
            TemplateTriggerStorage storage,
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
