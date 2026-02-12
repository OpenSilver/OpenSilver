
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
/// <remarks>
/// Memory optimization notes:
/// - Dictionaries are lazily initialized to avoid allocations when not needed
/// - Uses HashSet for active triggers instead of Dictionary with TriggerState objects
/// - Most styles only use property triggers, so DataTrigger/EventTrigger storage is often unused
/// </remarks>
internal sealed class TriggerStorage
{
    private readonly FrameworkElement _element;
    private readonly Style _style;
    private readonly bool _isThemeStyle;

    // Active triggers set - uses HashSet instead of Dictionary<TriggerBase, TriggerState>
    // to eliminate TriggerState object allocations (~24 bytes saved per trigger)
    private HashSet<TriggerBase> _activeTriggers;

    // Lazily initialized - only allocated when property triggers exist
    private Dictionary<DependencyProperty, List<TriggerBase>> _propertyTriggerMap;

    // Lazily initialized - only allocated when DataTriggers exist
    private Dictionary<TriggerBase, List<DataTriggerBindingHelper>> _dataTriggerHelpers;

    // Lazily initialized - only allocated when EventTriggers exist
    private Dictionary<EventTrigger, RoutedEventHandler> _eventTriggerHandlers;

    private bool _isProcessingTriggers;

    internal TriggerStorage(FrameworkElement element, Style style, bool isThemeStyle = false)
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
        if (!_style.HasTriggers)
        {
            return;
        }

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

            case DataTrigger dataTrigger:
                SetupDataTrigger(dataTrigger);
                break;

            case MultiTrigger multiTrigger:
                SetupMultiTrigger(multiTrigger);
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
        if (trigger.Property is null)
        {
            return;
        }

        // Register for property changes on the source element
        DependencyProperty dp = trigger.Property;
        _propertyTriggerMap ??= new Dictionary<DependencyProperty, List<TriggerBase>>();
        
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
                _propertyTriggerMap ??= new Dictionary<DependencyProperty, List<TriggerBase>>();
                
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

    private void SetupEventTrigger(EventTrigger trigger)
    {
        if (trigger.RoutedEvent is null)
        {
            return;
        }

        // Create a handler that invokes the trigger's actions
        RoutedEventHandler handler = (sender, e) =>
        {
            // Invoke all actions of the EventTrigger
            foreach (TriggerAction action in trigger.Actions)
            {
                action.Invoke((IInternalFrameworkElement)_element);
            }
        };

        _eventTriggerHandlers ??= new Dictionary<EventTrigger, RoutedEventHandler>();
        _eventTriggerHandlers[trigger] = handler;
        _element.AddHandler(trigger.RoutedEvent, handler, false);
    }

    private void SetupDataTriggerBinding(TriggerBase trigger, BindingBase bindingBase)
    {
        if (bindingBase is not Binding binding)
        {
            return;
        }

        // Note: DataTriggerBindingHelper inherits from FrameworkElement which has significant
        // memory overhead (~500+ bytes). This is necessary because the binding system requires
        // a FrameworkElement for proper DataContext resolution. Future optimization could
        // create a lighter-weight binding listener mechanism.
        var helper = new DataTriggerBindingHelper(this, trigger, _element);

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

        _dataTriggerHelpers ??= new Dictionary<TriggerBase, List<DataTriggerBindingHelper>>();
        if (!_dataTriggerHelpers.TryGetValue(trigger, out var helpers))
        {
            helpers = new List<DataTriggerBindingHelper>();
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
        if (!_style.HasTriggers)
        {
            return;
        }

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
                _activeTriggers ??= new HashSet<TriggerBase>();
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
        if (_dataTriggerHelpers is null ||
            !_dataTriggerHelpers.TryGetValue(trigger, out var helpers) || 
            helpers.Count == 0)
        {
            return false;
        }

        object currentValue = helpers[0].Value;
        return Trigger.Match(currentValue, trigger.Value);
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
        SetterBaseCollection setters = GetTriggerSetters(trigger);
        if (setters is null)
        {
            return;
        }

        foreach (SetterBase setterBase in setters)
        {
            if (setterBase is Setter setter && setter.Property is not null)
            {
                DependencyProperty dp = setter.Property;

                if (TryFindWinningTriggerValue(dp, out object winningValue))
                {
                    SetTriggerValue(dp, ResolveSetterValue(winningValue, dp, _element));
                }
                else
                {
                    ClearTriggerValue(dp);
                }
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

            SetterBaseCollection setters = GetTriggerSetters(candidateTrigger);
            if (setters is not null)
            {
                foreach (SetterBase setterBase in setters)
                {
                    if (setterBase is Setter setter && setter.Property == dp)
                    {
                        value = setter.ValueInternal;
                        found = true;
                        // Don't break - a later trigger in definition order takes precedence
                    }
                }
            }
        }

        return found;
    }

    /// <summary>
    /// Resolves a setter's value, converting BindingBase and DynamicResourceExtension
    /// to their runtime representations.
    /// </summary>
    private static object ResolveSetterValue(object value, DependencyProperty dp, DependencyObject target)
    {
        if (value is BindingBase bindingBase)
        {
            return bindingBase.CreateBindingExpression(target, dp, null);
        }
        if (value is DynamicResourceExtension dynamicResource)
        {
            return new ResourceReferenceExpression(dynamicResource.ResourceKey ??
                throw new InvalidOperationException(Strings.MarkupExtensionResourceKey));
        }
        return value;
    }

    private void SetTriggerValue(DependencyProperty dp, object value)
    {
        if (_isThemeStyle)
            _element.SetThemeStyleTriggerValue(dp, value);
        else
            _element.SetTriggerValue(dp, value);
    }

    private void ClearTriggerValue(DependencyProperty dp)
    {
        if (_isThemeStyle)
            _element.ClearThemeStyleTriggerValue(dp);
        else
            _element.ClearTriggerValue(dp);
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
        // Clear all properties set by active triggers
        if (_activeTriggers is not null)
        {
            foreach (TriggerBase trigger in _activeTriggers)
            {
                SetterBaseCollection setters = GetTriggerSetters(trigger);
                if (setters is not null)
                {
                    foreach (SetterBase setterBase in setters)
                    {
                        if (setterBase is Setter setter && setter.Property is not null)
                        {
                            ClearTriggerValue(setter.Property);
                        }
                    }
                }
            }
            _activeTriggers.Clear();
        }

        // Remove event trigger handlers
        if (_eventTriggerHandlers is not null)
        {
            foreach (var kvp in _eventTriggerHandlers)
            {
                if (kvp.Key.RoutedEvent is not null)
                {
                    _element.RemoveHandler(kvp.Key.RoutedEvent, kvp.Value);
                }
            }
            _eventTriggerHandlers.Clear();
        }

        _propertyTriggerMap?.Clear();
        _dataTriggerHelpers?.Clear();
    }

    /// <summary>
    /// Helper class to receive data trigger binding values.
    /// This class inherits from FrameworkElement to properly participate in DataContext inheritance
    /// and binding resolution.
    /// </summary>
    /// <remarks>
    /// Memory consideration: FrameworkElement has significant overhead (~500+ bytes per instance).
    /// This is required because the binding system uses FrameworkElement.FindMentor for DataContext
    /// resolution. A lighter-weight approach would require changes to the binding infrastructure.
    /// </remarks>
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
